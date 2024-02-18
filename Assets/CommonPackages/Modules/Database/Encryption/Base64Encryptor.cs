namespace Modules.Database
{
    using System.Security.Cryptography;
    using System.Text;
    using System;
    public class Base64Encryptor
    {
        public static string Encrypt(string raw)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        }

        public static string Descrypt(string raw)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(raw));
        }
    }
}