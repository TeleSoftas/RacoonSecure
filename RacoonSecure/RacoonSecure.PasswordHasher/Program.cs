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
            var dir = @"";
            var file = "";

            var filePath = Path.Combine(dir, file);
            var outputPath = Path.Combine(dir, "output.txt");
            
            using var fs = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using var sw = new StreamWriter(fs, Encoding.ASCII);
            
            var counter = 0;
            foreach (var hash in ReadFileLines(filePath))
            {
                var shortHash = hash.Substring(0, 10);
                sw.Write(shortHash);
                counter++;
                Console.WriteLine($"{counter} passwords processed ({hash} - {shortHash})");
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
    }
}