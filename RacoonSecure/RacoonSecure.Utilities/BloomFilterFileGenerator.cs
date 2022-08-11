using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator.ValidationRules.BloomFilter;

namespace RacoonSecure.Utilities
{
    public class BloomFilterFileGenerator
    {
        public async Task<BloomFilter> Generate(string inputPath)
        {
            var bloomFilter = FilterBuilder.Build(10000000, 0.0001);
            
            var passwords = ReadFileLines(inputPath);
            var counter = 0;
            await foreach (var password in passwords)
            {
                var delimiterIndex = password.IndexOf(':');
                var hashBytes = HexStringToByteArray(password[..delimiterIndex]);
                
                bloomFilter.Add(hashBytes);
                counter++;
                
                if(counter % 100 == 0)
                    Console.WriteLine($"Processed: {counter}");
            }
            
            return bloomFilter;
        }
        
        private static async IAsyncEnumerable<string> ReadFileLines(string filePath)
        {
            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(filePath, Encoding.UTF8);

            string line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }

        private static byte[] HexStringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}