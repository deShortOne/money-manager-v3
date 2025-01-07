import { AddNewTransaction } from "./add-new-transaction";

export default function SideBar() {
    return (
        <ul className="box-border">
            <li>
                <AddNewTransaction />
            </li>
        </ul>
    )
}
