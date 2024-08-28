using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models.Bill;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/bill/")]
    public class BillController : ControllerBase
    {

        private readonly ILogger<BillController> _logger;

        public BillController(ILogger<BillController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public Task<List<BillDTO>> Get()
        {
            return new Bill().GetAllBills();
        }

        [HttpPost]
        [Route("add")]
        public Task<BillDTO> Add([FromBody] NewBillDTO newBillDTO)
        {
            return new Bill().AddNewBill(newBillDTO);
        }
    }
}
