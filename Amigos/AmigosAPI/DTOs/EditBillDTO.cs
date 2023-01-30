using System.ComponentModel.DataAnnotations;

namespace AmigosAPI.DTOs
{
    public class EditBillDTO
    {
        [Required]
        public int ID { get; set; }
        public DateTime BillDate { get; set; }
        public double BillAmount { get; set; }
        public string Currency { get; set; }
        public string PaidByEmail { get; set; }
        public List<string> SharedByEmails { get; set; }
        public string Description { get; set; }
    }
}
