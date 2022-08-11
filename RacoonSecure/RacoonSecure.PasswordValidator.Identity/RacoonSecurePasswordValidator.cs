using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RacoonSecure.PasswordValidator;

namespace RacoonSecure.Core.Identity
{
    public class RacoonSecurePasswordValidator<TUser> : IPasswordValidator<TUser>
        where TUser : IdentityUser
    {
        private readonly Validator _validator;

        public RacoonSecurePasswordValidator(Validator validator)
        {
            _validator = validator;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var validationResult = await _validator.ValidateAsync(password);
            if(validationResult.IsValid())
                return IdentityResult.Success;

            var identityErrors = validationResult.Errors
                .Select(error => new IdentityError
                {
                    Code = error,
                    Description = "RacoonSecure considered password as not valid."
                }).ToArray();
            
            return IdentityResult.Failed(identityErrors);
        }
    }
}