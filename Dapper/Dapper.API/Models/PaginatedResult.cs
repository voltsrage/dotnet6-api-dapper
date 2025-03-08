namespace Dapper.API.Models
{
    /// <summary>
    /// Represents a paginated collection of items with metadata
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the PaginatedResult class
        /// </summary>
        /// <param name="items">The items on the current page</param>
        /// <param name="totalCount">The total number of items across all pages</param>
        /// <param name="page">The current page (1-based)</param>
        /// <param name="pageSize">The number of items per page</param>
        public PaginatedResult(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// The items on the current page
        /// </summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>
        /// The current page number (1-based)
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// The number of items per page
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// The total number of items across all pages
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// The total number of pages
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Whether there is a page after the current page
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Whether there is a page before the current page
        /// </summary>
        public bool HasPreviousPage => Page > 1;
    }
}
