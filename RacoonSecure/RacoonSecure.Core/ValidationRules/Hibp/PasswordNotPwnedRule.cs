using System.IO;
using System.Net;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.Hibp
{
    public class PasswordNotPwnedRule : IPasswordValidationRule
    {
        public string Validate(string password)
        {
            var hash = CryptoHelper.ComputeSha1Hash(password);

            var prefix = hash.Substring(0, 5);
            var suffix = hash.Substring(5, hash.Length - 5);
            
            //TODO: Investigate alternative ways to complete HTTP request.
            var request = WebRequest.Create($"https://api.pwnedpasswords.com/range/{prefix}");
            var response = request.GetResponse();

            bool isValid;
            using (var dataStream = response.GetResponseStream())
            {
                if (dataStream is null)
                    return string.Empty;

                var reader = new StreamReader(dataStream);
                isValid = !reader.ReadToEnd().Contains(suffix);
                
            }
            response.Close();

            return isValid
                ? string.Empty
                : ValidationError.PwnedPassword;
        }
    }
}