using System.Linq;
using System.Threading.Tasks;

namespace RacoonSecure.PasswordValidator.ValidationRules.Nist
{
    internal class NistComplianceRule : IPasswordValidationRule
    {
        public async Task<string> ValidateAsync(string password) => await CheckNistCompliance(password);

        /// <summary>
        ///
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Error string if password is not NIST compliant</returns>
        private static async Task<string> CheckNistCompliance(string password)
        {
            if (string.IsNullOrEmpty(password)) 
                return ValidationError.Empty;
            if (password.Length < 8)
                return ValidationError.TooShort;
            if (password.All(c => c == ' '))
                return ValidationError.OnlyWhitespace;

            return string.Empty;
        }
    }
}