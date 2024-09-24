using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;
using MoneyTracker.Shared.Models.ControllerToService.Budget;
using MoneyTracker.Shared.Models.RepositoryToService.Budget;
using MoneyTracker.Shared.Models.ServiceToController.Budget;
using MoneyTracker.Shared.Models.ServiceToRepository.Budget;

namespace MoneyTracker.Core;
public class BudgetService : IBudgetService
{
    private readonly IBudgetDatabase _dbService;

    public BudgetService(IBudgetDatabase dbService)
    {
        _dbService = dbService;
    }

    public async Task<List<BudgetGroupResponseDTO>> GetBudget()
    {
        return ConvertFromRepoDTOToDTO(await _dbService.GetBudget());
    }

    public async Task<BudgetCategoryResponseDTO> AddBudgetCategory(NewBudgetCategoryRequestDTO newBudget)
    {
        var dtoToDb = new NewBudgetCategoryDTO(newBudget.BudgetGroupId, newBudget.CategoryId, newBudget.Planned);
        var dtoFromDb = await _dbService.AddBudgetCategory(dtoToDb);
        return new BudgetCategoryResponseDTO(dtoFromDb.Name, dtoFromDb.Planned, dtoFromDb.Actual, dtoFromDb.Difference);
    }

    public async Task<List<BudgetGroupResponseDTO>> EditBudgetCategory(EditBudgetCategoryRequestDTO editBudgetCategory)
    {
        var dtoToDb = new EditBudgetCategoryDTO(editBudgetCategory.BudgetCategoryId, editBudgetCategory.BudgetGroupId, editBudgetCategory.BudgetCategoryPlanned);
        return ConvertFromRepoDTOToDTO(await _dbService.EditBudgetCategory(dtoToDb));
    }

    public Task<bool> DeleteBudgetCategory(DeleteBudgetCategoryRequestDTO deleteBudgetCategory)
    {
        var dtoToDb = new DeleteBudgetCategoryDTO(deleteBudgetCategory.BudgetCategoryId);
        return _dbService.DeleteBudgetCategory(dtoToDb);
    }

    private List<BudgetGroupResponseDTO> ConvertFromRepoDTOToDTO(List<BudgetGroupEntityDTO> billRepoDTO)
    {
        List<BudgetGroupResponseDTO> res = [];
        foreach (var bill in billRepoDTO)
        {
            List<BudgetCategoryResponseDTO> tmpCategoryLis = [];
            foreach (var category in bill.Categories)
            {
                tmpCategoryLis.Add(new(category.Name, category.Planned, category.Actual, category.Difference));
            }

            res.Add(new BudgetGroupResponseDTO(
                bill.Name,
                bill.Planned,
                bill.Actual,
                bill.Difference,
                tmpCategoryLis
           ));
        }

        return res;
    }
}
