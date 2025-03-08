using System.Globalization;

namespace Dapper.API.Helpers
{
    /// <summary>
    /// Helper class for date and time operations
    /// </summary>
    public class DateTimeHelper
    {
        /// <summary>
        /// Converts the current date and time to a Unix timestamp
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Converts a Unix timestamp to a DateTime object
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TimestampToDateTime(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
        }

        /// <summary>
        /// Formats a DateTime object to a string in the format "yyyy-MM-dd HH:mm:ss"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string FormatDateTimeInvariant(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
