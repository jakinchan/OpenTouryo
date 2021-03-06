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
//* クラス名        ：JwkSetStore
//* クラス日本語名  ：JwkSetStore
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/16  西野 大介         新規作成
//*  2018/11/28  西野 大介         jkuチェック対応の追加
//*  2020/06/20  西野 大介         jku無しでもjwks_uriを使うように変更
//**********************************************************************************

using System;
using System.Threading;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>JwkSetStore</summary>
    public class JwkSetStore
    {
        #region member variable

        /// <summary>Singleton (instance)</summary>
        private static JwkSetStore _jwkSetStore = new JwkSetStore();

        /// <summary>ReaderWriterLock</summary>
        private ReaderWriterLock _rwLock = new ReaderWriterLock();

        /// <summary>共有リソース(DateTime)</summary>
        private DateTime _dateTime = DateTime.MinValue;

        /// <summary>共有リソース(JwkSet)</summary>
        private JwkSet _jwkSet = null;

        #endregion

        #region constructor

        /// <summary>constructor</summary>
        public JwkSetStore()
        {
            if (string.IsNullOrEmpty(OAuth2AndOIDCParams.JwkSetUri))
            {
                this._jwkSet = new JwkSet();
            }
            else
            {
                // _jwkSet 更新
                this._jwkSet = JsonConvert.DeserializeObject<JwkSet>(
                    OAuth2AndOIDCClient.GetJwkSetAsync(
                        new Uri(OAuth2AndOIDCParams.JwkSetUri)).Result);

                // _dateTime 更新
                this._dateTime = DateTime.Now;

                if (this._jwkSet.keys.Count == 0)
                {
                    Debug.WriteLine("JwkSet was abnormally initarized with an empty state in JwkSetStore constructor.");
                }
                else
                {
                    Debug.WriteLine("JwkSet was initarized normally in JwkSetStore constructor.");
                }
            }
        }

        #endregion

        #region GetInstance

        /// <summary>GetInstance</summary>
        /// <returns>JwkSetStore</returns>
        public static JwkSetStore GetInstance()
        {
            return JwkSetStore._jwkSetStore;
        }

        #endregion

        /// <summary>GetJwkObject</summary>
        /// <param name="kid">string</param>
        /// <returns>JwkObject</returns>
        public JObject GetJwkObject(string kid)
        {
            try
            {
                // リーダーロックを取得
                this._rwLock.AcquireReaderLock(Timeout.Infinite);

                #region 読取

                // jwkを返す。
                return JwkSet.GetJwkObject(this._jwkSet, kid);

                #endregion
            }
            finally
            {
                // リーダーロックを解放
                this._rwLock.ReleaseReaderLock();
            }
        }

        /// <summary>SetJwkSetObjectAsync</summary>
        /// <param name="kid">string</param>
        /// <returns>JwkObject</returns>
        public JObject SetJwkSetObject(
            //string jku, // jku廃止
            string kid)
        {
            //if (jku != OAuth2AndOIDCParams.JwkSetUri)
            //{
            //    // 一致しなかった場合、以下の処理を施しリトライ。
            //    if (jku.EndsWith("/"))
            //    {
            //        jku = jku.Substring(0, jku.Length - 1);
            //    }
            //    else
            //    {
            //        jku = jku + "/";
            //    }

            //    if (jku != OAuth2AndOIDCParams.JwkSetUri)
            //    {
            //        return null; // 上位で証明書利用へ遷移
            //    }
            //}

            try
            {
                // ライターロックを取得
                this._rwLock.AcquireWriterLock(Timeout.Infinite);

                #region 書込
                
                TimeSpan timeSpan = DateTime.Now.Subtract(this._dateTime);

                if (timeSpan.TotalSeconds < OAuth2AndOIDCParams.JwkSetUpdateIntervalInSeconds)
                {
                    // ｘ秒（既定10秒）以内に更新済み ≒ 更新済みと判断。
                }
                else
                {
                    // ｘ秒（既定10秒）以内に更新済みでない
                    // ≒ 鍵変更後、更新済みでないと判断。

                    // JwkSetUri
                    string jwkSetString = 
                        OAuth2AndOIDCClient.GetJwkSetAsync(
                            new Uri(OAuth2AndOIDCParams.JwkSetUri)).Result;

                    if (string.IsNullOrEmpty(jwkSetString))
                    {
                        // jwkSetStringが空文字列
                        Debug.WriteLine("JwkSet was not updated, because jwkSetString is null or empty in JwkSetStore.SetJwkSetObject method.");
                    }
                    else
                    {
                        JwkSet jwkSet = JsonConvert.DeserializeObject<JwkSet>(jwkSetString);

                        // _jwkSet 更新
                        this._jwkSet = jwkSet;
                        // _dateTime 更新
                        this._dateTime = DateTime.Now;

                        Debug.WriteLine("JwkSet was updated normally in JwkSetStore.SetJwkSetObject method.");
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception was catched in JwkSetStore.SetJwkSetObject method: " + ex.ToString());
            }
            finally
            {
                // ライターロックを解放
                this._rwLock.ReleaseWriterLock();
            }

            // JwkSetからJwkを返す。
            return JwkSet.GetJwkObject(this._jwkSet, kid);
        }
    }
}
