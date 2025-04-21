'use server'

import { Account, NewAccountDto } from "@/interface/account";
import { ErrorResult, SuccessResult, Result } from "@/types/result";

export async function getAllAccounts(authToken: string): Promise<Result<Account[]>> {
    const response = await fetch(process.env.QUERY_SERVER_URL + `/Account/get`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned getting accounts");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}

export async function addNewAccount(authToken: string, newAccount: NewAccountDto): Promise<Result<Account>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/Account/add`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + authToken,
        },
        body: JSON.stringify({
            accountName: newAccount.name,
            doesUserOwnAccount: newAccount.doesUserOwnAccount,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.text())));
    }

    console.log("error returned add new account");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}
