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
            var dir = @"D:\Passwords";
            var file = "Common2.txt";

            var filePath = Path.Combine(dir, file);
            var outputPath = Path.Combine(dir, "CommonMinor");

            var commonPasswords = ReadCommonPasswords(filePath).ToList();
            using var fs = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write);
            foreach (var hash in commonPasswords)
            {
                var hashBytes = StringToByteArray(hash);
                fs.Write(hashBytes, 0, hashBytes.Length);
            }

            Console.WriteLine("Passwords successfully hashed, exiting.");
        }
        
        private static IEnumerable<string> ReadCommonPasswords(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(filePath, Encoding.UTF8);

            var buff = new char[10];
            while (sr.Read(buff, 0, buff.Length) != 0)
            {
                yield return new string(buff);
            }
        }

        private static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}