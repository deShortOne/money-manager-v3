'use client'

import { Button } from "@/components/ui/button";
import { useAccountModalSetting } from "../hooks/useEditAccountForm";
import { PlusCircleIcon } from "lucide-react";
import { addNewAccount } from "./action";

export function AddNewAccount() {
    const onOpen = useAccountModalSetting(state => state.onOpen);

    return (
        <Button variant="outline" onClick={() => onOpen(addNewAccount)}>
            <PlusCircleIcon />
            Add New Account
        </Button>
    )
}
