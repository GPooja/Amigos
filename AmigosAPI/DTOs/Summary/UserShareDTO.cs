using System.ComponentModel.DataAnnotations;

namespace AmigosAPI.DTOs.Summary
{
    public class UserShareDTO
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public double TotalAmountUserOwes { get; set; }
        public List<UserShare> UserShares { get; set; }
    }

    public class UserShare
    {
        public int BillID { get; set; }
        public double ShareAmount { get; set; }
        [StringLength(3)]
        public string CurrencyCode { get; set; }
        public string OwedToName { get; set; }
        public string OwedToEmail { get; set; }
    }
}
