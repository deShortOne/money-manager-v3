'use client'

import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
    Form,
    FormControl,
    FormDescription,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Result } from "@/types/result";
import { zodResolver } from "@hookform/resolvers/zod";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useCookies } from "react-cookie";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import { useBudgetModalSetting } from "../hooks/useEditBudgetForm";
import { queryKeyBudget, queryKeyCategories } from "@/app/data/queryKeys";
import { getAllBudgets, getAllCategories } from "./action";

export function UpdateBudgetForm() {
    const [cookies] = useCookies(['token']);
    const open = useBudgetModalSetting(state => state.isOpen);
    const defaultValues = useBudgetModalSetting(state => state.defaultValues);
    const closeUpdateBillForm = useBudgetModalSetting(state => state.onClose);
    const budgetAction = useBudgetModalSetting(state => state.updateBudgetAction);

    const [addNewBudgetButtonErrorMessage, setAddNewBillButtonErrorMessage] = useState("");
    const queryClient = useQueryClient();

    const [budgetGroups, setBudgetGroups] = useState<BudgetGroup[]>([]);

    const { status, data, error, isFetching } = useQuery<Result<BudgetGroup[]>>({
        queryKey: [queryKeyBudget],
        queryFn: () => getAllBudgets(cookies.token),
    });
    useEffect(() => {
        if (data == null || data.hasError || data.item == undefined) {

        } else {
            setBudgetGroups(data.item)
        }
    }, [data]);

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

    const formSchema = z.object({
        budgetGroupId: z.union([
            z.string({
                required_error: "You must select the budget group to assign to",
            }).transform((val, ctx) => {
                const parsed = parseInt(val);
                if (isNaN(parsed)) {
                    ctx.addIssue({
                        code: z.ZodIssueCode.custom,
                        message: "Not a number",
                    });
                    return z.NEVER;
                }
                return parsed;
            }),
            z.number()
        ]),
        categoryId: z.union([
            z.string({
                required_error: "You must select the category this will be",
            }).transform((val, ctx) => {
                const parsed = parseInt(val);
                if (isNaN(parsed)) {
                    ctx.addIssue({
                        code: z.ZodIssueCode.custom,
                        message: "Not a number",
                    });
                    return z.NEVER;
                }
                return parsed;
            }),
            z.number()
        ]),
        planned: z.coerce.number({
            required_error: "You must enter a planned amount",
        }),
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            budgetGroupId: undefined,
            categoryId: undefined,
            planned: 0,
        },
    });
    async function onSubmit(values: z.infer<typeof formSchema>) {
        const addNewBillResult = await budgetAction(cookies.token, {
            budgetGroupId: values.budgetGroupId,
            categoryId: values.categoryId,
            planned: values.planned,
        });

        if (addNewBillResult.hasError) {
            setAddNewBillButtonErrorMessage(addNewBillResult.errorMessage);
            return;
        }
        closeUpdateBillForm();
        queryClient.invalidateQueries({ queryKey: [queryKeyBudget] })
    }

    function CloseWithoutSaving() {
        form.clearErrors();
        closeUpdateBillForm();
    }

    useEffect(() => {
        form.reset({
            budgetGroupId: defaultValues.budgetGroupId,
            categoryId: defaultValues.categoryId,
            planned: defaultValues.planned,
        });
    }, [open]);

    return (
        <Dialog open={open} onOpenChange={closeUpdateBillForm}>
            <DialogContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="">
                        <DialogHeader>
                            <DialogTitle>Make a budget item</DialogTitle>
                            <DialogDescription>
                                Budgeting your money
                            </DialogDescription>
                        </DialogHeader>

                        <FormField
                            control={form.control}
                            name="budgetGroupId"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Budget Group:</FormLabel>
                                    <Select
                                        onValueChange={field.onChange}
                                        value={field.value?.toString()}
                                    >
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Select budget group" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {budgetGroups.map((budgetGroup) => (
                                                <SelectItem key={budgetGroup.id} value={budgetGroup.id.toString()}>
                                                    {budgetGroup.name}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormDescription>
                                        This is the budget group you are setting this category to.
                                    </FormDescription>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className="grid grid-cols-2">
                            <FormField
                                control={form.control}
                                name="categoryId"
                                render={({ field }) => (
                                    <FormItem className="mr-2">
                                        <FormLabel>Category:</FormLabel>
                                        <Select
                                            onValueChange={field.onChange}
                                            value={field.value?.toString()}
                                            disabled={defaultValues.categoryId != undefined}
                                        >
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder="Select category" />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {categories.map((category) => (
                                                    <SelectItem key={category.id} value={category.id.toString()}>
                                                        {category.name}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormDescription>
                                            This is the category to add to the budget group.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name="planned"
                                render={({ field }) => (
                                    <FormItem className="mr-2 my-2">
                                        <FormLabel>Planned</FormLabel>
                                        <FormControl>
                                            <Input placeholder="" {...field} />
                                        </FormControl>
                                        <FormDescription>
                                            This is the amount of money you plan on spending.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                        <DialogFooter>
                            <Button type="button" variant="secondary" onClick={CloseWithoutSaving}>
                                Close without saving
                            </Button>
                            <Button type="submit">
                                Add budget item
                            </Button>
                        </DialogFooter>
                        <FormMessage>{addNewBudgetButtonErrorMessage}</FormMessage>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    )
}
