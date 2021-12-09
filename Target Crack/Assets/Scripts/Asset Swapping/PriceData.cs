using Newtonsoft.Json;

public class PriceData
{
	[JsonProperty("currency")]
	public string Currency { get; set; }

	[JsonProperty("currencyType")]
	public string CurrencyType { get; set; }

	[JsonProperty("amount")]
	public int Amount { get; set; }

	[JsonProperty("scale")]
	public int Scale { get; set; }
}
