using System.Collections.Generic;
using System.Linq;

namespace RacoonSecure.PasswordValidator
{
    public class PasswordValidationResult
    {
        private readonly List<string> _errors;
        
        public PasswordValidationResult()
        {
            _errors = new List<string>();
        }

        public bool IsValid() => _errors == null || !_errors.Any();
        public IEnumerable<string> Errors => _errors.AsEnumerable();
        
        internal void AddError(string error)
        {
            if(!string.IsNullOrWhiteSpace(error))
                _errors.Add(error);
        }
    }
}