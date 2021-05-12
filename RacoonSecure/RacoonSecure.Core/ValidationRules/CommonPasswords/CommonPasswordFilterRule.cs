using System;
using System.Collections.Generic;
using System.Linq;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.CommonPasswords
{
    internal class CommonPasswordFilterRule : IPasswordValidationRule
    {
        private const int StoredPasswordLength = 8;
        private readonly List<byte[]> _commonPasswords;

        public CommonPasswordFilterRule()
        {
            _commonPasswords = InitializeCommonPasswordsAsBytes();
        }
        
        public string Validate(string password)
            => IsPasswordCommon(password) 
                ? ValidationError.CommonPassword 
                : string.Empty;
        
        private bool IsPasswordCommon(string password)
        {
            var hashBytes = CryptoHelper.ComputeSha1HashBytes(password, StoredPasswordLength);
            return _commonPasswords.Any(commonPassword => hashBytes.SequenceEqual(commonPassword));
        }
        
        private List<byte[]> InitializeCommonPasswordsAsBytes()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.CommonPasswords.Common");
            if(stream == null)
                throw new NullReferenceException("No common password resource located");

            var commonPasswords = new List<byte[]>();
            var buffer = new byte[StoredPasswordLength];
            while (stream.Read(buffer, 0, buffer.Length) != 0)
            {
                commonPasswords.Add(buffer.ToArray());
            }
            
            return commonPasswords;
        }
        
        private static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}