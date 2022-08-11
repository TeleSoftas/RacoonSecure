using System.Net.Http;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator;
using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class PasswordValidatorTests
    {
        [Theory]
        [InlineData("Lov1e2c5", true)]
        [InlineData("password1", false)]
        [InlineData("Welcome", false)]
        [InlineData("smile", false)]
        public async Task PasswordIsCommon(string password, bool shouldBeValid)
        {
            var validator = new ValidatorBuilder().UseCommonPasswordCheck().Build();
            var validationResult = await validator.ValidateAsync(password);
            
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
        
        [Theory]
        [InlineData("password1", false)]
        [InlineData("123456", false)]
        [InlineData("Welcome", false)]
        [InlineData("smile", false)]
        [InlineData("x4s512x4a", true)]
        [InlineData("a2s3z5w895", true)]
   
        public async Task PasswordIsNotInBloomFilter(string password, bool shouldBeValid)
        {
            var validator = new ValidatorBuilder().UseBloomFilter().Build();
            var validationResult = await validator.ValidateAsync(password); 
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
        public async Task PasswordIsNotPwned(string password, bool shouldBeValid)
        {
            var validator = new ValidatorBuilder()
                .UseHibpApi(new HttpClient())
                .Build();
            var validationResult = await validator.ValidateAsync(password);
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }

        [Fact]
        public async Task CombinedErrorsReturnedWhenPasswordViolatesMoreThanOneRule()
        {
            var password = "asd123";
            var validator = new ValidatorBuilder()
                .UseNistGuidelines()
                .UseBloomFilter()
                .Build();
            
            var validationResult = await validator.ValidateAsync(password);
            Assert.False(validationResult.IsValid());
            Assert.Collection(validationResult.Errors, 
                s => s.Equals(ValidationError.TooShort),
                s => s.Equals(ValidationError.PossiblyLeakedPassword)
            );
        }
    }
}