namespace SWP391_BE.DTO
{
    public class GrowthTrendsDTO
    {
        public List<GrowthTrendDataPointDTO> AverageHeight { get; set; }
        public List<GrowthTrendDataPointDTO> AverageWeight { get; set; }
    }

    public class GrowthTrendDataPointDTO
    {
        public string Month { get; set; }
        public double Value { get; set; }
        public int Count { get; set; }
    }
}