'use client'

import TransactionsDisplay from "./components/table";

import { UpdateTransactionForm } from "./components/update-transaction-form";
import SideBar from "./components/side-bar";

export default function Register() {
    return (
        <>
            <UpdateTransactionForm />
            <div className="flex">
                <SideBar />
                <TransactionsDisplay />
            </div>
        </>
    )
}
