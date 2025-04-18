import { TableRow, TableCell } from "@/components/ui/table";
import OverflowBill from "./overflow-bill";
import { useBillModalSetting } from "../hooks/useEditBillForm";
import { deleteBill, editBill } from "./action";
import { useState } from "react";
import { useCookies } from "react-cookie";
import { useQueryClient } from "@tanstack/react-query";
import { Pencil, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { queryKeyBills } from "@/app/data/queryKeys";
import { Bill, NewBillDto } from "@/interface/bill";

interface prop {
    bill: Bill,
}

export default function BillTableRow({ bill }: prop) {
    const [cookies] = useCookies(['token']);
    const onOpen = useBillModalSetting(state => state.onOpen);
    const [msg, setMsg] = useState("");
    console.log(bill);

    const queryClient = useQueryClient();

    function editBillForm() {
        onOpen(
            (authToken: string, newBill: NewBillDto) => {
                return editBill(authToken, {
                    id: bill.id,
                    payeeId: newBill.payeeId,
                    amount: newBill.amount,
                    nextDueDate: newBill.nextDueDate,
                    frequency: newBill.frequency,
                    categoryId: newBill.categoryId,
                    payerId: newBill.payerId,
                });
            },
            {
                amount: bill.amount,
                category: bill.category.id,
                frequency: bill.frequency,
                nextDueDate: new Date(bill.nextduedate),
                payee: bill.payee.id,
                payer: bill.payer.id,
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
            <TableCell>{bill.payee.name}</TableCell>
            <TableCell className="textright">{bill.amount}</TableCell>
            <TableCell className="flex">
                {bill.nextduedate}
                {bill.overduebill &&
                    <OverflowBill
                        overdueBillInfo={bill.overduebill}
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
