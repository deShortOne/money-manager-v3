
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
  payee: string,
  amount: number,
  nextDueDate: string,
  frequency: string,
  categoryId: string,
  accountId: string
}
