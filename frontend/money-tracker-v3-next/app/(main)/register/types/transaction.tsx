
interface Transaction {
    id: number,
    payee: string,
    amount: number,
    datePaid: string,
    category: string,
    accountName: string
}

interface Newtransaction {
    payee: number,
    amount: number,
    datePaid: Date,
    category: number,
    account: number,
}

interface UpdateTransaction {
    id: number,
    payee: number,
    amount: number,
    datePaid: Date,
    category: number,
    account: number,
}
