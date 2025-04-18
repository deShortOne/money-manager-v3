import { Account } from "./account"
import { Category } from "./category"

export interface Bill {
    id: number,
    payee: Account,
    amount: number,
    nextduedate: string,
    frequency: string,
    category: Category,
    overduebill: OverdueBillInfo,
    payer: Account
}

export interface OverdueBillInfo {
    daysOverDue: number,
    PastOccurences: string[],
}

export interface NewBillDto {
    payeeId: number,
    amount: number,
    nextDueDate: Date,
    frequency: string,
    categoryId: number,
    payerId: number
}

export interface EditBillDto {
    id: number,
    payeeId: number,
    amount: number,
    nextDueDate: Date,
    frequency: string,
    categoryId: number,
    payerId: number
}
