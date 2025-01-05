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
import {
    addNewBill,
    getAllAccounts,
    getAllCategories,
    getAllFrequencyNames,
} from "./action";
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
import { useBillModalSetting } from "../hooks/useEditBillForm";

export function UpdateBillForm() {
    const [cookies] = useCookies(['token']);
    const open = useBillModalSetting(state => state.isOpen);
    const defaultValues = useBillModalSetting(state => state.defaultValues);
    const onClose = useBillModalSetting(state => state.onClose);

    const [addNewBillButtonErrorMessage, setAddNewBillButtonErrorMessage] = useState("");
    const queryClient = useQueryClient();

    const [accounts, setAccounts] = useState<Account[]>([]);
    const { data: dataAccounts } = useQuery<Result<Account[]>>({
        queryKey: ['accounts'],
        queryFn: () => getAllAccounts(cookies.token),
    });
    useEffect(() => {
        if (dataAccounts == null || dataAccounts.hasError || dataAccounts.item == undefined) {

        } else {
            setAccounts(dataAccounts.item)
        }
    }, [dataAccounts]);

    const [frequencies, setFrequencies] = useState<string[]>([]);
    const { data: dataFrequencies } = useQuery<Result<string[]>>({
        queryKey: ['frequencies'],
        queryFn: () => getAllFrequencyNames(),
    });
    useEffect(() => {
        if (dataFrequencies == null || dataFrequencies.hasError || dataFrequencies.item == undefined) {

        } else {
            setFrequencies(dataFrequencies.item)
        }
    }, [dataFrequencies]);

    const [categories, setCategories] = useState<Category[]>([]);
    const { data: dataCategories } = useQuery<Result<Category[]>>({
        queryKey: ['categories'],
        queryFn: () => getAllCategories(),
    });
    useEffect(() => {
        if (dataCategories == null || dataCategories.hasError || dataCategories.item == undefined) {

        } else {
            setCategories(dataCategories.item)
            console.log(dataCategories.item);
        }
    }, [dataCategories]);

    const formSchema = z.object({
        payee: z.string({
            required_error: "You must select the account the funds will go to",
        })
            .min(1, { message: "You must select an account" })
            .transform((val, ctx) => {
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
        payer: z.string({
            required_error: "You must select the account the funds will come from",
        })
            .min(1, { message: "You must select an account" })
            .transform((val, ctx) => {
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
        amount: z.coerce.number({
            required_error: "You must enter an amount",
        }),
        nextDueDate: z.date({
            required_error: "You must select the next date this bill will occur.",
        }),
        frequency: z.string({
            required_error: "You must select the frequency of this bill.",
        }),
        category: z.string({
            required_error: "You must select a category.",
        })
            .min(1, { message: "You must select a category" })
            .transform((val, ctx) => {
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
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            payee: defaultValues.payee,
            payer: defaultValues.payer,
            amount: defaultValues.amount,
            nextDueDate: defaultValues.nextDueDate,
            frequency: defaultValues.frequency,
            category: defaultValues.category,
        },
    });
    async function onSubmit(values: z.infer<typeof formSchema>) {
        const addNewBillResult = await addNewBill(cookies.token, {
            payee: values.payee,
            amount: values.amount,
            nextDueDate: values.nextDueDate,
            frequency: values.frequency,
            categoryId: values.category,
            accountId: values.payer,
        });
        if (addNewBillResult.hasError) {
            setAddNewBillButtonErrorMessage(addNewBillResult.errorMessage);
            return;
        }
        onClose();
        queryClient.invalidateQueries({ queryKey: ['bills'] })
    }

    function CloseWithoutSaving() {
        form.clearErrors();
        onClose();
    }

    return (
        <Dialog open={open} onOpenChange={onClose}>
            <DialogContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="">
                        <DialogHeader>
                            <DialogTitle>Make scheduled payments</DialogTitle>
                            <DialogDescription>
                                Schedule payments
                            </DialogDescription>
                        </DialogHeader>

                        <FormField
                            control={form.control}
                            name="payee"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Pay to:</FormLabel>
                                    <Select onValueChange={field.onChange}>
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
                                        <Select onValueChange={field.onChange}>
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
                                name="nextDueDate"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Next scheduled date:</FormLabel>
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
                                            This is the account you are sending money to.
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
                                name="frequency"
                                render={({ field }) => (
                                    <FormItem className="my-2">
                                        <FormLabel>Frequency:</FormLabel>
                                        <Select onValueChange={field.onChange}>
                                            <FormControl>
                                                <SelectTrigger>
                                                    <SelectValue placeholder="Select frequency" />
                                                </SelectTrigger>
                                            </FormControl>
                                            <SelectContent>
                                                {frequencies.map((frequency) => (
                                                    <SelectItem key={frequency} value={frequency}>
                                                        {frequency}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                        <FormDescription>
                                            Number of occurences of payments.
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
                                        <Select onValueChange={field.onChange}>
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
                                Schedule Payment
                            </Button>
                        </DialogFooter>
                        <FormMessage>{addNewBillButtonErrorMessage}</FormMessage>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    )
}
