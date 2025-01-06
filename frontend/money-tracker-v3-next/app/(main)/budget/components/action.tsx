'use server'

import { ErrorResult, SuccessResult, Result } from "@/types/result";

export async function getAllBudgets(authToken: string): Promise<Result<BudgetGroup[]>> {
    const response = await fetch(`http://localhost:1235/Budget/get`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned login user");
    return JSON.parse(JSON.stringify(new ErrorResult("Username and password not found", false)));
}

export async function getAllCategories(): Promise<Result<Category[]>> {
    const response = await fetch(`http://localhost:1235/Category/get`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned get all categories");
    return JSON.parse(JSON.stringify(new ErrorResult("Error getting categories", false)));
}

export async function addNewBudgetCategory(authToken: string, budgetCategory: UpdateBudgetCategory): Promise<Result<BudgetGroup[]>> {
    const response = await fetch(`http://localhost:1234/Budget/add`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            "budgetGroupId": budgetCategory.budgetGroupId,
            "categoryId": budgetCategory.categoryId,
            "planned": budgetCategory.planned,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }
    console.log("error returned login user");
    return JSON.parse(JSON.stringify(new ErrorResult("Username and password not found", false)));
}
