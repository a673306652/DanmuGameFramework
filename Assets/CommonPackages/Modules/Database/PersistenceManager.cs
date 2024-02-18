namespace Modules.Database
{
    using System.IO;
    using Modules.Patterns;
    using SaveSystem;

    public class PersistenceManager : Singleton<PersistenceManager>
    {
        public void SetPrefs(string key, string val)
        {
            EasySave.Save(MD5Encryptor.Encrypt(key), AESEncryptor.Encrypt(val, DatabaseKeys.EncryptionKS));
        }

        public string GetPrefs(string key)
        {
            var result = EasySave.Load<string>(MD5Encryptor.Encrypt(key));
            if (null == result)
            {
                return null;
            }
            return AESEncryptor.Decrypt(result, DatabaseKeys.EncryptionKS);
        }

        public void DeletePrefs(string key)
        {
            EasySave.Delete<string>(MD5Encryptor.Encrypt(key));
        }

        public void SetFile(string key, string val)
        {
            FileSave fs = new FileSave(FileFormat.Binary);
            fs.WriteToFile("Files/" + MD5Encryptor.Encrypt(key) + ".save", AESEncryptor.Encrypt(val, DatabaseKeys.EncryptionKS));
        }

        public string GetFile(string key)
        {
            FileSave fs = new FileSave(FileFormat.Binary);
            var result = fs.ReadFromFile<string>("Files/" + MD5Encryptor.Encrypt(key) + ".save");
            if (null == result)
            {
                return null;
            }
            return AESEncryptor.Decrypt(result, DatabaseKeys.EncryptionKS);
        }

        public void DeleteFile(string key)
        {
            File.Delete("Files/" + MD5Encryptor.Encrypt(key) + ".save");
        }
    }
}