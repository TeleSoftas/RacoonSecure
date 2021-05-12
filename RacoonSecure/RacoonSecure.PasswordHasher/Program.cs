using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.PasswordHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = @"D:\Passwords";
            var file = "pwned100k.txt";

            var filePath = Path.Combine(dir, file);
            var outputPath = Path.Combine(dir, "Common");

            var commonPasswords = ReadCommonPasswordsFromSource(filePath).ToList();
            using var fs = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
            foreach (var hash in commonPasswords)
            {
                var hashBytes = CryptoHelper.HexStringToByteArray(hash);
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
        
    }
}