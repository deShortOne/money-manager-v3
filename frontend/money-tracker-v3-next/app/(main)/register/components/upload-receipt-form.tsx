'use client'

import { Button } from "@/components/ui/button"
import { useCookies } from "react-cookie"
import { useUploadReceiptModalSetting } from "../hooks/useUpdateReceiptForm"
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { useState } from "react";

export function UploadReceiptForm() {
    const [cookies, setCookies] = useCookies(['token', 'pending-receipt']);
    const open = useUploadReceiptModalSetting(state => state.isOpen);
    const closeUpdateTransactionForm = useUploadReceiptModalSetting(state => state.onClose);
    const registerAction = useUploadReceiptModalSetting(state => state.updateRegisterAction);
    const [fileToUpload, setFileToUpload] = useState<File | null>(null);
    const [addNewBillButtonErrorMessage, setAddNewBillButtonErrorMessage] = useState("");


    const handleImageUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
        event.preventDefault();

        const fileInput = event.target;
        const file = fileInput.files ? fileInput.files[0] : null;
        if (file) {
            setFileToUpload(file);
        }
    };

    const onSubmit = async (event: React.FormEvent) => {
        event.preventDefault();

        const form = new FormData();
        form.append('uploadReceipt', fileToUpload!);
        const result = await registerAction(cookies.token, form)
        if (result.hasError) {
            setAddNewBillButtonErrorMessage(result.errorMessage);
            return;
        }
        setCookies("pending-receipt", result.item, { sameSite: 'strict' });
        closeUpdateTransactionForm();
    }

    return (
        <Dialog open={open} onOpenChange={closeUpdateTransactionForm}>
            <DialogContent>
                <form onSubmit={onSubmit} className="">
                    <DialogHeader>
                        <DialogTitle>Upload receipt</DialogTitle>
                        <DialogDescription>
                            The receipt that you upload will create a transaction
                        </DialogDescription>
                    </DialogHeader>

                    <div>
                        <input type="file" accept="image/*" onChange={handleImageUpload} />
                    </div>
                    <Button type="submit">Submit</Button>
                    <div>{addNewBillButtonErrorMessage}</div>
                </form>
            </DialogContent>
        </Dialog>
    )
}
