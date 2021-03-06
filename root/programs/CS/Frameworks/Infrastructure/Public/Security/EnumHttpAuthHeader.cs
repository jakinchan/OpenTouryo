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
//* クラス名        ：EnumHttpAuthHeader
//* クラス日本語名  ：HTTPリクエスト・ヘッダ「認証・認可」列挙型
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/02/12  西野 大介         新規作成
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>EnumHttpAuthHeader</summary>
    [Flags]
    public enum EnumHttpAuthHeader
    {
        /// <summary>None</summary>
        None = 1,
        /// <summary>Basic</summary>
        Basic = 1 << 1,
        /// <summary>Bearer</summary>
        Bearer = 1 << 2
    }
}
