using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IdentityDemo.Utils
{
    public static class EncryptionDecryption
    {
        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {

            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 8, 5, 6, 8, 2, 3, 5, 6 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (var AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public static string EncryptText(string input)
        {
            // Get the bytes of the string
            string password = "1=+NameHere!!!!He@PENG";
            byte[] pwdbytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] pwdbytesHash = SHA256.Create().ComputeHash(pwdbytes);

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            byte[] saltBytes = GetRandomBytes();
            byte[] encryptedBytes = new byte[saltBytes.Length + inputBytes.Length];

            // Combine Salt + Text
            for (int i = 0; i < saltBytes.Length; i++)
                encryptedBytes[i] = saltBytes[i];
            for (int i = 0; i < inputBytes.Length; i++)
                encryptedBytes[i + saltBytes.Length] = inputBytes[i];

            encryptedBytes = AES_Encrypt(encryptedBytes, pwdbytesHash);
            string result = Convert.ToBase64String(encryptedBytes);
            result = result.Replace("/", "@");
            return result;
        }
        public static byte[] GetRandomBytes()
        {
            int saltLength = 8;
            var randonNumbern = RandomNumberGenerator.Create();
            byte[] bytes = new byte[saltLength];
            randonNumbern.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// Aes the s_ decrypt.
        /// </summary>
        /// <param name="bytesToBeDecrypted">The bytes to be decrypted.</param>
        /// <param name="passwordBytes">The password bytes.</param>
        /// <returns>System.Byte[].</returns>
        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 8, 5, 6, 8, 2, 3, 5, 6 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (var AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        /// <summary>
        /// Decrypts the text.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string DecryptText(string input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;
            input = input.Replace("%40", "@");// replacing %40 to @ to avoid decryption exception.
            // Get the bytes of the string
            string password = "1=+NameHere!!!!He@PENG";
            input = input.Replace("@", "/");
            byte[] pwdBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            byte[] pwdHashBytes = SHA256.Create().ComputeHash(pwdBytes);

            byte[] inputBytes = Convert.FromBase64String(input);

            byte[] decryptedBytes = AES_Decrypt(inputBytes, pwdHashBytes);

            // Remove salt
            int saltLength = 8;
            byte[] resultBytes = new byte[decryptedBytes.Length - saltLength];
            for (int i = 0; i < resultBytes.Length; i++)
                resultBytes[i] = decryptedBytes[i + saltLength];
            string result = Encoding.UTF8.GetString(resultBytes);
            return result;
        }

        public static string MakeIdHash(string id)
        {
            const string salt = "T31@n@!!Ebay&$12@3";
            byte[] bytes = Encoding.UTF8.GetBytes(salt + id);
            using (var sha = SHA1.Create())
                return string.Concat(sha.ComputeHash(bytes).Select(b => b.ToString("x2"))).Substring(8);
        }

    }

}
