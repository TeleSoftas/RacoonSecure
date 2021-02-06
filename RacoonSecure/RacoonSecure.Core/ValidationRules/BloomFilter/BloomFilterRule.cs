using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterRule : IPasswordValidationRule
    {
        private static readonly MD5 Md5 = MD5.Create();
        private static readonly byte[] BloomFilter = new byte[10000];

        public BloomFilterRule()
        {
            AddArray(PasswordStorage.LoadPasswords());
        }

        public string Validate(string password)
        {
            return PossiblyExists(password) ? ValidationError.CommonPassword : default;
        }


        private static bool PossiblyExists(string item)
        {
            var hash = BloomHash(ComputeBloomHash(item)) & 0x7FFFFFFF;
            var bit = (byte)(1 << (hash & 7)); // you have 8 bits;
            return (BloomFilter[hash % BloomFilter.Length] & bit) != 0;
        }

        private void AddArray(IEnumerable<string> passwords)
        {
            foreach (var t in passwords)
            {
                AddItem(t);
            }
        }
        
        private void AddItem(string password)
        {
            var hash = BloomHash(password) & 0x7FFFFFFF; // strips signed bit
            var bit = (byte)(1 << (hash & 7)); // you have 8 bits
            BloomFilter[hash % BloomFilter.Length] |= bit;
        }
        
        private static int BloomHash(string item)
        {
            var result = 17;
            for (var i = 0; i < item.Length; i++)
            {
                unchecked
                {
                    result *= item[i];
                }
            }
            
            return result;
        }

        private static string ComputeBloomHash(string input)
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = Md5.ComputeHash(inputBytes).Take(10).ToArray();
            
            var chars = new List<char>();
            for (var i = 0; i < 10; i += 2)
            {
                var subrange = hashBytes[i..(i + 2)];
                chars.Add(BitConverter.ToChar(subrange));
            }
            
            return string.Concat(chars);
        }
    }
}
