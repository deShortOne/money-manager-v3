'use server'

import { Newtransaction, Transaction, UpdateTransaction } from "@/interface/transaction";
import { ErrorResult, SuccessResult, Result } from "@/types/result";
import { convertDateToString } from "@/utils/date-converter";

export async function getAllTransactions(authToken: string): Promise<Result<Transaction[]>> {
    const response = await fetch(process.env.QUERY_SERVER_URL + `/Register/get`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned get transactions");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function addNewTransactions(authToken: string, transaction: Newtransaction): Promise<Result<Transaction[]>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Register/add`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            payeeId: transaction.payeeId,
            amount: transaction.amount,
            datePaid: convertDateToString(transaction.datePaid),
            categoryId: transaction.categoryId,
            payerId: transaction.payerId,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }
    console.log("error returned adding new transaction");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function editTransaction(authToken: string, transaction: UpdateTransaction): Promise<Result<Transaction[]>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Register/edit`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            id: transaction.id,
            payeeId: transaction.payeeId,
            amount: transaction.amount,
            datePaid: convertDateToString(transaction.datePaid),
            categoryId: transaction.categoryId,
            payerId: transaction.payerId,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }
    console.log("error returned editing transaction");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function deleteTransaction(authToken: string, transactionId: number): Promise<Result<Transaction>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Register/delete`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            id: transactionId,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned delete transaction");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function uploadReceipt(authToken: string, formData: FormData): Promise<Result<Transaction>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Register/upload-receipt`, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + authToken,
        },
        body: formData,
    });

    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned uploading receipt");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}
