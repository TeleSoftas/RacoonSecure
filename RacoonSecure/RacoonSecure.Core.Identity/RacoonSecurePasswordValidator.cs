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
            
            //Password is Valid
            if(validationResult.IsValid())
                return IdentityResult.Success;

            //Password is not valid
            var identityErrors = validationResult.Errors
                .Select(error => new IdentityError
                {
                    Code = error,
                    Description = "RacoonSecure password validator considered password as not valid."
                }).ToArray();
            
            return IdentityResult.Failed(identityErrors);
        }
    }
}