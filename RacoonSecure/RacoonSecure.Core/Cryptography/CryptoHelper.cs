using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RacoonSecure.Core.Cryptography
{
    public static class CryptoHelper
    {
        private static readonly SHA1Managed Sha1 = new SHA1Managed();

        public static string ComputeSha1Hash(string input)
        {
            var hashBytes = ComputeSha1HashBytes(input);
            var sb = new StringBuilder();

            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
        
        public static byte[] ComputeSha1HashBytes(string input, int length)
        {
            var hashBytes = ComputeSha1HashBytes(input);
            return hashBytes[..length];
        }
        
        public static byte[] ComputeSha1HashBytes(string input) => Sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
    }
}