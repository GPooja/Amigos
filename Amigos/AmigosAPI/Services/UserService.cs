using System.Threading.Tasks;
using AmigosAPI.Data;
using AmigosAPI.DTOs;
using AmigosAPI.Models;

namespace AmigosAPI.Services
{
    public class UserService
    {
        private readonly BillManagerContext _context;
        public UserService(BillManagerContext context) 
        {
            _context = context;
        }

        public User AddUser(NewUserDTO newUser)
        {
            User builtUser = new User()
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                DefaultCurrency = newUser.DefaultCurrency
            };
            var user = _context.Users.Add(builtUser);
             _context.SaveChanges();
            return user.Entity;
        }

        public List<UserDTO> GetUsers()
        {
            return _context.Users.Select(
                p => GetUserDTO(p)).ToList();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(p => p.Email == email);
        }

        public UserDTO GetUserDTO(User user)
        {
            return new UserDTO()
            {
                ID = user.ID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DefaultCurrency = user.DefaultCurrency
            };
        }

    }
}
