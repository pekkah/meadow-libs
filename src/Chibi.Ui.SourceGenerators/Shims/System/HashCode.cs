// ReSharper disable once CheckNamespace
namespace System
{
    internal static class HashCode
    {
        public static int Combine<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            if (value1 == null) throw new ArgumentNullException(nameof(value1));
            if (value2 == null) throw new ArgumentNullException(nameof(value2));
            if (value3 == null) throw new ArgumentNullException(nameof(value3));
            if (value4 == null) throw new ArgumentNullException(nameof(value4));

            unchecked
            {
                var hash = 17;
                hash = hash * 31 + value1.GetHashCode();
                hash = hash * 31 + value2.GetHashCode();
                hash = hash * 31 + value3.GetHashCode();
                hash = hash * 31 + value4.GetHashCode();
                return hash;
            }
        }
    }
}