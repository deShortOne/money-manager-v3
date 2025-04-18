'use server'

import { Bill, EditBillDto, NewBillDto } from "@/interface/bill";
import { ErrorResult, SuccessResult, Result } from "@/types/result";
import { convertDateToString } from "@/utils/date-converter";

export async function getAllBills(authToken: string): Promise<Result<Bill[]>> {
    const response = await fetch(process.env.QUERY_SERVER_URL + `/Bill/get`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned getting bills");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function addNewBill(authToken: string, newBill: NewBillDto): Promise<Result<Bill>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Bill/add`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            payeeId: newBill.payeeId,
            amount: newBill.amount,
            nextDueDate: convertDateToString(newBill.nextDueDate),
            frequency: newBill.frequency,
            categoryId: newBill.categoryId,
            payerId: newBill.payerId
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned add new bill");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function editBill(authToken: string, editBill: EditBillDto): Promise<Result<Bill>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Bill/edit`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            id: editBill.id,
            payeeId: editBill.payeeId,
            amount: editBill.amount,
            nextDueDate: convertDateToString(editBill.nextDueDate),
            frequency: editBill.frequency,
            categoryId: editBill.categoryId,
            payerId: editBill.payerId
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned edit old bill");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function deleteBill(authToken: string, billId: number): Promise<Result<Bill>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Bill/delete`, {
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
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}
