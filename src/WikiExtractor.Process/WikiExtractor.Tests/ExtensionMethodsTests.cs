using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Tests
{
    public class ExtensionMethodsTests
    {
        [TestCase]
        public void Should_Satisfy_FileCreateLogic()
        {
            bool fileExists = true;
            int totalDaysToBeCached = 10;
            var fileCreatedTime = DateTime.Now.AddDays(-12);

            var result = false;
            //if (!fileExists || (fileExists && File.GetCreationTime(_localFileName).AddDays(totalDaysToBeCached) < DateTime.Now))
            if (!fileExists || (fileExists && fileCreatedTime.AddDays(totalDaysToBeCached) < DateTime.Now))
            {
                if (fileExists)
                {
                    result = true;
                }
            }

            Assert.That(result, Is.True);
        }


        [Test]
        public void RandomizeSubset_ReturnsCorrectCount()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5 };
            int count = 3;

            // Act
            var result = RandomizeSubset(data, count);

            // Assert
            Assert.AreEqual(count, result.Count);
        }

        [Test]
        public void RandomizeSubset_NoDuplicatesInSubset()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5 };
            int count = 3;

            // Act
            var result = RandomizeSubset(data, count);

            // Assert
            Assert.AreEqual(result.Count, result.Distinct().Count());
        }

        [Test]
        public void RandomizeSubset_ThrowsException_WhenCountIsGreaterThanDataSize()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            int count = 5;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomizeSubset(data, count));
        }

        [Test]
        public void RandomizeSubset_ThrowsException_WhenCountIsNegative()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3 };
            int count = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => RandomizeSubset(data, count));
        }

        [Test]
        public void RandomizeSubset_ReturnsWholeList_WhenCountEqualsDataSize()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5 };
            int count = data.Count;

            // Act
            var result = RandomizeSubset(data, count);

            // Assert
            Assert.AreEqual(data.Count, result.Count);
            CollectionAssert.AreEquivalent(data, result);
        }

        [Test]
        public void RandomizeSubset_MultipleCallsReturnDifferentResults()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5 };
            int count = 3;

            // Act
            var result1 = RandomizeSubset(data, count);
            var result2 = RandomizeSubset(data, count);

            // Assert
            Assert.AreNotEqual(result1, result2); // There's a chance these could be the same, but unlikely
        }

        // RandomizeSubset method (same as previously defined)
        public static List<T> RandomizeSubset<T>(List<T> data, int count)
        {
            if (count < 0 || count > data.Count)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative and no greater than the size of the list.");

            var rng = new Random();
            int n = data.Count;
            var result = new List<T>(count);

            var dataCopy = new List<T>(data);

            while (count > 0 && n > 0)
            {
                n--;
                var k = rng.Next(n + 1);
                result.Add(dataCopy[k]);
                dataCopy[k] = dataCopy[n];
                dataCopy.RemoveAt(n);
                count--;
            }

            return result;
        }
    }
}
