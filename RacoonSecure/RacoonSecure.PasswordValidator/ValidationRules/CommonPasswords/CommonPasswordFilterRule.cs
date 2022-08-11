using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RacoonSecure.PasswordValidator.Cryptography;

namespace RacoonSecure.PasswordValidator.ValidationRules.CommonPasswords
{
    internal class CommonPasswordFilterRule : IPasswordValidationRule
    {
        private int _storedPasswordLength;
        private readonly HashSet<byte[]> _commonPasswords;

        public CommonPasswordFilterRule()
        {
            _commonPasswords = InitializeCommonPasswordsAsBytes();
        }
        
        public Task<string> ValidateAsync(string password)
            => IsPasswordCommon(password) 
                ? Task.FromResult(ValidationError.CommonPassword) 
                : Task.FromResult(string.Empty);
        
        private bool IsPasswordCommon(string password)
        {
            var hashBytes = CryptoHelper.ComputeSha1HashBytes(password, _storedPasswordLength);
            return _commonPasswords.TryGetValue(hashBytes, out _);
        }
        
        private HashSet<byte[]> InitializeCommonPasswordsAsBytes()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.PasswordValidator.ValidationRules.CommonPasswords.Common");
            if(stream == null)
                throw new NullReferenceException("No common password resource located");
            
            var storedPasswordLengthBytes = new byte[sizeof(short)];
            stream.Read(storedPasswordLengthBytes, 0, storedPasswordLengthBytes.Length);
            _storedPasswordLength = BitConverter.ToInt16(storedPasswordLengthBytes);

            var commonPasswords = new HashSet<byte[]>(new ByteArrayComparer());
            var buffer = new byte[_storedPasswordLength];
            while (stream.Read(buffer, 0, buffer.Length) != 0)
            {
                commonPasswords.Add(buffer.ToArray());
            }
            
            return commonPasswords;
        }
    }
}