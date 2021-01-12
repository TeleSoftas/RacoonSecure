using System.Collections;
using System.Collections.Generic;
using RacoonSecure.Core.Settings;
using RacoonSecure.Core.ValidationRules.BloomFilter;
using RacoonSecure.Core.ValidationRules.CommonPasswords;
using RacoonSecure.Core.ValidationRules.Nist;

namespace RacoonSecure.Core
{
    public class PasswordValidatorBuilder
    {
        private readonly IList<IValidationRule> _validationRules;
        
        public PasswordValidatorBuilder()
        {
            _validationRules = new List<IValidationRule>();
        }
        
        public PasswordValidatorBuilder UseNistGuidelines()
        {
            _validationRules.Add(new NistComplianceChecker());            
            return this;
        }
        
        public PasswordValidatorBuilder UseCommonPasswordCheck()
        {
            _validationRules.Add(new CommonPasswordFilterRule());
            return this;
        }

        public PasswordValidatorBuilder UseBloomFilter()
        {
            _validationRules.Add(new BloomFilterRule());            
            return this;
        }

        public PasswordValidator Build() => new PasswordValidator(_validationRules);
    }
}