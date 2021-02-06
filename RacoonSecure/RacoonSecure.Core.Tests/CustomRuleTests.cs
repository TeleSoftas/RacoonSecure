using System.Text.RegularExpressions;
using RacoonSecure.Core.ValidationRules;
using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class CustomRuleTests
    {
        private class CustomLengthRule : IPasswordValidationRule
        {
            public string Validate(string password)
            {
                return password.Length > 10
                    ? string.Empty
                    : "Password is too long";
            }
        }

        private class CustomRegexRule : IPasswordValidationRule
        {
            public string Validate(string password)
            {
                return Regex.IsMatch(password, @"^[\!\@\#\$\%\^\&\*\(\)]+$") 
                    ? string.Empty
                    : "Password doesn't match custom regex"; 
            }
        }

        private class OnlyNumberValidationRule : IPasswordValidationRule
        {
            public string Validate(string password)
            {
                return Regex.IsMatch(password, @"^\d+$") 
                    ? string.Empty 
                    : "Password must contain only numbers";
            }
        }

        [Fact]
        public void CustomLengthRuleValidatesCorrectly()
        {
            const string flawfulPassword = "Pass";
            const string correctPassword = "Password1234567";
            
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new CustomLengthRule())
                .Build();
            
            Assert.False(validator.Validate(flawfulPassword).IsValid());
            Assert.True(validator.Validate(correctPassword).IsValid());
        }
        
        [Fact]
        public void CustomEmailRuleValidatesCorrectly()
        {
            const string flawfulPassword = "&%*#^@(1!a";
            const string correctPassword = "!@#%$&@^#";
            
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new CustomRegexRule())
                .Build();
            
            Assert.False(validator.Validate(flawfulPassword).IsValid());
            Assert.True(validator.Validate(correctPassword).IsValid());
        }
        
        [Fact]
        public void CustomOnlyNumbersRuleValidatesCorrectly()
        {
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new OnlyNumberValidationRule())
                .Build();
            
            Assert.True(validator.Validate("456789542135462").IsValid());
            Assert.False(validator.Validate("4c678x54213a462").IsValid());
        }
    }
}