using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using Project.Circiut.CircuitBreaker;
using Project.Circiut.Controllers;
using Project.Circiut.Elasticsearch.Abstract;
using Project.Circiut.Elasticsearch.Concrate;
using Project.Circiut.Elasticsearch.ElasticsearchModels;
using Project.Circiut.Redis.Abstract;
using Project.Circiut.Redis.Concrate;
using Project.Circiut.Redis.RedisModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<RedisConfig>(builder.Configuration.GetSection(nameof(RedisConfig)));
builder.Services.Configure<ElasticsearchConfig>(builder.Configuration.GetSection("ElasticsearchConfig"));

// Register RedisService with ILogger and IOptions<RedisConfig>
builder.Services.AddSingleton<IRedisService>(serviceProvider =>
{
	var logger = serviceProvider.GetRequiredService<ILogger<RedisService>>();
	var redisConfig = serviceProvider.GetRequiredService<IOptions<RedisConfig>>();
	return new RedisService(redisConfig, logger);
});

// Register ElasticsearchService with ILogger and IOptions<ElasticsearchConfig>
builder.Services.AddSingleton<IElasticsearchService>(serviceProvider =>
{
	var logger = serviceProvider.GetRequiredService<ILogger<ElasticsearchService>>();
	var elasticsearchConfig = serviceProvider.GetRequiredService<IOptions<ElasticsearchConfig>>();
	return new ElasticsearchService(elasticsearchConfig, logger);
});

builder.Services.AddSingleton<AsyncCircuitBreakerPolicy>(serviceProvider =>
{
	var logger = serviceProvider.GetRequiredService<ILogger<SearchController>>();
	return CircuitPolicy.CreatePolicy(5, TimeSpan.FromSeconds(30), logger);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
