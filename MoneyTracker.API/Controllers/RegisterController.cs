using Microsoft.AspNetCore.Authorization;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterController(ILogger<RegisterController> logger, IRegisterService service,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _database = service;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("get")]
        [Authorize]
        public Task<List<TransactionResponseDTO>> Get()
        {
            var token = ControllerHelper.GetToken(_httpContextAccessor);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No token provided");
            }
            return _database.GetAllTransactions(token);
        }

        [HttpPost]
        [Route("add")]
        [Authorize]
        public Task Add([FromBody] NewTransactionRequestDTO newRegisterDTO)
        {
            var token = ControllerHelper.GetToken(_httpContextAccessor);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No token provided");
            }
            return _database.AddTransaction(token, newRegisterDTO);
        }

        [HttpPut]
        [Route("edit")]
        [Authorize]
        public Task Edit([FromBody] EditTransactionRequestDTO editRegisterDTO)
        {
            var token = ControllerHelper.GetToken(_httpContextAccessor);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No token provided");
            }
            return _database.EditTransaction(token, editRegisterDTO);
        }

        [HttpDelete]
        [Route("delete")]
        [Authorize]
        public Task Delete([FromBody] DeleteTransactionRequestDTO deleteTransaction)
        {
            var token = ControllerHelper.GetToken(_httpContextAccessor);
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No token provided");
            }
            return _database.DeleteTransaction(token, deleteTransaction);
        }
    }
}
