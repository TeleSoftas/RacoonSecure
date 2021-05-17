using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RacoonSecure.PasswordHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = "";
            var file = "";

            var filePath = Path.Combine(dir, file);
            var outputPath = Path.Combine(dir, "");

            var commonPasswords = ReadCommonPasswordsFromSource(filePath).ToList();
            using var fs = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
            foreach (var hash in commonPasswords)
            {
                var hashBytes = HexStringToByteArray(hash);
                fs.Write(hashBytes, 0, 8);
            }

            Console.WriteLine("Passwords successfully hashed, exiting.");
        }
        
        private static IEnumerable<string> ReadCommonPasswordsFromSource(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(filePath, Encoding.UTF8);

            var pass = string.Empty;
            while ((pass = sr.ReadLine()) != null)
            {
                yield return new string(pass.Split(':').First());
            }
        }
        
        public static byte[] HexStringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}