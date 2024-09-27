using Pj.Library.Helpers.Database.Repository;
using System;

namespace WikiExtractor.Process.DbModels
{
    public class QuizResponse : ModelBase
    {
        public int MasterId { get; set; }
        public string MetadataKey { get; set; }
        public int UserResponse { get; set; }
        public int QuestionSetId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
