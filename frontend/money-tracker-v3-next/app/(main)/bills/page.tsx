'use client'

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
            <div className="flex">
                <SideBar />
                <BillsDisplay />
            </div>
        </QueryClientProvider>
    )
}
