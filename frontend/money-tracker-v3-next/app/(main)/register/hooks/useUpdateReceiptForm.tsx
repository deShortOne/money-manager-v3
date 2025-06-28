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

type UploadReceiptModalSetting = {
    isOpen: boolean
    updateRegisterAction: (authToken: string, transaction: FormData) => Promise<Result<Transaction[]>>
    onOpen: (
        registerAction: (authToken: string, transaction: FormData) => Promise<Result<Transaction[]>>,
    ) => void
    onClose: () => void
}

export const useUploadReceiptModalSetting = create<UploadReceiptModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    updateRegisterAction: () => {
        return new Promise(() => new ErrorResult<Transaction[]>("error transaction action not updated", true));
    },
    onOpen: (
        registerAction: (authToken: string, transaction: FormData) => Promise<Result<Transaction[]>>,
    ) => set(
        {
            updateRegisterAction: registerAction,
            isOpen: true,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
