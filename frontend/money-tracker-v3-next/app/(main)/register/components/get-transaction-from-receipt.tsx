'use client'

import { Button } from "@/components/ui/button";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { PlusCircleIcon } from "lucide-react";
import { addNewTransactions, getTemporaryTransaction } from "./action";
import { useCookies } from "react-cookie";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";

export function TemporaryTransactionEditor() {
    const onOpen = useRegisterModalSetting(state => state.onOpen);
    const { data } = usePollingStatus();
    const disableButton = data == null || data.hasError || data.hasFailed ||
        data.item == null || data.item.receiptProcessingState === "Processing";

    const temporaryData = {
        payeeId: data?.item?.temporaryTransaction?.payee?.id ?? undefined,
        amount: data?.item?.temporaryTransaction?.amount ?? 0,
        datePaid: data?.item?.temporaryTransaction?.datePaid ?? undefined,
        categoryId: data?.item?.temporaryTransaction?.category?.id ?? undefined,
        payerId: data?.item?.temporaryTransaction?.payer?.id ?? undefined,
    };

    return (
        <Button variant="outline" onClick={() => onOpen(addNewTransactions, temporaryData)} disabled={disableButton}>
            <PlusCircleIcon />
            Get Temporary Transaction
        </Button>
    )
}

const usePollingStatus = () => {
    const [cookies] = useCookies(['token', 'pending-receipt']);
    const [stillComputing, setStillComputing] = useState(true)

    return useQuery({
        queryKey: ['pending-receipt'],
        queryFn: async () => {
            const result = await getTemporaryTransaction(cookies.token, cookies["pending-receipt"]);
            if (result?.item?.receiptProcessingState === "Pending") {
                setStillComputing(false);
            }
            return result;
        },
        refetchInterval: () => {
            return stillComputing ? 2000 : false;
        },
        refetchIntervalInBackground: true,
        enabled: cookies["pending-receipt"] != null
    });
};
