using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace AmigosAPI.Models
{
    public class PersonBill
    {
        [Key, Column(Order = 0)]
        public int PersonID { get; set; }
        [Key, Column(Order = 1)]
        public int BillID { get; set; }

        public virtual Person Person { get; set; }
        public virtual Bill Bill { get; set; }

        public double Amount { get; set; }
        public bool IsSettled { get; set; }
    }
}
