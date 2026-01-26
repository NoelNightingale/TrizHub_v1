namespace TRiZHub.Models
{
    public class GridModel
    {
        public int? CurrentPage { get; set; }
        public int? RecordsPerPage { get; set; }
        public string SortKey { get; set; }
        public string SortOrder { get; set; }
        public string Searchfor { get; set; }
        public bool ShowInactive { get; set; }

        public ScorecardModels.ScorecardCustomSearchModel CustomSearchModel { get; set; }

        public int Verify()
        {
            if (string.IsNullOrWhiteSpace(Searchfor))
                Searchfor = "null";
            if (string.IsNullOrWhiteSpace(SortKey))
                SortKey = string.Empty;
            if (string.IsNullOrWhiteSpace(SortOrder))
                SortOrder = "ASC";

            SortKey = SortKey.ToLower();
            if (CurrentPage <= 0)
                CurrentPage = 1;
            if (CurrentPage == null)
                CurrentPage = 1;
            if (RecordsPerPage == null)
                RecordsPerPage = 100;

            var begin = (CurrentPage.Value - 1)*RecordsPerPage.Value;
            return begin;
        }
    }
}