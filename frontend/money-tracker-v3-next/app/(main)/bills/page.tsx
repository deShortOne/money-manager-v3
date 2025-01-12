'use client'

import { UpdateBillForm } from "./components/update-bill-form";
import SideBar from "./components/side-bar";
import BillsDisplay from "./components/table";
import { useCookies } from 'react-cookie';

export default function Register() {
    const [cookies] = useCookies(['token']);
    if (cookies == null) {

    }
    return (
        <>
            <UpdateBillForm />
            <div className="flex">
                <SideBar />
                <BillsDisplay />
            </div>
        </>
    )
}
