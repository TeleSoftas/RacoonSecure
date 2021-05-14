using Xunit;

namespace RacoonSecure.Core.Tests
{
    public class PasswordValidatorBuilderTests
    {
        [Fact]
        public void BuilderTrowsIfNoRulesConfigured()
        {
            var builder = new PasswordValidatorBuilder();
            Assert.Throws<InitializationException>(() => builder.Build());
        }
    }
}