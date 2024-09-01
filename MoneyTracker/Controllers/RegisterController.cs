using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Models.Transaction;

namespace MoneyTracker.API.Controllers
{
    [ApiController]
    [Route("/api/register/")]
    public class RegisterController : ControllerBase
    {

        private readonly ILogger<RegisterController> _logger;

        public RegisterController(ILogger<RegisterController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public Task<List<TransactionDTO>> Get()
        {
            return new Register().GetAllTransactions();
        }

        [HttpPost]
        [Route("add")]
        public Task<TransactionDTO> Add([FromBody] NewTransactionDTO newRegisterDTO)
        {
            return new Register().AddNewTransaction(newRegisterDTO);
        }

        [HttpPut]
        [Route("edit")]
        public Task<TransactionDTO> Edit([FromBody] EditTransactionDTO editRegisterDTO)
        {
            return new Register().EditTransaction(editRegisterDTO);
        }

        [HttpDelete]
        [Route("delete")]
        public Task<bool> Delete([FromBody] DeleteTransactionDTO deleteTransaction)
        {
            return new Register().DeleteTransaction(deleteTransaction);
        }
    }
}
