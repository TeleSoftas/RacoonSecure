namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    /// <summary>
    /// BloomFilter Builder
    /// </summary>
    public static class FilterBuilder
    {
      
        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static BloomFilter Build(int expectedElements, double errorRate)
        {
            return new BloomFilter(expectedElements, errorRate, null);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <param name="filterArray">Pre-initialized filter array</param>
        /// <returns></returns>
        public static IValidationBloomFilter Build(int expectedElements, double errorRate, byte[] filterArray)
        {
            return new BloomFilter(expectedElements, errorRate, filterArray);
        }
    }
}