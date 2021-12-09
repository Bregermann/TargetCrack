using Newtonsoft.Json;

public class CreatorCoinData
{
	[JsonProperty("symbol")]
	public string Symbol { get; set; }

	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("creatorAccountId")]
	public string CreatorAccountId { get; set; }

	[JsonProperty("imageUrl")]
	public string ImageUrl { get; set; }

	[JsonProperty("price")]
	public PriceData Price { get; set; }

	public override string ToString()
	{
		return $"{Symbol}\n{Name}\n{CreatorAccountId}\n{ImageUrl}\n{Price.Currency}\n{Price.CurrencyType}\n{Price.Amount}\n{Price.Scale}";
	}
}
