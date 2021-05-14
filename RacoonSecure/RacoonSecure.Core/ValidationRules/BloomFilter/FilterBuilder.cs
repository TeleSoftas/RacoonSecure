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
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="hashFunction">The hash function.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements)
        {
            return Build<T>(expectedElements, 0.01);
        }


        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <returns></returns>
        public static Filter<T> Build<T>(int expectedElements, double errorRate)
        {
            return new FilterMemory<T>(expectedElements, errorRate, null);
        }

        /// <summary>
        /// Creates a BloomFilter for the specified expected element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedElements">The expected elements.</param>
        /// <param name="errorRate">The error rate.</param>
        /// <param name="hashMethod">The hash method.</param>
        /// <param name="filterArray">Pre-initialized filter array</param>
        /// <returns></returns>
        public static IValidationBloomFilter<T> Build<T>(int expectedElements, double errorRate, byte[] filterArray)
        {
            return new FilterMemory<T>(expectedElements, errorRate, filterArray);
        }
    }
}