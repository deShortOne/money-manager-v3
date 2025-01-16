'use server'

import { BudgetGroup, UpdateBudgetCategory } from "@/interface/budgetGroup";
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
    console.log("error returned getting budget");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
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
    console.log("error returned adding new budget category");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function editBudgetCategory(authToken: string, budgetCategory: UpdateBudgetCategory): Promise<Result<BudgetGroup[]>> {
    console.log({
        "budgetGroupId": budgetCategory.budgetGroupId,
        "budgetCategoryId": budgetCategory.categoryId,
        "budgetCategoryPlanned": budgetCategory.planned,
    })
    const response = await fetch(`http://localhost:1234/Budget/edit`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            "budgetGroupId": budgetCategory.budgetGroupId,
            "budgetCategoryId": budgetCategory.categoryId,
            "budgetCategoryPlanned": budgetCategory.planned,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log(response)
    console.log("error returned editing budget category");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function deleteBudgetCategory(authToken: string, budgetGroupId: number, budgetCategoryId: number): Promise<Result<BudgetGroup[]>> {
    const response = await fetch(`http://localhost:1234/Budget/delete`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            "budgetGroupId": budgetGroupId,
            "budgetCategoryId": budgetCategoryId,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }
    console.log("error returned deleting budget category");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}
