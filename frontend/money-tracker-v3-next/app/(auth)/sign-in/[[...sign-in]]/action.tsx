'use server'

import { ErrorResult, SuccessResult, Result } from "@/types/result";

export async function loginUser(username: string, password: string): Promise<Result<string>> {
    const response = await fetch(`http://localhost:1234/User/login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({ 
            username: username,
            password: password,
        }),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(await getUserToken(username, password)));
    }
    console.log("error returned login user");
    return JSON.parse(JSON.stringify(new ErrorResult("Username and password not found", false)));
}

async function getUserToken(username: string, password: string) : Promise<Result<string>> {
    const response = await fetch(`http://localhost:1235/User/get-token`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({ 
            username: username,
            password: password,
        }),
    });
    if (response.ok) {
        return new SuccessResult(await response.text());
    }
    console.log("error returned cannot find user token");
    return new ErrorResult("Username and password not found", false);
}
