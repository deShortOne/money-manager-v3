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
    onOpen: (defaultValues?: defaultValuesProp) => void
    onClose: () => void
}

export const useBillModalSetting = create<BillModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    onOpen: (defaultValues = defaultDefaultValues) => set(
        {
            isOpen: true,
            defaultValues: defaultValues,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
