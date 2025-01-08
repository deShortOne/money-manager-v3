
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
import { queryKeyBudget } from "@/app/data/queryKeys";
import BudgetTableCategoryRow from "./budget-table-row";
import { BudgetGroup } from "@/interface/budgetGroup";

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
    const [budgetGroups, setBudgetGroups] = useState<BudgetGroup[]>([]);
    const { status, data, error, isFetching } = useQuery<Result<BudgetGroup[]>>({
        queryKey: [queryKeyBudget],
        queryFn: () => {
            return getAllBudgets(cookies.token)
        },
    });
    useEffect(() => {
        if (data == null || data.hasError || data.item == undefined) {

        } else {
            setExpandedBudgetCategory(data.item.map(i => i.name))
            setBudgetGroups(data.item)
        }
    }, [data]);

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead className="">Budget Category</TableHead>
                    <TableHead className="text-right">Planned</TableHead>
                    <TableHead className="text-right">Actual</TableHead>
                    <TableHead className="text-right">Difference</TableHead>
                    <TableHead></TableHead>{/* row actions */}
                </TableRow>
            </TableHeader>
            <TableBody>
                {budgetGroups.map((budgetGroup) => (
                    <React.Fragment key={budgetGroup.name}>
                        <TableRow>
                            <TableCell>
                                <Button variant="ghost" onClick={() => toggleRow(budgetGroup.name)}>
                                    {budgetGroup.name}
                                </Button>
                            </TableCell>
                            <TableCell className="text-right">{budgetGroup.planned}</TableCell>
                            <TableCell className="text-right">{budgetGroup.actual}</TableCell>
                            <TableCell className="text-right">{budgetGroup.difference}</TableCell>
                        </TableRow>

                        {expandedBudgetCategory.includes(budgetGroup.name) && (
                            <>
                                {budgetGroup.categories.map((category) => (
                                    <BudgetTableCategoryRow
                                        budgetGroupId={budgetGroup.id}
                                        budgetCategory={category}
                                        key={category.name}
                                    />
                                ))}
                            </>
                        )}
                    </React.Fragment>
                ))}
            </TableBody>
        </Table>
    )
}
