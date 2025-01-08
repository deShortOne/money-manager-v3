import {
    HoverCard,
    HoverCardContent,
    HoverCardTrigger
} from "@/components/ui/hover-card";
import { OverdueBillInfo } from "@/interface/bill";
import { CircleAlert } from "lucide-react";

export default function OverflowBill({ overdueBillInfo }: { overdueBillInfo: OverdueBillInfo }) {
    if (overdueBillInfo.daysOverDue == 0)
        return (<></>);

    return (
        <HoverCard>
            <HoverCardTrigger asChild>
                <CircleAlert className="text-red-700 pl-1" />
            </HoverCardTrigger>
            <HoverCardContent className="w-80">
                You are {overdueBillInfo.daysOverDue} days overdue, on these days
                {
                    overdueBillInfo.pastOccurences.length <= 5 &&
                    <ul>
                        {overdueBillInfo.pastOccurences.map(previousDueDate => (
                            <li key={previousDueDate}>
                                {previousDueDate}
                            </li>
                        ))}
                    </ul>
                }
                {
                    overdueBillInfo.pastOccurences.length > 5 &&
                    <ul>
                        {overdueBillInfo.pastOccurences.slice(0, 4).map(previousDueDate => (
                            <li key={previousDueDate}>
                                {previousDueDate}
                            </li>
                        ))}
                        <li>...and {overdueBillInfo.pastOccurences.length - 4} more</li>
                    </ul>
                }
            </HoverCardContent>
        </HoverCard>

    );
}
