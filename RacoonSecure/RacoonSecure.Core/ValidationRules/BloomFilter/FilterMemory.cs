﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloomFilter
{
    /// <summary>
    /// Bloom Filter In Mempory Implement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="BloomFilter.Filter{T}" />
    public class FilterMemory<T> : Filter<T>
    {
        private readonly BitArray _hashBits;

        public override byte[] SerializeData()
        {
            var baBits = new byte[(int)Math.Ceiling(_hashBits.Count / 8m)];
            _hashBits.CopyTo(baBits, 0);
            return baBits;
        }
        
        public override byte[] SerializeDataAlt()
        {
            var result = new List<byte>(_hashBits.Count);
            foreach (var b in _hashBits)
                result.Add((byte)(((bool)b) ? 49 : 48));
            return result.ToArray();
        }

        private readonly object sync = new object();

        private readonly static Task Empty = Task.FromResult(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <param name="filterArray"></param>
        public FilterMemory(int expectedElements, double errorRate, HashFunction hashFunction, byte[] filterArray)
            : base(expectedElements, errorRate, hashFunction)
        {
            _hashBits = filterArray != null 
                ? new BitArray(filterArray) 
                : new BitArray(Capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterMemory{T}"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="hashes">The hashes.</param>
        /// <param name="hashFunction">The hash function.</param>
        public FilterMemory(int size, int hashes, HashFunction hashFunction)
            : base(size, hashes, hashFunction)
        {
            _hashBits = new BitArray(Capacity);
        }

        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Add(byte[] element)
        {
            bool added = false;
            var positions = ComputeHash(element);
            lock (sync)
            {
                foreach (int position in positions)
                {
                    if (!_hashBits.Get(position))
                    {
                        added = true;
                        _hashBits.Set(position, true);
                    }
                }
            }
            return added;
        }

        /// <summary>
        /// Adds the passed value to the filter.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override Task<bool> AddAsync(byte[] element)
        {
            return Task.FromResult(Add(element));
        }

        /// <summary>
        /// Tests whether an element is present in the filter
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public override bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            lock (sync)
            {
                foreach (int position in positions)
                {
                    if (!_hashBits.Get(position))
                        return false;
                }
            }
            return true;
        }

        public override Task<bool> ContainsAsync(byte[] element)
        {
            return Task.FromResult(Contains(element));
        }

        /// <summary>
        /// Removes all elements from the filter
        /// </summary>
        public override void Clear()
        {
            lock (sync)
            {
                _hashBits.SetAll(false);
            }
        }

        public override Task ClearAsync()
        {
            Clear();
            return Empty;
        }

        public override void Dispose()
        {

        }
    }
}