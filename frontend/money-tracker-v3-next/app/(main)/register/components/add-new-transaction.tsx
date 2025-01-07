'use client'

import { Button } from "@/components/ui/button";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { PlusCircleIcon } from "lucide-react";
import { addNewTransactions } from "./action";

export function AddNewTransaction() {
    const onOpen = useRegisterModalSetting(state => state.onOpen);

    return (
        <Button variant="outline" onClick={() => onOpen(addNewTransactions)}>
            <PlusCircleIcon />
            Add Transaction Item
        </Button>
    )
}
