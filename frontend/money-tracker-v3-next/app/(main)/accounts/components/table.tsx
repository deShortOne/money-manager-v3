import {
    Table,
    TableBody,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import { useEffect, useState } from "react";
import { getAllAccounts } from "./action";
import { useCookies } from "react-cookie";
import { useQuery } from "@tanstack/react-query";
import { Result } from "@/types/result";
import AccountTableRow from "./account-table-row";
import { queryKeyAccounts } from "@/app/data/queryKeys";
import { Account } from "@/interface/account";

export default function AccountsDisplay() {
    const [cookies] = useCookies(['token']);

    const [transactions, setTransactions] = useState<Account[]>([]);

    const { data } = useQuery<Result<Account[]>>({
        queryKey: [queryKeyAccounts],
        queryFn: () => getAllAccounts(cookies.token),
    });

    useEffect(() => {
        if (data == null || data.hasError || data.item == undefined) {

        } else {
            setTransactions(data.item);
        }
    }, [data]);

    return (
        <Table>
            <TableHeader>
                <TableRow>
                    <TableHead className="w-[100px]">Id</TableHead>
                    <TableHead>Account name</TableHead>
                    <TableHead className="text-right">Amount</TableHead>
                    <TableHead></TableHead>{/* row actions */}
                </TableRow>
            </TableHeader>
            <TableBody>
                {transactions.map((transaction) => (
                    <AccountTableRow account={transaction} key={transaction.id} />
                ))}
            </TableBody>
        </Table>
    )
}
