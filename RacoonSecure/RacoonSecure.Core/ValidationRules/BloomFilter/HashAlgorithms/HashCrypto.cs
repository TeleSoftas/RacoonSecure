using Core.Security.Cryptography;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;

namespace BloomFilter.HashAlgorithms
{

    public class HashCryptoSHA1 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA1.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA256 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA256.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA384 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA384.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA512 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA512.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }
    public class HashCryptoSHA3 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm =  new Keccak512Managed();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public abstract class HashCrypto : HashFunction
    {
        protected int[] ComputeHashNew(HashAlgorithm hashAlgorithm, byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            int computedHashes = 0;

            byte[] digest = hashAlgorithm.ComputeHash(data);
            using var h2 = SHA1.Create();
            using var h3 = SHA256.Create();
            using var h4 = MD5.Create();
            //digest = digest.Concat(h2.ComputeHash(data)).Concat(h3.ComputeHash(data)).Concat(h4.ComputeHash(data)).ToArray();            
            //byte[] output = new byte[hashAlgorithm.HashSize / 8];            


            Random r = new Random(BitConverter.ToInt32(digest.AsSpan().Slice(0, 4)));

            for (int i = 0; i < k; i++)
            {
                if (i*4 + 4 < digest.Length)
                    positions[i] = Math.Abs(BitConverter.ToInt32(digest.AsSpan().Slice(i*4, 4)))%m;
                else
                    positions[i] = r.Next(m);
            }

            return positions;
        }
        protected int[] ComputeHash(HashAlgorithm hashAlgorithm, byte[] data, int m, int k)
        {
            int[] positions = new int[k];

            int computedHashes = 0;

            byte[] digest = new byte[0];
            byte[] output = new byte[hashAlgorithm.HashSize / 8];

            while (computedHashes < k)
            {
                hashAlgorithm.TransformBlock(digest, 0, digest.Length, output, 0);
                digest = hashAlgorithm.ComputeHash(data, 0, data.Length);

                BitArray hashed = new BitArray(digest);

                int filterSize = 32 - (int)NumberOfLeadingZeros((uint)m);

                int hashBits = digest.Length * 8;

                for (int split = 0; split < (hashBits / filterSize) && computedHashes < k; split++)
                {
                    int from = split * filterSize;
                    int to = ((split + 1) * filterSize);

                    int intHash = BitToIntOne(hashed, from, to);

                    if (intHash < m)
                    {
                        positions[computedHashes] = intHash;
                        computedHashes++;
                    }
                }
            }

            return positions;
        }
    }
}