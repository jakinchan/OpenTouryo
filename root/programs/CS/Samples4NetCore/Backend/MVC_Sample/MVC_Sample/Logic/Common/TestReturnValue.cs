﻿//**********************************************************************************
//* フレームワーク・テストクラス（引数・戻り値）
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：TestReturnValue
//* クラス日本語名  ：テスト用の戻り値クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using Touryo.Infrastructure.Business.Common;

namespace MVC_Sample.Logic.Common
{
    public class TestReturnValue : MyReturnValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>テスト用エリア</summary>
        public object Obj2;
    }
}