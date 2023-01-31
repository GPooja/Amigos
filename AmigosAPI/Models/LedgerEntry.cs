using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmigosAPI.Models
{
    public enum LedgerEntryType
    {
        Payment = 1,
        Loan = 2
    }

    [Table("AmigosLedger")]
    [PrimaryKey(nameof(ID))]
    public class LedgerEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public LedgerEntryType EntryType { get; set; }
        /* EntryType - Meaning
         * Payment   - PaidBy
         * Loan      - LentBy */
        [Required]
        [ForeignKey(nameof(GivenBy))]
        public int GivenByUserID { get; set; }
        /* EntryType - Meaning
         * Payment   - PaidTo
         * Loan      - LentTo */
        [Required]
        [ForeignKey(nameof(GivenTo))]
        public int GivenToUserID { get; set; }  

        /*--- Navigation ---*/
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public User GivenBy { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public User GivenTo { get; set; }
        public int? BillID { get; set; }
        public bool IsDeleted { get; set; }
    }
}
