'use client'

import { Checkbox } from "@/components/ui/checkbox"
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
import { zodResolver } from "@hookform/resolvers/zod";
import { useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { useCookies } from "react-cookie";
import { useAccountModalSetting } from "../hooks/useEditAccountForm";
import { queryKeyAccounts } from "@/app/data/queryKeys";

export function UpdateAccountsForm() {
    const [cookies] = useCookies(['token']);
    const open = useAccountModalSetting(state => state.isOpen);
    const defaultValues = useAccountModalSetting(state => state.defaultValues);
    const closeUpdateAccountForm = useAccountModalSetting(state => state.onClose);
    const accountAction = useAccountModalSetting(state => state.updateAccountAction);

    const [addNewAccountButtonErrorMessage, setAddNewAccountButtonErrorMessage] = useState("");
    const queryClient = useQueryClient();

    const formSchema = z.object({
        accountName: z.string({
            required_error: "You must set the name for this new account",
        }),
        doesUserOwnAccount: z.boolean(),
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            accountName: undefined,
            doesUserOwnAccount: false,
        },
    });
    async function onSubmit(values: z.infer<typeof formSchema>) {
        const addNewAccountResult = await accountAction(cookies.token, {
            name: values.accountName,
            doesUserOwnAccount: values.doesUserOwnAccount
        });

        if (addNewAccountResult.hasError) {
            setAddNewAccountButtonErrorMessage(addNewAccountResult.errorMessage);
            return;
        }
        closeUpdateAccountForm();
        queryClient.invalidateQueries({ queryKey: [queryKeyAccounts] })
    }

    function CloseWithoutSaving() {
        form.clearErrors();
        closeUpdateAccountForm();
    }

    useEffect(() => {
        form.reset({
            accountName: defaultValues.name,
            doesUserOwnAccount: defaultValues.doesUserOwnAccount,
        });
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [open]);

    return (
        <Dialog open={open} onOpenChange={closeUpdateAccountForm}>
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
                            name="accountName"
                            render={({ field }) => (
                                <FormItem className="mr-2 my-2">
                                    <FormLabel>Account name</FormLabel>
                                    <FormControl>
                                        <Input placeholder="" {...field} />
                                    </FormControl>
                                    <FormDescription>
                                        This is the new account you want to keep a record of.
                                    </FormDescription>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="doesUserOwnAccount"
                            render={({ field }) => (
                                <FormItem className="mr-2 my-2">
                                    <div className="flex items-center space-x-2">
                                        <FormControl>
                                            <Checkbox
                                                checked={field.value}
                                                onCheckedChange={field.onChange}
                                            />
                                        </FormControl>
                                        <div>
                                            <FormLabel>Is this account yours?</FormLabel>
                                            <FormDescription>
                                                Your accounts will be listed at the top.
                                            </FormDescription>
                                        </div>
                                    </div>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <DialogFooter>
                            <Button type="button" variant="secondary" onClick={CloseWithoutSaving}>
                                Close without saving
                            </Button>
                            <Button type="submit">
                                Create account
                            </Button>
                        </DialogFooter>
                        <FormMessage>{addNewAccountButtonErrorMessage}</FormMessage>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    )
}
