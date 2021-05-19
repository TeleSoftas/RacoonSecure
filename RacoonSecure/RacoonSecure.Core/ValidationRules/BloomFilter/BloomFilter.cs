using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    /// <summary>
    /// Bloom Filter In Memory Implementation
    /// </summary>
    public class BloomFilter : IValidationBloomFilter
    {
        /// <summary>
        /// Bloom filter bits
        /// </summary>
        private readonly BitArray _filterBits;
        
        /// <summary>
        /// <see cref="HashFunction"/>
        /// </summary>
        private Murmur3HashFunction HashFunction { get; }

        /// <summary>
        /// the Capacity of the Bloom filter
        /// </summary>
        private int Capacity { get; }

        /// <summary>
        /// number of hash functions
        /// </summary>
        private int Hashes { get; }

        /// <summary>
        ///  the expected elements.
        /// </summary>
        private int ExpectedElements { get; }

        /// <summary>
        /// the number of expected elements
        /// </summary>
        private double ErrorRate { get; }

        private readonly object _sync = new object();

        /// <summary>
        /// Initializes a new instance of the <see><cref>FilterMemory{T}</cref></see>
        /// class.
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="filterArray">Pre-made filter array to initialize with</param>
        public BloomFilter(int expectedElements, double errorRate, byte[] filterArray)
        {
            if (expectedElements < 1)
                throw new ArgumentOutOfRangeException(nameof(expectedElements), expectedElements, "expectedElements must be > 0");
            if (errorRate >= 1 || errorRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(errorRate), errorRate, $"errorRate must be between 0 and 1, exclusive. Was {errorRate}");

            ExpectedElements = expectedElements;
            ErrorRate = errorRate;
            HashFunction = new Murmur3HashFunction();

            Capacity = BestM(expectedElements, errorRate);
            Hashes = BestK(expectedElements, Capacity);

            
            _filterBits = filterArray != null 
                ? new BitArray(filterArray) 
                : new BitArray(Capacity);
        }

        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Add(byte[] element)
        {
            var added = false;
            var positions = ComputeHash(element);
            lock (_sync)
            {
                foreach (var position in positions)
                {
                    if (!_filterBits.Get(position))
                    {
                        added = true;
                        _filterBits.Set(position, true);
                    }
                }
            }
            return added;
        }

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            lock (_sync)
            {
                if (positions.Any(position => !_filterBits.Get(position)))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Removes all elements from the filter
        /// </summary>
        public void Clear()
        {
            lock (_sync)
            {
                _filterBits.SetAll(false);
            }
        }
        
        public byte[] SerializeData()
        {
            var baBits = new byte[(int)Math.Ceiling(_filterBits.Count / 8m)];
            _filterBits.CopyTo(baBits, 0);
            return baBits;
        }

        /// <summary>
        ///  Hashes the specified value.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerable<int> ComputeHash(byte[] data)
        {
            return HashFunction.ComputeHash(data, Capacity, Hashes);
        }
        
        /// <summary>
        /// Calculates the optimal size of the bloom filter in bits given expectedElements (expected
        /// number of elements in bloom filter) and falsePositiveProbability (tolerable false positive rate).
        /// </summary>
        /// <param name="n">Expected number of elements inserted in the bloom filter</param>
        /// <param name="p">Tolerable false positive rate</param>
        /// <returns>the optimal siz of the bloom filter in bits</returns>
        private static int BestM(long n, double p)
        {
            return (int)Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2));
        }

        /// <summary>
        /// Calculates the optimal hashes (number of hash function) given expectedElements (expected number of
        /// elements in bloom filter) and size (size of bloom filter in bits).
        /// </summary>
        /// <param name="n">Expected number of elements inserted in the bloom filter</param>
        /// <param name="m">The size of the bloom filter in bits.</param>
        /// <returns>the optimal amount of hash functions hashes</returns>
        private static int BestK(long n, long m)
        {
            return (int)Math.Ceiling((Math.Log(2) * m) / n);
        }
        
        public override string ToString()
        {
            return $"Capacity:{Capacity},Hashes:{Hashes},ExpectedElements:{ExpectedElements},ErrorRate:{ErrorRate}";
        }
    }
}