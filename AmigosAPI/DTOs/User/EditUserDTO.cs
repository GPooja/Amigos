using System.ComponentModel.DataAnnotations;

namespace AmigosAPI.DTOs.User
{
    public class EditUserDTO
    {
        [Required]
        public int UserID { get; set; }
        [StringLength(35)]
        public string FirstName { get; set; }
        [StringLength(35)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
    }
}
