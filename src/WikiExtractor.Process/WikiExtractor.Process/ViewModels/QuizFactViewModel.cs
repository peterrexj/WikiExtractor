namespace WikiExtractor.ViewModels
{
    /// <summary>
    /// Represents a quiz fact that can be displayed to the user during loading screens
    /// </summary>
    public class QuizFactViewModel
    {
        /// <summary>
        /// The ID of the master entity this fact relates to
        /// </summary>
        public int MasterId { get; set; }

        /// <summary>
        /// The metadata key identifying the type of fact
        /// </summary>
        public string MetadataKey { get; set; }

        /// <summary>
        /// The formatted fact text with MasterId and AnswerId replaced
        /// </summary>
        public string FactText { get; set; }

        /// <summary>
        /// The name of the master entity
        /// </summary>
        public string MasterName { get; set; }

        /// <summary>
        /// Optional path to the master's image
        /// </summary>
        public string MasterImagePath { get; set; }

        /// <summary>
        /// The answer/value for this metadata
        /// </summary>
        public string AnswerValue { get; set; }
    }
}
