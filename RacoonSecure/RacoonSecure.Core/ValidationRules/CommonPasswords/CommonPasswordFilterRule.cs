using System;
using System.Collections.Generic;
using System.IO;
using RacoonSecure.Core.Cryptography;

namespace RacoonSecure.Core.ValidationRules.CommonPasswords
{
    internal class CommonPasswordFilterRule : IPasswordValidationRule
    {
        private readonly List<string> _commonPasswordHashes;

        public CommonPasswordFilterRule()
        {
            _commonPasswordHashes = new List<string>();
            InitializeCommonPasswords(_commonPasswordHashes);
        }
        
        public string Validate(string password)
        {
            return IsPasswordCommon(password) ? ValidationError.CommonPassword : string.Empty;
        }

        private bool IsPasswordCommon(string password)
        {
            var hash = CryptoHelper.ComputeSha1Hash(password, 10);
            return _commonPasswordHashes.Contains(hash);
        }

        private void InitializeCommonPasswords(ICollection<string> list)
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("RacoonSecure.Core.ValidationRules.CommonPasswords.Common.txt");
            if(stream == null)
                throw new NullReferenceException("No common password resource located");

            using var sr = new StreamReader(stream);

            var buffer = new char[10];
            while (!sr.EndOfStream)
            {
                sr.Read(buffer, 0, buffer.Length);
                list.Add(new string(buffer));                
            }
        }
    }
}