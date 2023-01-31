using System.ComponentModel.DataAnnotations;

namespace AmigosAPI.DTOs.Bill
{
    public class NewBillDTO
    {
        [Required]
        public DateTime BillDate { get; set; }
        [Required]
        public double BillAmount { get; set; }
        [Required, StringLength(3)]
        public string CurrencyCode { get; set; }
        [Required,EmailAddress]
        public string PaidByEmail { get; set; }
        [Required]
        public List<string> SharedByEmails { get; set; }
        public string Description { get; set; }
    }
}
