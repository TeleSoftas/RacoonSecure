using System;
using System.IO;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterRule : IPasswordValidationRule
    {
        private readonly IValidationBloomFilter<byte[]> _bloomFilter;

        public BloomFilterRule()
        {
            var filterBytes = ReadBloomFilterFromResource();
            _bloomFilter = FilterBuilder.Build<byte[]>(10000000, 0.0001, filterBytes);
        }

        public string Validate(string password)
        {
            var passwordBytes = CryptoHelper.ComputeSha1HashBytes(password);
            return _bloomFilter.Contains(passwordBytes) ? ValidationError.CommonPassword : string.Empty;
        }
        
        private byte[] ReadBloomFilterFromResource()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.BloomFilter.FilterData");
            if(stream == null)
                throw new NullReferenceException("No bloom filter resource located");

            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
