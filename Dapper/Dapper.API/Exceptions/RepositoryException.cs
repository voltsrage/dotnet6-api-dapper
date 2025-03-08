namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Repository Layer Exceptions
    /// </summary>
    public class RepositoryException : BaseException
    {
        /// <summary>
        /// The repository that the exception occurred in
        /// </summary>
        public string Repository { get; }

        /// <summary>
        /// The operation that the exception occurred in
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Used to create an instance of the RepositoryException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="repository"></param>
        /// <param name="function"></param>
        /// <param name="operation"></param>
        /// <param name="innerException"></param>
        /// <param name="additionalData"></param>
        public RepositoryException(
            string message,
            string repository,
            string function,
            string operation,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, "REPO_ERROR", repository, function, innerException, additionalData)
        {
            Repository = repository;
            Operation = operation;
        }
    }
}
