using AmigosAPI.Data;
using AmigosAPI.DTOs.Bill;
using AmigosAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AmigosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly BillManagerContext _context;
        private BillService _billService;

        public BillController(BillManagerContext context, IConfiguration config)
        {
            _context = context;
            _billService = new BillService(context,config);
        }

        [Route("AddBill")]
        [HttpPost]
        public async Task<BillDTO> AddBill(NewBillDTO newBill)
        {
            return await _billService.AddBillAsync(newBill);
        }

        [Route("EditBill")]
        [HttpPost]
        public async Task<BillDTO> EditBill(EditBillDTO dto) {
            return await _billService.EditBillAsync(dto);
        }

        [HttpDelete]
        public ActionResult DeleteBill(int billID) {
            var bill = _billService.GetBillByID(billID);
            if(bill == null)
            {
                return BadRequest("Bill Not Found");
            }
            if (_billService.DeleteBill(billID))
            {
                return Ok();
            };
            return Problem("Unable to Delete Bill", "Bill", (int)HttpStatusCode.InternalServerError);
        }
    }
}
