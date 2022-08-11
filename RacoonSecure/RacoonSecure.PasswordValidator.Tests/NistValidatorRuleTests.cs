using System.Linq;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator;
using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class NistValidatorRuleTests
    {
        [Theory]
        [InlineData("1ALLEY", false)]
        [InlineData("LalaLand", true)]
        public async Task PasswordIsNistCompliant(string password, bool shouldBeValid)
        {
            var validator = new ValidatorBuilder().UseNistGuidelines().Build();
            var validationResult = await validator.ValidateAsync(password);
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }

        [Fact]
        public async Task EmptyPasswordIsNotNistCompliant()
        {
            var password = "";

            var validator = new ValidatorBuilder().UseNistGuidelines().Build();
            var validationResult = await validator.ValidateAsync(password);

            Assert.False(validationResult.IsValid());
            Assert.Equal(ValidationError.Empty, validationResult.Errors.First());
        }

        [Fact]
        public async Task OnlyWhitespacePasswordIsNotNistCompliant()
        {
            var password = "         ";

            var validator = new ValidatorBuilder().UseNistGuidelines().Build();
            var validationResult = await validator.ValidateAsync(password);

            Assert.False(validationResult.IsValid());
            Assert.Equal(ValidationError.OnlyWhitespace, validationResult.Errors.First());
        }

        [Fact]
        public async Task TooShortPasswordIsNotNistCompliant()
        {
            var password = "123";

            var validator = new ValidatorBuilder().UseNistGuidelines().Build();
            var validationResult = await validator.ValidateAsync(password);

            Assert.False(validationResult.IsValid());
            Assert.Equal(ValidationError.TooShort, validationResult.Errors.First());
        }
    }
}