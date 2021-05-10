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

        /// <summary>
        /// Initializes bloom filter
        /// </summary>
        /// <param name="filterSize">Filter size in bytes</param>
        public BloomFilter(int filterSize)
        {
            _filterSize = filterSize * 8;
            _filterBits = new BitArray(_filterSize);
            
            _hashAlgorithms = new HashAlgorithm[]
            {
                MurmurHash.Create128(),
                // MD5.Create(),
                // SHA256.Create(), 
            };
        }

        public bool Contains(string input)
        {
            var indexes = ComputeIndexes(input);
            return indexes.All(index => _filterBits[index]);
        }
        
        public void Add(string input)
        {
            var indexes = ComputeIndexes(input);
            foreach (var index in indexes)
            {
                _filterBits[index] = true;
            }
        }
        
        public BitArray GetFilterBits()
        {
            return _filterBits;
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