using System;
using System.IO;
using System.Net.Http;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.Hibp
{
    public class PasswordNotPwnedRule : IPasswordValidationRule
    {
        private readonly HttpClient _httpClient;
        
        public PasswordNotPwnedRule(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
        }
        
        public string Validate(string password)
        {
            var hash = CryptoHelper.ComputeSha1Hash(password);

            var prefix = hash[..5];
            var suffix = hash[5..^5];
            
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