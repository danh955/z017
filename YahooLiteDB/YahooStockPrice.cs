namespace YahooLiteDB
{
    using System;

    public sealed class YahooStockPrice
    {
        public DateOnly Date { get; set; }

        public double? Open { get; set; }

        public double? High { get; set; }

        public double? Low { get; set; }

        public double? Close { get; set; }

        public double? AdjClose { get; set; }

        public long? Volume { get; set; }

        public override string ToString()
        {
            return $"{Date}, {Open}, {High}, {Low}, {Close}, {AdjClose}, {Volume}";
        }
    }
}