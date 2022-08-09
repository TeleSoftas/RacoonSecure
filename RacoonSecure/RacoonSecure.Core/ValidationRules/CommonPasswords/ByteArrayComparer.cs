using System.Collections.Generic;
using System.Linq;

namespace RacoonSecure.Core.ValidationRules.CommonPasswords
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            return !a.Where((t, i) => t != b[i]).Any();
        }
        public int GetHashCode(byte[] a)
        {
            var b = a.Aggregate<byte, uint>(0, (current, t) => ((current << 23) | (current >> 9)) ^ t);
            return unchecked((int)b);
        }
    }
}