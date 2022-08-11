using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator.Cryptography;

namespace RacoonSecure.PasswordValidator.ValidationRules.Hibp
{
    public class PasswordNotPwnedRule : IPasswordValidationRule
    {
        private readonly HttpClient _httpClient;

        public PasswordNotPwnedRule(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
        }
        
        public async Task<string> ValidateAsync(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return ValidationError.PwnedPassword;
            
            var hash = CryptoHelper.ComputeSha1Hash(password);

            var prefix = hash[..5];
            var suffix = hash[5..^5];
            
            var response = await _httpClient.GetAsync($"range/{prefix}");
            await using var stream = await response.Content.ReadAsStreamAsync();

            if (stream is null)
                return string.Empty;

            string line;
            using var reader = new StreamReader(stream);
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.Contains(suffix))
                    return ValidationError.PwnedPassword;
            }

            return string.Empty;
        }
    }
}