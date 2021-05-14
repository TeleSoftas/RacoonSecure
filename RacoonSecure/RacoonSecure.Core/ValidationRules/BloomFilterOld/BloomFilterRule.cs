namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterOldRule : IPasswordValidationRule
    {
        private readonly BloomFilter _bloomFilter;

        public BloomFilterOldRule()
        {
            _bloomFilter = new BloomFilter();
        }

        public string Validate(string password)
        {
            return _bloomFilter.Contains(password) ? ValidationError.CommonPassword : string.Empty;
        }
    }
}
