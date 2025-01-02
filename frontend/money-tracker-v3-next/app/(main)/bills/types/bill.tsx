
interface Bill {
  id: number,
  payee: string,
  amount: number,
  nextDueDate: string,
  frequency: string,
  category: string,
  overDueBill: OverdueBillInfo,
  accountName: string
}

interface OverdueBillInfo {
  daysOverDue: number,
  pastOccurences: string[],
}

interface NewBillDto {
  payee: number,
  amount: number,
  nextDueDate: Date,
  frequency: string,
  categoryId: number,
  accountId: number
}
