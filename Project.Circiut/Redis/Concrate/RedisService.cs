using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using Project.Circiut.CircuitBreaker;
using Project.Circiut.Redis.Abstract;
using Project.Circiut.Redis.RedisModels;
using StackExchange.Redis;

namespace Project.Circiut.Redis.Concrate
{
	public class RedisService : IRedisService
	{
		private readonly ConnectionMultiplexer _redisConnection;
		private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
		private readonly ILogger<RedisService> _logger;

		public RedisService(IOptions<RedisConfig> redisConfig, ILogger<RedisService> logger)
		{
			_logger = logger;
			_redisConnection = ConnectionMultiplexer.Connect(redisConfig.Value.ConnectionString);
			_circuitBreakerPolicy = CircuitPolicy.CreatePolicy(
				exceptionsAllowedBeforeBreaking: 5,
				durationOfBreak: TimeSpan.FromSeconds(30),
				logger: _logger // ILogger nesnesini CreatePolicy'e geçiriyoruz.
			);
		}

		public async Task<string> GetASync(string key)
		{
			return await _circuitBreakerPolicy.ExecuteAsync(async () =>
			{
				var db = _redisConnection.GetDatabase();
				return await db.StringGetAsync(key);
			});
		}

		public async Task SetAsync(string key, string value)
		{
			await _circuitBreakerPolicy.ExecuteAsync(async () =>
			{
				var db = _redisConnection.GetDatabase();
				await db.StringSetAsync(key, value);
			});
		}
	}
}
