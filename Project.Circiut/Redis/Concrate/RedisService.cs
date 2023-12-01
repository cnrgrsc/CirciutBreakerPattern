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

			// Circuit Breaker politikasını oluşturun
			_circuitBreakerPolicy = CircuitPolicy.CreatePolicy(
				exceptionsAllowedBeforeBreaking: 1,
				durationOfBreak: TimeSpan.FromSeconds(30),
				logger: _logger
			);

			// Redis bağlanmayı dene ve hataları yakala
			try
			{
				_redisConnection = ConnectionMultiplexer.Connect(redisConfig.Value.ConnectionString);
			}
			catch (RedisConnectionException ex)
			{
				_logger.LogError(ex, "İlk Redis bağlantısı başarısız oldu.");
				// Redise yeniden bağlanmayı dene
			}
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
