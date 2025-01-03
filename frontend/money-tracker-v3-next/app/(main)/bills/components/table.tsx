
import {
    Table,
    TableBody,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { useEffect, useState } from "react";
import { getAllTransactions } from "./action";
import { useCookies } from "react-cookie";
import { useQuery } from "@tanstack/react-query";
import { Result } from "@/types/result";
import BillTableRow from "./bill-table-row";

export default function BillsDisplay() {
    const [cookies] = useCookies(['token']);

    const [transactions, setTransactions] = useState<Bill[]>([]);

    const { status, data, error, isFetching } = useQuery<Result<Bill[]>>({
        queryKey: ['bills'],
        queryFn: () => getAllTransactions(cookies.token),
    });

    useEffect(() => {
        if (data == null || data.hasError || data.item == undefined) {

        } else {
            setTransactions(data.item);
        }
    }, [data]);

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead className="w-[100px]">Id</TableHead>
                    <TableHead>Payee</TableHead>
                    <TableHead className="text-right">Amount</TableHead>
                    <TableHead>Due Date</TableHead>
                    <TableHead>Frequency</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {transactions.map((transaction) => (
                    <BillTableRow transaction={transaction} key={transaction.id} />
                ))}
            </TableBody>
        </Table>
    )
}
