using AmigosAPI.DTOs.Ledger;
using AmigosAPI.DTOs.User;

namespace AmigosAPI.DTOs.Bill
{
    public class BillDTO
    {
        public int ID { get; set; }
        public DateTime BillDate { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
        public double AmountInCAD { get; set; }
        public double ConversionRate { get; set; }
        public UserDTO PaidBy { get; set; }
        public List<BillShareDTO> BillShares { get; set; }
    }
}
