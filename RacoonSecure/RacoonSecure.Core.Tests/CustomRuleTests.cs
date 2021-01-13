using System.Text.RegularExpressions;
using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class CustomRuleTests
    {
        public class CustomLengthRule : IPasswordValidationRule
        {
            public ValidationError Validate(string password)
            {
                return password.Length > 10
                    ? ValidationError.None
                    : ValidationError.Unknown;
            }
        }
        
        public class CustomRegexRule : IPasswordValidationRule
        {
            public ValidationError Validate(string password)
            {
                //TODO: Remake validation error to return string error, so that user could supply custom error;
                return Regex.IsMatch(password, @"^[\!\@\#\$\%\^\&\*\(\)]+$") 
                    ? ValidationError.None
                    : ValidationError.Unknown; 
            }
        }

        [Fact]
        public void CustomLengthRuleValidatesCorrectly()
        {
            var flawfulPassword = "Pass";
            var correctPassword = "Password1234567";
            
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new CustomLengthRule())
                .Build();
            
            Assert.False(validator.Validate(flawfulPassword).IsValid());
            Assert.True(validator.Validate(correctPassword).IsValid());
        }
        
        [Fact]
        public void CustomEmailRuleValidatesCorrectly()
        {
            var flawfulPassword = "&%*#^@(1!a";
            var correctPassword = "!@#%$&@^#";
            
            var validator = new PasswordValidatorBuilder()
                .UseCustom(new CustomRegexRule())
                .Build();
            
            Assert.False(validator.Validate(flawfulPassword).IsValid());
            Assert.True(validator.Validate(correctPassword).IsValid());
        }
    }
}