
export async function GET() {
    const response = await fetch(process.env.QUERY_SERVER_URL + `/Bill/get-all-frequency-names`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
    });
    if (response.ok) {
        return Response.json(JSON.parse(JSON.stringify(await response.json())));
    }

    console.log("error getting frequencies");
    return Response.json(JSON.parse(JSON.stringify("Error getting frequency data")), {
        status: 400,
    });
}
