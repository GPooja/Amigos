using System.Reflection.Metadata.Ecma335;

namespace AmigosAPI.DTOs.User
{
    public class UserDTO
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
