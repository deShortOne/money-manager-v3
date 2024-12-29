
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
import { Button } from "@/components/ui/button";
import React from "react";

export default function BudgetsDisplay() {
    const [expandedBudgetCategory, setExpandedBudgetCategory] = useState<string[]>([])
    const toggleRow = (index: string) => {
        if (expandedBudgetCategory.includes(index)) {
            setExpandedBudgetCategory(expandedBudgetCategory.filter((i) => i !== index))
        } else {
            setExpandedBudgetCategory([...expandedBudgetCategory, index])
        }
    }

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
            setExpandedBudgetCategory(data.item.map(i => i.name))
            setTransactions(data.item)
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
                    <React.Fragment key={transaction.name}>
                        <TableRow>
                            <TableCell>
                                <Button variant="ghost" onClick={() => toggleRow(transaction.name)}>
                                    {transaction.name}
                                </Button>
                            </TableCell>
                            <TableCell className="text-right">{transaction.actual}</TableCell>
                            <TableCell className="text-right">{transaction.planned}</TableCell>
                            <TableCell className="text-right">{transaction.difference}</TableCell>
                        </TableRow>

                        {expandedBudgetCategory.includes(transaction.name) && (
                            <>
                                {transaction.categories.map((category) => (
                                    <TableRow key={category.name}>
                                        <TableCell className="">
                                            <span className="ml-8">{category.name}</span>
                                        </TableCell>
                                        <TableCell className="text-right">{category.actual}</TableCell>
                                        <TableCell className="text-right">{category.planned}</TableCell>
                                        <TableCell className="text-right">{category.difference}</TableCell>
                                    </TableRow>
                                ))}
                            </>
                        )}
                    </React.Fragment>
                ))}
            </TableBody>
        </Table>
    )
}
