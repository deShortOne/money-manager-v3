
interface Bill {
  id: number,
  payee: string,
  amount: number,
  nextDueDate: string,
  frequency: string,
  category: string,
  overDueBill: {
    daysOverDue: number,
    pastOccurences: string[]
  },
  accountName: string
}

interface NewBillDto {
  payee: number,
  amount: number,
  nextDueDate: Date,
  frequency: string,
  categoryId: number,
  accountId: number
}
