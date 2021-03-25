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
        [InlineData("password1", false)]
        [InlineData("1ALLEY", true)]
        [InlineData("LalaLand", true)]
        [InlineData("password3", false)]
        public void PasswordIsNotInBloomFilter(string password, bool shouldBeValid)
        {
            var validator = new PasswordValidatorBuilder().UseBloomFilter().Build();
            var validationResult = validator.Validate(password); 
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
        
        [Theory]
        [InlineData("password1", false)]
        [InlineData("lalala", false)]
        [InlineData("asdasd", false)]
        [InlineData("qwerty", false)]
        [InlineData("x1s54r89s54s23a", true)]
        [InlineData("l3orksmkf", true)]
        [InlineData("ld7s889e54s23d@", true)]
        public void PasswordIsNotPwned(string password, bool shouldBeValid)
        {
            var validator = new PasswordValidatorBuilder().UseHibpApi().Build();
            var validationResult = validator.Validate(password);
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
    }
}