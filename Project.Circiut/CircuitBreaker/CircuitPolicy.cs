using Polly;
using Polly.CircuitBreaker;
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
			return Policy
				.Handle<Exception>()
				.CircuitBreakerAsync(
					exceptionsAllowedBeforeBreaking,
					durationOfBreak,
					onBreak: (exception, breakDelay) =>
					{
						// Loglama örneği:
						logger.LogError($"Circuit breaker opened for {breakDelay.TotalSeconds} seconds due to: {exception.Message}");
					},
					onReset: () =>
					{
						// Circuit Breaker normale döndüğünde yapılacak işlemler:
						logger.LogInformation("Circuit breaker reset.");
					},
					onHalfOpen: () =>
					{
						// Circuit Breaker yarı açık duruma geçtiğinde yapılacak işlemler:
						logger.LogInformation("Circuit breaker is half-open. Next call is a trial.");
					}
				);
		}
	}
}
