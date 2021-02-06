namespace RacoonSecure.Core.ValidationRules
{
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