using System.Threading.Tasks;
using AmigosAPI.Data;
using AmigosAPI.DTOs.User;
using AmigosAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AmigosAPI.Services
{
    public class UserService
    {
        private readonly BillManagerContext _context;
        public UserService(BillManagerContext context) 
        {
            _context = context;
        }

        public UserDTO AddUser(NewUserDTO newUser)
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
            return GetUserDTO(user.Entity);
        }

        public List<UserDTO> GetUsers()
        {
            return _context.Users.Select(
                p => GetUserDTO(p)).ToList();
        }

        public UserDTO EditUser(EditUserDTO userDTO)
        {
            var userInDB = _context.Users.Where(u => u.ID == userDTO.UserID).SingleOrDefault();
            if (userInDB != null)
            {
                User user = new User()
                {
                    ID = userDTO.UserID,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    DefaultCurrency = userDTO.DefaultCurrency,
                    Modified = DateTime.Now
                };
                _context.Entry(userInDB).CurrentValues.SetValues(user);
                _context.SaveChanges();
                return GetUserDTO(user);
            }
            return null;
        }
        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(p => p.Email == email);
        }

        public UserDTO GetUserDTOByEmail(string email)
        {
            return GetUserDTO(GetUserByEmail(email));
        }

        public User GetUserByID(int ID)
        {
            return _context.Users.SingleOrDefault(p => p.ID == ID);
        }

        public UserDTO GetUserDTOByID(int ID)
        {
            return GetUserDTO(GetUserByID(ID));
        }

        public UserDTO GetUserDTO(User user) {
            if (user != null)
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
            return null;
        }

        public bool DeleteUser(User user)
        {
            user.IsDeleted = true;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }        
    }
}
