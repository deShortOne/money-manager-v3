
export interface Transaction {
    id: number,
    payee: string,
    amount: number,
    datePaid: string,
    category: string,
    accountName: string
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
