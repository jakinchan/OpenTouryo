﻿//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：EnumToStringByExpressionTreeExtensions
//* クラス日本語名  ：EnumToStringByExpressionTreeExtensions
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/16  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Text;
using System.Collections.Concurrent;

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Touryo.Infrastructure.Public.FastReflection
{
    // 詳しくは、EnumToString1Extensions側を参照。
    // net側でMSBuildでビルドできないのでnetstandard側のみ提供。

    /// <summary>EnumToStringByExpressionTreeExtensions</summary>
    public static class EnumToStringByExpressionTreeExtensions
    {
        /// <summary>スレッドセーフ</summary>
        private static ConcurrentDictionary<Type, MulticastDelegate>
            ToStringMethods = new ConcurrentDictionary<Type, MulticastDelegate>();

        #region public

        /// <summary>ToStringByExpressionTree（式木版）</summary>
        /// <typeparam name="T">struct(Enum Field)</typeparam>
        /// <param name="value">値</param>
        /// <returns>列挙型を文字列化</returns>
        public static string ToStringByExpressionTree<T>(this Nullable<T> value) where T : struct
        {
            if (value.HasValue == true)
            {
                return EnumToStringByExpressionTreeExtensions.ToStringByExpressionTree(value.Value);
            }
            else
            {
                return "";
            }
        }

        /// <summary>GetString（式木版）</summary>
        /// <typeparam name="T">struct(Enum Field)</typeparam>
        /// <param name="value">値</param>
        /// <returns>列挙型を文字列化</returns>
        public static string ToStringByExpressionTree<T>(this T value) where T : struct
        {
            // Enum Field
            Type type = typeof(T);

            if (type.IsEnum == false)
            {
                throw new ArgumentException("value must be a enum type");
            }
            else
            {
                MulticastDelegate multicastDelegate = null;

                // MulticastDelegateのロード
                if (!EnumToStringByExpressionTreeExtensions.ToStringMethods.TryGetValue(type, out multicastDelegate))
                {
                    // こちらは、FlagsAttributeを処理可能。
                    multicastDelegate = EnumToStringByExpressionTreeExtensions.CreateToString<T>();
                    // MulticastDelegateをキャッシュ
                    EnumToStringByExpressionTreeExtensions.ToStringMethods[type] = multicastDelegate;
                }

                // MulticastDelegateでFastReflection
                Func<T, string> f = (Func<T, string>)multicastDelegate;
                return f(value);
            }
        }

        #endregion

        #region private

        /// <summary>MulticastDelegateの生成</summary>
        /// <typeparam name="T">struct(Enum)</typeparam>
        /// <returns>MulticastDelegate</returns>
        private static Func<T, string> CreateToString<T>() where T : struct
        {
            // C# によるプログラミング入門 | ++C++; // 未確認飛行 C
            // 式木（Expression Trees） > 式木 4.0（構文木）
            // https://ufcpp.net/study/csharp/sp3_expression.html#ast

            // StringBuilderを用いて、AppendとToStringのMethodInfoを取得
            MethodInfo append = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new[] { typeof(string) });
            MethodInfo toString = typeof(StringBuilder).GetMethod(nameof(StringBuilder.ToString), Type.EmptyTypes);

            // パラメタ
            ParameterExpression valueOfField = Expression.Parameter(typeof(T));

            // 変数
            // - StringBuilder
            ParameterExpression sbBuffer = Expression.Variable(typeof(StringBuilder));
            // - valueOfField値
            ParameterExpression valueLong = Expression.Variable(typeof(int));

            // 列挙型のFlagsAttributeの有無
            bool hasFlgAttr = typeof(T).GetTypeInfo().IsDefined(typeof(FlagsAttribute));

            #region パーツ

            // IFステートメント
            // 0 < sbBuffer.Lengthの場合、.Append(", ")する。
            ConditionalExpression separator =
                Expression.IfThen(
                    Expression.LessThan( // ＜
                        Expression.Constant((int)0, typeof(int)),                     // left
                        Expression.Property(sbBuffer, nameof(StringBuilder.Length))), // right
                    Expression.Call(sbBuffer, append, Expression.Constant(", ", typeof(string))));

            // 匿名型の 'a 配列 = EnumのField値の配列に対応
            var members = ((T[])Enum.GetValues(typeof(T))).Distinct().Select(x =>
            {
                // EnumのFieldの数値
                int value = Convert.ToInt32(x);
                // EnumのFieldの数値
                ConstantExpression flagValue = Expression.Constant(value);
                // EnumのFieldの文字列値
                ConstantExpression label = Expression.Constant(x.ToString(), typeof(string));

                // Select引数のλの戻り（ ≒ 匿名型の 'a）
                return new
                {
                    Value = value,                  // フィールドの数値
                    Expression = (value == 0) ?     // Expression
                        (Expression)label :         // value == 0 ならフィールドの文字列値
                        Expression.IfThen(          // value != 0 なら、if(){}で FlagsAttributeの処理
                            Expression.Equal(       // 以下を比較
                                flagValue,          // - EnumのFieldの数値
                                                    // - （FlagsAttributeがある場合で異なる）値
                                                    //   - ある場合 : flagValue && valueLong
                                                    //   - ない場合 : valueLong
                                hasFlgAttr ? (Expression)Expression.And(flagValue, valueLong) : valueLong),
                            Expression.Block( // ビット演算の処理
                                separator, // 前述のIFステートメントで、   // .Append(", ")する。
                                Expression.Call(sbBuffer, append, label))) // .Append(label)する。
                };
            }).ToArray();

            #endregion

            #region 本体

            // MulticastDelegateの組立とコンパイル
            return Expression.Lambda<Func<T, string>>(
                // Body
                Expression.Block(
                    new[] { sbBuffer, valueLong },
                    // 変数宣言
                    // StringBuilder
                    Expression.Assign(sbBuffer, Expression.New(typeof(StringBuilder))),
                    // valueOfField値
                    Expression.Assign(valueLong, Expression.Convert(valueOfField, typeof(int))),
                    // if(valueLong == 0){} else{}
                    Expression.IfThenElse(
                        Expression.Equal(valueLong, Expression.Constant(0, typeof(int))),
                        // if(valueLong == 0){ sbBuffer.Append(flagValue == 0 の Expression); }
                        Expression.Call(sbBuffer, append, // 以下の何れかの値
                            (members.FirstOrDefault(x => x.Value == 0)?.Expression)   // label
                            ?? (Expression)Expression.Constant("0", typeof(string))), // "0"
                        // else{ sbBuffer.Append(flagValue != 0 の Expression); }
                        // members.Expression内で、valueLongを使用してビット演算などを行う。
                        Expression.Block(members.Where(x => x.Value != 0).Select(x => x.Expression))
                        ),
                    // sbBuffer.ToString();
                    Expression.Call(sbBuffer, toString)),
                // Parameter
                valueOfField
                ).Compile();

            #endregion
        }

        #endregion
    }
}