'use client'

import { WageCalculatorForm } from "./components/form";
import SideBar from "./components/side-bar";

export default function WageCalculator() {
    return (
        <>
            <div className="flex">
                <SideBar />
                <WageCalculatorForm />
            </div>
        </>
    )
}
