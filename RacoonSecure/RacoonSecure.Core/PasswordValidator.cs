﻿using System.Collections.Generic;
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
        
        public PasswordValidationResult Validate(string password)
        {
            var result = new PasswordValidationResult();

            foreach (var validationRule in _validationRules)
            {
                var error = validationRule.Validate(password);
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