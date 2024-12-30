"use client"

import Link from "next/link"

import {
    NavigationMenu,
    NavigationMenuItem,
    NavigationMenuList,
} from "@/components/ui/navigation-menu"

const components: { title: string; href: string; }[] = [
    {
        title: "Register",
        href: "/register",
    },
    {
        title: "Bills",
        href: "/bills",
    },
    {
        title: "Budget",
        href: "/budget",
    },
]

export function NavigationMenuForMain() {
    return (
        <NavigationMenu>
            <NavigationMenuList>
                {components.map((component) => (
                    <NavigationMenuItem key={component.title}>
                        <Link href={component.href}>
                            {component.title}
                        </Link>
                    </NavigationMenuItem>
                ))}
            </NavigationMenuList>
        </NavigationMenu>
    )
}
