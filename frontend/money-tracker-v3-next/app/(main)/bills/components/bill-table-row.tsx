import { TableRow, TableCell } from "@/components/ui/table";
import OverflowBill from "./overflow-bill";
import { useBillModalSetting } from "../hooks/useEditBillForm";
import { deleteBill, editBill } from "./action";
import { useState } from "react";
import { useCookies } from "react-cookie";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Pencil, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { queryKeyAccounts, queryKeyBills, queryKeyCategories } from "@/app/data/queryKeys";
import { Bill, NewBillDto } from "@/interface/bill";
import { Account } from "@/interface/account";
import { Category } from "@/interface/category";

interface prop {
    bill: Bill,
}

export default function BillTableRow({ bill }: prop) {
    const [cookies] = useCookies(['token']);
    const onOpen = useBillModalSetting(state => state.onOpen);
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

    function editBillForm() {
        const categoryId = dataCategories.find(x => x.name == bill.category);
        if (categoryId == null) {
            setMsg("Category not found");
            return;
        }

        const payee = dataAccounts.find(x => x.name == bill.payee);
        if (payee == null) {
            setMsg("Payee not found");
            return;
        }
        const payer = dataAccounts.find(x => x.name == bill.accountName);
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
        queryClient.invalidateQueries({ queryKey: [queryKeyBills] })
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
