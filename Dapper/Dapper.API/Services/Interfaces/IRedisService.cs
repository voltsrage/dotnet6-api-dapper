namespace Dapper.API.Services.Interfaces
{
    public interface IRedisService
    {
        /// <summary>
        /// Add entry to redis
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        Task<bool> AddValueToRedis(string key, string value, TimeSpan expiryTime);

        /// <summary>
        /// Get value from redis if exists
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> GetValueFromRedis(string key);

        /// <summary>
        /// Create new redis entry if not exist, update if already there
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiryTime"></param>
        /// <returns></returns>
        Task<bool> UpsertRedisValue(string key, string value, TimeSpan expiryTime);

        /// <summary>
        /// Delete redis entry by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> DeleteRedisEntry(string key);

        /// <summary>
        /// Add value to redis list
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddToRedisList(string key, string value);

        /// <summary>
        /// Remove value from redis list
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> RemoveFromRedisList(string key, string value);

        /// <summary>
        /// Check if value exists in redis list
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> CheckRedisListContainsValue(string key, string value);

        /// <summary>
        /// Use pattern to get list of values from redis
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        Task<List<string>> GetListOfValuesByPattern(string pattern);

        /// <summary>
        /// Get values from redis list
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<string>> GetValuesFromRedisList(string key);

        /// <summary>
        /// Get list of keys from redis using pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        Task<List<string>> GetKeysAsync(string pattern);
    }
}
