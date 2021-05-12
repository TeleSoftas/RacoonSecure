using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Murmur;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Utilities
{
    public class BloomFilter
    {
        private readonly BitArray _filterBits;
        private readonly int _filterSize;

        private readonly HashAlgorithm _hashAlgorithm;

        /// <summary>
        /// Initializes bloom filter
        /// </summary>
        public BloomFilter()
        {
            _filterSize = 47456108 * 8;
            _filterBits = new BitArray(_filterSize);
            _hashAlgorithm = MurmurHash.Create128();
        }
        
        public void Add(byte[] input)
        {
            var indexes = ComputeIndexes(input);
            foreach (var index in indexes)
                _filterBits[index] = true;
        }
        
        public bool Contains(string input)
        {
            var inputSha = CryptoHelper.ComputeSha1HashBytes(input);
            var indexes = ComputeIndexes(inputSha);
            return indexes.All(index => _filterBits[index]);
        }
        
        public BitArray GetFilterBits()
        {
            return _filterBits;
        }

        private IEnumerable<int> ComputeIndexes(byte[] input)
        {
            var hashBytes = _hashAlgorithm.ComputeHash(input);
            var indexByteEnumerable = Split(hashBytes, 4);

            foreach (var b in indexByteEnumerable)
            {
                var idx = Math.Abs(BitConverter.ToInt32(b, 0) % _filterSize);
                yield return idx;    
            }
        }

        private static IEnumerable<byte[]> Split(IReadOnlyCollection<byte> value, int bufferLength){
            var countOfArrays = value.Count / bufferLength;
            if (value.Count % bufferLength > 0)
            {
                countOfArrays ++;
            }
            
            for(var i=0; i<countOfArrays; i++)
            {
                yield return value.Skip(i * bufferLength).Take(bufferLength).ToArray();
            }
        }
    }
}