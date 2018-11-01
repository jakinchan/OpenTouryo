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
//* クラス名        ：AuthEncrypt
//* クラス日本語名  ：認証付き暗号（AEAD）抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/01  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Collections.Generic;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>AeadResult</summary>
    public class AeadResult
    {
        /// <summary>暗号文</summary>
        public byte[] Ciphert = null;
        /// <summary>認証タグ（MAC）</summary>
        public byte[] Tag = null;

        /// <summary>暗号文と認証タグ（MAC）を結合</summary>
        /// <returns>ciphert + tag</returns>
        public byte[] CombineByteArrayForDecrypt()
        {
            return PubCmnFunction.CombineByteArray(this.Ciphert, this.Tag);
        }
    }

    /// <summary>
    /// 認証付き暗号（AEAD）抽象クラス
    /// </summary>
    public abstract class AuthEncrypt
    {
    }
}