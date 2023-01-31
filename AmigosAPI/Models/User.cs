using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;

namespace AmigosAPI.Models
{
    [Table("User")]
    [PrimaryKey(nameof(ID))]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required, StringLength(35)]
        public string FirstName { get; set; }
        [Required, StringLength(35)]
        public string LastName { get; set; }
        [Required, StringLength(255)]
        public string Email { get; set; }
        [Required, StringLength(3)]
        public string DefaultCurrency { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool IsDeleted { get; set; } //For SoftDelete

    }
}
