using System.Security.Cryptography;
using System.Text;

namespace RacoonSecure.Core.Cryptography
{
    //TODO: back from public to internal
    public static class CryptoHelper
    {
        private static readonly SHA1Managed Sha1 = new SHA1Managed();

        public static string ComputeSha1Hash(string input)
        {
            var hashBytes = Sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();

            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
        
        public static string ComputeSha1Hash(string input, int length)
        {
            var hashBytes = Sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hashBytes.Length * 2);

            foreach (var b in hashBytes)
            {
                // can be "x2" if you want lowercase
                sb.Append(b.ToString("X2"));

                if (sb.Length == length) break;
            }

            return sb.ToString();
        }
    }
}