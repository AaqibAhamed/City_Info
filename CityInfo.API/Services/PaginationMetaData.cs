namespace CityInfo.API.Services
{

    public class PaginationMetaData
    {
        public int TotalItemCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNumber { get; set; }
        public int TotalPageCount { get; set; }

        public PaginationMetaData(int totalItemCount, int pageSize, int currentPageNumber)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPageNumber = currentPageNumber;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)PageSize);

        }



    }
}
