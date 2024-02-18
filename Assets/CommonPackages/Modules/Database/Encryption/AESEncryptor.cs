using System;
using System.Security.Cryptography;
using System.Text;

namespace Modules.Database
{
    public static class AESEncryptor
    {

        /// <summary>
        /// Encrypts a given text string with a password
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <param name="password">The password which will be required to decrypt it</param>
        /// <returns>An encrypted text string.</returns>
        public static string Encrypt(string plainText, string password)
        { 
            byte [] keyBytes = UTF8Encoding.UTF8.GetBytes(password);    
            using(RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = keyBytes;
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;
                ICryptoTransform ict = rm.CreateEncryptor();
                byte [] contentBytes = UTF8Encoding.UTF8.GetBytes(plainText);
                byte [] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                return Convert.ToBase64String(resultBytes, 0, resultBytes.Length);
            }
        }

        /// <summary>
        /// Decrypts an AESEncryptedText with a password
        /// </summary>
        /// <param name="encryptedText">The AESEncryptedText object to decrypt</param>
        /// <param name="password">The password to use when decrypting</param>
        /// <returns>The original plainText string.</returns>
        public static string Decrypt(string encryptedText, string password)
        {
            byte [] keyBytes = UTF8Encoding.UTF8.GetBytes(password);
            using(RijndaelManaged rm = new RijndaelManaged())
            {
                rm.Key = keyBytes;
                rm.Mode = CipherMode.ECB;
                rm.Padding = PaddingMode.PKCS7;
                ICryptoTransform ict = rm.CreateDecryptor();
                byte [] contentBytes = Convert.FromBase64String(encryptedText);
                byte [] resultBytes = ict.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                return UTF8Encoding.UTF8.GetString(resultBytes);
            }
        }
    }
}