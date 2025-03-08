namespace Dapper.API.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    namespace Scheduler.API.Exceptions
    {
        /// <summary>
        /// Exception thrown when data access operations fail at the database level
        /// </summary>
        [Serializable]
        public class DataAccessException : Exception
        {
            /// <summary>
            /// Gets the database operation that failed
            /// </summary>
            public string Operation { get; }

            /// <summary>
            /// Initializes a new instance of the DataAccessException class
            /// </summary>
            public DataAccessException()
                : base("A database operation failed")
            {
            }

            /// <summary>
            /// Initializes a new instance of the DataAccessException class with a specified error message
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception</param>
            public DataAccessException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the DataAccessException class with a specified error message
            /// and a reference to the inner exception that is the cause of this exception
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception</param>
            /// <param name="innerException">The exception that is the cause of the current exception</param>
            public DataAccessException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            /// <summary>
            /// Initializes a new instance of the DataAccessException class with operation information
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception</param>
            /// <param name="operation">The database operation that failed</param>
            /// <param name="innerException">The exception that is the cause of the current exception</param>
            public DataAccessException(string message, string operation, Exception innerException)
                : base(message, innerException)
            {
                Operation = operation;
            }

            /// <summary>
            /// Initializes a new instance of the DataAccessException class with serialized data
            /// </summary>
            /// <param name="info">The SerializationInfo that holds the serialized object data</param>
            /// <param name="context">The StreamingContext that contains contextual information</param>
            protected DataAccessException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                Operation = info.GetString(nameof(Operation));
            }

            /// <summary>
            /// Sets the SerializationInfo with information about the exception
            /// </summary>
            /// <param name="info">The SerializationInfo that holds the serialized object data</param>
            /// <param name="context">The StreamingContext that contains contextual information</param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue(nameof(Operation), Operation);
            }
        }
    }
}
