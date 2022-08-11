using RacoonSecure.PasswordValidator;
using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class PasswordValidatorBuilderTests
    {
        [Fact]
        public void BuilderTrowsIfNoRulesConfigured()
        {
            var builder = new ValidatorBuilder();
            Assert.Throws<InitializationException>(() => builder.Build());
        }
    }
}