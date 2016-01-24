// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable CheckNamespace
namespace Common
{
    public static class EqualityExtensions
    {
        public static bool CompareWithHashCodeAndEquals<T>(this T source, T other)
        {
            if (source == null)
                return other == null || other.Equals(source);
            if (other == null)
                return source.Equals(other);

            return source.GetHashCode() == other.GetHashCode() &&
                   source.Equals(other);
        }
    }
}