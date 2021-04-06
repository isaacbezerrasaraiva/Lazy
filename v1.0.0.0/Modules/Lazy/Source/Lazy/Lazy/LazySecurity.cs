// LazySecurity.cs
//
// This file is integrated part of Lazy project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, November 30

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Security.Cryptography;

namespace Lazy
{
    public static class LazySecurity
    {
        public static class Hash
        {
            public static class SHA256
            {
                #region Variables
                #endregion Variables

                #region Methods

                /// <summary>
                /// Generate and hashed string based on SHA 256 algorithm
                /// </summary>
                /// <param name="data">The string to be hashed</param>
                /// <returns>The hashed string</returns>
                public static String Generate(String data)
                {
                    HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
                    Byte[] buffer = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
                    hashAlgorithm.Dispose();
                    hashAlgorithm = null;

                    return Convert.ToBase64String(buffer);
                }

                #endregion Methods

                #region Properties
                #endregion Properties
            }

            public static class SHA384
            {
                #region Variables
                #endregion Variables

                #region Methods

                /// <summary>
                /// Generate and hashed string based on SHA 384 algorithm
                /// </summary>
                /// <param name="data">The string to be hashed</param>
                /// <returns>The hashed string</returns>
                public static String Generate(String data)
                {
                    HashAlgorithm hashAlgorithm = new SHA384CryptoServiceProvider();
                    Byte[] buffer = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
                    hashAlgorithm.Dispose();
                    hashAlgorithm = null;

                    return Convert.ToBase64String(buffer);
                }

                #endregion Methods

                #region Properties
                #endregion Properties
            }

            public static class SHA512
            {
                #region Variables
                #endregion Variables

                #region Methods

                /// <summary>
                /// Generate and hashed string based on SHA 512 algorithm
                /// </summary>
                /// <param name="data">The string to be hashed</param>
                /// <returns>The hashed string</returns>
                public static String Generate(String data)
                {
                    HashAlgorithm hashAlgorithm = new SHA512CryptoServiceProvider();
                    Byte[] buffer = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
                    hashAlgorithm.Dispose();
                    hashAlgorithm = null;

                    return Convert.ToBase64String(buffer);
                }

                #endregion Methods

                #region Properties
                #endregion Properties
            }
        }

        public static class Cryptography
        {
            public static class Aes
            {
                #region Variables
                #endregion Variables

                #region Methods

                /// <summary>
                /// Encrypt a message
                /// </summary>
                /// <param name="messageToEncrypt">The message to be encrypted</param>
                /// <param name="secretKey">The 32 characters long secret key to be used on the encryption</param>
                /// <param name="secretVector">The 16 characters long secret vector to be used on the encryption</param>
                /// <returns>The encrypted message</returns>
                public static String Encrypt(String messageToEncrypt, String secretKey, String secretVector)
                {
                    String messageEncrypted = null;

                    using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
                    {
                        aes.Key = System.Text.Encoding.UTF8.GetBytes(secretKey);
                        aes.IV = System.Text.Encoding.UTF8.GetBytes(secretVector);

                        ICryptoTransform iCryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, iCryptoTransform, CryptoStreamMode.Write))
                            {
                                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                                {
                                    streamWriter.Write(messageToEncrypt);
                                }

                                messageEncrypted = Convert.ToBase64String(memoryStream.ToArray());
                            }
                        }
                    }

                    return messageEncrypted;
                }

                /// <summary>
                /// Decrypt a message
                /// </summary>
                /// <param name="messageToDecrypt">The message to be decrypted</param>
                /// <param name="secretKey">The 32 characters long secret key to be used on the encryption</param>
                /// <param name="secretVector">The 16 characters long secret vector to be used on the encryption</param>
                /// <returns>The decrypted message</returns>
                public static String Decrypt(String messageToDecrypt, String secretKey, String secretVector)
                {
                    String messageDecrypted = null;

                    using (System.Security.Cryptography.Aes aes = System.Security.Cryptography.Aes.Create())
                    {
                        aes.Key = System.Text.Encoding.UTF8.GetBytes(secretKey);
                        aes.IV = System.Text.Encoding.UTF8.GetBytes(secretVector);

                        ICryptoTransform iCryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(messageToDecrypt)))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, iCryptoTransform, CryptoStreamMode.Read))
                            {
                                using (StreamReader streamReader = new StreamReader(cryptoStream))
                                {
                                    messageDecrypted = streamReader.ReadToEnd();
                                }
                            }
                        }
                    }

                    return messageDecrypted;
                }

                #endregion Methods

                #region Properties
                #endregion Properties
            }
        }
    }
}