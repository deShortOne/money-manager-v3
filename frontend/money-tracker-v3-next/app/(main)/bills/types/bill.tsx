
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
