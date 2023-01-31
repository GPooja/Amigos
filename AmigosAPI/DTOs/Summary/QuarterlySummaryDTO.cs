namespace AmigosAPI.DTOs.Summary
{
    public class QuarterlySummaryDTO
    {
        public int UserID { get; set; }
        public string CurrencyCode { get; set; }
        public List<QuarterlyData> Quarters { get; set; }

    }

    public class QuarterlyData
    {
        public string QuarterName { get; set; }
        public double TotalAmountOnBills { get; set; }
        public double AmountUserOwes { get; set; }
        public double OthersOweUser { get; set; }
    }
}
