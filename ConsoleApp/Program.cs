using System.Reflection;
using YahooLiteDB;

using var yahooDb = new YahooLiteDatabase(@"C:\Code\DB\Yahoo.LiteDb", new(2020, 1, 1));
await GetAllSP500(yahooDb);

static async Task GetAllSP500(YahooLiteDatabase yahooDb)
{
    var stream = Assembly
                   .GetExecutingAssembly()
                   .GetManifestResourceStream("ConsoleApp.SP500SymbolsAsOf20221025.txt")
                   ?? throw new NullReferenceException();

    using var reader = new StreamReader(stream);

    string? symbol;
    while ((symbol = reader.ReadLine()) != null)
    {
        var data = await yahooDb.GetStockAsync(symbol);
        if (data == null)
        {
            Console.WriteLine($"{symbol} not found");
        }
        else
        {
            Console.WriteLine($"{data.Symbol,-6} as {data.Prices.Count()} prices.");
        }
    }
}