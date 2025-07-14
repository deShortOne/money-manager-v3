import { Newtransaction, Transaction } from "@/interface/transaction";
import { ErrorResult, Result } from "@/types/result";
import { create } from "zustand";

interface defaultValuesProp {
    payeeId: number | undefined,
    amount: number,
    datePaid: Date | undefined,
    categoryId: number | undefined,
    payerId: number | undefined,
}

const defaultDefaultValues = {
    payeeId: undefined,
    amount: 0,
    datePaid: undefined,
    categoryId: undefined,
    payerId: undefined,
}

type RegisterModalSetting = {
    isOpen: boolean
    defaultValues: defaultValuesProp
    defaultReceiptId: string
    updateRegisterAction: (authToken: string, transaction: Newtransaction) => Promise<Result<Transaction[]>>
    onOpen: (
        registerAction: (authToken: string, transaction: Newtransaction) => Promise<Result<Transaction[]>>,
        defaultValues?: defaultValuesProp,
        defaultReceiptId?: string,
    ) => void
    onClose: () => void
}

export const useRegisterModalSetting = create<RegisterModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    defaultReceiptId: "",
    updateRegisterAction: () => {
        return new Promise(() => new ErrorResult<Transaction[]>("error transaction action not updated", true));
    },
    onOpen: (
        registerAction: (authToken: string, transaction: Newtransaction) => Promise<Result<Transaction[]>>,
        defaultValues = defaultDefaultValues,
        defaultReceiptIda = "" as string,
    ) => set(
        {
            updateRegisterAction: registerAction,
            isOpen: true,
            defaultValues: defaultValues,
            defaultReceiptId: defaultReceiptIda,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
