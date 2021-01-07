namespace RacoonSecure.Core.PasswordLookup
{
    internal class NistComplianceResponse
    {
        public NistIncompatibility? Incompatibility { get; set; }
        
        public bool IsCompliant => Incompatibility == null;
    }

    internal enum NistIncompatibility
    {
        Empty,
        OnlyWhitespace,
        TooShort
    }
}