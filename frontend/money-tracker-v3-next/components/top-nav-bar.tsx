'use client'

import Link from "next/link";
import { NavigationMenu, NavigationMenuItem, NavigationMenuList } from "@/components/ui/navigation-menu";
import { usePathname } from "next/navigation";
import { useCookies } from "react-cookie";

enum WhatToShow {
    "BLANK", "SIGN-IN-UP", "LOGGED-IN"
}

function decideWhatContentToShow(): WhatToShow {
    const pathname = usePathname();
    const pathIsSignInOrSignUpPage = ["/sign-in", "/sign-up"];
    if (pathIsSignInOrSignUpPage.includes(pathname))
        return WhatToShow.BLANK;

    const [token] = useCookies(["token"]);
    if (token.token != "")
        return WhatToShow["LOGGED-IN"];

    return WhatToShow["SIGN-IN-UP"];
}

export default function TopNavBar() {
    let contentToShowIs: WhatToShow = decideWhatContentToShow();

    return (
        <div className="flex justify-between">
            <h1><Link href="/">Money Manager V3</Link></h1>
            {contentToShowIs == WhatToShow["LOGGED-IN"] && (
                <div>

                </div>
            )}

            {contentToShowIs == WhatToShow["SIGN-IN-UP"] && (
                <NavigationMenu>
                    <NavigationMenuList>
                        <NavigationMenuItem>
                            <Link href={`sign-up`}>
                                Sign Up
                            </Link>
                        </NavigationMenuItem>
                        <NavigationMenuItem >
                            <Link href={`sign-in`}>
                                Sign In
                            </Link>
                        </NavigationMenuItem>
                    </NavigationMenuList>
                </NavigationMenu>
            )}
        </div>
    )
}
