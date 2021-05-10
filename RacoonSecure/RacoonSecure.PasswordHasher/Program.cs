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
            var file = "CommonMinor.txt";

            var filePath = Path.Combine(dir, file);
            var outputPath = Path.Combine(dir, "CommonMinor");

            var commonPasswords = ReadCommonPasswords(filePath).ToList();
            
            
            
            var fsCreate = File.Create(outputPath);  
            fsCreate.Close();
            
            using var fs = new FileStream(outputPath, FileMode.Append, FileAccess.Write);
            // using var sw = new StreamWriter(fs, Encoding.ASCII);
            var bw = new BinaryWriter(fs);
            
            foreach (var hash in commonPasswords)
            {
                bw.Write(hash, 0, hash.Length);
            }

            Console.WriteLine("Passwords successfully hashed, exiting.");
        }

        private static IEnumerable<string> ReadFileLines(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(filePath, Encoding.UTF8);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }
        
        private static IEnumerable<byte[]> ReadCommonPasswords(string filePath)
        {
            var commonPasswordsBytes = File.ReadAllBytes(filePath);
            var ms = new MemoryStream(commonPasswordsBytes);
            
            var buffer = new byte[10];
            var commonPasswords = new List<byte[]>();
            while (ms.Read(buffer, 0, buffer.Length) != 0)
            {
                commonPasswords.Add(buffer);        
            }

            return commonPasswords;
        }
    }
}