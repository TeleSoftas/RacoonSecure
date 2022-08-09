using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.CommonPasswords
{
    internal class CommonPasswordFilterRule : IPasswordValidationRule
    {
        private const int StoredPasswordLength = 8;
        private readonly HashSet<byte[]> _commonPasswords;

        public CommonPasswordFilterRule()
        {
            _commonPasswords = InitializeCommonPasswordsAsBytes();
        }
        
        public async Task<string> ValidateAsync(string password)
            => IsPasswordCommon(password) ? ValidationError.CommonPassword : string.Empty;
        
        private bool IsPasswordCommon(string password)
        {
            var hashBytes = CryptoHelper.ComputeSha1HashBytes(password, StoredPasswordLength);
            return _commonPasswords.TryGetValue(hashBytes, out _);
        }
        
        private HashSet<byte[]> InitializeCommonPasswordsAsBytes()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.CommonPasswords.Common");
            if(stream == null)
                throw new NullReferenceException("No common password resource located");

            var commonPasswords = new HashSet<byte[]>(new ByteArrayComparer());
            var buffer = new byte[StoredPasswordLength];
            while (stream.Read(buffer, 0, buffer.Length) != 0)
            {
                commonPasswords.Add(buffer.ToArray());
            }
            
            return commonPasswords;
        }
    }
}