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
using AmigosAPI.DTOs.User;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

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

        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetUser(int ID)
        {
            var user = _userService.GetUserDTOByID(ID);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            return Ok(user);
        }

        [HttpPost, Route("GetUser")]
        public ActionResult<UserDTO> GetUser(string email)
        {
            var user = _userService.GetUserDTOByEmail(email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            return Ok(user);
        }

        [Route("AddUser")]
        [HttpPost]
        public UserDTO AddUser(NewUserDTO user)
        {
            return _userService.AddUser(user);
        }

        [Route("EditUser")]
        [HttpPost]
        public ActionResult<UserDTO> EditUser(EditUserDTO user)
        {

            var userDTO = _userService.EditUser(user);
            if (userDTO == null)
            {
                return BadRequest();
            }
            return userDTO;

        }

        // DELETE: api/User/5
        [HttpDelete]
        public ActionResult DeleteUser(string email)
        {
            var user = _userService.GetUserByEmail(email);
            if(user == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_userService.DeleteUser(user))
            {
                return Ok();               
            }
            return Problem("Unable to Delete User", "User", (int)HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        [Route("QuarterlySummaries")]
        public Object GetQuarterlySummary(string userEmail)
        {
            return null;
        }
    }
}
