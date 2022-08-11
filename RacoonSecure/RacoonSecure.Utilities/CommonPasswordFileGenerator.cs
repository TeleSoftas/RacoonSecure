using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacoonSecure.Utilities
{
    public class CommonPasswordFileGenerator
    {
        private const short HashLength = 8;

        public async Task<byte[]> GenerateFileBytes(string inputFileName)
        {
            var commonPasswords = ReadCommonPasswordsFromSource(inputFileName).ToList();
            var passwordBytes = commonPasswords.Select(HexStringToByteArray).ToList();

            var headerBytes = BitConverter.GetBytes(HashLength);
            var contentBytes = passwordBytes.SelectMany(x => x).ToArray();
            
            var result = headerBytes.Concat(contentBytes).ToArray();
            return result;
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
        
        private static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .Take(HashLength)
                .ToArray();
        }
    }
}