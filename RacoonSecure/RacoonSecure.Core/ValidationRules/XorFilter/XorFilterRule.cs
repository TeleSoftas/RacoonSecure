using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.XorFilter
{
    public class XorFilterRule : IPasswordValidationRule
    {
        public string Validate(string password)
        {
            return string.Empty;
        }
    }
}