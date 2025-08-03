namespace NewRefApp.Data.DTOs
{
    public class InvestmentPlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string RevenueDurationValue { get; set; }
        public int? VipLevel { get; set; }
        public decimal? DailyEarningsPerUnit { get; set; }
        public decimal? HourlyEarningsPerUnit { get; set; }
        public decimal InvestmentAmount { get; set; }
        public IFormFile ImageFile { get; set; }
        public string ExistingImagePath { get; set; }
    }
}
