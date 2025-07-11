import { AddNewTransaction } from "./add-new-transaction";
import { UploadReceipt } from "./create-transaction-from-receipt";
import { TemporaryTransactionEditor } from "./get-transaction-from-receipt";
import { PendingReceiptStates } from "./show-number-of-temporary-receipts";

export default function SideBar() {
    return (
        <ul className="box-border">
            <li>
                <AddNewTransaction />
                <UploadReceipt />
                <TemporaryTransactionEditor />
                <PendingReceiptStates />
            </li>
        </ul>
    )
}
