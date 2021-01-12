using System.Linq;

namespace RacoonSecure.Core.ValidationRules.Nist
{
    internal class NistComplianceChecker : IValidationRule
    {
        public ValidationError Validate(string password) => IsNistCompliant(password);

        public ValidationError IsNistCompliant(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) 
                return ValidationError.Empty;
            if (password.Length < 8)
                return ValidationError.TooShort;
            if (password.All(c => c == ' '))
                return ValidationError.OnlyWhitespace;

            return default;
        }
    }
}