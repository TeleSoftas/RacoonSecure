using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RacoonSecure.PasswordValidator;

namespace RacoonSecure.Core.Identity
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register RacoonSecurePasswordValidator as IdentityFramework password validator.
        /// </summary>
        /// <param name="builder">This</param>
        /// <param name="validator">RacoonSecure password validator</param>
        /// <param name="overrideValidationRules">If set to TRUE, default IdentityFramework password validations will be overwritten</param>
        /// <typeparam name="TUser">Type of IdentityUser used in IdentityContext</typeparam>
        public static IdentityBuilder AddRacoonSecurePasswordValidator<TUser>(this IdentityBuilder builder, Validator validator, bool overrideValidationRules = true)
            where TUser : IdentityUser
        {
            if (overrideValidationRules)
            {
                var currentValidator = builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IPasswordValidator<TUser>));
                builder.Services.Remove(currentValidator);
            }
            
            builder.Services.AddScoped<IPasswordValidator<TUser>, RacoonSecurePasswordValidator<TUser>>(
                _ => new RacoonSecurePasswordValidator<TUser>(validator)
            );
            
            return builder;
        }
    }
}