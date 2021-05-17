using System.Collections.Generic;
using System.Threading.Tasks;
using RacoonSecure.Core.ValidationRules;

namespace RacoonSecure.Core
{
    public class PasswordValidator
    {
        private readonly IEnumerable<IPasswordValidationRule> _validationRules;

        internal PasswordValidator(IEnumerable<IPasswordValidationRule> validationRules)
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
                    break;
                }
            }

            return result;
        }
    }
}