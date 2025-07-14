'use client'

import { Button } from "@/components/ui/button";
import { useUpdateTemporaryTransactionCounter } from "../hooks/useTemporaryTransactions";
import { PlusCircleIcon } from "lucide-react";
import { getReceiptStates } from "./action";
import { useCookies } from "react-cookie";
import { useQuery } from "@tanstack/react-query";

export function PendingReceiptStates() {
    const onOpen = useUpdateTemporaryTransactionCounter(state => state.onOpen);
    const numberOfTemporaryTransactions = useUpdateTemporaryTransactionCounter(state => state.numberOfTransaction);
    usePollingStatus();

    return (
        <Button variant="outline" onClick={() => onOpen()}>
            <PlusCircleIcon />
            {numberOfTemporaryTransactions} Temporary Transactions
        </Button>
    )
}

const usePollingStatus = () => {
    const [cookies] = useCookies(['token']);
    const setNumberOfTemporaryTransactions = useUpdateTemporaryTransactionCounter(state => state.setNumberOfTransactions);
    return useQuery({
        queryKey: ['pending-receipts-states'],
        queryFn: async () => {
            const temp = await getReceiptStates(cookies.token);
            if (temp.item != undefined) {
                setNumberOfTemporaryTransactions(temp.item.length);
            }
            return temp;
        },
        refetchInterval: () => 2000,
        refetchIntervalInBackground: true,
    });
};
