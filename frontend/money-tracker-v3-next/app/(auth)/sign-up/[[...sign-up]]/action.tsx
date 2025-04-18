'use server'

import { ErrorResult, Result } from "@/types/result";
import { loginUser } from "../../sign-in/[[...sign-in]]/action";

export async function addUserAndLogin(username: string, password: string): Promise<Result<string>> {
    const response = await fetch(process.env.COMMAND_SERVER_URL + `/User/add`, {
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
        return await loginUser(username, password);
    }
    console.log("error returned add user");
    return JSON.parse(JSON.stringify(new ErrorResult("Username already exists", false)));
}
