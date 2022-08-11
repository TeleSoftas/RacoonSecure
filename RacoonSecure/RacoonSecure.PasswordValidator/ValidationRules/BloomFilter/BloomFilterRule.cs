using System;
using System.IO;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator.Cryptography;

namespace RacoonSecure.PasswordValidator.ValidationRules.BloomFilter
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
            var (elementCount, errorRate, filterBytes) = await ReadBloomFilterFromResourceAsync();
            return FilterBuilder.Build(elementCount, errorRate, filterBytes);
        }
        
        public async Task<string> ValidateAsync(string password)
        {
            var passwordBytes = CryptoHelper.ComputeSha1HashBytes(password);
            var filter = await _bloomFilter.Value; 
            return filter.Contains(passwordBytes) 
                ? ValidationError.PossiblyLeakedPassword 
                : string.Empty;
        }
        
        private async Task<(int elementCount, float errorRate, byte[] bloomFilterBytes)> ReadBloomFilterFromResourceAsync()
        {
            await using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.PasswordValidator.ValidationRules.BloomFilter.FilterData");
            if(stream == null)
                throw new NullReferenceException("No bloom filter resource located");

            await using var ms = new MemoryStream();

            var elementCountBytes = new byte[sizeof(int)];
            await stream.ReadAsync(elementCountBytes, 0, elementCountBytes.Length);
            var elementCount = BitConverter.ToInt32(elementCountBytes);

            var errorRateBytes = new byte[sizeof(float)];
            await stream.ReadAsync(errorRateBytes, 0, errorRateBytes.Length);
            var errorRate = BitConverter.ToSingle(errorRateBytes);
            
            await stream.CopyToAsync(ms);
            return (elementCount, errorRate, ms.ToArray());
        }
    }
}
