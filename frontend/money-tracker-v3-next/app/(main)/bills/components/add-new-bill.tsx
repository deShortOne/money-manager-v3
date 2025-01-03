'use client'

import { Button } from "@/components/ui/button";
import { useBillModalSetting } from "../hooks/useEditBillForm";
import { PlusCircleIcon } from "lucide-react";

export function AddNewBill() {
    const onOpen = useBillModalSetting(state => state.onOpen);

    return (
        <Button variant="outline" onClick={() => onOpen()}>
            <PlusCircleIcon />
            Add New Bill
        </Button>
    )
}
