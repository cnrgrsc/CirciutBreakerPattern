namespace Project.Circiut.Models
{
	public class ProductData
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public decimal Price { get; set; }

		public string ToJson()
		{
			return System.Text.Json.JsonSerializer.Serialize(this);
		}
	}
}
