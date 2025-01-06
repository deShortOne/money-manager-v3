
interface BudgetGroup {
    id: number,
    name: string,
    categories: BudgetCategory[],
    planned: number,
    actual: number,
    difference: number
}

interface BudgetCategory {
    name: string,
    planned: number,
    actual: number,
    difference: number
}

interface UpdateBudgetCategory {
    budgetGroupId: number,
    categoryId: number,
    planned: number,
}
