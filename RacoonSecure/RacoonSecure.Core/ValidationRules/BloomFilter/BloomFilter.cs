using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Murmur;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    public class BloomFilter
    {
        private readonly BitArray _filterBits;
        private readonly int _filterSize;

        private readonly HashAlgorithm[] _hashAlgorithms;

        public BloomFilter()
        {
            _filterSize = 2048;
            _filterBits = new BitArray(_filterSize);
            
            _hashAlgorithms = new HashAlgorithm[]
            {
                MurmurHash.Create128(),
                MD5.Create(),
                SHA256.Create(), 
            };
            
            
            Add("password1");
            Add("password2");
            Add("password3");
        }

        public bool Contains(string input)
        {
            var indexes = ComputeIndexes(input);
            return indexes.All(index => _filterBits[index]);
        }
        
        private void Add(string input)
        {
            var indexes = ComputeIndexes(input);
            foreach (var index in indexes)
            {
                _filterBits[index] = true;
            }
        }

        private IEnumerable<int> ComputeIndexes(string input)
        {
            var data = Encoding.ASCII.GetBytes(input);
            foreach (var algorithm in _hashAlgorithms)
            {
                var bytes = algorithm.ComputeHash(data);
                var idx = Math.Abs(BitConverter.ToInt32(bytes, 0) % _filterSize);
                yield return idx;                
            }
        }
    }
}