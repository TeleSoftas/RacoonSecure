using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RacoonSecure.Core.Cryptography;
using RacoonSecure.Core.ValidationRules.XorFilter;
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
            var validator = new PasswordValidatorBuilder().UseHIBPApi().Build();
            var validationResult = validator.Validate(password);
            Assert.True(validationResult.IsValid() == shouldBeValid);
        }
        
        // [Fact]
        // public void PasswordIsNotInXorFilter()
        // {
        //     var passwords = new List<string> {"test", "test1", "test2", "test3", "test4"};
        //     var hashes = passwords
        //         .Select(x => BitConverter.ToUInt64(Encoding.ASCII.GetBytes(CryptoHelper.ComputeSha1Hash(x, 10))))
        //         .ToArray();
        //     
        //     var xorfiler = Xor16.Construction(hashes);
        //     Assert.False(xorfiler.Contains(123548798456));
        //     Assert.True(xorfiler.Contains(hashes[0]));
        // }
    }
}