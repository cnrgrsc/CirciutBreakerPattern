using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Circiut.Elasticsearch.Abstract;
using Project.Circiut.Models;
using Project.Circiut.Redis.Abstract;

namespace Project.Circiut.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DataController : ControllerBase
	{
		private readonly IRedisService _redisService;
		private readonly IElasticsearchService _elasticsearchService;

		public DataController(IRedisService redisService, IElasticsearchService elasticsearchService)
		{
			_redisService = redisService;
			_elasticsearchService = elasticsearchService;
		}

		[HttpPost]
		[Route("write")]
		public async Task<IActionResult> WriteData([FromBody] ProductData data)
		{
			// Redis'e veri yazma
			var redisKey = $"product:{data.Id}";
			await _redisService.SetAsync(redisKey, data.ToJson());

			// Elasticsearch'e veri yazma
			var result = await _elasticsearchService.IndexDocumentAsync("products", data.Id.ToString(), data);

			if (result)
			{
				return Ok(new { Message = "Data written to both Redis and Elasticsearch" });
			}
			else
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Data writing to Elasticsearch failed" });
			}
		}
	}
}
