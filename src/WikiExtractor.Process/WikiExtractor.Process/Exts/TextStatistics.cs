namespace WikiExtractor.Process.Exts
{
    public class TextStatistics
    {
        public List<(string Text, int Length)> TextData { get; set; }
        public int TotalItems { get; private set; }

        public TextStatistics(List<(string Text, int Length)> textData)
        {
            TextData = textData;
            TotalItems = TextData.Count; // Set the total count here
        }

        public List<(string Range, int Count, double Percentage)> GetLengthStatistics(int bucketCount)
        {
            var maxLength = TextData.Max(t => t.Length);
            var minLength = TextData.Min(t => t.Length);
            var rangeSize = (maxLength - minLength) / (double)bucketCount;

            var bucketList = new List<(string Range, int Count, double Percentage)>();

            for (int i = 0; i < bucketCount; i++)
            {
                int bucketMin = (int)(minLength + i * rangeSize);
                int bucketMax = (int)(minLength + (i + 1) * rangeSize);

                var countInBucket = TextData.Count(t => t.Length >= bucketMin && t.Length < bucketMax);
                double percentage = (double)countInBucket / TotalItems * 100;

                bucketList.Add(($"{bucketMin}-{bucketMax}", countInBucket, percentage));
            }

            return bucketList;
        }
    }
}
