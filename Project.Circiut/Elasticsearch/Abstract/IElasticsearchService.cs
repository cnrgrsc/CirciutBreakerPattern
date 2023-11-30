namespace Project.Circiut.Elasticsearch.Abstract
{
	public interface IElasticsearchService
	{
		Task<string> SearchAsync(string query);

		Task<bool> IndexDocumentAsync(string index, string id, object document);
	}
}
