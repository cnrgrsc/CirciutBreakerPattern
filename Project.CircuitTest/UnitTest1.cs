using FluentAssertions;
using Moq;
using Project.Circiut.Redis.Abstract;

namespace Project.CircuitTest
{
	public class UnitTest1
	{
		[Fact]
		public async Task GetAsync_ReturnsCachedValue_WhenKeyExists()
		{
			// Arrange
			var cacheKey = "product:1";
			var expectedValue = "{\"name\":\"Sample Product\"}";
			var mockRedisService = new Mock<IRedisService>();
			mockRedisService.Setup(service => service.GetASync(cacheKey))
							.ReturnsAsync(expectedValue);

			var redisService = mockRedisService.Object;

			// Act
			var actualValue = await redisService.GetASync(cacheKey);

			// Assert
			actualValue.Should().Be(expectedValue);
		}
	}
}