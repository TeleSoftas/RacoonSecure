using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RacoonSecure.Core.Identity
{
    public class RacoonSecurePasswordValidator<TUser> : IPasswordValidator<TUser>
        where TUser : IdentityUser
    {
        private readonly PasswordValidator _passwordValidator;

        public RacoonSecurePasswordValidator(PasswordValidator passwordValidator)
        {
            _passwordValidator = passwordValidator;
        }

        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var validationResult = await _passwordValidator.ValidateAsync(password);
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