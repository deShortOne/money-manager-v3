import { ErrorResult, SuccessResult } from "@/types/result";
import { NextRequest } from "next/server";

export async function POST(request: NextRequest) {
    const token = request.cookies.get('token');
    const data = await request.json();
    const { receiptId } = data;

    const response = await fetch(process.env.QUERY_SERVER_URL + `/Register/get-temporary-transaction?` + new URLSearchParams({
        filename: receiptId,
    }).toString(), {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + token?.value,
        },
    });
    if (response.ok) {
        return Response.json(JSON.parse(JSON.stringify(new SuccessResult(JSON.parse(await response.text())))));
    }

    const text = await response.text()
    return Response.json(JSON.parse(JSON.stringify(new ErrorResult(text, false))));
}
