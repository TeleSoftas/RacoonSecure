using System.Collections;
using System.Collections.Generic;
using RacoonSecure.Core.Settings;
using RacoonSecure.Core.ValidationRules;
using RacoonSecure.Core.ValidationRules.BloomFilter;
using RacoonSecure.Core.ValidationRules.CommonPasswords;
using RacoonSecure.Core.ValidationRules.Nist;

namespace RacoonSecure.Core
{
    public class PasswordValidatorBuilder
    {
        private readonly IList<IPasswordValidationRule> _validationRules;
        
        public PasswordValidatorBuilder()
        {
            _validationRules = new List<IPasswordValidationRule>();
        }
        
        public PasswordValidatorBuilder UseNistGuidelines()
        {
            _validationRules.Add(new NistComplianceRule());            
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
        
        public PasswordValidatorBuilder UseCustom(IPasswordValidationRule passwordValidationRule)
        {
            _validationRules.Add(passwordValidationRule);
            return this;
        }

        public PasswordValidator Build() => new PasswordValidator(_validationRules);
    }
}