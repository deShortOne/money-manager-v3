
interface Transaction {
    id: number,
    payee: string,
    amount: number,
    datePaid: string,
    category: string,
    account: string
}

interface Newtransaction {
    payee: number,
    amount: number,
    datePaid: Date,
    category: number,
    account: number,
}
