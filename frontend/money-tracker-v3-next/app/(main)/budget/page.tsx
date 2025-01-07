'use client'

import BudgetsDisplay from "./components/table";

import {
    QueryClient,
    QueryClientProvider,
} from "@tanstack/react-query";
import { UpdateBudgetForm } from "./components/update-budget-form";
import SideBar from "./components/side-bar";

const queryClient = new QueryClient();

export default function Register() {
    return (
        <QueryClientProvider client={queryClient}>
            <UpdateBudgetForm />
            <div className="flex">
                <SideBar />
                <BudgetsDisplay />
            </div>
        </QueryClientProvider>
    )
}
