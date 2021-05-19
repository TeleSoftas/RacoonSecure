namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    public interface IValidationBloomFilter
    {
        public bool Contains(byte[] element);
    }
}