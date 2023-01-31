using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace AmigosAPI.Models
{
    [Table("Bill")]
    [PrimaryKey(nameof(ID))]
    public class Bill
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public DateTime BillDate { get; set; }
        public string Description { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required,MaxLength(3)]
        public string CurrencyCode { get; set; }
        public double AmountCAD { get; set; }
        public double ConversionRate { get; set; } //for 1 CAD
        [ForeignKey(nameof(PaidBy))]
        public int PaidByUserID { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public int CreatedByUserID { get; set; }
        public DateTime Created { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        public int ModifiedByUserID { get; set; }
        public DateTime Modified { get; set; }

        /*--- Navigation ---*/
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User PaidBy { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User CreatedBy { get; set; }
        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User ModifiedBy { get; set; }
        public bool IsDeleted { get; set; } //For Soft Delete
    }
}
