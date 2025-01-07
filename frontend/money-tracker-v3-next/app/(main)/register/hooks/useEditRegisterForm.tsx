import { ErrorResult, Result } from "@/types/result";
import { create } from "zustand";

interface defaultValuesProp {
    payee: number | undefined,
    amount: number,
    datePaid: Date | undefined,
    category: number | undefined,
    accountId: number | undefined,
}

const defaultDefaultValues = {
    payee: undefined,
    amount: 0,
    datePaid: undefined,
    category: undefined,
    accountId: undefined,
}

type RegisterModalSetting = {
    isOpen: boolean
    defaultValues: defaultValuesProp
    updateRegisterAction: (authToken: string, transaction: Newtransaction) => Promise<Result<Transaction[]>>
    onOpen: (
        registerAction: (authToken: string, transaction: Newtransaction) => Promise<Result<Transaction[]>>,
        defaultValues?: defaultValuesProp
    ) => void
    onClose: () => void
}

export const useRegisterModalSetting = create<RegisterModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    updateRegisterAction: (authToken: string, transaction: Newtransaction) => {
        return new Promise(() => new ErrorResult<Transaction[]>("error transaction action not updated", true));
    },
    onOpen: (
        registerAction: (authToken: string, transaction: Newtransaction) => Promise<Result<Transaction[]>>,
        defaultValues = defaultDefaultValues
    ) => set(
        {
            updateRegisterAction: registerAction,
            isOpen: true,
            defaultValues: defaultValues,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
