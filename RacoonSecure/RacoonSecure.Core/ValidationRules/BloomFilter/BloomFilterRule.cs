namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterRule : IPasswordValidationRule
    {
        private readonly BloomFilter _bloomFilter;

        public BloomFilterRule()
        {
            _bloomFilter = new BloomFilter(4086);
        }

        public string Validate(string password)
        {
            return _bloomFilter.Contains(password) ? ValidationError.CommonPassword : string.Empty;
        }
    }
}
