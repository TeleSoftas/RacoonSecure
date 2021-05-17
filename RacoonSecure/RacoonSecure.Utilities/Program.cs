using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RacoonSecure.Core.ValidationRules.BloomFilter;

namespace RacoonSecure.Utilities
{
    static class Program
    {
        
        private const string InputPath = @"D:\Passwords\pwned10M.txt";
        private const string OutputPath = @"D:\Passwords\Filter";
        
        static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            
            //Generate Bloom filter
            sw.Start();
            var filter = await GenerateBloomFilter(InputPath);
            sw.Stop();
            Console.WriteLine($"Executed in: {sw.ElapsedMilliseconds}ms");
            
            //Save bloom filter bits to file
            Console.WriteLine("Exporting BloomFilter bits...");
            SaveBloomFilter(filter, OutputPath);
            Console.WriteLine("Done.");
        }

        private static async Task<Filter<byte[]>> GenerateBloomFilter(string inputPath)
        {
            Console.WriteLine($"Starting GenerateBloomFilter(input: {inputPath}");
            var bloomFilter = FilterBuilder.Build<byte[]>(10000000, 0.0001);
            
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
            
            Console.WriteLine("Finished GenerateBloomFilter");
            return bloomFilter;
        }
        
        private static void SaveBloomFilter(Filter<byte[]> bloomFilter, string outputPath)
        {
            Console.WriteLine("Starting SaveBloomFilter");

            var filterBytes = bloomFilter.SerializeData();
            SaveFile(outputPath, filterBytes);
            
            Console.WriteLine("Finished SaveBloomFilter");
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

        private static void SaveFile(string filePath, byte[] content)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(content, 0, content.Length);
        }

        private static byte[] BitArrayToByteArray(BitArray bits)
        {
            var ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
        
        public static byte[] HexStringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}