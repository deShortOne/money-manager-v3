'use client'

import { UpdateBillForm } from "./components/update-bill-form";
import SideBar from "./components/side-bar";
import BillsDisplay from "./components/table";

import {
    QueryClient,
    QueryClientProvider,
} from "@tanstack/react-query";

const queryClient = new QueryClient();

export default function Register() {
    return (
        <QueryClientProvider client={queryClient}>
            <UpdateBillForm />
            <div className="flex">
                <SideBar />
                <BillsDisplay />
            </div>
        </QueryClientProvider>
    )
}
