namespace RacoonSecure.PasswordValidator.ValidationRules.BloomFilter
{
    public interface IValidationBloomFilter
    {
        public bool Contains(byte[] element);
    }
}