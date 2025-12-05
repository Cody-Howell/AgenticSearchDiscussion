import { useState, FormEvent } from "react";
import EditableItem from "../components/EditableItem";
import Spinner from "../components/Spinner";
import { useCurrent } from "../contexts/CurrentContext";
import { breakTodo, answerAllTodos } from "../api/todoApi";

export default function TodoManager() {
  const { currentId, todoItems, addItem, deleteItem } = useCurrent();
  const [newText, setNewText] = useState("");
  const [breakMessage, setBreakMessage] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loadingAdd, setLoadingAdd] = useState(false);
  const [loadingBreak, setLoadingBreak] = useState(false);
  const [loadingAnswerAll, setLoadingAnswerAll] = useState(false);
  const [loadingDelete, setLoadingDelete] = useState<string | null>(null);

  const onAdd = async (e?: FormEvent) => {
    e?.preventDefault();
    if (!newText.trim()) return;
    if (!currentId) {
      setError("Set a valid id before adding items");
      return;
    }
    setLoadingAdd(true);
    try {
      await addItem(newText.trim());
      setNewText("");
    } catch (err) {
      console.error(err);
      setError("Failed to add item");
    } finally {
      setLoadingAdd(false);
    }
  };

  const onBreak = async (e?: FormEvent) => {
    e?.preventDefault();
    if (!breakMessage.trim()) return;
    if (!currentId) {
      setError("Set a valid id before sending break message");
      return;
    }
    setLoadingBreak(true);
    try {
      await breakTodo(currentId, breakMessage.trim());
      setBreakMessage("");
      setError(null);
    } catch (err) {
      console.error(err);
      setError("Failed to send break message");
    } finally {
      setLoadingBreak(false);
    }
  };

  const onDelete = async (itemId: string) => {
    setLoadingDelete(itemId);
    try {
      await deleteItem(itemId);
      setError(null);
    } catch (err) {
      console.error(err);
      setError("Failed to delete item");
    } finally {
      setLoadingDelete(null);
    }
  };

  const onUpdate = async (id: string, newText: string) => {
    if (!newText.trim() || !currentId) return;
    try {
      await addItem(newText.trim());
      await deleteItem(id);
      setError(null);
    } catch (err) {
      setError("Failed to update item");
    }
  };

  const onAnswerAll = async () => {
    if (!currentId) {
      setError("Set a valid id before answering all items");
      return;
    }
    if (todoItems.length === 0) return;
    setLoadingAnswerAll(true);
    try {
      await answerAllTodos(currentId);
      setError(null);
    } catch (err) {
      console.error(err);
      setError("Failed to answer all items");
    } finally {
      setLoadingAnswerAll(false);
    }
  };

  return (
    <div className="p-6 border border-emerald-900/70 rounded-2xl max-w-xl bg-emerald-950/50 shadow-2xl shadow-emerald-950/60 space-y-4">
      {error ? <div className="text-red-400 mb-2">{error}</div> : null}

      <div className="space-y-3">
        <ul className="space-y-2 mt-2">
          {todoItems.length === 0 ? (
            <li className="text-sm text-emerald-200/70">No items</li>
          ) : (
            todoItems.map((it, i) => (
              <div key={i} className="flex items-center gap-2 group w-full">
                <EditableItem
                  id={i}
                  title={it.Text}
                  isActive={false}
                  onClick={() => {}}
                  onUpdate={(_, newTitle) => onUpdate(it.Id, newTitle)}
                />
                <button
                  onClick={() => onDelete(it.Id)}
                  disabled={loadingDelete === it.Id}
                  className="text-emerald-200/80 hover:text-emerald-50 hover:bg-emerald-800/70 rounded-full px-3 py-1 text-sm font-semibold opacity-0 group-hover:opacity-100 transition-all duration-200 cursor-pointer disabled:opacity-60 disabled:cursor-not-allowed disabled:hover:bg-transparent"
                  aria-label="Delete todo"
                >
                  {loadingDelete === it.Id ? <Spinner /> : "✕"}
                </button>
              </div>
            ))
          )}
        </ul>
        {currentId && todoItems.length > 0 ? (
          <div className="pt-2">
            <button
              onClick={onAnswerAll}
              disabled={loadingAnswerAll}
              className="px-4 py-2 bg-emerald-600 text-emerald-50 rounded-lg disabled:bg-emerald-800/60 disabled:text-emerald-200/60 disabled:cursor-not-allowed flex items-center gap-2 transition-all duration-200 cursor-pointer hover:-translate-y-0.5 hover:shadow-lg hover:shadow-emerald-950/50"
            >
              {loadingAnswerAll ? (
                <>
                  <Spinner /> Answering...
                </>
              ) : (
                "Answer All"
              )}
            </button>
          </div>
        ) : null}
      </div>

      <form onSubmit={onAdd} className="flex gap-2">
        <input
          className="flex-1 p-3 border border-emerald-800/70 rounded-lg bg-emerald-950/60 text-emerald-50 placeholder:text-emerald-200/60 focus:outline-none focus:ring-2 focus:ring-emerald-500"
          value={newText}
          onChange={(e) => setNewText(e.target.value)}
          placeholder="New todo text"
          disabled={loadingAdd}
        />
        <button
          type="submit"
          disabled={loadingAdd}
          className="px-4 py-2 bg-emerald-600 text-emerald-50 rounded-lg disabled:bg-emerald-800/60 disabled:text-emerald-200/60 disabled:cursor-not-allowed flex items-center gap-2 transition-all duration-200 cursor-pointer hover:-translate-y-0.5 hover:shadow-lg hover:shadow-emerald-950/50"
        >
          {loadingAdd ? (
            <>
              <Spinner /> Adding...
            </>
          ) : (
            "Add"
          )}
        </button>
      </form>

      <div className="pt-4 border-t border-emerald-900/70 space-y-2">
        <h3 className="text-lg font-semibold text-emerald-50">Break A Paragraph</h3>
        <form onSubmit={onBreak} className="flex flex-col gap-2">
          <textarea
            className="w-full p-3 border border-emerald-800/70 rounded-lg bg-emerald-950/60 text-emerald-50 placeholder:text-emerald-200/60 focus:outline-none focus:ring-2 focus:ring-emerald-500"
            value={breakMessage}
            onChange={(e) => setBreakMessage(e.target.value)}
            placeholder="Enter your question or message..."
            rows={3}
            disabled={loadingBreak}
          />
          <button
            type="submit"
            disabled={loadingBreak}
            className="px-4 py-2 bg-emerald-600 text-emerald-50 rounded-lg self-start disabled:bg-emerald-800/60 disabled:text-emerald-200/60 disabled:cursor-not-allowed flex items-center gap-2 transition-all duration-200 cursor-pointer hover:-translate-y-0.5 hover:shadow-lg hover:shadow-emerald-950/50"
          >
            {loadingBreak ? (
              <>
                <Spinner /> Sending...
              </>
            ) : (
              "Send Break"
            )}
          </button>
        </form>
      </div>
    </div>
  );
}
