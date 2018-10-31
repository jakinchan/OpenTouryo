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
//* クラス名        ：DigitalSignX509
//* クラス日本語名  ：DigitalSignX509クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2018/10/31  西野 大介         アルゴリズム判定コードの見直し。
//*  2018/10/31  西野 大介         AsymmetricSignatureFormatter, Deformatterに揃えた。
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// DigitalSignX509クラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class DigitalSignX509 : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        /// <summary>
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </summary>
        string _hashAlgorithmName = "";

        /// <summary>X.509証明書</summary>
        public X509Certificate2 X509Certificate { get; protected set; }

        /// <summary>X.509証明書の秘密鍵</summary>
        public string X509PrivateKey
        {
            get
            {
                return "- hidden -";
            }
        }

        /// <summary>X.509証明書の公開鍵</summary>
        public string X509PublicKey
        {
            get
            {
                return CustomEncode.ToBase64String(this.X509Certificate.GetPublicKey());
            }
        }

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </param>
        public DigitalSignX509(string certificateFilePath, string password, string hashAlgorithmName) :
            this(certificateFilePath, password, hashAlgorithmName, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public DigitalSignX509(string certificateFilePath, string password, string hashAlgorithmName, X509KeyStorageFlags flag)
        {
            this.X509Certificate = new X509Certificate2(certificateFilePath, password, flag);

            this._hashAlgorithmName = hashAlgorithmName;
        }

        #endregion

        #region デジタル署名(X509Certificate)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            // アルゴリズム（RSA or DSA）は、ココで決まるもよう。
            AsymmetricAlgorithm aa = this.X509Certificate.PrivateKey;

            // ハッシュ
            byte[] hashedByte = RsaAndDsaCmnFunc.GetHashAlgorithmFromName(this._hashAlgorithmName).ComputeHash(data);
            // デジタル署名
            byte[] signedByte = null;

            if (aa is RSACryptoServiceProvider)
            {
                /*// RSACryptoServiceProvider : ExportParametersして生成し直している。
                RSAParameters rsaParam = ((RSACryptoServiceProvider)(aa)).ExportParameters(true);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(this.X509Certificate.PrivateKey.KeySize);
                rsa.ImportParameters(rsaParam);*/
                //signedByte = ((RSACryptoServiceProvider)aa).SignData(data, this._hashAlgorithmName);

                // RSAPKCS1SignatureFormatterオブジェクトを作成
                RSAPKCS1SignatureFormatter rsaSignatureFormatter = new RSAPKCS1SignatureFormatter(aa);
                rsaSignatureFormatter.SetHashAlgorithm(this._hashAlgorithmName);
                signedByte = rsaSignatureFormatter.CreateSignature(hashedByte);
            }
            else if (aa is DSACryptoServiceProvider)
            {
                /*// DSACryptoServiceProvider : ExportParametersして生成し直している。
                DSAParameters dsaParam = ((DSACryptoServiceProvider)(aa)).ExportParameters(true);
                DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(this.X509Certificate.PrivateKey.KeySize);
                dsa.ImportParameters(dsaParam);*/
                //signedByte = ((DSACryptoServiceProvider)aa).SignData(data); // SHA-1 固定のため？

                // DSASignatureFormatterオブジェクトを作成
                DSASignatureFormatter dsaSignatureFormatter = new DSASignatureFormatter(aa);
                dsaSignatureFormatter.SetHashAlgorithm(CryptoConst.SHA1); // DSAはSHA1固定
                signedByte = dsaSignatureFormatter.CreateSignature(hashedByte);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }

            return signedByte;
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            AsymmetricAlgorithm aa = null;

            // 検証結果フラグ ( true:検証成功, false:検証失敗 )
            bool flg = false;

            if (this.X509Certificate.PrivateKey == null)
            {
                // *.cer
                // アルゴリズム（RSA or DSA）は、ココで決まるもよう。
                aa = this.X509Certificate.PublicKey.Key;
            }
            else
            {
                // *.pfx
                // アルゴリズム（RSA or DSA）は、ココで決まるもよう。
                aa = this.X509Certificate.PrivateKey;
            }

            if (aa is RSACryptoServiceProvider)
            {
                /*// RSACryptoServiceProvider : ExportParametersして生成し直している。
                RSAParameters rsaParam = ((RSACryptoServiceProvider)(aa)).ExportParameters(true); // 検証ならfalseでイイ
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(this.X509Certificate.PrivateKey.KeySize);
                rsa.ImportParameters(rsaParam);*/
                //flg = ((RSACryptoServiceProvider)aa).VerifyData(data, this._hashAlgorithmName, sign);

                // RSAPKCS1SignatureDeformatterオブジェクトを作成
                RSAPKCS1SignatureDeformatter rsaSignatureDeformatter = new RSAPKCS1SignatureDeformatter(aa);
                rsaSignatureDeformatter.SetHashAlgorithm(this._hashAlgorithmName);
                flg = rsaSignatureDeformatter.VerifySignature(data, sign);
            }
            else if (aa is DSACryptoServiceProvider)
            {
                /*// DSACryptoServiceProvider : ExportParametersして生成し直している。
                DSAParameters dsaParam = ((DSACryptoServiceProvider)(aa)).ExportParameters(true); // 検証ならfalseでイイ
                DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(this.X509Certificate.PrivateKey.KeySize);
                dsa.ImportParameters(dsaParam);*/
                //flg = ((DSACryptoServiceProvider)aa).VerifyData(data, sign); // SHA-1 固定のため？

                // DSASignatureDeformatterオブジェクトを作成
                DSASignatureDeformatter dsaSignatureDeformatter = new DSASignatureDeformatter(aa);
                dsaSignatureDeformatter.SetHashAlgorithm(CryptoConst.SHA1);
                flg = dsaSignatureDeformatter.VerifySignature(data, sign);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);

                // Signで使ってないRSACngをVerifyで実装する意味がない。
                // こちらは、HashAlgorithmName, RSASignaturePadding が指定できるが、
                // 今のところ、AsymmetricSignatureFormatter, Deformatterで問題ない。
                /*
#if !NET45
                else if (aa is RSACng)
                {
                    HashAlgorithmName hashAlgorithmName = HashAlgorithmName.MD5;
                    if (this._hashAlgorithmName == CryptoConst.MD5)
                    {
                        hashAlgorithmName = HashAlgorithmName.MD5;
                    }
                    else if (this._hashAlgorithmName == CryptoConst.SHA1)
                    {
                        hashAlgorithmName = HashAlgorithmName.SHA1;
                    }
                    else if (this._hashAlgorithmName == CryptoConst.SHA256)
                    {
                        hashAlgorithmName = HashAlgorithmName.SHA256;
                    }
                    else if (this._hashAlgorithmName == CryptoConst.SHA384)
                    {
                        hashAlgorithmName = HashAlgorithmName.SHA384;
                    }
                    else if (this._hashAlgorithmName == CryptoConst.SHA512)
                    {
                        hashAlgorithmName = HashAlgorithmName.SHA512;
                    }

                    // 
                    return ((RSACng)aa).VerifyData(data, sign, hashAlgorithmName, RSASignaturePadding.Pkcs1);
                }
#endif
                 */
            }

            return flg;
        }

#endregion

        // こちらは、MyDispose (派生の末端を呼ぶ) の実装は不要。
    }
}