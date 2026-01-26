namespace TRiZHub.Models
{
    public class PagedRequestModel
    {
        public int? CurrentPage { get; set; }

        public int? RecordsPerPage { get; set; }

        public int Verify()
        {
            if (CurrentPage == null || CurrentPage <= 0)
                CurrentPage = 1;
            if (RecordsPerPage == null)
                RecordsPerPage = 100;

            var begin = (CurrentPage.Value - 1)*RecordsPerPage.Value;
            return begin;
        }
    }
}