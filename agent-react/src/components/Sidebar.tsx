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
    <aside className="w-64 h-full bg-emerald-950/60 border border-emerald-900/70 shadow-lg shadow-emerald-950/40 rounded-2xl backdrop-blur-sm flex-shrink-0 overflow-hidden">
      <div className="p-4 space-y-4 h-full flex flex-col">
        <div className="flex items-center justify-between flex-none">
          <h2 className="text-xl font-bold text-emerald-100 tracking-tight">Chats</h2>
          <button
            onClick={handleNewChat}
            className="px-3 py-1.5 bg-emerald-600 hover:bg-emerald-500 text-emerald-50 rounded-lg transition-all duration-200 font-semibold text-sm cursor-pointer hover:-translate-y-0.5 hover:shadow-lg shadow-emerald-900/50"
            title="Create new chat"
          >
            + New
          </button>
        </div>
        <ul className="space-y-2 overflow-y-auto pr-1 flex-1">
          {chats.length === 0 ? (
            <li className="text-sm text-emerald-200/70">No chats yet</li>
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
