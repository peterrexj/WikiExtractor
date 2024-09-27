using Pj.Library.Helpers.Database.Repository;

namespace WikiExtractor.DbModels
{
    public class Master : ModelBase
    {
        public string Name { get; set; }
        public string Route { get; set; }
        public int Sequence { get; set; }
    }
}
