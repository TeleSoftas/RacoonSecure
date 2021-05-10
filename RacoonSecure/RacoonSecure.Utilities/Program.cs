using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RacoonSecure.Core.ValidationRules.BloomFilter;

namespace RacoonSecure.Utilities
{
    static class Program
    {
        private static readonly BloomFilter BloomFilter = new(10485760);

        private const string InputPath = @"D:\Passwords\pwned-passwords-sha1-ordered-by-hash-v7.txt";
        private const string OutputPath = @"D:\Passwords\filter";
        
        static async Task Main(string[] args)
        {
            var sw = new Stopwatch();
            
            sw.Start();
            await GenerateBloomFilter(InputPath, OutputPath);
            sw.Stop();
            
            Console.WriteLine($"Executed in: {sw.ElapsedMilliseconds}ms");
        }

        private static async Task GenerateBloomFilter(string inputPath, string outputPath)
        {
            Console.WriteLine($"Starting GenerateBloomFilter(input: {inputPath}, output: {outputPath})");
            var passwords = ReadFileLines(inputPath);

            var counter = 0;
            await foreach (var password in passwords)
            {
                var delimiterIndex = password.IndexOf(':');
                BloomFilter.Add(password[..delimiterIndex]);
                counter++;
                
                if(counter % 100 == 0)
                    Console.WriteLine($"Processed: {counter}");
            }
            
            var filterBytes = BitArrayToByteArray(BloomFilter.GetFilterBits());
            SaveFile(outputPath, filterBytes);
            
            Console.WriteLine("Finished GenerateBloomFilter");
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