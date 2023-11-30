using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Project.Circiut.Elasticsearch.Abstract;
using Project.Circiut.Elasticsearch.ElasticsearchModels;

namespace Project.Circiut.Elasticsearch.Concrate
{
	public class ElasticsearchService : IElasticsearchService
	{
		private readonly ElasticLowLevelClient _elasticClient;
		private readonly ILogger _logger;

		public ElasticsearchService(IOptions<ElasticsearchConfig> elasticsearchConfig, ILogger<ElasticsearchService> logger)
		{
			var settings = new ConnectionConfiguration(new Uri(elasticsearchConfig.Value.Uri));
			_elasticClient = new ElasticLowLevelClient(settings);
			_logger = logger;
		}

		public async Task<bool> IndexDocumentAsync(string index, string id, object document)
		{
			var response = await _elasticClient.IndexAsync<StringResponse>(index, id, PostData.Serializable(document));
			if (!response.Success)
			{
				_logger.LogError("Failed to index document {Document} in index {Index}: {ErrorMessage}", document, index, response.DebugInformation);
				return false;
			}
			return true;
		}

		public async Task<string> SearchAsync(string query)
		{
			try
			{
				var response = await _elasticClient.SearchAsync<StringResponse>(index: "products", query);
				return response.Body;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while executing Elasticsearch query: {Query}", query);
				throw;
			}
			
		}
	}
}
