import { Account } from "./account";
import { Category } from "./category";

export interface Transaction {
    id: number,
    payee: Account,
    amount: number,
    datePaid: string,
    category: Category,
    payer: Account
}

export interface Newtransaction {
    payeeId: number,
    amount: number,
    datePaid: Date,
    categoryId: number,
    payerId: number,
}

export interface UpdateTransaction {
    id: number,
    payeeId: number,
    amount: number,
    datePaid: Date,
    categoryId: number,
    payerId: number,
}
