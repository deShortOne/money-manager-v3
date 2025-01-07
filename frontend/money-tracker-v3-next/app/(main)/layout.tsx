import { NavigationMenuForMain } from "./components/navigation";

export default function MainAreaLayout({
    children,
  }: Readonly<{
    children: React.ReactNode;
  }>) {
    return (
      <>
        <NavigationMenuForMain />
        {children}
      </>
    );
  }
  