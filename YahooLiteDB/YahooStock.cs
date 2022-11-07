namespace YahooLiteDB;

using LiteDB;

public sealed class YahooStock
{
    public YahooStock()
    {
        PriceSet = new(new YahooStockPriceComparer());
    }

    public ObjectId? Id { get; set; } = ObjectId.Empty;

    public string Symbol { get; set; } = String.Empty;

    public IEnumerable<YahooStockPrice> Prices
    {
        get { return PriceSet; }
        set { PriceSet = new(value, new YahooStockPriceComparer()); }
    }

    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.Now;

    [BsonIgnore]
    internal SortedSet<YahooStockPrice> PriceSet { get; set; }

    [BsonIgnore]
    public DateOnly FirstStockDate => PriceSet.Min(s => s.Date);

    [BsonIgnore]
    public DateOnly LastStockDate => PriceSet.Max(s => s.Date);
}