using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models;
using MoneyTracker.Controllers;

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
    }
}
