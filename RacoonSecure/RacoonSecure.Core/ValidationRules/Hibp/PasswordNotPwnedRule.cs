using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.Hibp
{
    public class PasswordNotPwnedRule : IPasswordValidationRule
    {
        private readonly HttpClient _httpClient;
        
        public PasswordNotPwnedRule(Action<HttpClient> httpClientAction = null)
        {
            _httpClient = new HttpClient();
            httpClientAction?.Invoke(_httpClient);
            
            _httpClient.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
        }
        
        public async Task<string> ValidateAsync(string password)
        {
            var hash = CryptoHelper.ComputeSha1Hash(password);

            var prefix = hash[..5];
            var suffix = hash[5..^5];
            
            //TODO: Read on value tasks
            var response = _httpClient.GetAsync($"range/{prefix}").Result;
            using var stream = response.Content.ReadAsStreamAsync().Result;

            if (stream is null)
                return string.Empty;

            var reader = new StreamReader(stream);
            var isValid = !reader.ReadToEnd().Contains(suffix);
            
            return isValid
                ? string.Empty
                : ValidationError.PwnedPassword;
        }
    }
}