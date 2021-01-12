using System.Collections.Generic;
using System.Linq;

namespace RacoonSecure.Core
{
    public class PasswordValidationResult
    {
        private List<ValidationError> _errors { get; }
        
        public PasswordValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        public bool IsValid() => _errors == null || !_errors.Any();

        public IEnumerable<string> Errors
        {
            get
            {
                return _errors.Select(x => x.ToString());           
            }
        }

        internal void AddError(ValidationError error)
        {
            if(error != ValidationError.None)
                _errors.Add(error);
        }
    }
}