'use client'

import { Button } from "@/components/ui/button"
import { useCookies } from "react-cookie"
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogTitle,
} from "@/components/ui/dialog";
import { useQuery } from "@tanstack/react-query";
import { addNewTransactions, getReceiptStates } from "./action";
import { useUpdateTemporaryTransactionCounter } from "../hooks/useTemporaryTransactions";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { LoadingSpinner } from "@/components/ui/loading-spinner";

export function SelectTemporaryTransactionForm() {
    const open = useUpdateTemporaryTransactionCounter(state => state.isOpen);
    const closeUpdateTransactionForm = useUpdateTemporaryTransactionCounter(state => state.onClose);

    const { data: receiptStates } = usePollingStatus();

    const onOpen = useRegisterModalSetting(state => state.onOpen);

    return (
        <Dialog open={open} onOpenChange={closeUpdateTransactionForm}>
            <DialogContent>
                <DialogTitle>List of receipts</DialogTitle>
                <DialogDescription>Ya</DialogDescription>
                {receiptStates?.item?.map((data) => {
                    return (
                        <Button
                            key={data.id}
                            disabled={data.state === "Processing"}
                            onClick={() => onOpen(addNewTransactions, undefined, data.id)}
                        >
                            {data.id} {data.state === "Processing" && <LoadingSpinner />}
                        </Button>
                    )
                })}
            </DialogContent>
        </Dialog>
    )
}

const usePollingStatus = () => {
    const [cookies] = useCookies(['token']);
    return useQuery({
        queryKey: ['pending-receipts-states'],
        queryFn: async () => getReceiptStates(cookies.token),
        refetchInterval: () => false,
        refetchIntervalInBackground: false,
    });
};
