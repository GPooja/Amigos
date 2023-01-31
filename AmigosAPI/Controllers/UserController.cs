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
using AmigosAPI.DTOs.Bill;
using AmigosAPI.DTOs.Summary;
using System.ComponentModel.DataAnnotations;

namespace AmigosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BillManagerContext _context;
        private UserService _userService;
        private SummaryService _summaryService;

        public UserController(BillManagerContext context, IConfiguration config)
        {
            _context = context;
            _userService = new UserService(context, config);
            _summaryService = new SummaryService(context, config);
        }

        // GET: api/User
        [HttpGet]
        public ActionResult<List<UserDTO>> GetUsers()
        {
            try { 
                return _userService.GetUsers();
            }
            catch (Exception ex)
            {
                return Problem("Unable to get users", "Users", (int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetUser(int ID)
        {
            UserDTO user = null;
            try
            {
                user = _userService.GetUserDTOByID(ID);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            return Ok(user);
        }

        [HttpPost, Route("GetUser")]
        public ActionResult<UserDTO> GetUser([EmailAddress] string email)
        {
            UserDTO user = null;
            try
            {
                user = _userService.GetUserDTOByEmail(email);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            return Ok(user);
        }

        [Route("AddUser")]
        [HttpPost]
        public ActionResult<UserDTO> AddUser(NewUserDTO user)
        {   
            try { 
                return _userService.AddUser(user);
            }
            catch (Exception ex)
            {
                return Problem("Unable to add user", "User", (int)HttpStatusCode.InternalServerError);
            }
        }

        [Route("AddUserMany")]
        [HttpPost]
        public ActionResult<List<UserDTO>> AddUser(List<NewUserDTO> users)
        {
            try
            {
                return _userService.AddUserMany(users);
            }
            catch (Exception ex)
            {
                return Problem("Unable to add user", "User", (int)HttpStatusCode.InternalServerError);
            }
        }


        [Route("EditUser")]
        [HttpPost]
        public ActionResult<UserDTO> EditUser(EditUserDTO user)
        {
            UserDTO userDTO = null;
            try { 
                userDTO = _userService.EditUser(user);
            }
            catch (Exception ex)
            {
                return Problem("Unable to edit user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (userDTO == null)
            {
                return BadRequest();
            }
            return userDTO;

        }

        // DELETE: api/User/5
        [HttpDelete]
        public ActionResult DeleteUser([EmailAddress]string email)
        {   
            User user = null;
            try { 
                user = _userService.GetUserByEmail(email);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            if (!_userService.DeleteUser(user))
            {
                return Ok();
            }
            return Problem("Unable to Delete User", "User", (int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        [Route("QuarterlySummaries")]
        public ActionResult<List<QuarterlySummaryDTO>> GetQuarterlySummary([EmailAddress] string userEmail, [StringLength(3)]string currency)
        {
            User user = null;
            try
            {
                user = _userService.GetUserByEmail(userEmail);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            var qsList = _summaryService.GetQuarterlySummaries(user, currency);
            if(qsList != null)
            {
                return Ok(qsList);
            }
            return Problem("Unable to Delete User", "User", (int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        [Route("UserShares")]
        public ActionResult<List<UserShareDTO>> GetUserShares([EmailAddress] string userEmail, [StringLength(3)] string currency)
        {
            User user = null;
            try
            {
                user = _userService.GetUserByEmail(userEmail);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            var qsList = _summaryService.GetUserShares(user, currency);
            if (qsList != null)
            {
                return Ok(qsList);
            }
            return Problem("Unable to Delete User", "User", (int)HttpStatusCode.InternalServerError);
        }
    }
}
