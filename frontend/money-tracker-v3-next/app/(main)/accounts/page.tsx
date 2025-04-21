'use client'

import { UpdateAccountsForm } from "./components/update-account-form";
import SideBar from "./components/side-bar";
import AccountsDisplay from "./components/table";
import { useCookies } from 'react-cookie';

export default function Register() {
    const [cookies] = useCookies(['token']);
    if (cookies == null) {

    }
    return (
        <>
            <UpdateAccountsForm />
            <div className="flex">
                <SideBar />
                <AccountsDisplay />
            </div>
        </>
    )
}
