using System.Linq;

namespace RacoonSecure.Core.ValidationRules.Nist
{
    internal class NistComplianceRule : IPasswordValidationRule
    {
        public string Validate(string password) => CheckNistCompliance(password);

        /// <summary>
        ///
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Error string if password is not NIST compliant</returns>
        private static string CheckNistCompliance(string password)
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