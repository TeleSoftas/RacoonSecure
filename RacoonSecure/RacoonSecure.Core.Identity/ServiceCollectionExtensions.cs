using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace RacoonSecure.Core.Identity
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register RacoonSecurePasswordValidator as Identity IPasswordValidator.
        /// Warning! Call this method before calling services.AddIdentity $lt;TUser, TRole&gt;() if you want
        /// to override default password validation.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="validator">RacoonSecure password validator</param>
        /// <typeparam name="TUser">Type of IdentityUser used in IdentityContext</typeparam>
        public static void AddRacoonSecurePasswordValidator<TUser>(this IServiceCollection services, PasswordValidator validator)
            where TUser : IdentityUser
        {
            services.AddSingleton<IPasswordValidator<TUser>, RacoonSecurePasswordValidator<TUser>>(
                _ => new RacoonSecurePasswordValidator<TUser>(validator)
            );
        }
    }
}