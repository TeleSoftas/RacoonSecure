using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.CommonPasswords
{
    //TODO: Common.txt should be saved as bytes in file.
    internal class CommonPasswordFilterRule : IPasswordValidationRule
    {
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
            var hexHash = CryptoHelper.ComputeSha1Hash(password, 10);
            var hexHashBytes = StringToByteArray(hexHash);
            
            var hashBytes = CryptoHelper.ComputeSha1HashBytes(password, 5);
            return _commonPasswords.Any(commonPassword => hashBytes.SequenceEqual(commonPassword));
        }
        
        private List<byte[]> InitializeCommonPasswordsAsBytes()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.CommonPasswords.Common");
            if(stream == null)
                throw new NullReferenceException("No common password resource located");

            var commonPasswords = new List<byte[]>();
            var buffer = new byte[5];
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