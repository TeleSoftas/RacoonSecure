using System.Collections.Generic;
using System.Linq;

namespace RacoonSecure.Core
{
    public class PasswordValidationResult
    {
        private List<ValidationError> Errors { get; set; }
        
        public PasswordValidationResult()
        {
            Errors = new List<ValidationError>();
        }

        public bool IsValid() => Errors == null || !Errors.Any();
        
        public IEnumerable<string> GetErrors() => Errors.Select(x => x.ToString()).ToArray();
        
        internal void AddError(ValidationError error) => Errors.Add(error);
    }
}