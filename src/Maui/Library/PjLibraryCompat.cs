// PjLibraryCompat.cs
// This file provides compatibility for Pj.Library when using .NET 9 SDK with .NET 8 target frameworks

#if NET9_0_OR_GREATER
// Undefine NET9_0_OR_GREATER to prevent collection expressions from being used in Pj.Library
#undef NET9_0_OR_GREATER
#endif

namespace Pj.Library.Compatibility
{
    /// <summary>
    /// This class exists solely to provide compatibility between .NET 9 SDK features
    /// and libraries that aren't compatible with those features yet.
    /// </summary>
    internal static class PjLibraryCompatibility
    {
        /// <summary>
        /// This method demonstrates the traditional array initialization syntax that works in both .NET 8 and .NET 9
        /// </summary>
        internal static int[] GetCompatibleArray()
        {
            // Using traditional array initialization syntax that works in both .NET 8 and .NET 9
            return new int[] { 1, 2, 3 };
        }

        /// <summary>
        /// This method does nothing and is never called.
        /// It exists only to ensure this file is included in the compilation.
        /// </summary>
        internal static void EnsureCompatibility()
        {
            // This method intentionally left empty
            
            // Example of traditional array syntax that should be used instead of collection expressions
            var items = new[] { 1, 2, 3 }; // Traditional syntax works in both .NET 8 and .NET 9
        }
    }
}