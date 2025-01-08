import { queryKeyAccounts, queryKeyCategories, queryKeyTransactions } from "@/app/data/queryKeys";
import { TableCell, TableRow } from "@/components/ui/table";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { deleteTransaction, editTransaction } from "./action";
import { useCookies } from "react-cookie";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { Pencil, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Newtransaction, Transaction } from "@/interface/transaction";
import { Account } from "@/interface/account";
import { Category } from "@/interface/category";

interface prop {
    transaction: Transaction
}

export default function RegisterTableRow({ transaction }: prop) {
    const [cookies] = useCookies(['token']);
    const onOpen = useRegisterModalSetting(state => state.onOpen);
    const [msg, setMsg] = useState("");

    const queryClient = useQueryClient();

    const { data: dataAccounts } = useQuery<Account[]>({
        queryKey: [queryKeyAccounts],
        queryFn: () => fetch("api/accounts", {
            method: "GET",
            headers: {
                'Content-Type': 'application/json',
            }
        }).then(async (x) => await x.json()),
        initialData: [],
    });

    const { data: dataCategories } = useQuery<Category[]>({
        queryKey: [queryKeyCategories],
        queryFn: () => fetch("api/categories", {
            method: "GET",
            headers: {
                'Content-Type': 'application/json',
            }
        }).then(async (x) => await x.json()),
        initialData: [],
    });

    function editTransactionForm() {
        const categoryId = dataCategories.find(x => x.name == transaction.category);
        if (categoryId == null) {
            setMsg("Category not found");
            return;
        }

        const payee = dataAccounts.find(x => x.name == transaction.payee);
        if (payee == null) {
            setMsg("Payee not found");
            return;
        }
        const payer = dataAccounts.find(x => x.name == transaction.accountName);
        if (payer == null) {
            setMsg("Payer not found");
            return;
        }

        onOpen(
            (authToken: string, newTransaction: Newtransaction) => {
                return editTransaction(authToken, {
                    id: transaction.id,
                    payee: newTransaction.payee,
                    amount: newTransaction.amount,
                    datePaid: newTransaction.datePaid,
                    category: newTransaction.category,
                    account: newTransaction.account,
                });
            },
            {
                amount: transaction.amount,
                category: categoryId.id,
                datePaid: new Date(transaction.datePaid),
                payee: payee.id,
                accountId: payer.id,
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
            <TableCell>{transaction.payee}</TableCell>
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
