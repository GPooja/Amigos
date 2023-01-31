using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace AmigosAPI.DTOs.User
{
    public class NewUserDTO
    {
        [StringLength(35)]
        public string FirstName { get; set; }
        [StringLength(35)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(3)]
        public string DefaultCurrency { get; set; }
    }
}
