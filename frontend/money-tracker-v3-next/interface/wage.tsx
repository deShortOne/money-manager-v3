export interface WageRequest {
    grossIncome: number,
    frequencyOfIncome: string,
    taxCode: string,
    payNationalInsurance: boolean,
    pension: {
        pensionType: string | undefined,
        value: number | undefined,
        pensionCalculationType: string | undefined,
    } | undefined,
    studentLoanOptions: {
        plan1: boolean | undefined,
        plan2: boolean | undefined,
        plan4: boolean | undefined,
        plan5: boolean | undefined,
        postgraduate: boolean | undefined,
    } | undefined
}

export interface WageResponse {
    grossYearlyIncome: {
        amount: number,
    },
    wages: {
        amount: number
    }[]
}
