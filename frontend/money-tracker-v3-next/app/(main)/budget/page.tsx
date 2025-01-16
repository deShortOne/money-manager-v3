'use client'

import BudgetsDisplay from "./components/table";

import { UpdateBudgetForm } from "./components/update-budget-form";
import SideBar from "./components/side-bar";

export default function Register() {
    return (
        <>
            <UpdateBudgetForm />
            <div className="flex">
                <SideBar />
                <BudgetsDisplay />
            </div>
        </>
    )
}
