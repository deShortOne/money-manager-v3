import { TableRow, TableCell } from "@/components/ui/table";
import OverflowBill from "./overflow-bill";
import { useBillModalSetting } from "../hooks/useEditBillForm";
import { deleteBill, editBill, getAllAccounts, getAllCategories } from "./action";
import { useEffect, useState } from "react";
import { useCookies } from "react-cookie";
import { Result } from "@/types/result";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";

interface prop {
    bill: Bill,
}

export default function BillTableRow({ bill }: prop) {
    console.log(bill)
    const [cookies] = useCookies(['token']);
    const onOpen = useBillModalSetting(state => state.onOpen);
    const [msg, setMsg] = useState("");

    const queryClient = useQueryClient();

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

    function editBillForm() {
        const categoryId = categories.find(x => x.name == bill.category);
        if (categoryId == null) {
            setMsg("Category not found");
            return;
        }

        const payee = accounts.find(x => x.name == bill.payee);
        if (payee == null) {
            setMsg("Payee not found");
            return;
        }
        const payer = accounts.find(x => x.name == bill.accountName);
        if (payer == null) {
            setMsg("Payer not found");
            return;
        }

        onOpen(
            (authToken: string, newBill: NewBillDto) => {
                return editBill(authToken, {
                    id: bill.id,
                    payee: newBill.payee,
                    amount: newBill.amount,
                    nextDueDate: newBill.nextDueDate,
                    frequency: newBill.frequency,
                    categoryId: newBill.categoryId,
                    accountId: newBill.accountId,
                });
            },
            {
                amount: bill.amount,
                category: categoryId.id,
                frequency: bill.frequency,
                nextDueDate: new Date(bill.nextDueDate),
                payee: payee.id,
                payer: payer.id,
            });
    }

    async function deleteBillFunc() {
        const deleteBillResult = await deleteBill(cookies.token, bill.id);

        if (deleteBillResult.hasError) {
            setMsg(deleteBillResult.errorMessage);
            return;
        }
        queryClient.invalidateQueries({ queryKey: ['bills'] })
    }

    return (
        <TableRow>
            <TableCell className="fontmedium">{bill.id}</TableCell>
            <TableCell>{bill.payee}</TableCell>
            <TableCell className="textright">{bill.amount}</TableCell>
            <TableCell className="flex">
                {bill.nextDueDate}
                {bill.overDueBill &&
                    <OverflowBill
                        overdueBillInfo={bill.overDueBill}
                    />
                }
            </TableCell>
            <TableCell>{bill.frequency}</TableCell>
            <TableCell>
                {msg}
                <Button onClick={() => editBillForm()}>
                    <Pencil />
                </Button>
                <Button onClick={() => deleteBillFunc()}>
                    <Trash2 />
                </Button>
            </TableCell>
        </TableRow>
    )
}
