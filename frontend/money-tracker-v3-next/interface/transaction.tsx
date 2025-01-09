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
    payee: number,
    amount: number,
    datePaid: Date,
    category: number,
    account: number,
}

export interface UpdateTransaction {
    id: number,
    payee: number,
    amount: number,
    datePaid: Date,
    category: number,
    account: number,
}
