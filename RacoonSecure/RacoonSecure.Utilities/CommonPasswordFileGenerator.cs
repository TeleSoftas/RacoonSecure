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
        public async Task<List<byte[]>> Generate(string inputFileName)
        {
            var result = new List<byte[]>();
            var commonPasswords = ReadCommonPasswordsFromSource(inputFileName).ToList();
            foreach (var hashBytes in commonPasswords.Select(HexStringToByteArray))
            {
                result.Add(hashBytes);
            }

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
                .Take(8)
                .ToArray();
        }
    }
}