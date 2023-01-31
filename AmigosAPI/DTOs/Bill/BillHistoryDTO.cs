namespace AmigosAPI.DTOs.Bill
{
    public class BillHistoryDTO
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public int TotalBills { get; set; }
        public double TotalBillAmount { get; set; }
        public List<BillDTO> Bills { get; set; }
    }
}
