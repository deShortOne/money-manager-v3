'use client'

import { Checkbox } from "@/components/ui/checkbox"
import {
    Form,
    FormControl,
    FormDescription,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { calculateWage } from "./action";
import { Button } from "@/components/ui/button";

export function WageCalculatorForm() {
    const studentLoanPlans: { id: "studentLoanOptions.plan1" | "studentLoanOptions.plan2" | "studentLoanOptions.plan4" | "studentLoanOptions.plan5" | "studentLoanOptions.postgraduate", name: string }[] = [
        {
            id: "studentLoanOptions.plan1",
            name: "Plan 1",
        },
        {
            id: "studentLoanOptions.plan2",
            name: "Plan 2",
        },
        {
            id: "studentLoanOptions.plan4",
            name: "Plan 4",
        },
        {
            id: "studentLoanOptions.plan5",
            name: "Plan 5",
        },
        {
            id: "studentLoanOptions.postgraduate",
            name: "Post Graduate",
        },
    ];

    const pensionSchema = z.object({
        type: z.string(),
        value: z.coerce.number().gt(0),
        rate: z.string(),
    });
    const formSchema = z.object({
        grossIncome: z.coerce.number({
            required_error: "You must enter your income before taxes",
        }),
        frequencyOfIncome: z.string({
            required_error: "You must enter how often you receive said income",
        }),
        taxCode: z.string(),
        paysNationalInsurance: z.boolean(),
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        pension: z.preprocess((input: any) => {
            if (input == null)
                return null;

            if (input.type === "")
                input.type = null;
            if (input.value === "")
                input.value = null;
            if (input.rate === "")
                input.rate = null;

            if (input.type === null &&
                input.value === null &&
                input.rate === null) return null;
            return input;
        }, z.union([
            z.null(),
            pensionSchema,
        ])),
        studentLoanOptions: z.object({
            plan1: z.boolean(),
            plan2: z.boolean(),
            plan4: z.boolean(),
            plan5: z.boolean(),
            postgraduate: z.boolean(),
        }),
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            grossIncome: 30000,
            frequencyOfIncome: "Yearly",
            taxCode: "1257L",
            paysNationalInsurance: true,
            pension: null,
            studentLoanOptions: {
                plan1: false,
                plan2: false,
                plan4: false,
                plan5: false,
                postgraduate: false,
            }
        },
    });

    async function onSubmit(values: z.infer<typeof formSchema>) {
        const calculateWageResult = await calculateWage({
            grossIncome: values.grossIncome,
            frequencyOfIncome: values.frequencyOfIncome,
            taxCode: values.taxCode,
            payNationalInsurance: values.paysNationalInsurance,
            pension: values.pension,
            studentLoanOptions: values.studentLoanOptions,
        });
        if (calculateWageResult.hasError)
            console.log(calculateWageResult.errorMessage)
        else
            console.log(calculateWageResult.item)
    }

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="">
                <FormField
                    control={form.control}
                    name="grossIncome"
                    render={({ field }) => (
                        <FormItem className="mr-2 my-2">
                            <FormLabel>Gross Income</FormLabel>
                            <FormControl>
                                <Input placeholder="" {...field} />
                            </FormControl>
                            <FormDescription>
                                This is the amount of money you are making before taxes.
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="frequencyOfIncome"
                    render={({ field }) => (
                        <FormItem className="mr-2 my-2">
                            <FormLabel>Gross Income</FormLabel>
                            <FormControl>
                                <Input placeholder="" {...field} />
                            </FormControl>
                            <FormDescription>
                                This is the how often you earn the income you&apos;ve mentioned.
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="taxCode"
                    render={({ field }) => (
                        <FormItem className="mr-2 my-2">
                            <FormLabel>Gross Income</FormLabel>
                            <FormControl>
                                <Input placeholder="" {...field} />
                            </FormControl>
                            <FormDescription>
                                This is the tax code you have, defaults to 1257L.
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="paysNationalInsurance"
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
                                    <FormLabel>Do you pay national insurance?</FormLabel>
                                    <FormDescription>
                                        You won&apos;t pay if you are over state pension age unless
                                        you&apos;re self-employed and pay class4 contributions.
                                    </FormDescription>
                                </div>
                            </div>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="pension.type"
                    render={({ field }) => (
                        <FormItem className="mr-2 my-2">
                            <FormLabel>Pension Type</FormLabel>
                            <FormControl>
                                <Input placeholder="" {...field} />
                            </FormControl>
                            <FormDescription>
                                AutoEnrolment or Personal.
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="pension.value"
                    render={({ field }) => (
                        <FormItem className="mr-2 my-2">
                            <FormLabel>Value from income</FormLabel>
                            <FormControl>
                                <Input placeholder="" {...field} />
                            </FormControl>
                            <FormDescription>
                                How much is taken off each month?
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />
                <FormField
                    control={form.control}
                    name="pension.rate"
                    render={({ field }) => (
                        <FormItem className="mr-2 my-2">
                            <FormLabel>Pension Type</FormLabel>
                            <FormControl>
                                <Input placeholder="" {...field} />
                            </FormControl>
                            <FormDescription>
                                Amount or Percentage.
                            </FormDescription>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                {studentLoanPlans.map((studentLoanPlan) => (
                    <FormField
                        key={studentLoanPlan.id}
                        control={form.control}
                        name={studentLoanPlan.id}
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
                                        <FormLabel>{studentLoanPlan.name}</FormLabel>
                                    </div>
                                </div>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                ))}

                <FormMessage></FormMessage>
                <Button type="submit">
                    Calculate
                </Button>
            </form>
        </Form >
    )
}
