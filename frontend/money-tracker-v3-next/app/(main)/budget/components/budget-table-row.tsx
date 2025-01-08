import { Button } from "@/components/ui/button";
import {
    TableCell,
    TableRow
} from "@/components/ui/table";
import { Pencil, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { useBudgetModalSetting } from "../hooks/useEditBudgetForm";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { Result } from "@/types/result";
import { queryKeyBudget, queryKeyCategories } from "@/app/data/queryKeys";
import { deleteBudgetCategory, editBudgetCategory, getAllCategories } from "./action";
import { useCookies } from "react-cookie";
import { BudgetCategory, UpdateBudgetCategory } from "@/interface/budgetGroup";
import { Category } from "@/interface/category";

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

    const [categories, setCategories] = useState<Category[]>([]);
    const { data: dataCategories } = useQuery<Result<Category[]>>({
        queryKey: [queryKeyCategories],
        queryFn: () => getAllCategories(),
    });
    useEffect(() => {
        if (dataCategories == null || dataCategories.hasError || dataCategories.item == undefined) {

        } else {
            setCategories(dataCategories.item);
        }
    }, [dataCategories]);

    function editBudgetForm() {
        const categoryId = categories.find(x => x.name == budgetCategory.name);
        if (categoryId == null) {
            setMsg("Category not found");
            return;
        }

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
                categoryId: categoryId.id,
                planned: budgetCategory.planned,
            });
    }

    async function deleteBudgetFunc() {
        const categoryId = categories.find(x => x.name == budgetCategory.name);
        if (categoryId == null) {
            setMsg("Category not found");
            return;
        }

        const deleteBillResult = await deleteBudgetCategory(cookies.token, budgetGroupId, categoryId.id);

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
