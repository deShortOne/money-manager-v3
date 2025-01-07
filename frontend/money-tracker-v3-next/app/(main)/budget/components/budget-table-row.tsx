import { Button } from "@/components/ui/button";
import {
    TableCell,
    TableRow
} from "@/components/ui/table";
import { Pencil } from "lucide-react";
import { useEffect, useState } from "react";
import { useBudgetModalSetting } from "../hooks/useEditBudgetForm";
import { useQuery } from "@tanstack/react-query";
import { Result } from "@/types/result";
import { queryKeyCategories } from "@/app/data/queryKeys";
import { editBudgetCategory, getAllCategories } from "./action";

interface prop {
    budgetGroupId: number,
    budgetCategory: BudgetCategory,
}

export default function BudgetTableCategoryRow({
    budgetGroupId,
    budgetCategory
}: prop) {
    const [msg, setMsg] = useState("");
    const onOpen = useBudgetModalSetting(state => state.onOpen);

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

    function editBillForm() {
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
                <Button onClick={() => editBillForm()}>
                    <Pencil />
                </Button>
            </TableCell>
        </TableRow>
    )
}
