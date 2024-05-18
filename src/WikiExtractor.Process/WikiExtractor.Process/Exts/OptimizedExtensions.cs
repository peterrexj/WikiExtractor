using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Exts
{
    public static class OptimizedExtensions
    {
        public static bool HasValueOptimized(this string text)
        {
            return !text.IsEmptyOptimized();
        }
        public static bool IsEmptyOptimized(this string text)
        {
            if (text == null)
            {
                return true;
            }

            return text.AsSpan().IsEmpty();
        }
        public static bool IsEmpty(this ReadOnlySpan<char> text)
        {
            foreach (var ch in text)
            {
                if (!char.IsWhiteSpace(ch))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsOptimized(this string haystack, string pin, StringComparison comparisonOptions)
        {
            return Contains(haystack.AsSpan(), pin.AsSpan(), comparisonOptions);
        }

        public static bool Contains(this ReadOnlySpan<char> haystack, ReadOnlySpan<char> pin, StringComparison comparisonOptions)
        {
            if (haystack.IsEmpty || pin.IsEmpty)
            {
                return false;
            }

            return haystack.IndexOf(pin, comparisonOptions) >= 0;
        }
    }
}
