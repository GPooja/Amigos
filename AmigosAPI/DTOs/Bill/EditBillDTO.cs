using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace AmigosAPI.DTOs.Bill
{
    public class EditBillDTO
    {
        [Required]
        public int ID { get; set; }
        public DateTime BillDate { get; set; }
        public double BillAmount { get; set; }
        [StringLength(3)]
        public string CurrencyCode { get; set; }
        [EmailAddress]
        public string PaidByEmail { get; set; }        
        public List<string> SharedByEmails { get; set; }
        public string Description { get; set; }
    }
}
