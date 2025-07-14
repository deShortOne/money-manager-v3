'use client'

import { Button } from "@/components/ui/button";
import { useUploadReceiptModalSetting } from "../hooks/useUpdateReceiptForm";
import { PlusCircleIcon } from "lucide-react";
import { uploadReceipt } from "./action";

export function UploadReceipt() {
    const onOpen = useUploadReceiptModalSetting(state => state.onOpen);

    return (
        <Button variant="outline" onClick={() => onOpen(uploadReceipt)}>
            <PlusCircleIcon />
            Create Transaction Item From Receipt
        </Button>
    )
}
