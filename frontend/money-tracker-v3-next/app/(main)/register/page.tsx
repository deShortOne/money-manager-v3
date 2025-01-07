'use client'

import TransactionsDisplay from "./components/table";

import {
    QueryClient,
    QueryClientProvider,
} from "@tanstack/react-query";
import { UpdateTransactionForm } from "./components/update-transaction-form";
import SideBar from "./components/side-bar";

const queryClient = new QueryClient();

export default function Register() {
    return (
        <QueryClientProvider client={queryClient}>
            <UpdateTransactionForm />
            <div className="flex">
                <SideBar />
                <TransactionsDisplay />
            </div>
        </QueryClientProvider>
    )
}
