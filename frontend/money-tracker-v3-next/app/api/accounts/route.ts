import { NextRequest } from "next/server";

export async function GET(request: NextRequest) {
    const token = request.cookies.get('token');

    const response = await fetch(process.env.QUERY_SERVER_URL + `/Account/get`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + token?.value,
        },
    });
    if (response.ok) {
        return Response.json(JSON.parse(JSON.stringify(await response.json())));
    }

    console.log("error getting accounts");
    return Response.json(JSON.parse(JSON.stringify("Error getting accounts data")), {
        status: 400,
    });
}
