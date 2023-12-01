using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using Project.Circiut.Elasticsearch.Abstract;
using Project.Circiut.Redis.Abstract;

namespace Project.Circiut.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SearchController : ControllerBase
	{
		private readonly IRedisService _redisService;
		private readonly IElasticsearchService _elasticsearchService;
		private readonly ILogger<SearchController> _logger;


		public SearchController(IRedisService redisService, IElasticsearchService elasticsearchService, ILogger<SearchController> logger)
		{
			_redisService = redisService;
			_elasticsearchService = elasticsearchService;
			_logger = logger;
		}

		[HttpGet("{query}")]
		public async Task<IActionResult> Search(string query)
		{
			string cachedResult = null;
			try
			{
				cachedResult = await _redisService.GetASync(query);
				if (!string.IsNullOrEmpty(cachedResult))
				{
					return Ok(cachedResult);
				}
			}
			catch(BrokenCircuitException bce)
			{
				// Redis Circuit Breaker açık ise bu bloğa girecek ve loglama yapılacak
				_logger.LogWarning(bce, "Redis Devre Kesici açık. Sorgu için Elasticsearch'e geri dönme: {Query}", query);
			}
			catch (Exception ex)
			{
				// Redis'te beklenmeyen bir hata oluştuğunda buraya düşer ve loglama yapılacak
				_logger.LogError(ex, "Sorgu için Redise erişilirken bir hata oluştu: {Query}", query);
			}
			try
			{
				// Eğer Redis'den veri alınamazsa veya Circuit Breaker açıksa Elasticsearch'e düşer
				var searchResult = await _elasticsearchService.SearchAsync(query);
				return Ok(searchResult);
			}
			catch (Exception ex)
			{
				// Elasticsearch'te beklenmeyen bir hata oluştuğunda buraya düşer ve loglama yapılacak
				_logger.LogError(ex, "Sorgu için Elasticsearch'e erişilirken bir hata oluştu: {Query}", query);
				// Burada hata döndürmek yerine boş sonuç dönmeyi tercih edebilirsiniz.
				return Ok(new { message = "Bir hata oluştu ve hiçbir sonuç alınamadı." });
			}
		}
	}
}
