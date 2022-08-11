using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using RacoonSecure.PasswordValidator.ValidationRules;
using RacoonSecure.PasswordValidator.ValidationRules.BloomFilter;
using RacoonSecure.PasswordValidator.ValidationRules.CommonPasswords;
using RacoonSecure.PasswordValidator.ValidationRules.Hibp;
using RacoonSecure.PasswordValidator.ValidationRules.Nist;

namespace RacoonSecure.PasswordValidator
{
    public class ValidatorBuilder
    {
        private readonly IList<IPasswordValidationRule> _validationRules;

        public ValidatorBuilder()
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
        public ValidatorBuilder UseNistGuidelines()
        {
            _validationRules.Add(new NistComplianceRule());
            return this;
        }

        /// <summary>
        /// Validator performs check among 100,000 most common passwords,
        /// and determines whether password that is being validated is not found amongst them
        /// </summary>
        /// <returns></returns>
        public ValidatorBuilder UseCommonPasswordCheck()
        {
            _validationRules.Add(new CommonPasswordFilterRule());
            return this;
        }

        /// <summary>
        /// Validator performs call to HaveIBeenPwned API to check whether password has been leaked
        /// https://haveibeenpwned.com/
        /// </summary>
        /// <returns></returns>
        public ValidatorBuilder UseHibpApi(HttpClient httpClient = null)
        {
            _validationRules.Add(new PasswordNotPwnedRule(httpClient));
            return this;
        }
        
        /// <summary>
        /// Validator performs leaked password check using in-build leaked password database, utilising bloom filter.
        /// </summary>
        /// <returns></returns>
        public ValidatorBuilder UseBloomFilter()
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
        public ValidatorBuilder UseCustom(IPasswordValidationRule passwordValidationRule)
        {
            _validationRules.Add(passwordValidationRule);
            return this;
        }
        
        /// <summary>
        /// Build configured PasswordValidator
        /// </summary>
        /// <returns></returns>
        public Validator Build()
        {
            if (!_validationRules.Any())
                throw new InitializationException("No rules have been configured");

            return new Validator(_validationRules);
        }
    }
}