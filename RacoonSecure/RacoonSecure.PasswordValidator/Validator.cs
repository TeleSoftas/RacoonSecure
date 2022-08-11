using System.Collections.Generic;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator.ValidationRules;

namespace RacoonSecure.PasswordValidator
{
    public class Validator
    {
        private readonly IEnumerable<IPasswordValidationRule> _validationRules;

        internal Validator(IEnumerable<IPasswordValidationRule> validationRules)
        {
            _validationRules = validationRules;
        }
        
        public async Task<PasswordValidationResult> ValidateAsync(string password)
        {
            var result = new PasswordValidationResult();

            foreach (var validationRule in _validationRules)
            {
                var error = await validationRule.ValidateAsync(password);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    result.AddError(error);
                }
            }

            return result;
        }
    }
}