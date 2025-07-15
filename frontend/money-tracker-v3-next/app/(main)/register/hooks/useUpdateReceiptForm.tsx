import { ErrorResult, Result } from "@/types/result";
import { create } from "zustand";

const defaultDefaultValues = {
    payeeId: undefined,
    amount: 0,
    datePaid: undefined,
    categoryId: undefined,
    payerId: undefined,
}

type UploadReceiptModalSetting = {
    isOpen: boolean
    updateRegisterAction: (authToken: string, transaction: FormData) => Promise<Result<string>>
    onOpen: (
        registerAction: (authToken: string, transaction: FormData) => Promise<Result<string>>,
    ) => void
    onClose: () => void
}

export const useUploadReceiptModalSetting = create<UploadReceiptModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    updateRegisterAction: () => {
        return new Promise(() => new ErrorResult<string>("error transaction action not updated", true));
    },
    onOpen: (
        registerAction: (authToken: string, transaction: FormData) => Promise<Result<string>>,
    ) => set(
        {
            updateRegisterAction: registerAction,
            isOpen: true,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
