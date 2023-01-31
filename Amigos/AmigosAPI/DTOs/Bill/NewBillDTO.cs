using Microsoft.Build.Framework;

namespace AmigosAPI.DTOs.Bill
{
    public class NewBillDTO
    {
        [Required]
        public DateTime BillDate { get; set; }
        [Required]
        public double BillAmount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string PaidByEmail { get; set; }
        [Required]
        public List<string> SharedByEmails { get; set; }
        public string Description { get; set; }
    }
}
