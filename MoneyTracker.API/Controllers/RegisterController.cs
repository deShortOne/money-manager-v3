using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.Transaction;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/register/")]
    public class RegisterController : ControllerBase
    {

        private readonly ILogger<RegisterController> _logger;
        private readonly IRegisterService _database;

        public RegisterController(ILogger<RegisterController> logger, IRegisterService service)
        {
            _logger = logger;
            _database = service;
        }

        [HttpGet]
        [Route("get")]
        public Task<List<TransactionDTO>> Get()
        {
            return _database.GetAllTransactions();
        }

        [HttpPost]
        [Route("add")]
        public Task<TransactionDTO> Add([FromBody] NewTransactionDTO newRegisterDTO)
        {
            return _database.AddTransaction(newRegisterDTO);
        }

        [HttpPut]
        [Route("edit")]
        public Task<TransactionDTO> Edit([FromBody] EditTransactionDTO editRegisterDTO)
        {
            return _database.EditTransaction(editRegisterDTO);
        }

        [HttpDelete]
        [Route("delete")]
        public Task<bool> Delete([FromBody] DeleteTransactionDTO deleteTransaction)
        {
            return _database.DeleteTransaction(deleteTransaction);
        }
    }
}
