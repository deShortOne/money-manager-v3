"use server"

export async function loginUser(username: string, password: string): Promise<string> {
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
        const responseUserToken = await getUserToken(username, password);
        return responseUserToken;
    } else {
        console.log("error returned login user");
        console.log(response);
    }
    return "";
}

async function getUserToken(username: string, password: string) : Promise<string> {
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
        const result = await response.text();
        console.log(response)
        console.log("ABC");
        console.log(result);
        return result;
    } else {
        console.log("error returned user token")
        console.log(response)
    }
    return "";
}
