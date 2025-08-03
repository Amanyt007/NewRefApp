namespace NewRefApp.Data.DTOs
{
    public class BalanceDetailsDto
    {
        public int userId { get; set; }
        public int vipLevel { get; set; }
        public decimal totalBalance { get; set; }
        public decimal rechargeBalance { get; set; }
        public decimal earningBalance { get; set; }
    }
}
