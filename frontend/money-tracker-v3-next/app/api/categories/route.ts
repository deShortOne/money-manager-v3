
export async function GET() {
    const response = await fetch(process.env.QUERY_SERVER_URL + `/Category/get`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });
    if (response.ok) {
        return Response.json(JSON.parse(JSON.stringify(await response.json())));
    }

    console.log("error getting categories");
    return Response.json(JSON.parse(JSON.stringify("Error getting category data")), {
        status: 400,
    });
}
