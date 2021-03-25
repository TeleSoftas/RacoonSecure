using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RacoonSecure.Core.ValidationRules.BloomFilter
{
    internal class BloomFilterRule : IPasswordValidationRule
    {
        private readonly BloomFilter _bloomFilter;

        public BloomFilterRule()
        {
            _bloomFilter = new BloomFilter();
        }

        public string Validate(string password)
        {
            return _bloomFilter.Contains(password) ? ValidationError.CommonPassword : string.Empty;
        }
    }
}
