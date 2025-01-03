import { TableRow, TableCell } from "@/components/ui/table";
import OverflowBill from "./overflow-bill";

interface prop {
    transaction: Bill,
}

export default function BillTableRow({ transaction }: prop) {
    return (
        <TableRow>
            <TableCell className="font-medium">{transaction.id}</TableCell>
            <TableCell>{transaction.payee}</TableCell>
            <TableCell className="text-right">{transaction.amount}</TableCell>
            <TableCell className="flex">
                {transaction.nextDueDate}
                {transaction.overDueBill &&
                    <OverflowBill
                        overdueBillInfo={transaction.overDueBill}
                    />
                }
            </TableCell>
            <TableCell>{transaction.frequency}</TableCell>
        </TableRow>
    )
}
