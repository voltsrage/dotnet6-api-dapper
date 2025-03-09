namespace Dapper.API.Models.Pagination
{
    /// <summary>
    /// Represents a request for paginated data
    /// </summary>
    public class PaginationRequest
    {
        private int _page = 1;
        private int _pageSize = 10;

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page (default: 10, max: 100)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
        }

        /// <summary>
        /// Optional search term to filter results
        /// </summary>
        public string? SearchTerm { get; set; }
    }
}
