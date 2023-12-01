using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;
using System;

namespace Project.Circiut.CircuitBreaker
{
	public static class CircuitPolicy
	{
		public static AsyncCircuitBreakerPolicy CreatePolicy(
		int exceptionsAllowedBeforeBreaking,
		TimeSpan durationOfBreak,
		ILogger logger)
		{
			// Policy kütüphanesini kullanarak bir Circuit Breaker politikası oluşturuluyor.
			return Policy
				// RedisConnectionException hatasını yakala.
				.Handle<RedisConnectionException>()
				// Herhangi bir türde Exception yakala.
				.Or<Exception>()
				// Belirli sayıda hata alındığında devreye girer.
				.CircuitBreakerAsync(
					exceptionsAllowedBeforeBreaking, // Circuit Breaker'ın açılması için izin verilen hata sayısı.
					durationOfBreak, // Circuit Breaker açık kaldığında ne kadar süre geçmesi gerektiği.
					onBreak: (exception, breakDelay) =>
					{
						// Circuit Breaker açıldığında çalışacak kod bloğu.
						// Loglama: Circuit Breaker açıldığında bir log mesajı yaz.
						logger.LogError($"Circuit breaker opened for {breakDelay.TotalSeconds} seconds due to: {exception.Message}");

						
					},
					onReset: () =>
					{
						// Circuit Breaker normale döndüğünde çalışacak kod bloğu.
						// Loglama: Circuit Breaker normale döndüğünde bir log mesajı yaz.
						logger.LogInformation("Circuit breaker reset.");
					},
					onHalfOpen: () =>
					{
						// Circuit Breaker yarı açık (half-open) durumuna geçtiğinde çalışacak kod bloğu.
						// Bu durum, Circuit Breaker'ın yeniden etkinleşmeye hazır olduğu ancak tam olarak kapanmadığı anlamına gelir.
						// Loglama: Circuit Breaker yarı açık durumuna geçtiğinde bir log mesajı yaz.
						logger.LogInformation("Circuit breaker is half-open. Next call is a trial.");
					}
				);
		}
	}
}
