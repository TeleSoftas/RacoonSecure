using System.Collections.Generic;

namespace RacoonSecure.Core
{
    public class PasswordValidator
    {
        private readonly IEnumerable<IValidationRule> _validationRules;

        internal PasswordValidator(IEnumerable<IValidationRule> validationRules)
        {
            _validationRules = validationRules;
        }
        
        public PasswordValidationResult Validate(string password)
        {
            var result = new PasswordValidationResult();
            
            foreach (var validationRule in _validationRules)
            {
                result.AddError(validationRule.Validate(password));
            }
            
            return result;
        }
    }
}