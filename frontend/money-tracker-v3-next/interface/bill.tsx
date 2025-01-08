
export interface Bill {
    id: number,
    payee: string,
    amount: number,
    nextDueDate: string,
    frequency: string,
    category: string,
    overDueBill: OverdueBillInfo,
    accountName: string
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
