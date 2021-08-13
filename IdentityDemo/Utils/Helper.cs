using System.Security.Cryptography;
using System.Text;

namespace IdentityDemo.Utils
{
    public static class Helper
    {
        public static string CreateClientSecretKey()
        {
            SymmetricAlgorithm symAlg = SymmetricAlgorithm.Create("IdentityAppRegistrationPOC");

            symAlg.KeySize = 128;

            byte[] key = symAlg.Key;

            StringBuilder sb = new StringBuilder(key.Length * 2);

            foreach (byte b in key)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }
    }
}
