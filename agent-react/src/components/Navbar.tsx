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
      className="px-3 py-2 rounded-md hover:bg-blue-700 transition-colors"
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
    <nav className="bg-blue-600 text-white shadow-lg">
      <div className="max-w-7xl mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center space-x-8">
            <Link 
              to="/" 
              className="text-xl font-bold hover:text-blue-200 transition-colors"
            >
              Green_Needle
            </Link>
            <div className="flex space-x-4">
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
    JSON.stringify({ title: "Todo", path: "/todo" }),
    JSON.stringify({ title: "Files", path: "/file" }),
    JSON.stringify({ title: "Chat", path: "/chat" }),
  ];

  return <GenericNavbar items={navItems} />;
}
