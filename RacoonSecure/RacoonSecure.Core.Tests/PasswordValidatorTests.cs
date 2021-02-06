using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class PasswordValidatorTests
    {
        [Theory]
        [InlineData("1ALLEY", false)]
        [InlineData("LalaLand", true)]
        public void PasswordIsNistCompliant(string password, bool shouldBeValid)
        {
            var validator = new PasswordValidatorBuilder().UseNistGuidelines().Build();
            var validationResult = validator.Validate(password);
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
        
        [Theory]
        [InlineData("Lov1e2c5", true)]
        [InlineData("Password1", false)]
        [InlineData("Welcome", false)]
        [InlineData("smile", false)]
        public void PasswordIsCommon(string password, bool shouldBeValid)
        {
            var validator = new PasswordValidatorBuilder().UseCommonPasswordCheck().Build();
            var validationResult = validator.Validate(password);
            
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
        
        [Theory]
        [InlineData("1ALLEY", false)]
        [InlineData("LalaLand", true)]
        public void PasswordIsNotInBloomFilter(string password, bool shouldBeValid)
        {
            var validator = new PasswordValidatorBuilder().UseBloomFilter().Build();
            var validationResult = validator.Validate(password); 
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
    }
}