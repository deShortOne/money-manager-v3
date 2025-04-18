import { BudgetGroup, UpdateBudgetCategory } from "@/interface/budgetGroup";
import { ErrorResult, Result } from "@/types/result";
import { create } from "zustand";

interface defaultValuesProp {
    budgetGroupId: number | undefined,
    categoryId: number | undefined,
    planned: number,
}

const defaultDefaultValues = {
    budgetGroupId: undefined,
    categoryId: undefined,
    planned: 0,
}

type BudgetModalSetting = {
    isOpen: boolean
    defaultValues: defaultValuesProp
    updateBudgetAction: (authToken: string, budgetCategory: UpdateBudgetCategory) => Promise<Result<BudgetGroup[]>>
    onOpen: (
        budgetAction: (authToken: string, budgetCategory: UpdateBudgetCategory) => Promise<Result<BudgetGroup[]>>,
        defaultValues?: defaultValuesProp
    ) => void
    onClose: () => void
}

export const useBudgetModalSetting = create<BudgetModalSetting>((set) => ({
    isOpen: false,
    defaultValues: defaultDefaultValues,
    updateBudgetAction: () => {
        return new Promise(() => new ErrorResult<BudgetGroup[]>("error budget action not updated", true));
    },
    onOpen: (
        budgetAction: (authToken: string, budgetCategory: UpdateBudgetCategory) => Promise<Result<BudgetGroup[]>>,
        defaultValues = defaultDefaultValues
    ) => set(
        {
            updateBudgetAction: budgetAction,
            isOpen: true,
            defaultValues: defaultValues,
        }
    ),
    onClose: () => set({ isOpen: false }),
}))
