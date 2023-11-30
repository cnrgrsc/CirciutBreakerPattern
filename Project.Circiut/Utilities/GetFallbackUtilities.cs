namespace Project.Circiut.Utilities
{
	public  static class GetFallbackUtilities
	{

		public static string GetFallbackResults()
		{
			return "{ \"message\": \"Service is currently unavailable. Please try again later.\" }";
		}
		//public static async Task<T> GetFallbackAsync<T>(Func<Task<T>> action, Func<Task<T>> fallbackAction, ILogger logger)
		//{
		//	try
		//	{
		//		return await action();
		//	}
		//	catch (BrokenCircuitException bce)
		//	{
		//		logger.LogWarning(bce, "Circuit Breaker is open. Falling back to fallback action.");
		//		return await fallbackAction();
		//	}
		//	catch (Exception ex)
		//	{
		//		logger.LogError(ex, "An error occurred while accessing Redis.");
		//		return await fallbackAction();
		//	}
		//}
	}
}
