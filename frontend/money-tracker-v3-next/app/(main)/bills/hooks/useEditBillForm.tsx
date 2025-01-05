import { ErrorResult, Result } from "@/types/result";
import { create } from "zustand";

interface defaultValuesProp {
    payee: number | undefined,
    payer: number | undefined,
    amount: number,
    nextDueDate: Date | undefined,
    frequency: string | undefined,
    category: number | undefined,
}

const defaultDefaultValues = {
    payee: undefined,
    payer: undefined,
    amount: 0,
    nextDueDate: undefined,
    frequency: undefined,
    category: undefined,
}

type BillModalSetting = {
    isOpen: boolean
    defaultValues: defaultValuesProp
    updateBillAction: (authToken: string, newBill: NewBillDto) => Promise<Result<Bill>>
    onOpen: (
        billAction: (authToken: string, newBill: NewBillDto) => Promise<Result<Bill>>,
        defaultValues?: defaultValuesProp
    ) => void
    onClose: () => void
}

export const useBillModalSetting = create<BillModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    updateBillAction: (authToken: string, newBill: NewBillDto) => {
        return new Promise(() => new ErrorResult<Bill>("error bill action not updated", true));
    },
    onOpen: (
        billAction: (authToken: string, newBill: NewBillDto) => Promise<Result<Bill>>,
        defaultValues = defaultDefaultValues
    ) => set(
        {
            updateBillAction: billAction,
            isOpen: true,
            defaultValues: defaultValues,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
