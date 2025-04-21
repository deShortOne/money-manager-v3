import { TableRow, TableCell } from "@/components/ui/table";
import { Account } from "@/interface/account";

interface prop {
    account: Account,
}

export default function AccountTableRow({ account }: prop) {
    const accountNameStle = account.doesUserOwnAccount ? "font-bold" : "";
    return (
        <TableRow>
            <TableCell className="fontmedium">{account.id}</TableCell>
            <TableCell className={accountNameStle}>{account.name}</TableCell>
            <TableCell>
            </TableCell>
        </TableRow>
    )
}
