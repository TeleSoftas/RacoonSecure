using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RacoonSecure.Core.ValidationRules.XorFilter
{
    public class Xor16
    {
        private readonly ushort[] _fingerprints;
        public readonly ulong Seed;
        private readonly int _blockLength;
        private readonly int _blockLength2;
        private const int NumHashes = 3;
        private const ulong C1 = 0xff51afd7ed558ccd;
        private const ulong C2 = 0xc4ceb9fe1a85ec53;
        private const double Factor = 1.23;
        private const int SecondThird = 21;
        private const int FinalThird = 42;
        public Xor16(ushort[] fingerprints, ulong seed)
        {
            _fingerprints = fingerprints;
            Seed = seed;
            int capacity = fingerprints.Length;
            _blockLength = capacity / NumHashes;
            _blockLength2 = 2 * _blockLength;
        }
        public ushort[] Data => _fingerprints;
        private static int FloorMultipleThree(int n) => n / 3 * 3;
        private static int GetCapacity(int numIntegers) => FloorMultipleThree((int)(Factor * numIntegers) + 32);
        // Algorithm 1: Membership test
        public bool Contains(ulong key)
        {
            ulong hash = MurmurFinalizer(key + Seed);
            var f = (ushort)(hash ^ (hash >> 32));
            ulong r0 = hash;
            ulong r1 = RotateLeft(hash, SecondThird);
            ulong r2 = RotateLeft(hash, FinalThird);
            int h0 = FastModulo((uint)r0, _blockLength);
            int h1 = FastModulo((uint)r1, _blockLength) + _blockLength;
            int h2 = FastModulo((uint)r2, _blockLength) + _blockLength2;
            return f == (_fingerprints[h0] ^ _fingerprints[h1] ^ _fingerprints[h2]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong SplitMix64(ulong x)
        {
            x += 0x9e3779b97f4a7c15;
            x = (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9;
            x = (x ^ (x >> 27)) * 0x94d049bb133111eb;
            return x ^ (x >> 31);
        }
    private static (ulong hash, int h0, int h1, int h2) GetHashes(ulong keySeed, int blockLength)
        {
            ulong hash = MurmurFinalizer(keySeed);
            ulong r0 = hash;
            ulong r1 = RotateLeft(hash, SecondThird);
            ulong r2 = RotateLeft(hash, FinalThird);
            int h0 = FastModulo((uint)r0, blockLength);
            int h1 = FastModulo((uint)r1, blockLength);
            int h2 = FastModulo((uint)r2, blockLength);
            return (hash, h0, h1, h2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHash0(ulong hash, int blockLength) =>
            FastModulo((uint)hash, blockLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHash1(ulong hash, int blockLength) =>
            FastModulo((uint)RotateLeft(hash, SecondThird), blockLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHash2(ulong hash, int blockLength) =>
            FastModulo((uint)RotateLeft(hash, FinalThird), blockLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong value, int count) => (value << count) | (value >> (64 - count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FastModulo(uint hash, int max) => (int)((hash * (ulong)max) >> 32);
        // Algorithm 2: Construction
        public static Xor16 Construction(ulong[] keys)
        {
            int capacity = GetCapacity(keys.Length);
            int blockLength = capacity / NumHashes;
            ulong seed = 1;
            Stack<(ulong Key, int Index)> sigma;
            while (true)
            {
                seed = SplitMix64(seed);
                bool success;
                (success, sigma) = Map(keys, seed, capacity, blockLength);
                if (success) break;
            }
            ushort[] fingerprints = Assign(sigma, capacity, blockLength);
            return new Xor16(fingerprints, seed);
        }
        private struct HashCount
        {
            public ulong Hash;
            public int Count;
            public void Add(ulong key)
            {
                Hash ^= key;
                Count++;
            }
            public void Remove(ulong key)
            {
                Hash ^= key;
                Count--;
            }
        }
        // Algorithm 3: Mapping Step
        private static (bool Success, Stack<(ulong, int)> Sigma) Map(ulong[] keys, ulong seed, int capacity, int blockLength)
        {
            var H = new HashCount[capacity];
            Span<HashCount> hSpan = H.AsSpan();
            Span<HashCount> h0Span = hSpan.Slice(0, blockLength);
            Span<HashCount> h1Span = hSpan.Slice(blockLength, blockLength);
            Span<HashCount> h2Span = hSpan.Slice(2 * blockLength);
            // add keys to H
            foreach (ulong key in keys)
            {
                (ulong hash, int h0, int h1, int h2) = GetHashes(key + seed, blockLength);
                h0Span[h0].Add(hash);
                h1Span[h1].Add(hash);
                h2Span[h2].Add(hash);
            }
            // enqueue the single entries
            var Q0 = new Queue<(ulong, int)>(blockLength);
            var Q1 = new Queue<(ulong, int)>(blockLength);
            var Q2 = new Queue<(ulong, int)>(blockLength);
            for (var i = 0; i < blockLength; i++)
            {
                HashCount hashCount = h0Span[i];
                if (hashCount.Count == 1)
                {
                    Q0.Enqueue((hashCount.Hash, i));
                }
                hashCount = h1Span[i];
                if (hashCount.Count == 1)
                {
                    Q1.Enqueue((hashCount.Hash, i));
                }
                hashCount = h2Span[i];
                if (hashCount.Count == 1)
                {
                    Q2.Enqueue((hashCount.Hash, i));
                }
            }
            var sigma = new Stack<(ulong, int)>();
            while (Q0.Count + Q1.Count + Q2.Count > 0)
            {
                // handle Q0
                while (Q0.Count > 0)
                {
                    (ulong hash, int index) = Q0.Dequeue();
                    if (h0Span[index].Count == 0) continue;
                    int h1 = GetHash1(hash, blockLength);
                    int h2 = GetHash2(hash, blockLength);
                    sigma.Push((hash, index));
                    h1Span[h1].Remove(hash);
                    h2Span[h2].Remove(hash);
                    if (h1Span[h1].Count == 1) Q1.Enqueue((h1Span[h1].Hash, h1));
                    if (h2Span[h2].Count == 1) Q2.Enqueue((h2Span[h2].Hash, h2));
                }
                // handle Q1
                while (Q1.Count > 0)
                {
                    (ulong hash, int index) = Q1.Dequeue();
                    if (h1Span[index].Count == 0) continue;
                    int h0 = GetHash0(hash, blockLength);
                    int h2 = GetHash2(hash, blockLength);
                    index += blockLength;
                    sigma.Push((hash, index));
                    h0Span[h0].Remove(hash);
                    h2Span[h2].Remove(hash);
                    if (h0Span[h0].Count == 1) Q0.Enqueue((h0Span[h0].Hash, h0));
                    if (h2Span[h2].Count == 1) Q2.Enqueue((h2Span[h2].Hash, h2));
                }
                // handle Q2
                while (Q2.Count > 0)
                {
                    (ulong hash, int index) = Q2.Dequeue();
                    if (h2Span[index].Count == 0) continue;
                    int h0 = GetHash0(hash, blockLength);
                    int h1 = GetHash1(hash, blockLength);
                    index += 2 * blockLength;
                    sigma.Push((hash, index));
                    h0Span[h0].Remove(hash);
                    h1Span[h1].Remove(hash);
                    if (h0Span[h0].Count == 1) Q0.Enqueue((h0Span[h0].Hash, h0));
                    if (h1Span[h1].Count == 1) Q1.Enqueue((h1Span[h1].Hash, h1));
                }
            }
            return sigma.Count != keys.Length ? (false, null) : (true, sigma);
        }
        // Algorithm 4: Assigning Step
        private static ushort[] Assign(Stack<(ulong, int)> sigma, int capacity, int blockLength)
        {
            var fingerprints = new ushort[capacity];
            int blockLength2 = blockLength * 2;
            foreach ((ulong hash, int index) in sigma)
            {
                int h0 = GetHash0(hash, blockLength);
                int h1 = GetHash1(hash, blockLength) + blockLength;
                int h2 = GetHash2(hash, blockLength) + blockLength2;
                var f = (ushort)(hash ^ (hash >> 32));
                if (index < blockLength)
                {
                    f ^= fingerprints[h1];
                    f ^= fingerprints[h2];
                }
                else if (index < blockLength2)
                {
                    f ^= fingerprints[h0];
                    f ^= fingerprints[h2];
                }
                else
                {
                    f ^= fingerprints[h0];
                    f ^= fingerprints[h1];
                }
                fingerprints[index] = f;
            }
            return fingerprints;
        }
        // Algorithm 5: 64-bit hash function - randomly seeded murmur finalizer
        // h = x+s
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong MurmurFinalizer(ulong h)
        {
            h ^= h >> 33;
            h *= C1;
            h ^= h >> 33;
            h *= C2;
            h ^= h >> 33;
            return h;
        }
    }
}