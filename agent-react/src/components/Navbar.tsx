import { useAuth } from "react-oidc-context";
import { Link } from "react-router";
import GenericHorizontalLayout from "./generics/GenericHorizontalLayout";

interface NavItem {
  title: string;
  path: string;
}

interface NavLinkProps {
  item: NavItem;
}

function NavLink({ item }: NavLinkProps) {
  return (
    <Link 
      to={item.path} 
      className="px-3 py-2 rounded-lg cursor-pointer text-emerald-50 hover:bg-emerald-700/40 hover:text-emerald-50 transition-all duration-200"
    >
      {item.title}
    </Link>
  );
}

interface NavbarProps {
  items: NavItem[];
}

function AppNavbar({ items }: NavbarProps) {
  return (
    <nav className="bg-emerald-950 text-emerald-50 shadow-xl shadow-emerald-950/50 border border-emerald-900/70 rounded-2xl">
      <div className="px-4">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center gap-6">
            <a 
              href="https://www.youtube.com/watch?v=1okD66RmktA"
              className="text-xl font-bold tracking-tight hover:text-emerald-300 transition-colors duration-200 cursor-pointer"
            >
              Green_Needle
            </a>
            <GenericHorizontalLayout 
              items={items} 
              ItemComponent={NavLink}
              getKey={(item) => item.path}
            />
          </div>
        </div>
      </div>
    </nav>
  );
}

export default function Navbar() {
  const auth = useAuth();
  const navItems: NavItem[] = [
    { title: "Home", path: "/" },
    { title: "Auth", path: "/auth" },
    { title: "Files", path: "/file" },
    ...(auth.isAuthenticated ? [
      { title: "Todo", path: "/todo" },
      { title: "Chat", path: "/chat" }
    ] : [])
  ];
  return <AppNavbar items={navItems} />;
}
