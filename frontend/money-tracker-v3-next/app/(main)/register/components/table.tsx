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
import { queryKeyTransactions } from "@/app/data/queryKeys";
import RegisterTableRow from "./register-table-row";
import { Transaction } from "@/interface/transaction";

export default function TransactionsDisplay() {
    const [cookies] = useCookies(['token']);

    const [transactions, setTransactions] = useState<Transaction[]>([]);

    const { data } = useQuery<Result<Transaction[]>>({
        queryKey: [queryKeyTransactions],
        queryFn: () => {
            return getAllTransactions(cookies.token)
        },
    });

    useEffect(() => {
        if (data == null || data.hasError || data.item == undefined) {

        } else {
            setTransactions(data?.item)
        }
    }, [data]);

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead className="w-[100px]">Id</TableHead>
                    <TableHead>Date</TableHead>
                    <TableHead>Payee</TableHead>
                    <TableHead className="text-right">Payment</TableHead>
                    <TableHead></TableHead>{/* row actions */}
                </TableRow>
            </TableHeader>
            <TableBody>
                {transactions.map((transaction) => (
                    <RegisterTableRow key={transaction.id} transaction={transaction} />
                ))}
            </TableBody>
        </Table>
    )
}
