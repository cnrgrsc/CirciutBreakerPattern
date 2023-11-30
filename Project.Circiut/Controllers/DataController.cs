using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Circiut.Elasticsearch.Abstract;
using Project.Circiut.Models;
using Project.Circiut.Redis.Abstract;
using Project.Circiut.Utilities;

namespace Project.Circiut.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DataController : ControllerBase
	{
		private readonly IRedisService _redisService;
		private readonly IElasticsearchService _elasticsearchService;
		private readonly ILogger<DataController> _logger;

		public DataController(IRedisService redisService, IElasticsearchService elasticsearchService, ILogger<DataController> logger)
		{
			_redisService = redisService;
			_elasticsearchService = elasticsearchService;
			_logger = logger;
		}

		[HttpPost]
		[Route("write")]
		public async Task<IActionResult> WriteData([FromBody] ProductData data)
		{
			var redisKey = $"product:{data.Id}";

			try
			{
				// Redis'e veri yazmayı dene
				await _redisService.SetAsync(redisKey, data.ToJson());
				return Ok(new { Message = "Data written to Redis" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to write data to Redis, attempting to write to Elasticsearch.");
				try
				{
					// Redis'e yazma başarısız oldu, Elasticsearch'e yaz
					var result = await _elasticsearchService.IndexDocumentAsync("products", data.Id.ToString(), data);
					if (result)
					{
						return Ok(new { Message = "Data written to Elasticsearch" });
					}
					else
					{
						_logger.LogError("Failed to write data to Elasticsearch.");
						return StatusCode(StatusCodes.Status503ServiceUnavailable, GetFallbackUtilities.GetFallbackResults());
					}
				}
				catch (Exception exElastic)
				{
					// Elasticsearch'e yazma da başarısız oldu
					_logger.LogError(exElastic, "Failed to write data to both Redis and Elasticsearch.");
					return StatusCode(StatusCodes.Status503ServiceUnavailable, GetFallbackUtilities.GetFallbackResults());
				}
			}
		}
	}
}
