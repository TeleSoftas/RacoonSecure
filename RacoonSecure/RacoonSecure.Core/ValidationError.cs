namespace RacoonSecure.Core
{
    public enum ValidationError
    {
        None = 0,
        Empty = 1,
        OnlyWhitespace = 2,
        TooShort = 3,
        CommonPassword = 4, 
        Unknown = 5
    }
}