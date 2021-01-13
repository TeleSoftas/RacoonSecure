namespace RacoonSecure.Core
{
    public interface IPasswordValidationRule
    {
        ValidationError Validate(string password);
    }
}