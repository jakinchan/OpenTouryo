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
//* クラス名        ：JwtConst
//* クラス日本語名  ：JwtConst
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/30  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JWT系定数</summary>
    public class JwtConst
    {
        /// <summary>type:JWT</summary>
        public const string JWT = "JWT";

        #region JWS

        /// <summary>alg:HS256</summary>
        public const string HS256 = "HS256";

        /// <summary>alg:RS256</summary>
        public const string RS256 = "RS256";

        /// <summary>alg:ES256</summary>
        public const string ES256 = "ES256";

        #endregion

        #region JWE

        #endregion

        #region JWK

        /// <summary>keys</summary>
        public const string keys = "keys";

        /// <summary>kid: Key IDパラメタ</summary>
        public const string kid = "kid";

        /// <summary>alg: algorithmパラメタ</summary>
        public const string alg = "alg";

        /// <summary>kty: key typeパラメタ</summary>
        public const string kty = "kty";

        /// <summary>use : key useパラメタ</summary>
        public const string use = "use";

        #region Keys

        #region Symmetric Keys

        /// <summary>Symmetric Keys: k</summary>
        public const string k = "k";

        #endregion

        #region ASymmetric Keys

        #region RSA Keys

        /// <summary>kty : RSA</summary>
        public const string RSA = "RSA";

        #region RSA Public Keys

        /// <summary>RSA Public Keys: n</summary>
        public const string n = "n";

        /// <summary>RSA Public Keys: e</summary>
        public const string e = "e";

        #endregion

        #region RSA Private Keys

        /// <summary>RSA Public Keys: d</summary>
        public const string d = "d";

        /// <summary>RSA Public Keys: p</summary>
        public const string p = "p";

        /// <summary>RSA Public Keys: q</summary>
        public const string q = "q";

        /// <summary>RSA Public Keys: dp</summary>
        public const string dp = "dp";

        /// <summary>RSA Public Keys: dq </summary>
        public const string dq = "dq";

        /// <summary>RSA Public Keys: qi</summary>
        public const string qi = "qi";

        /// <summary>RSA Public Keys: oth </summary>
        public const string oth = "oth";

        /// <summary>RSA Public Keys: r(oth) </summary>
        public const string oth_r = "r";

        /// <summary>RSA Public Keys: d(oth) </summary>
        public const string oth_d = "d";

        /// <summary>RSA Public Keys: t(oth) </summary>
        public const string oth_t = "t";

        #endregion

        #endregion

        #region ECC  Keys

        /// <summary>kty : EC</summary>
        public const string EC = "EC";

        /// <summary>crv : Curveパラメタ</summary>
        public const string crv = "crv";

        #region ECC Public Keys
        /// <summary>x : X Coordinateパラメタ</summary>
        public const string x = "x";

        /// <summary>y : Y Coordinateパラメタ</summary>
        public const string y = "y";
        #endregion

        #region ECC Private Keys
        ///// <summary>d : ECC Private Keyパラメタ</summary>
        //public const string d = "d";　// RSAと重複
        #endregion

        #endregion

        #endregion

        #endregion

        #endregion
    }
}

