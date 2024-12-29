'use client'

import BudgetsDisplay from "./components/table";

import {
    QueryClient,
    QueryClientProvider,
} from "@tanstack/react-query";

const queryClient = new QueryClient();

export default function Register() {
    return (
        <QueryClientProvider client={queryClient}>
            <BudgetsDisplay />
        </QueryClientProvider>
    )
}
