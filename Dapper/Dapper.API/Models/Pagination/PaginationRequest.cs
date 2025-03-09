

using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

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

        /// <summary>
        /// Number of records to skip
        /// </summary>
        [JsonIgnore]
        public int Skip => (Page - 1) * PageSize;

        /// <summary>
        /// Indicates if a search term is provided
        /// </summary>
        [JsonIgnore]
        public bool HasSearch => !string.IsNullOrWhiteSpace(SearchTerm);

        /// <summary>
        /// Optional column to sort by
        /// </summary>
        public string? SortColumn { get; set; } = "Id";

        /// <summary>
        /// Sort direction (asc or desc)
        /// </summary>
        public string? SortDirection { get; set; } = "asc";

        /// <summary>
        /// Indicates if sorting is specified
        /// </summary>
        [JsonIgnore]
        public bool HasSorting => !string.IsNullOrWhiteSpace(SortColumn);

        /// <summary>
        /// Indicates if sort direction is ascending
        /// </summary>
        [JsonIgnore]
        public bool IsAscending => string.IsNullOrEmpty(SortDirection) ||
                                  SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);
    }
}
