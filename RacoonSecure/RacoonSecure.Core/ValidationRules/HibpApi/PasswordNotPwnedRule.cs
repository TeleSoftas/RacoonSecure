using System.IO;
using System.Net;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.HibpApi
{
    public class PasswordNotPwnedRule : IPasswordValidationRule
    {
        public string Validate(string password)
        {
            var hash = CryptoHelper.ComputeSha1Hash(password, 25);
            var request = WebRequest.Create($"https://api.pwnedpasswords.com/range/{hash.Substring(0, 5)}");
            
            var response = request.GetResponse();

            bool isValid;
            using (var dataStream = response.GetResponseStream())
            {
                if (dataStream is null)
                    return string.Empty;

                var reader = new StreamReader(dataStream);
                isValid = !reader.ReadToEnd().Contains(hash);
                
            }
            response.Close();

            return isValid
                ? string.Empty
                : ValidationError.PwnedPassword;
        }
    }
}