namespace Modules.Database
{
    using System.Security.Cryptography;
    using System.Text;
    public class MD5Encryptor
    {
        public static string Encrypt(string raw)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte [] array = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array [i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static bool IsMD5Hash(string raw, string toVerify)
        {
            return toVerify == Encrypt(raw);
        }
    }
}