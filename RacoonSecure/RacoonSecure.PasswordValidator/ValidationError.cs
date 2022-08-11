namespace RacoonSecure.PasswordValidator
{
    public static class ValidationError
    {
        public const string Empty = nameof(Empty);
        public const string OnlyWhitespace = nameof(OnlyWhitespace);
        public const string TooShort = nameof(TooShort);
        public const string CommonPassword = nameof(CommonPassword);
        public const string PossiblyLeakedPassword = nameof(PossiblyLeakedPassword);
        public const string PwnedPassword = nameof(PwnedPassword);
    }
}