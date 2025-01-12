import { NextRequest } from "next/server";

export async function POST(request: NextRequest) {
    const token = request.cookies.get('token');

    const response = await fetch(`http://localhost:1235/User/is-token-valid`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            token: token?.value,
        }),
    });
    if (response.ok) {
        return Response.json(JSON.parse(JSON.stringify(true)));
    }

    return Response.json(JSON.parse(JSON.stringify(false)), {
        status: 400,
    });
}
