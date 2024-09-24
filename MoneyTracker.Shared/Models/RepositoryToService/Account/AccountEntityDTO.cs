
namespace MoneyTracker.Shared.Models.RepositoryToService.Account;
public class AccountEntityDTO(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
