using System;

namespace WikiExtractor.Maui.App.Services
{
    /// <summary>
    /// Helper class for enum operations.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    public static class EnumHelper<T> where T : struct, Enum
    {
        /// <summary>
        /// Converts a string to an enum value.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>The enum value.</returns>
        public static T FromString(string value)
        {
            if (Enum.TryParse<T>(value, out var result))
            {
                return result;
            }
            
            throw new ArgumentException($"Could not convert {value} to {typeof(T).Name}");
        }
    }
}