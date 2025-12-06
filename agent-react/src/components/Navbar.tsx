import { Link } from "react-router";

interface NavItem {
  title: string;
  path: string;
}

interface NavLinkProps {
  itemString: string;
}

function NavLink({ itemString }: NavLinkProps) {
  const item: NavItem = JSON.parse(itemString);
  
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
  items: string[];
}

function GenericNavbar({ items }: NavbarProps) {
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
            <div className="flex gap-2 text-sm">
              {items.map((itemString) => {
                const item: NavItem = JSON.parse(itemString);
                return <NavLink key={item.path} itemString={itemString} />;
              })}
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
}

export default function Navbar() {
  const navItems: string[] = [
    JSON.stringify({ title: "Home", path: "/" }),
    JSON.stringify({ title: "Auth", path: "/auth" }),
    JSON.stringify({ title: "Files", path: "/file" }),
    JSON.stringify({ title: "Todo", path: "/todo" }),
    JSON.stringify({ title: "Chat", path: "/chat" }),
  ];

  return <GenericNavbar items={navItems} />;
}
