namespace YahooLiteDB
{
    using System.Collections.Generic;

    internal class YahooStockPriceComparer : IComparer<YahooStockPrice>
    {
        public int Compare(YahooStockPrice? x, YahooStockPrice? y)
        {
            return x == null
                    ? y == null
                        ? 0
                        : -1
                    : y == null
                        ? 1
                        : x.Date.CompareTo(y.Date);
        }
    }
}