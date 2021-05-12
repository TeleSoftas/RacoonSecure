using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RacoonSecure.Core.ValidationRules.BloomFilter;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Utilities
{
    static class Program
    {
        
        private const string InputPath = "";
        private const string OutputPath = "";
        
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
            await SaveBloomFilter(filter, OutputPath);
            Console.WriteLine("Done.");
        }

        private static async Task<BloomFilter> GenerateBloomFilter(string inputPath)
        {
            Console.WriteLine($"Starting GenerateBloomFilter(input: {inputPath}");
            var bloomFilter = new BloomFilter();
            var passwords = ReadFileLines(inputPath);

            var counter = 0;
            await foreach (var password in passwords)
            {
                var delimiterIndex = password.IndexOf(':');
                var hashBytes = CryptoHelper.HexStringToByteArray(password[..delimiterIndex]);
                
                bloomFilter.Add(hashBytes);
                counter++;
                
                if(counter % 100 == 0)
                    Console.WriteLine($"Processed: {counter}");
            }
            
            Console.WriteLine("Finished GenerateBloomFilter");
            return bloomFilter;
        }
        
        private static async Task SaveBloomFilter(BloomFilter bloomFilter, string outputPath)
        {
            Console.WriteLine("Starting SaveBloomFilter");
            
            var filterBytes = BitArrayToByteArray(bloomFilter.GetFilterBits());
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
    }
}