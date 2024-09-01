using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Data.Global;
using MoneyTracker.Shared.Models.Budget;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("/api/budget/")]
    public class BudgetController : ControllerBase
    {

        private readonly ILogger<BudgetController> _logger;
        private readonly IBudget _database;

        public BudgetController(ILogger<BudgetController> logger, IBudget db)
        {
            _logger = logger;
            _database = db;
        }

        [HttpGet]
        [Route("get")]
        public Task<List<BudgetGroupDTO>> Get()
        {
            return _database.GetBudget();
        }

        [HttpPost]
        [Route("category/add")]
        public Task<BudgetCategoryDTO> AddBudgetCategory([FromBody] NewBudgetCategoryDTO newBudget)
        {
            return _database.AddBudgetCategory(newBudget);
        }

        [HttpPut]
        [Route("category/edit")]
        public Task<List<BudgetGroupDTO>> EditBudgetCategory([FromBody] EditBudgetCategory editBudgetCategory)
        {
            return _database.EditBudgetCategory(editBudgetCategory);
        }

        [HttpDelete]
        [Route("category/delete")]
        public Task<bool> DeleteBudgetCategory([FromBody] DeleteBudgetCategory deleteBudgetCategory)
        {
            return _database.DeleteBudgetCategory(deleteBudgetCategory);
        }
    }
}
