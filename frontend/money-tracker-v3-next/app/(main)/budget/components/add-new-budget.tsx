'use client'

import { Button } from "@/components/ui/button";
import { useBudgetModalSetting } from "../hooks/useEditBudgetForm";
import { PlusCircleIcon } from "lucide-react";
import { addNewBudgetCategory } from "./action";

export function AddNewBudget() {
    const onOpen = useBudgetModalSetting(state => state.onOpen);

    return (
        <Button variant="outline" onClick={() => onOpen(addNewBudgetCategory)}>
            <PlusCircleIcon />
            Add Budget Item
        </Button>
    )
}
