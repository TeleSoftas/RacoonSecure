using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using RacoonSecure.Core.ValidationRules;
using RacoonSecure.Core.ValidationRules.BloomFilter;
using RacoonSecure.Core.ValidationRules.CommonPasswords;
using RacoonSecure.Core.ValidationRules.Hibp;
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

        /// <summary>
        /// Validator checks password for common NIST password guidelines, including:
        /// PasswordIsNotNullOrWhiteSpace
        /// Password is at least 8 characters long
        /// Password doesn't consist only of whitespace characters 
        /// </summary>
        /// <returns></returns>
        public PasswordValidatorBuilder UseNistGuidelines()
        {
            _validationRules.Add(new NistComplianceRule());
            return this;
        }

        /// <summary>
        /// Validator performs check among 100,000 most common passwords,
        /// and determines whether password that is being validated is not found amongst them
        /// </summary>
        /// <returns></returns>
        public PasswordValidatorBuilder UseCommonPasswordCheck()
        {
            _validationRules.Add(new CommonPasswordFilterRule());
            return this;
        }

        /// <summary>
        /// Validator performs call to HaveIBeenPwned API to check whether password has been leaked
        /// https://haveibeenpwned.com/
        /// </summary>
        /// <returns></returns>
        public PasswordValidatorBuilder UseHibpApi(HttpClient httpClient)
        { 
            _validationRules.Add(new PasswordNotPwnedRule(httpClient));
            return this;
        }

        /// <summary>
        /// Validator performs leaked password check using in-build leaked password database, utilising bloom filter.
        /// </summary>
        /// <returns></returns>
        public PasswordValidatorBuilder UseBloomFilter()
        {
            _validationRules.Add(new BloomFilterRule());
            return this;
        }
        
        /// <summary>
        /// Register custom validation rule.
        /// Inherit IPasswordValidationRule and pass instance of your validator.
        /// Validate() method will be called in validation pipeline
        /// </summary>
        /// <param name="passwordValidationRule">Custom password validator</param>
        /// <returns></returns>
        public PasswordValidatorBuilder UseCustom(IPasswordValidationRule passwordValidationRule)
        {
            _validationRules.Add(passwordValidationRule);
            return this;
        }
        
        /// <summary>
        /// Build configured PasswordValidator
        /// </summary>
        /// <returns></returns>
        public PasswordValidator Build()
        {
            if (!_validationRules.Any())
                throw new InitializationException("No rules have been configured");

            return new PasswordValidator(_validationRules);
        }
    }
}