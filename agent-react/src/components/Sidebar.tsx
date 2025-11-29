import { useCurrent } from "../contexts/CurrentContext";

interface SidebarItem {
  id: number;
  title: string;
}

export default function Sidebar() {
  const { currentId, setCurrentId } = useCurrent();

  // Hardcoded items for now
  const sidebarItems: SidebarItem[] = [
    { id: 1, title: "Project Alpha" },
    { id: 2, title: "Project Beta" },
    { id: 3, title: "Project Gamma" },
    { id: 4, title: "Project Delta" },
    { id: 5, title: "Project Epsilon" },
  ];

  return (
    <aside className="w-64 bg-white shadow-lg h-screen sticky top-0">
      <div className="p-4">
        <h2 className="text-xl font-bold text-gray-800 mb-4">Projects</h2>
        <ul className="space-y-2">
          {sidebarItems.map((item) => (
            <li key={item.id}>
              <button
                onClick={() => setCurrentId(item.id)}
                className={`w-full text-left px-4 py-2 rounded-lg transition-colors ${
                  currentId === item.id
                    ? "bg-blue-600 text-white font-semibold"
                    : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
              >
                {item.title}
              </button>
            </li>
          ))}
        </ul>
      </div>
    </aside>
  );
}
