using System;
using System.IO;
using System.Threading.Tasks;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterRule : IPasswordValidationRule
    {
        private readonly Lazy<Task<IValidationBloomFilter<byte[]>>> _bloomFilter;

        public BloomFilterRule()
        {
            _bloomFilter = new Lazy<Task<IValidationBloomFilter<byte[]>>>(InitializeBloomFilter);
        }
        
        private async Task<IValidationBloomFilter<byte[]>> InitializeBloomFilter()
        {
            var filterBytes = await ReadBloomFilterFromResource();
            return FilterBuilder.Build<byte[]>(10000000, 0.0001, filterBytes);
        }
        
        public async Task<string> ValidateAsync(string password)
        {
            var passwordBytes = CryptoHelper.ComputeSha1HashBytes(password);
            return await (await _bloomFilter.Value).ContainsAsync(passwordBytes) 
                ? ValidationError.CommonPassword 
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
