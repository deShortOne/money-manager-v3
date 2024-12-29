'use server'

import { ErrorResult, SuccessResult, Result } from "@/types/result";

export async function getAllTransactions(authToken: string): Promise<Result<Transaction[]>> {
    const response = await fetch(`http://localhost:1235/Register/get`, {
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
