using System;
using RacoonSecure.Core.BloomFilter;
using RacoonSecure.Core.CommonPasswords;
using RacoonSecure.Core.PasswordLookup;
using RacoonSecure.Core.Settings;

namespace RacoonSecure.Core
{
    public class PasswordValidator
    {
        private readonly NistComplianceChecker _nistComplianceChecker;
        private readonly CommonPasswordFilter _commonPasswordChecker;
        private readonly PasswordBloomFilter _bloomFilter;
        
        
        internal PasswordValidator(
            NistSettings nistSettings,
            CommonPasswordCheckSettings commonPasswordCheckSettings,
            BloomFilterSettings bloomFilterSettings)
        {
            if (nistSettings.IsEnabled)
                _nistComplianceChecker = new NistComplianceChecker();
            if(commonPasswordCheckSettings.IsEnabled)
                _commonPasswordChecker = new CommonPasswordFilter();
            if (bloomFilterSettings.IsEnabled)
                _bloomFilter = new PasswordBloomFilter();
        }
        
        public PasswordValidationResult Validate(string password)
        {
            var result = new PasswordValidationResult();
            
            if (_nistComplianceChecker != null)
            {
                var response = _nistComplianceChecker.IsNistCompliant(password);
                if (!response.IsCompliant)
                {
                    var error = response.Incompatibility switch
                    {
                        NistIncompatibility.Empty => ValidationError.Empty,
                        NistIncompatibility.OnlyWhitespace => ValidationError.OnlyWhitespace,
                        NistIncompatibility.TooShort => ValidationError.TooShort,
                        _ => ValidationError.Unknown 
                    };
                    
                    result.AddError(error);
                    return result;
                }
            }

            if (_commonPasswordChecker != null)
            {
                if (_commonPasswordChecker.IsPasswordCommon(password))
                {
                    result.AddError(ValidationError.CommonPassword);
                    return result;
                }
            }

            if (_bloomFilter != null)
            {
                if (_bloomFilter.PossiblyExists(password))
                {
                    result.AddError(ValidationError.CommonPassword);
                    return result;
                }
            }

            return result;
        }   
    }
}