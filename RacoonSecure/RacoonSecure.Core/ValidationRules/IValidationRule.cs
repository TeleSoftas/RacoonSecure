namespace RacoonSecure.Core
{
    public interface IValidationRule
    {
        ValidationError Validate(string password);
    }
}