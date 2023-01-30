using AmigosAPI.Data;
using AmigosAPI.DTOs;
using AmigosAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
        public Task<BillDTO> EditBill(EditBillDTO dto) {
            return await _billService.EditBillAsync(dto);
        }
    }
}
