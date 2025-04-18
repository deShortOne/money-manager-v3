'use client'

import { useSearchParams, useRouter } from 'next/navigation'
import { useCookies } from 'react-cookie'
import { loginUser } from "./action"
import { Button } from "@/components/ui/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormMessage,
} from "@/components/ui/form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { z } from "zod"
import { Result } from '@/types/result'
import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { LoadingSpinner } from '@/components/ui/loading-spinner'

export default function SignInPage() {
    const [cookies, setCookies] = useCookies(['token']);
    const [signInButtonErrorMsg, setSignInButtonErrorMsg] = useState("");
    const searchParams = useSearchParams();
    const redirectUrl = "/" + (searchParams.get('redirect_url') ?? "budget");
    const router = useRouter();

    const formSchema = z.object({
        // TODO: add validation
        username: z.string(),
        password: z.string(),
    });
    const form = useForm<z.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            username: "",
            password: "",
        },
    });

    const { data: dataTokenCheck, isFetching: isFecthingTokenCheck } = useQuery<boolean>({
        queryKey: ["f"],
        queryFn: () => fetch("api/token-validation", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            }
        }).then(async (x) => await x.json()),
        initialData: false,
        enabled: cookies.token != null
    });

    if (isFecthingTokenCheck) {
        return (<LoadingSpinner suppressHydrationWarning></LoadingSpinner>)
    }
    if (dataTokenCheck) {
        router.replace(redirectUrl);
        return (<div suppressHydrationWarning>You&apos;ve logged in, navigating to {redirectUrl}</div>);
    }

    async function onSubmit(values: z.infer<typeof formSchema>) {
        const cookie: Result<string> = await loginUser(values.username, values.password);
        if (cookie.hasError) {
            setSignInButtonErrorMsg(cookie.errorMessage);
        } else {
            setCookies("token", cookie.item, { sameSite: 'strict' });
            setSignInButtonErrorMsg("");
            router.replace(redirectUrl);
        }
    }

    return (
        <div className="inline-flex items-center justify-center w-full h-[90vh]">
            <Card className="w-1/4">
                <CardHeader>
                    <CardTitle>Welcome back!</CardTitle>
                    <CardDescription>Please log in</CardDescription>
                </CardHeader>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="">
                        <CardContent>
                            <FormField
                                control={form.control}
                                name="username"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormControl>
                                            <Input placeholder="Username" {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                            <div className="mt-2" />
                            <FormField
                                control={form.control}
                                name="password"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormControl>
                                            <Input placeholder="Password" {...field} />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />
                        </CardContent>
                        <CardFooter className="block">
                            <Button type="submit">Sign in</Button>
                            <FormMessage>{signInButtonErrorMsg}</FormMessage>
                        </CardFooter>
                    </form>
                </Form>
            </Card>
        </div >
    )
}
