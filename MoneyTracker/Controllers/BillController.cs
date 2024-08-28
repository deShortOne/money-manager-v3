using Microsoft.AspNetCore.Mvc;
using MoneyTracker.API.Database;
using MoneyTracker.API.Models;
using MoneyTracker.Controllers;

namespace MoneyTracker.API.Controllers
{
    public class BillController : ControllerBase
    {

        private readonly ILogger<BillController> _logger;

        public BillController(ILogger<BillController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public Task<IEnumerable<BillDTO>> Get()
        {
            return null;
        }
    }
}
