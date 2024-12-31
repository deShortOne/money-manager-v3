'use server'

import { ErrorResult, SuccessResult, Result } from "@/types/result";

export async function getAllTransactions(authToken: string): Promise<Result<Bill[]>> {
    const response = await fetch(`http://localhost:1235/Bill/get`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned get transactions");
    return JSON.parse(JSON.stringify(new ErrorResult("Unknown error with getting your transactions. Do TODO.", false)));
}

export async function getAllAccounts(authToken: string): Promise<Result<Account[]>> {
    const response = await fetch(`http://localhost:1235/Account/get`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned get all accounts");
    return JSON.parse(JSON.stringify(new ErrorResult("Unknown error with getting your accounts. Do TODO", false)));
}

export async function addNewBill(authToken: string, newBill: NewBillDto): Promise<Result<Bill>> {
    const response = await fetch(`http://localhost:1234/Bill/add`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify(newBill),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }

    console.log("error returned add new bill");
    return JSON.parse(JSON.stringify(new ErrorResult("Error adding new bill", false)));
}
