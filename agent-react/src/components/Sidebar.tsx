import { useState } from "react";
import { useCurrent } from "../contexts/CurrentContext";
import EditableItem from "./EditableItem";

interface SidebarItem {
  id: number;
  title: string;
}

export default function Sidebar() {
  const { currentId, setCurrentId } = useCurrent();

  // Hardcoded items for now - will be replaced with state
  const [sidebarItems, setSidebarItems] = useState<SidebarItem[]>([
    { id: 1, title: "Project Alpha" },
    { id: 2, title: "Project Beta" },
    { id: 3, title: "Project Gamma" },
    { id: 4, title: "Project Delta" },
    { id: 5, title: "Project Epsilon" },
  ]);

  const handleUpdateItem = (id: number, newTitle: string) => {
    setSidebarItems((prevItems) =>
      prevItems.map((item) =>
        item.id === id ? { ...item, title: newTitle } : item
      )
    );
    // TODO: Call context/API to persist the update
    console.log(`Updated item ${id} to: ${newTitle}`);
  };

  return (
    <aside className="w-64 bg-white shadow-lg h-screen sticky top-0">
      <div className="p-4">
        <h2 className="text-xl font-bold text-gray-800 mb-4">Projects</h2>
        <ul className="space-y-2">
          {sidebarItems.map((item) => (
            <EditableItem
              key={item.id}
              id={item.id}
              title={item.title}
              isActive={currentId === item.id}
              onClick={() => setCurrentId(item.id)}
              onUpdate={handleUpdateItem}
            />
          ))}
        </ul>
      </div>
    </aside>
  );
}
