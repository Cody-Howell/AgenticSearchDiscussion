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
    <div className="p-4 border rounded-md max-w-lg">
      {error ? <div className="text-red-600 mb-2">{error}</div> : null}

      <div className="mb-4">
        <ul className="space-y-2 mt-2">
          {todoItems.length === 0 ? (
            <li className="text-sm text-gray-500">No items</li>
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
                  className="text-red-600 hover:text-red-800 hover:bg-red-50 rounded px-2 py-1 text-sm font-semibold opacity-0 group-hover:opacity-100 transition-opacity disabled:opacity-100 disabled:cursor-not-allowed disabled:text-gray-400 disabled:hover:bg-transparent disabled:hover:text-gray-400"
                  aria-label="Delete todo"
                >
                  {loadingDelete === it.Id ? <Spinner /> : "✕"}
                </button>
              </div>
            ))
          )}
        </ul>
        {currentId && todoItems.length > 0 ? (
          <div className="mt-3">
            <button
              onClick={onAnswerAll}
              disabled={loadingAnswerAll}
              className="px-4 py-2 bg-purple-600 text-white rounded disabled:bg-purple-400 disabled:cursor-not-allowed flex items-center gap-2"
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
          className="flex-1 p-2 border rounded"
          value={newText}
          onChange={(e) => setNewText(e.target.value)}
          placeholder="New todo text"
          disabled={loadingAdd}
        />
        <button
          type="submit"
          disabled={loadingAdd}
          className="px-4 py-2 bg-blue-600 text-white rounded disabled:bg-blue-400 disabled:cursor-not-allowed flex items-center gap-2"
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

      <div className="mt-4 pt-4 border-t">
        <h3 className="text-lg font-semibold mb-2">Break A Paragraph</h3>
        <form onSubmit={onBreak} className="flex flex-col gap-2">
          <textarea
            className="w-full p-2 border rounded"
            value={breakMessage}
            onChange={(e) => setBreakMessage(e.target.value)}
            placeholder="Enter your question or message..."
            rows={3}
            disabled={loadingBreak}
          />
          <button
            type="submit"
            disabled={loadingBreak}
            className="px-4 py-2 bg-green-600 text-white rounded self-start disabled:bg-green-400 disabled:cursor-not-allowed flex items-center gap-2"
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
