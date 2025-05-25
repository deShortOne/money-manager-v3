export interface WageRequest {
    grossIncome: number,
    frequencyOfIncome: string,
    taxCode: string,
    payNationalInsurance: boolean,
    pension: {
        type: string,
        value: number,
        rate: string,
    } | null,
    studentLoanOptions: {
        plan1: boolean,
        plan2: boolean,
        plan4: boolean,
        plan5: boolean,
        postgraduate: boolean,
    }
}

export interface WageResponse {
    grossYearlyIncome: {
        amount: number,
    },
    wages: {
        amount: number
    }[]
}
