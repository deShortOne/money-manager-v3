'use client'

import {
    AlertDialog,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
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
import { useQuery } from "@tanstack/react-query";
import { CalendarIcon, PlusCircleIcon } from "lucide-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { getAllAccounts, getAllFrequencyNames } from "./action";
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

export function AddNewBill() {
    const [cookies] = useCookies(['token']);
    const [open, setOpen] = useState(false);

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
        amount: z.number(),
        nextDueDate: z.date({
            required_error: "You must select the next date this bill will occur.",
        }),
        frequency: z.string(),
        category: z.string(),
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            payee: undefined,
            payer: undefined,
            amount: 0,
            nextDueDate: undefined,
            frequency: "",
            category: "",
        },
    });
    function onSubmit(values: z.infer<typeof formSchema>) {
        // Do something with the form values.
        // ✅ This will be type-safe and validated.
        console.log(values);
        setOpen(false);
    }

    return (
        <AlertDialog open={open} onOpenChange={setOpen}>
            <AlertDialogTrigger asChild>
                <Button variant="outline">
                    <PlusCircleIcon />
                    Add New Bill
                </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="">
                        <AlertDialogHeader>
                            <AlertDialogTitle>Make scheduled payments</AlertDialogTitle>
                            <AlertDialogDescription>
                                Schedule payments
                            </AlertDialogDescription>
                        </AlertDialogHeader>

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
                                        <Popover>
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
                                        <FormControl>
                                            <Input placeholder="" {...field} />
                                        </FormControl>
                                        <FormDescription>
                                            Category of payment.
                                        </FormDescription>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </div>
                        <AlertDialogFooter>
                            <AlertDialogCancel>Cancel</AlertDialogCancel>
                            <Button type="submit">
                                Schedule Payment
                            </Button>
                        </AlertDialogFooter>
                    </form>
                </Form>
            </AlertDialogContent>
        </AlertDialog>
    )
}
