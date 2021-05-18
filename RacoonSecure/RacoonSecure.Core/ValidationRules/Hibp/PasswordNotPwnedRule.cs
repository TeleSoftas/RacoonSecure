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

        //TODO: Consider passing httpclient through constructor 
        public PasswordNotPwnedRule(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
        }
        
        public async Task<string> ValidateAsync(string password)
        {
            var hash = CryptoHelper.ComputeSha1Hash(password);

            var prefix = hash[..5];
            var suffix = hash[5..^5];
            
            //TODO: Read on value tasks
            var response = await _httpClient.GetAsync($"range/{prefix}");
            await using var stream = await response.Content.ReadAsStreamAsync();

            if (stream is null)
                return string.Empty;

            //TODO: Optimize stream reader logic. 
            using var reader = new StreamReader(stream);
            var isValid = !(await reader.ReadToEndAsync()).Contains(suffix);
            
            return isValid
                ? string.Empty
                : ValidationError.PwnedPassword;
        }
    }
}