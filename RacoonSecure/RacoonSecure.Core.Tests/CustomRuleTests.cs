using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RacoonSecure.Core.ValidationRules;
using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class CustomRuleTests
    {
        private class CustomLengthRule : IPasswordValidationRule
        {
            public Task<string> ValidateAsync(string password)
            {
                return Task.FromResult(password.Length > 10
                    ? string.Empty
                    : "TooLong");
            }
        }

        private class CustomRegexRule : IPasswordValidationRule
        {
            public Task<string> ValidateAsync(string password)
            {
                return Task.FromResult(Regex.IsMatch(password, @"^[\!\@\#\$\%\^\&\*\(\)]+$")
                    ? string.Empty
                    : "RegexMismatch"); 
            }
        }

        private class OnlyNumberValidationRule : IPasswordValidationRule
        {
            public Task<string> ValidateAsync(string password)
            {
                return Task.FromResult(Regex.IsMatch(password, @"^\d+$") 
                    ? string.Empty 
                    : "OnlyNumerics");
            }
        }

        [Fact]
        public async Task CustomLengthRuleValidatesCorrectly()
        {
            const string flawfulPassword = "Pass";
            const string correctPassword = "Password1234567";
            
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new CustomLengthRule())
                .Build();
            
            Assert.False((await validator.ValidateAsync(flawfulPassword)).IsValid());
            Assert.True((await validator.ValidateAsync(correctPassword)).IsValid());
        }
        
        [Fact]
        public async Task CustomEmailRuleValidatesCorrectly()
        {
            const string flawfulPassword = "&%*#^@(1!a";
            const string correctPassword = "!@#%$&@^#";
            
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new CustomRegexRule())
                .Build();
            
            Assert.False((await validator.ValidateAsync(flawfulPassword)).IsValid());
            Assert.True((await validator.ValidateAsync(correctPassword)).IsValid());
        }
        
        [Fact]
        public async Task CustomOnlyNumbersRuleValidatesCorrectly()
        {
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new OnlyNumberValidationRule())
                .Build();
            
            Assert.True((await validator.ValidateAsync("456789542135462")).IsValid());
            Assert.False((await validator.ValidateAsync("4c678x54213a462")).IsValid());
        }
    }
}