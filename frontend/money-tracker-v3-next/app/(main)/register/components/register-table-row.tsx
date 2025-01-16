import { queryKeyTransactions } from "@/app/data/queryKeys";
import { TableCell, TableRow } from "@/components/ui/table";
import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { deleteTransaction, editTransaction } from "./action";
import { useCookies } from "react-cookie";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { Pencil, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Newtransaction, Transaction } from "@/interface/transaction";

interface prop {
    transaction: Transaction
}

export default function RegisterTableRow({ transaction }: prop) {
    const [cookies] = useCookies(['token']);
    const onOpen = useRegisterModalSetting(state => state.onOpen);
    const [msg, setMsg] = useState("");

    const queryClient = useQueryClient();

    function editTransactionForm() {
        onOpen(
            (authToken: string, newTransaction: Newtransaction) => {
                return editTransaction(authToken, {
                    id: transaction.id,
                    payeeId: newTransaction.payeeId,
                    amount: newTransaction.amount,
                    datePaid: newTransaction.datePaid,
                    categoryId: newTransaction.categoryId,
                    payerId: newTransaction.payerId,
                });
            },
            {
                amount: transaction.amount,
                categoryId: transaction.category.id,
                datePaid: new Date(transaction.datePaid),
                payeeId: transaction.payee.id,
                payerId: transaction.payer.id,
            });
    }

    async function deleteRegisterFunc() {
        const deleteBillResult = await deleteTransaction(cookies.token, transaction.id);

        if (deleteBillResult.hasError) {
            setMsg(deleteBillResult.errorMessage);
            return;
        }
        queryClient.invalidateQueries({ queryKey: [queryKeyTransactions] })
    }

    return (
        <TableRow key={transaction.id}>
            <TableCell className="font-medium">{transaction.id}</TableCell>
            <TableCell>{transaction.datePaid}</TableCell>
            <TableCell>{transaction.payee.name}</TableCell>
            <TableCell className="text-right">{transaction.amount}</TableCell>
            <TableCell>
                {msg}
                <Button onClick={() => editTransactionForm()}>
                    <Pencil />
                </Button>
                <Button onClick={() => deleteRegisterFunc()}>
                    <Trash2 />
                </Button>
            </TableCell>
        </TableRow>
    )
}
