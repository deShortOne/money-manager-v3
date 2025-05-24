'use server'

import { WageRequest, WageResponse } from "@/interface/wage";
import { ErrorResult, SuccessResult, Result } from "@/types/result";

export async function calculateWage(request: WageRequest): Promise<Result<WageResponse[]>> {
    const response = await fetch(process.env.QUERY_SERVER_URL + `/Wage/calculate`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });
    if (response.ok) {
        return JSON.parse(JSON.stringify(new SuccessResult(await response.json())));
    }
    console.log("error returned calculating wage");
    return JSON.parse(JSON.stringify(new ErrorResult(await response.text(), false)));
}
