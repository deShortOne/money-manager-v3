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
import { Calendar } from "@/components/ui/calendar"
import { format } from "date-fns"
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
import { CalendarIcon } from "lucide-react";
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
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { cn } from "@/lib/utils";
import { useRegisterModalSetting } from "../hooks/useEditRegisterForm";
import { queryKeyAccounts, queryKeyCategories, queryKeyTransactions } from "@/app/data/queryKeys";
import { getAllAccounts, getAllCategories } from "./action";
import { Category } from "@/interface/category";
import { Account } from "@/interface/account";

export function UpdateTransactionForm() {
    const [cookies] = useCookies(['token']);
    const open = useRegisterModalSetting(state => state.isOpen);
    const defaultValues = useRegisterModalSetting(state => state.defaultValues);
    const closeUpdateTransactionForm = useRegisterModalSetting(state => state.onClose);
    const registerAction = useRegisterModalSetting(state => state.updateRegisterAction);

    const [addNewBillButtonErrorMessage, setAddNewBillButtonErrorMessage] = useState("");
    const queryClient = useQueryClient();

    const [accounts, setAccounts] = useState<Account[]>([]);
    const { data: dataAccounts } = useQuery<Result<Account[]>>({
        queryKey: [queryKeyAccounts],
        queryFn: () => getAllAccounts(cookies.token),
    });
    useEffect(() => {
        if (dataAccounts == null || dataAccounts.hasError || dataAccounts.item == undefined) {

        } else {
            setAccounts(dataAccounts.item)
        }
    }, [dataAccounts]);

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
        payee: z.union([
            z.string({
                required_error: "You must select the account the funds will go to",
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
        payer: z.union([
            z.string({
                required_error: "You must select the account the funds will come from",
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
        amount: z.coerce.number({
            required_error: "You must enter an amount",
        }),
        datePaid: z.date({
            required_error: "You must select the next date this bill will occur.",
        }),
        category: z.union([
            z.string({
                required_error: "You must select a category.",
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
            z.number(),
        ]),
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            payee: undefined,
            payer: undefined,
            amount: 0,
            datePaid: undefined,
            category: undefined,
        },
    });

    async function onSubmit(values: z.infer<typeof formSchema>) {
        const addNewBillResult = await registerAction(cookies.token, {
            payee: values.payee,
            amount: values.amount,
            datePaid: values.datePaid,
            category: values.category,
            account: values.payer,
        });

        if (addNewBillResult.hasError) {
            setAddNewBillButtonErrorMessage(addNewBillResult.errorMessage);
            return;
        }
        closeUpdateTransactionForm();
        queryClient.invalidateQueries({ queryKey: [queryKeyTransactions] })
    }

    function CloseWithoutSaving() {
        form.clearErrors();
        closeUpdateTransactionForm();
    }

    useEffect(() => {
        form.reset({
            payee: defaultValues.payee,
            payer: defaultValues.accountId,
            amount: defaultValues.amount,
            datePaid: defaultValues.datePaid,
            category: defaultValues.category,
        });
    }, [open]);

    return (
        <Dialog open={open} onOpenChange={closeUpdateTransactionForm}>
            <DialogContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="">
                        <DialogHeader>
                            <DialogTitle>Save transaction</DialogTitle>
                            <DialogDescription>
                                Transaction that you've made or will make
                            </DialogDescription>
                        </DialogHeader>

                        <FormField
                            control={form.control}
                            name="payee"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Pay to:</FormLabel>
                                    <Select
                                        onValueChange={field.onChange}
                                        value={field.value?.toString()}
                                    >
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Select payee's account" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            {accounts.map((account) => (
                                                <SelectItem key={account.id} value={account.id.toString()}>
                                                    {account.name}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                    <FormDescription>
                                        This is the account you are sending money to.
                                    </FormDescription>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <div className="grid grid-cols-2">
                            <FormField
                                control={form.control}
                                name="payer"
                                render={({ field }) => (
                                    <FormItem className="mr-2">
                                        <FormLabel>Pay from:</FormLabel>
                                        <Select
                                            onValueChange={field.onChange}
                                            value={field.value?.toString()}
                                        >
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder="Select payee's account" />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {accounts.map((account) => (
                                                    <SelectItem key={account.id} value={account.id.toString()}>
                                                        {account.name}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormDescription>
                                            This is the account you are sending money from.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name="datePaid"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Date transaction was made:</FormLabel>
                                        <Popover modal={true}>
                                            <PopoverTrigger asChild>
                                                <FormControl>
                                                    <Button
                                                        variant={"outline"}
                                                        className={cn(
                                                            "w-[240px] pl-3 text-left font-normal",
                                                            !field.value && "text-muted-foreground"
                                                        )}
                                                    >
                                                        {field.value ? (
                                                            format(field.value, "PPP")
                                                        ) : (
                                                            <span>Pick a date</span>
                                                        )}
                                                        <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                                                    </Button>
                                                </FormControl>
                                            </PopoverTrigger>
                                            <PopoverContent className="w-auto p-0" align="start">
                                                <Calendar
                                                    mode="single"
                                                    selected={field.value}
                                                    onSelect={field.onChange}
                                                    initialFocus
                                                />
                                            </PopoverContent>
                                        </Popover>
                                        <FormDescription>
                                            This is when this transaction was made.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name="amount"
                                render={({ field }) => (
                                    <FormItem className="mr-2 my-2">
                                        <FormLabel>Amount</FormLabel>
                                        <FormControl>
                                            <Input placeholder="" {...field} />
                                        </FormControl>
                                        <FormDescription>
                                            This is the amount of money you are sending.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <FormField
                                control={form.control}
                                name="category"
                                render={({ field }) => (
                                    <FormItem className="col-span-2">
                                        <FormLabel>Category:</FormLabel>
                                        <Select
                                            onValueChange={field.onChange}
                                            value={field.value?.toString()}
                                        >
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder="Select category" />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {categories.map((categoryA) => (
                                                    <SelectItem key={categoryA.id} value={categoryA.id.toString()}>
                                                        {categoryA.name}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormDescription>
                                            Category of payment.
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
                                Save transaction
                            </Button>
                        </DialogFooter>
                        <FormMessage>{addNewBillButtonErrorMessage}</FormMessage>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    )
}
