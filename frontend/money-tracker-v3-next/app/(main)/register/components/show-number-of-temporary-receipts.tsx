'use client'

import { Button } from "@/components/ui/button";
import { useUpdateTemporaryTransactionCounter } from "../hooks/useTemporaryTransactions";
import { PlusCircleIcon } from "lucide-react";
import { getReceiptStates } from "./action";
import { useCookies } from "react-cookie";
import { useQuery } from "@tanstack/react-query";
import {
    HoverCard,
    HoverCardContent,
    HoverCardTrigger,
} from "@/components/ui/hover-card"

export function PendingReceiptStates() {
    const onOpen = useUpdateTemporaryTransactionCounter(state => state.onOpen);
    const numberOfTemporaryTransactions = useUpdateTemporaryTransactionCounter(state => state.numberOfTransaction);
    usePollingStatus();

    return (
        <HoverCard>
            <HoverCardTrigger asChild>
                <Button variant="outline" onClick={() => onOpen()} disabled={numberOfTemporaryTransactions === 0}>
                    <PlusCircleIcon />
                    {numberOfTemporaryTransactions} Temporary Transactions
                </Button>
            </HoverCardTrigger>
            <HoverCardContent>
                Upload a receipt first to create a transaction from it
            </HoverCardContent>
        </HoverCard>
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
