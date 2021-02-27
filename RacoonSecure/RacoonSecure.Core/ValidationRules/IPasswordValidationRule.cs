namespace RacoonSecure.Core.ValidationRules
{
    /// <summary>
    /// PasswordValidationRule interface.
    /// Can be inherited to create custom validation rules and added using PasswordValidatorBuilder.AddCustom(). 
    /// </summary>
    public interface IPasswordValidationRule
    {
        /// <summary>
        /// Returns error string if password is not valid
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string Validate(string password);
    }
}