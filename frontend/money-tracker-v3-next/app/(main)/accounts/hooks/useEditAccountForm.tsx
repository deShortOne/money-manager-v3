import { ErrorResult, Result } from "@/types/result";
import { create } from "zustand";
import { Account, NewAccountDto } from "@/interface/account";

interface defaultValuesProp {
    name: string | undefined,
    doesUserOwnAccount: boolean,
}

const defaultDefaultValues = {
    name: undefined,
    doesUserOwnAccount: false,
}

type AccountModalSetting = {
    isOpen: boolean
    defaultValues: defaultValuesProp
    updateAccountAction: (authToken: string, newAccount: NewAccountDto) => Promise<Result<Account>>
    onOpen: (
        billAction: (authToken: string, newAccount: NewAccountDto) => Promise<Result<Account>>,
        defaultValues?: defaultValuesProp
    ) => void
    onClose: () => void
}

export const useAccountModalSetting = create<AccountModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    updateAccountAction: () => {
        return new Promise(() => new ErrorResult<Account>("error account action not updated", true));
    },
    onOpen: (
        billAction: (authToken: string, newAccount: NewAccountDto) => Promise<Result<Account>>,
        defaultValues = defaultDefaultValues
    ) => set(
        {
            updateAccountAction: billAction,
            isOpen: true,
            defaultValues: defaultValues,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
