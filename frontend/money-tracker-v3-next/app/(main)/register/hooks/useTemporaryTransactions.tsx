import { create } from "zustand";

type UpdateTemporaryTransactionCounter = {
    isOpen: boolean
    numberOfTransaction: number
    setNumberOfTransactions: (numberOfTransaction: number) => void
    onOpen: () => void
    onClose: () => void
}

export const useUpdateTemporaryTransactionCounter = create<UpdateTemporaryTransactionCounter>((set) => ({
    isOpen: false,
    numberOfTransaction: 0,
    setNumberOfTransactions: (numberOfTransaction: number) => set({ numberOfTransaction: numberOfTransaction }),
    onOpen: () => set({ isOpen: true }),
    onClose: () => set({ isOpen: false }),
}))
