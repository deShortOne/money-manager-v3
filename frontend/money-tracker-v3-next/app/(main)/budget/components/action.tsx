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
