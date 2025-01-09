import { Button } from "@/components/ui/button";
import {
    TableCell,
    TableRow
} from "@/components/ui/table";
import { Pencil, Trash2 } from "lucide-react";
import { useState } from "react";
import { useBudgetModalSetting } from "../hooks/useEditBudgetForm";
import { useQueryClient } from "@tanstack/react-query";
import { queryKeyBudget } from "@/app/data/queryKeys";
import { deleteBudgetCategory, editBudgetCategory } from "./action";
import { useCookies } from "react-cookie";
import { BudgetCategory, UpdateBudgetCategory } from "@/interface/budgetGroup";

interface prop {
    budgetGroupId: number,
    budgetCategory: BudgetCategory,
}

export default function BudgetTableCategoryRow({
    budgetGroupId,
    budgetCategory
}: prop) {
    const [cookies] = useCookies(['token']);
    const [msg, setMsg] = useState("");
    const onOpen = useBudgetModalSetting(state => state.onOpen);

    const queryClient = useQueryClient();

    function editBudgetForm() {
        onOpen(
            (authToken: string, updateBudgetCategory: UpdateBudgetCategory) => {
                return editBudgetCategory(authToken, {
                    budgetGroupId: updateBudgetCategory.budgetGroupId,
                    categoryId: updateBudgetCategory.categoryId,
                    planned: updateBudgetCategory.planned,
                });
            },
            {
                budgetGroupId: budgetGroupId,
                categoryId: budgetCategory.id,
                planned: budgetCategory.planned,
            });
    }

    async function deleteBudgetFunc() {
        const deleteBillResult = await deleteBudgetCategory(cookies.token, budgetGroupId, budgetCategory.id);

        if (deleteBillResult.hasError) {
            setMsg(deleteBillResult.errorMessage);
            return;
        }
        queryClient.invalidateQueries({ queryKey: [queryKeyBudget] })
    }

    return (
        <TableRow>
            <TableCell className="">
                <span className="ml-8">{budgetCategory.name}</span>
            </TableCell>
            <TableCell className="text-right">{budgetCategory.planned}</TableCell>
            <TableCell className="text-right">{budgetCategory.actual}</TableCell>
            <TableCell className="text-right">{budgetCategory.difference}</TableCell>
            <TableCell>
                {msg}
                <Button onClick={() => editBudgetForm()}>
                    <Pencil />
                </Button>
                <Button onClick={() => deleteBudgetFunc()}>
                    <Trash2 />
                </Button>
            </TableCell>
        </TableRow>
    )
}
