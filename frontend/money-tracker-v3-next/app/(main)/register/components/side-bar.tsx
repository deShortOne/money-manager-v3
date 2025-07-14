import { AddNewTransaction } from "./add-new-transaction";
import { UploadReceipt } from "./create-transaction-from-receipt";
import { PendingReceiptStates } from "./show-number-of-temporary-receipts";

export default function SideBar() {
    return (
        <ul className="box-border">
            <li>
                <AddNewTransaction />
            </li>
            <li>
                <UploadReceipt />
            </li>
            <li>
                <PendingReceiptStates />
            </li>
        </ul>
    )
}
