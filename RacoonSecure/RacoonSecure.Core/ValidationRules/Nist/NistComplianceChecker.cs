using System.Linq;

namespace RacoonSecure.Core.ValidationRules.Nist
{
    internal class NistComplianceChecker : IPasswordValidationRule
    {
        public string Validate(string password) => IsNistCompliant(password);

        public string IsNistCompliant(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) 
                return ValidationError.Empty;
            if (password.Length < 8)
                return ValidationError.TooShort;
            if (password.All(c => c == ' '))
                return ValidationError.OnlyWhitespace;

            return string.Empty;
        }
    }
}