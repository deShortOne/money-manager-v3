import { Account } from "./account"
import { Category } from "./category"

export interface Bill {
    id: number,
    payee: Account,
    amount: number,
    nextDueDate: string,
    frequency: string,
    category: Category,
    overDueBill: OverdueBillInfo,
    payer: Account
}

export interface OverdueBillInfo {
    daysOverDue: number,
    pastOccurences: string[],
}

export interface NewBillDto {
    payee: number,
    amount: number,
    nextDueDate: Date,
    frequency: string,
    categoryId: number,
    accountId: number
}

export interface EditBillDto {
    id: number,
    payee: number,
    amount: number,
    nextDueDate: Date,
    frequency: string,
    categoryId: number,
    accountId: number
}
