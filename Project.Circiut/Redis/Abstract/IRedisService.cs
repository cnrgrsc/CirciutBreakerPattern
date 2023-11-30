namespace Project.Circiut.Redis.Abstract
{
	public interface IRedisService
	{
		Task<string> GetASync(string key);
		Task SetAsync(string key, string value);
	}
}
