using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RacoonSecure.Utilities
{
    static class Program
    {
        private const string BloomInputPath = "";
        private const string CommonInputPath = "";
        private const string BloomFileOutputPath = "";
        private const string CommonFileOutputPath = "";
        
        static async Task Main(string[] args)
        {
            var option = Menu();
            switch (option)
            {
                case 1: await GenerateBloomFilter(BloomInputPath, BloomFileOutputPath); break;
                case 2: await GenerateCommonPasswordsFile(CommonInputPath, CommonFileOutputPath); break;
                case 0: return;
            }
        }

        private static async Task GenerateBloomFilter(string inputFilePath, string outputFilePath)
        {
            var generator = new BloomFilterFileGenerator();
            var filter = await generator.Generate(inputFilePath);
            
            var filterBytes = filter.SerializeData();
            SaveFile(outputFilePath, filterBytes);
        }

        private static async Task GenerateCommonPasswordsFile(string inputFilePath, string outputFilePath)
        {
            var generator = new CommonPasswordFileGenerator();
            var commonPasswords = await generator.Generate(inputFilePath);

            SaveFile(outputFilePath, commonPasswords.SelectMany(x => x).ToArray());
        }

        private static void SaveFile(string filePath, byte[] content)
        {
            using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(content, 0, content.Length);
        }

        private static int Menu()
        {
            int intVal;
            string option;
            
            do
            {
                Console.Clear();
                Console.WriteLine("Available actions:");
                Console.WriteLine("1. Generate bloom filter");
                Console.WriteLine("2. Generate common passwords file"); 
                Console.WriteLine("0. Exit"); 
                option = Console.ReadLine();
            } while (!int.TryParse(option, out intVal));

            return intVal;
        }
    }
}