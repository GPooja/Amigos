using System.ComponentModel.DataAnnotations.Schema;

namespace AmigosAPI.Models
{
    public class Bill
    {
        public int ID { get; set; }
        public DateTime BillDate { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        [ForeignKey("Person")]
        public int PaidByPersonID { get; set; }
        [ForeignKey("Person")]
        public int CreatedByPersonID { get; set; }
        public DateTime Created { get; set; }
        [ForeignKey("Person")]
        public int ModifiedByPersonID { get; set; }
        public DateTime Modified { get; set; }

        /*--- Navigation ---*/
        [NotMapped]
        public Person PaidBy { get; set; }
        [NotMapped]
        public Person CreatedBy { get; set; }
        public ICollection<PersonBill> PersonBills { get; set; }
    }
}
