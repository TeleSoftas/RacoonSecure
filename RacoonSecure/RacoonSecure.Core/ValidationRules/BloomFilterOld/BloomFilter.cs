using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Murmur;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    public class BloomFilter
    {
        private BitArray _filterBits;
        private readonly int _filterSize;
        
        private readonly HashAlgorithm _hashAlgorithm;
        
        /// <summary>
        /// Initializes bloom filter
        /// </summary>
        /// <param name="filterSize">Filter size in bytes</param>
        public BloomFilter()
        {
            _filterSize = 47456108 * 8;
            _filterBits = new BitArray(ReadBloomFilterFromResource());
            _hashAlgorithm = MurmurHash.Create128();
        }
        
        public bool Contains(string input)
        {
            var inputSha = CryptoHelper.ComputeSha1HashBytes(input);
            var indexes = ComputeIndexes(inputSha);
            return indexes.All(index => _filterBits[index]);
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
                countOfArrays++;
            }
            
            for(var i=0; i < countOfArrays; i++)
            {
                yield return value.Skip(i * bufferLength).Take(bufferLength).ToArray();
            }
        }
        
        private byte[] ReadBloomFilterFromResource()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.BloomFilter.Filter");
            if(stream == null)
                throw new NullReferenceException("No bloom filter resource located");

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}