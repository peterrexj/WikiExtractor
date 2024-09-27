using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiApp
{
    public static class RandomGeneratorHelper
    {
        public static List<T> RandomizeList<T>(List<T> data)
        {
            var rng = new Random();
            int n = data.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (data[k], data[n]) = (data[n], data[k]);
            }
            return data;
        }

        public static List<T> RandomizeSubset<T>(List<T> data, int count, bool ensureUnique)
        {
            if (count < 0 || count > data.Count)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative and no greater than the size of the list.");

            // If ensureUnique is true, remove duplicates from the data list
            var dataCopy = ensureUnique ? new HashSet<T>(data).ToList() : new List<T>(data);

            // Check if we still have enough items to return the required count
            if (count > dataCopy.Count)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be less than or equal to the number of unique items in the list.");

            var rng = new Random();
            int n = dataCopy.Count;
            var result = new List<T>(count);

            while (count > 0 && n > 0)
            {
                n--;
                var k = rng.Next(n + 1);

                // Add the random item to the result list
                result.Add(dataCopy[k]);

                // Remove the selected item from the copy list to avoid duplication
                dataCopy[k] = dataCopy[n];
                dataCopy.RemoveAt(n);

                count--;
            }

            return result;
        }
    }
}
