using Pj.Library.Helpers.Database.Repository;

namespace WikiExtractor.DbModels.UserStore
{
    public class FavouriteTrackerModel : ModelBase
    {
        public string ItemIdentifier { get; set; }
        public int IsFavourite { get; set; }
        public bool IsFavouriteAsBool => IsFavourite != 0;
    }
}
