'use client'

import TransactionsDisplay from "./components/table";

import { UpdateTransactionForm } from "./components/update-transaction-form";
import SideBar from "./components/side-bar";
import { UploadReceiptForm } from "./components/upload-receipt-form";
import { SelectTemporaryTransactionForm } from "./components/select-temporary-transaction-form";

export default function Register() {
    return (
        <>
            <UploadReceiptForm />
            <UpdateTransactionForm />
            <SelectTemporaryTransactionForm />
            <div className="flex">
                <SideBar />
                <TransactionsDisplay />
            </div>
        </>
    )
}
