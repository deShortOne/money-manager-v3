using MoneyTracker.Core;
using MoneyTracker.Data.Postgres;
using MoneyTracker.Shared.Core;
using MoneyTracker.Shared.Data;

namespace MoneyTracker.API;

public class Startup
{
    public static void SetBillDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<IBillService, BillService>()
            .AddSingleton<IBillDatabase, BillDatabase>();
    }

    public static void SetBudgetDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<IBudgetService, BudgetService>()
            .AddSingleton<IBudgetDatabase, BudgetDatabase>();
    }

    public static void SetCategoryDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<ICategoryDatabase, CategoryDatabase>();
    }

    public static void SetRegisterDependencyInjection(IServiceCollection services)
    {
        services
            .AddSingleton<IRegisterService, RegisterService>()
            .AddSingleton<IRegisterDatabase, RegisterDatabase>();
    }
}
