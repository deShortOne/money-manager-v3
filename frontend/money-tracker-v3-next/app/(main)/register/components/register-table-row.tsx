import { queryKeyAccounts, queryKeyCategories } from "@/app/data/queryKeys";
import { TableCell, TableRow } from "@/components/ui/table";
import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import { editTransaction, getAllAccounts, getAllCategories } from "./action";
import { useCookies } from "react-cookie";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { Result } from "@/types/result";
import { Pencil } from "lucide-react";
import { Button } from "@/components/ui/button";

interface prop {
    transaction: Transaction
}

export default function RegisterTableRow({ transaction }: prop) {
    const [cookies] = useCookies(['token']);
    const onOpen = useRegisterModalSetting(state => state.onOpen);
    const [msg, setMsg] = useState("");

    const [accounts, setAccounts] = useState<Account[]>([]);
    const { data: dataAccounts } = useQuery<Result<Account[]>>({
        queryKey: [queryKeyAccounts],
        queryFn: () => getAllAccounts(cookies.token),
    });
    useEffect(() => {
        if (dataAccounts == null || dataAccounts.hasError || dataAccounts.item == undefined) {

        } else {
            setAccounts(dataAccounts.item)
        }
    }, [dataAccounts]);

    const [categories, setCategories] = useState<Category[]>([]);
    const { data: dataCategories } = useQuery<Result<Category[]>>({
        queryKey: [queryKeyCategories],
        queryFn: () => getAllCategories(),
    });
    useEffect(() => {
        if (dataCategories == null || dataCategories.hasError || dataCategories.item == undefined) {

        } else {
            setCategories(dataCategories.item);
        }
    }, [dataCategories]);

    function editTransactionForm() {
        const categoryId = categories.find(x => x.name == transaction.category);
        if (categoryId == null) {
            setMsg("Category not found");
            return;
        }

        const payee = accounts.find(x => x.name == transaction.payee);
        if (payee == null) {
            setMsg("Payee not found");
            return;
        }
        const payer = accounts.find(x => x.name == transaction.accountName);
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
            </TableCell>
        </TableRow>
    )
}
