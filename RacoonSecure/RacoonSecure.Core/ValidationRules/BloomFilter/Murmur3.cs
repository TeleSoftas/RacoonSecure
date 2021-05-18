﻿using System;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    /// <summary>
    /// Building a Better Bloom Filter" by Adam Kirsch and Michael Mitzenmacher,
    /// https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
    /// </summary>
    public class Murmur3 : HashFunction
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            var positions = new int[k];
            uint seed = 0;
            var hashes = 0;
            while (hashes < k)
            {
                seed = MurmurHash3_32(seed, data, 0, data.Length);
                var hash = Rejection(seed, m);
                if (hash != -1)
                {
                    positions[hashes++] = hash;
                }
            }
            return positions;
        }

        private static uint MurmurHash3_32(uint seed, byte[] data, int offset, int count)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            var blocks = count / 4;
            var hash = seed;

            uint k1;
            // body
            for (var i = 0; i < blocks; ++i)
            {
                k1 = BitConverter.ToUInt32(data, offset + i * 4);

                k1 *= c1;
                k1 = RotateLeft(k1, 15);
                k1 *= c2;

                hash ^= k1;
                hash = RotateLeft(hash, 13);
                hash = hash * 5 + 0xe6546b64;
            }

            // tail
            k1 = 0;
            var tailIdx = offset + blocks * 4;
            switch (count & 3)
            {
                case 3:
                    k1 ^= (uint)(data[tailIdx + 2]) << 16;
                    goto case 2;
                case 2:
                    k1 ^= (uint)(data[tailIdx + 1]) << 8;
                    goto case 1;
                case 1:
                    k1 ^= data[tailIdx + 0];
                    k1 *= c1; k1 = RotateLeft(k1, 15);
                    k1 *= c2;
                    hash ^= k1;
                    break;
            }

            // finalization
            hash ^= (uint)count;

            hash ^= hash >> 16;
            hash *= 0x85ebca6b;
            hash ^= hash >> 13;
            hash *= 0xc2b2ae35;
            hash ^= hash >> 16;

            return hash;
        }
    }
}