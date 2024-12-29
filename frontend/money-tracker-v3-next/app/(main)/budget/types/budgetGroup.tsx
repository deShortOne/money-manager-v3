
  interface BudgetGroup {
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
