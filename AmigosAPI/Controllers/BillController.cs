using AmigosAPI.Data;
using AmigosAPI.DTOs.Bill;
using AmigosAPI.Models;
using AmigosAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AmigosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly BillManagerContext _context;
        private BillService _billService;
        private UserService _userService;

        public BillController(BillManagerContext context, IConfiguration config)
        {
            _context = context;
            _billService = new BillService(context, config);
            _userService = new UserService(context, config);
        }

        [Route("AddBill")]
        [HttpPost]
        public async Task<ActionResult<BillDTO>> AddBill(NewBillDTO newBill)
        {
            try
            {
                return await _billService.AddBillAsync(newBill);
            }
            catch (Exception ex)
            {
                return Problem("Unable to add bill", "Bills", (int)HttpStatusCode.InternalServerError);
            }
        }

        [Route("EditBill")]
        [HttpPost]
        public async Task<ActionResult<BillDTO>> EditBill(EditBillDTO dto)
        {
            try { 
                return await _billService.EditBillAsync(dto);
            }
            catch (Exception ex)
            {
                return Problem("Unable to edit bill", "Bills", (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        public ActionResult DeleteBill(int billID)
        {
            Bill bill = null;
            try
            {
                bill = _billService.GetBillByID(billID);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get bill", "Bill", (int)HttpStatusCode.InternalServerError);
            }

            if (bill == null)
            {
                return BadRequest("Bill Not Found");
            }
            if (_billService.DeleteBill(billID))
            {
                return Ok();
            };
            return Problem("Unable to Delete Bill", "Bill", (int)HttpStatusCode.InternalServerError);
        }

        [HttpPost, Route("BillHistoryByID")]
        public ActionResult<BillHistoryDTO> GetBillHistory(int userID, [StringLength(3)] string currency)
        {
            User user = null;
            try{
                user = _userService.GetUserByID(userID);
            }
            catch (Exception ex)
            {
                return Problem("Unable to get user", "User", (int)HttpStatusCode.InternalServerError);
            }
            if (user != null)
            {
                var billList = _billService.GetBillHistory(user, currency);
                if (billList != null)
                {
                    return Ok(billList);
                }
                else
                {
                    return Problem("Unable to retrieve data", "Bills", (int)HttpStatusCode.InternalServerError);
                }
            }
            return BadRequest("Cannot find user");
        }

        [HttpPost, Route("BillHistoryByEmail")]
        public ActionResult<BillHistoryDTO> GetBillHistory([EmailAddress] string userEmail, [StringLength(3)] string currency)
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
            if (user != null)
            {
                var billList = _billService.GetBillHistory(user, currency);
                if (billList != null)
                {
                    return Ok(billList);
                }
                else
                {
                    return Problem("Unable to retrieve data", "Bills", (int)HttpStatusCode.InternalServerError);
                }
            }
            return BadRequest("Cannot find user");
        }
    }
}
