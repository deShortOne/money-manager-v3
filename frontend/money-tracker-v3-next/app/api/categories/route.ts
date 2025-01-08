
export async function GET() {
    const response = await fetch(`http://localhost:1235/Category/get`, {
        method: "POST",
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
