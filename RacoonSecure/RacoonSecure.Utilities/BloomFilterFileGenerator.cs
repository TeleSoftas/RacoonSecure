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
        private const int ExpectedElements = 10000000;
        private const float ErrorRate = 0.0001f;
        
        public async Task<byte[]> GenerateFile(string inputPath)
        {
            var bloomFilter = FilterBuilder.Build(ExpectedElements, ErrorRate);
            
            var passwords = ReadFileLines(inputPath);
            var counter = 0;
            await foreach (var password in passwords)
            {
                var delimiterIndex = password.IndexOf(':');
                var hashBytes = HexStringToByteArray(password[..delimiterIndex]);
                
                bloomFilter.Add(hashBytes);
                counter++;

                if (counter % 1000 == 0)
                {
                    Console.WriteLine($"Processed: {counter}");
                    break;
                }
            }

            var expectedElementsBytes = BitConverter.GetBytes(ExpectedElements);
            var errorRateBytes = BitConverter.GetBytes(ErrorRate);
            var contentBytes = bloomFilter.SerializeData();
            
            return expectedElementsBytes.Concat(errorRateBytes).Concat(contentBytes).ToArray();
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