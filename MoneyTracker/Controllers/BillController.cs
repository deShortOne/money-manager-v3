using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Bill;

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

        [HttpPut]
        [Route("edit")]
        public Task<BillDTO> Edit([FromBody] EditBillDTO editBillDTO)
        {
            return new Bill().EditBill(editBillDTO);
        }

        [HttpDelete]
        [Route("delete")]
        public Task<bool> Delete([FromBody] DeleteBillDTO billId)
        {
            return new Bill().DeleteBill(billId);
        }
    }
}
