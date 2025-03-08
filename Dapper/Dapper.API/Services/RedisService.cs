using Dapper.API.Services.Interfaces;
using StackExchange.Redis;

namespace Dapper.API.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly ILogger<RedisService> _logger;

        public RedisService
            (IConnectionMultiplexer multiplexer,
            ILogger<RedisService> logger)
        {
            _multiplexer = multiplexer;
            _logger = logger;
        }

        public async Task<bool> CheckRedisListContainsValue(string key, string value)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return false;
                }
                var cache = _multiplexer.GetDatabase();

                return await cache.SetContainsAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> AddToRedisList(string key, string value)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return false;
                }
                var cache = _multiplexer.GetDatabase();

                return await cache.SetAddAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return false;
            }
        }

        public async Task<List<string>> GetValuesFromRedisList(string key)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return new List<string>();
                }
                var cache = _multiplexer.GetDatabase();

                var result = await cache.SetMembersAsync(key);

                if (result == null)
                {
                    return new List<string>();
                }

                return result.Select(x => x.ToString()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return new List<string>();
            }
        }

        public async Task<bool> RemoveFromRedisList(string key, string value)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return false;
                }
                var cache = _multiplexer.GetDatabase();

                return await cache.SetRemoveAsync(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return false;
            }
        }

        public async Task<string> GetValueFromRedis(string key)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return string.Empty;
                }
                var cache = _multiplexer.GetDatabase();

                var result = await cache.StringGetAsync(key);

                if (result.IsNull)
                {
                    return string.Empty;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return string.Empty;
            }
        }

        public async Task<bool> AddValueToRedis(string key, string value, TimeSpan expiryTime)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return false;
                }
                var cache = _multiplexer.GetDatabase();

                var result = await cache.StringSetAsync(key, value);
                await cache.KeyExpireAsync(key, expiryTime);

                if (!result)
                {
                    return false;
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> UpsertRedisValue(string key, string value, TimeSpan expiryTime)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return false;
                }
                var cache = _multiplexer.GetDatabase();
                await cache.StringSetAsync(key, value);
                await cache.KeyExpireAsync(key, expiryTime);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRedisEntry(string key)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return false;
                }
                var cache = _multiplexer.GetDatabase();

                //var isDeleted = await cache.StringGetDeleteAsync(key);
                var isDeleted = await cache.KeyDeleteAsync(key);
                return isDeleted;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error:{Message}", ex.Message);
                return false;
            }
        }

        public async Task<List<string>> GetListOfValuesByPattern(string pattern)
        {
            if (!_multiplexer.IsConnected)
            {
                return new List<string>();
            }

            var server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());

            var keys = server.Keys(pattern: pattern + "*")
                .Select(k => k.ToString().Split(':').Last())
                            .ToList();

            return keys;
        }

        public async Task<List<string>> GetKeysAsync(string pattern)
        {
            try
            {
                if (!_multiplexer.IsConnected)
                {
                    return new List<string>();
                }

                var server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());
                var keys = new List<string>();

                await foreach (var key in server.KeysAsync(pattern: pattern))
                {
                    keys.Add(key.ToString());
                }

                return keys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting keys for pattern: {Pattern}", pattern);
                return new List<string>();
            }
        }
    }
}
