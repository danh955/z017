namespace YahooLiteDB;

using System;
using Hilres.Yahoo.ApiClient;
using LiteDB;

public sealed class YahooLiteDatabase : IDisposable
{
    private readonly LiteDatabase db;
    private readonly ILiteCollection<YahooStock> stocks;
    private readonly YahooClient yahooClient;

    public YahooLiteDatabase(string databasePathName, DateOnly? firstDate = null)
    {
        db = new LiteDatabase(databasePathName, GetMapper());
        stocks = db.GetCollection<YahooStock>("YahooStocks");
        stocks.EnsureIndex(s => s.Symbol, unique: true);

        yahooClient = new YahooClient();

        FirstDate = firstDate;
    }

    /// <summary>
    /// Fist date to retrieve data.  Null for all data.
    /// </summary>
    public DateOnly? FirstDate { get; set; }

    public void Dispose()
    {
        ((IDisposable)db).Dispose();
    }

    /// <summary>
    /// Get a stock by its symbol name.
    /// </summary>
    /// <param name="symbol">Stock symbol.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Yahoo stock.  Null if not found.</returns>
    public async Task<YahooStock?> GetStockAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        symbol = symbol.Trim().ToUpper();

        var data = stocks.Query().Where(s => s.Symbol == symbol).FirstOrDefault();
        if (data == null)
        {
            return await GetStockFromYahoo(symbol, cancellationToken).ConfigureAwait(false);
        }

        return data;
    }

    private static BsonMapper GetMapper()
    {
        var mapper = new BsonMapper();

        mapper.RegisterType<DateOnly>(
            value => value.Year * 10000 + value.Month * 100 + value.Day,
            bson =>
            {
                var value = (int)bson;
                var year = value / 10000;
                var month = value % 10000 / 100;
                var day = value % 100;
                return new DateOnly(year, month, day);
            });

        return mapper;
    }

    private async Task<YahooStock?> GetStockFromYahoo(string symbol, CancellationToken cancellationToken)
    {
        var parser = await yahooClient.GetPricesParserAsync(symbol, FirstDate, null, cancellationToken: cancellationToken);
        if (parser.IsSuccessful)
        {
            var stock = new YahooStock()
            {
                Symbol = symbol,
            };

            await foreach (var item in parser.Prices.ConfigureAwait(false))
            {
                stock.PriceSet.Add(new YahooStockPrice()
                {
                    Date = item.Date,
                    Open = item.Open,
                    High = item.High,
                    Low = item.Low,
                    Close = item.Close,
                    AdjClose = item.AdjClose,
                    Volume = item.Volume,
                });
            }

            stocks.Insert(stock);
            return stock;
        }

        return null;
    }
}