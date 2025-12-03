import { useCurrent } from "../contexts/CurrentContext";
import EditableItem from "./EditableItem";

export default function Sidebar() {
  const { currentId, setCurrentId, chats, updateChatTitle, createChat } = useCurrent();

  const handleUpdateItem = async (id: number, newTitle: string) => {
    await updateChatTitle(id, newTitle);
  };

  const handleNewChat = async () => {
    await createChat();
  };

  return (
    <aside className="w-64 bg-white shadow-lg h-screen sticky top-0">
      <div className="p-4">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-bold text-gray-800">Chats</h2>
          <button
            onClick={handleNewChat}
            className="px-3 py-1 bg-green-500 hover:bg-green-600 text-white rounded-md transition-colors font-semibold text-sm"
            title="Create new chat"
          >
            + New
          </button>
        </div>
        <ul className="space-y-2">
          {chats.length === 0 ? (
            <li className="text-sm text-gray-500">No chats yet</li>
          ) : (
            chats.map((chat) => (
              <EditableItem
                key={chat.id}
                id={chat.id}
                title={chat.title}
                isActive={currentId === chat.id}
                onClick={() => setCurrentId(chat.id)}
                onUpdate={handleUpdateItem}
              />
            ))
          )}
        </ul>
      </div>
    </aside>
  );
}
