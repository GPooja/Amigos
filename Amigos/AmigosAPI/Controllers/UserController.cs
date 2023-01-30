using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmigosAPI.Data;
using AmigosAPI.Models;
using AmigosAPI.Services;
using AmigosAPI.DTOs;

namespace AmigosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BillManagerContext _context;
        private UserService _userService;

        public UserController(BillManagerContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        // GET: api/User
        [HttpGet]
        public List<UserDTO> GetUsers()
        {
            return _userService.GetUsers();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public User GetUser(string email)
        {
            return null;
        }

        [HttpPost]
        public User AddUser(NewUserDTO user)
        {
            return _userService.AddUser(user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public void DeleteUser(string email)
        {
            
        }

        [HttpGet]
        [Route("QuarterlySummaries")]
        public Object GetQuarterlySummary(string userEmail)
        {
            return null;
        }
    }
}
