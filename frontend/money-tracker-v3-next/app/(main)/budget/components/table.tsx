
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { useEffect, useState } from "react";
import { getAllBudgets } from "./action";
import { useCookies } from "react-cookie";
import { useQuery } from "@tanstack/react-query";
import { Result } from "@/types/result";

export default function BudgetsDisplay() {
    const [cookies] = useCookies(['token']);

    const [transactions, setTransactions] = useState<BudgetGroup[]>([]);

    const { status, data, error, isFetching } = useQuery<Result<BudgetGroup[]>>({
        queryKey: ['transactions'],
        queryFn: () => {
            return getAllBudgets(cookies.token)
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
                    <TableHead className="">Budget Category</TableHead>
                    <TableHead className="text-right">Actual</TableHead>
                    <TableHead className="text-right">Budgeted</TableHead>
                    <TableHead className="text-right">Difference</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {transactions.map((transaction) => (
                    <TableRow key={transaction.name}>
                        <TableCell>{transaction.name}</TableCell>
                        <TableCell className="text-right">{transaction.actual}</TableCell>
                        <TableCell className="text-right">{transaction.planned}</TableCell>
                        <TableCell className="text-right">{transaction.difference}</TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </Table>
    )
}
