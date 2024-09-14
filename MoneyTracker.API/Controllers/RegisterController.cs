using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Models.ControllerToService.Transaction;
using MoneyTracker.Shared.Models.ServiceToController.Transaction;
using MoneyTracker.Shared.Models.ServiceToRepository.Transaction;

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
        public Task<List<TransactionResponseDTO>> Get()
        {
            return _database.GetAllTransactions();
        }

        [HttpPost]
        [Route("add")]
        public Task<TransactionResponseDTO> Add([FromBody] NewTransactionRequestDTO newRegisterDTO)
        {
            return _database.AddTransaction(newRegisterDTO);
        }

        [HttpPut]
        [Route("edit")]
        public Task<TransactionResponseDTO> Edit([FromBody] EditTransactionRequestDTO editRegisterDTO)
        {
            return _database.EditTransaction(editRegisterDTO);
        }

        [HttpDelete]
        [Route("delete")]
        public Task<bool> Delete([FromBody] DeleteTransactionRequestDTO deleteTransaction)
        {
            return _database.DeleteTransaction(deleteTransaction);
        }
    }
}
