import { TableRow, TableCell } from "@/components/ui/table";
import OverflowBill from "./overflow-bill";
import { useBillModalSetting } from "../hooks/useEditBillForm";
import { getAllAccounts, getAllCategories } from "./action";
import { useEffect, useState } from "react";
import { useCookies } from "react-cookie";
import { Result } from "@/types/result";
import { useQuery } from "@tanstack/react-query";
import { Pencil } from "lucide-react";
import { Button } from "@/components/ui/button";

interface prop {
    transaction: Bill,
}

export default function BillTableRow({ transaction }: prop) {
    const [cookies] = useCookies(['token']);
    const onOpen = useBillModalSetting(state => state.onOpen);
    const [msg, setMsg] = useState("");

    const [accounts, setAccounts] = useState<Account[]>([]);
    const { data: dataAccounts } = useQuery<Result<Account[]>>({
        queryKey: ['accounts'],
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
        queryKey: ['categories'],
        queryFn: () => getAllCategories(),
    });
    useEffect(() => {
        if (dataCategories == null || dataCategories.hasError || dataCategories.item == undefined) {

        } else {
            setCategories(dataCategories.item);
        }
    }, [dataCategories]);

    function asdf() {
        console.log("FFFFFF");
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

        onOpen({
            amount: transaction.amount,
            category: categoryId.id,
            frequency: transaction.frequency,
            nextDueDate: new Date(transaction.nextDueDate),
            payee: payee.id,
            payer: payer.id,
        });
    }

    return (
        <TableRow>
            <TableCell className="fontmedium">{transaction.id}</TableCell>
            <TableCell>{transaction.payee}</TableCell>
            <TableCell className="textright">{transaction.amount}</TableCell>
            <TableCell className="flex">
                {transaction.nextDueDate}
                {transaction.overDueBill &&
                    <OverflowBill
                        overdueBillInfo={transaction.overDueBill}
                    />
                }
            </TableCell>
            <TableCell>{transaction.frequency}</TableCell>
            <TableCell>
                {msg}
                <Button onClick={() => asdf()}>
                    <Pencil />
                </Button>
            </TableCell>
        </TableRow>
    )
}
