using System.Threading.Tasks;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    public interface IValidationBloomFilter<in T>
    {
        public Task<bool> ContainsAsync(T element);
        public bool Contains(T element);
    }
}