'use server'

import { Account } from "@/interface/account";
import { Bill, EditBillDto, NewBillDto } from "@/interface/bill";
import { Category } from "@/interface/category";
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

export async function getAllFrequencyNames(): Promise<Result<string[]>> {
    const response = await fetch(`http://localhost:1235/Bill/get-all-frequency-names`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned get all frequencies");
    return JSON.parse(JSON.stringify(new ErrorResult("Error getting frequency names", false)));
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

export async function addNewBill(authToken: string, newBill: NewBillDto): Promise<Result<Bill>> {
    const offset = newBill.nextDueDate.getTimezoneOffset()
    newBill.nextDueDate = new Date(newBill.nextDueDate.getTime() - (offset * 60 * 1000))

    const dateToPass: string = newBill.nextDueDate.toISOString().split('T')[0]

    const response = await fetch(`http://localhost:1234/Bill/add`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            "payee": newBill.payee,
            "amount": newBill.amount,
            "nextDueDate": dateToPass,
            "frequency": newBill.frequency,
            "categoryId": newBill.categoryId,
            "payer": newBill.accountId
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned add new bill");
    return JSON.parse(JSON.stringify(new ErrorResult("Error adding new bill", false)));
}

export async function editBill(authToken: string, editBill: EditBillDto): Promise<Result<Bill>> {
    const offset = editBill.nextDueDate.getTimezoneOffset()
    editBill.nextDueDate = new Date(editBill.nextDueDate.getTime() - (offset * 60 * 1000))

    const dateToPass: string = editBill.nextDueDate.toISOString().split('T')[0]

    const response = await fetch(`http://localhost:1234/Bill/edit`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            id: editBill.id,
            payee: editBill.payee,
            amount: editBill.amount,
            nextDueDate: dateToPass,
            frequency: editBill.frequency,
            categoryId: editBill.categoryId,
            payer: editBill.accountId
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned edit old bill");
    return JSON.parse(JSON.stringify(new ErrorResult("Error editing bill", false)));
}

export async function deleteBill(authToken: string, billId: number): Promise<Result<Bill>> {
    const response = await fetch(`http://localhost:1234/Bill/delete`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            id: billId,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned delete bill");
    return JSON.parse(JSON.stringify(new ErrorResult("Error deleting bill", false)));
}
