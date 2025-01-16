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
