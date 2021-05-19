using System;
using System.IO;
using System.Threading.Tasks;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterRule : IPasswordValidationRule
    {
        private readonly Lazy<Task<IValidationBloomFilter>> _bloomFilter;

        public BloomFilterRule()
        {
            _bloomFilter = new Lazy<Task<IValidationBloomFilter>>(InitializeBloomFilter);
        }
        
        private async Task<IValidationBloomFilter> InitializeBloomFilter()
        {
            var filterBytes = await ReadBloomFilterFromResource();
            return FilterBuilder.Build(10000000, 0.0001, filterBytes);
        }
        
        public async Task<string> ValidateAsync(string password)
        {
            var passwordBytes = CryptoHelper.ComputeSha1HashBytes(password);
            var filter = await _bloomFilter.Value; 
            return filter.Contains(passwordBytes) 
                ? ValidationError.PossiblyLeakedPassword 
                : string.Empty;
        }
        
        private async Task<byte[]> ReadBloomFilterFromResource()
        {
            await using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.BloomFilter.FilterData");
            if(stream == null)
                throw new NullReferenceException("No bloom filter resource located");

            await using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
