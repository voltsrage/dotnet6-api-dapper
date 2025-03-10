namespace Dapper.API.Models
{
    /// <summary>
    /// Result of a bulk delete operation
    /// </summary>
    public class BulkDeleteResult
    {
        /// <summary>
        /// IDs of hotels that were successfully deleted
        /// </summary>
        public List<int> SuccessfullyDeletedIds { get; set; }

        /// <summary>
        /// IDs of hotels that were not found
        /// </summary>
        public List<int> NotFoundIds { get; set; }

        /// <summary>
        /// IDs of hotels that failed to delete with error messages
        /// </summary>
        public Dictionary<int, string> FailedIds { get; set; }

        /// <summary>
        /// Total number of hotels that were successfully deleted
        /// </summary>
        public int SuccessCount => SuccessfullyDeletedIds?.Count() ?? 0;

        /// <summary>
        /// Total number of hotels that were not found
        /// </summary>
        public int NotFoundCount => NotFoundIds?.Count() ?? 0;

        /// <summary>
        /// Total number of hotels that failed to delete
        /// </summary>
        public int FailedCount => FailedIds?.Count ?? 0;

        /// <summary>
        /// Indicates whether all requested deletions were successful
        /// </summary>
        public bool AllSuccessful => FailedCount == 0;
    }
}
