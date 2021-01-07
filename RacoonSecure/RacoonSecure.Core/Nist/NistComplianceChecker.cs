using System.Linq;

namespace RacoonSecure.Core.PasswordLookup
{
    internal class NistComplianceChecker
    {
        public NistComplianceResponse IsNistCompliant(string password)
        {
            var response = new NistComplianceResponse();

            if (string.IsNullOrWhiteSpace(password))
            {
                response.Incompatibility = NistIncompatibility.Empty;
                return response;
            }
            if (password.Length < 8)
            {
                response.Incompatibility = NistIncompatibility.TooShort;
                return response;
            }
            if (password.All(c => c == ' '))
            {
                response.Incompatibility = NistIncompatibility.OnlyWhitespace;
                return response;
            }

            return response;
        }
    }
}