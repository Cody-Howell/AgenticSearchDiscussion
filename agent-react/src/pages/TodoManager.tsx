import { useState, FormEvent } from "react";
import { useCurrent } from "../contexts/CurrentContext";
import { breakTodo } from "../api/todoApi";

export default function TodoManager() {
  const { currentId, todoItems, addItem } = useCurrent();
  const [newText, setNewText] = useState("");
  const [breakMessage, setBreakMessage] = useState("");
  const [error, setError] = useState<string | null>(null);

  const onAdd = async (e?: FormEvent) => {
    e?.preventDefault();
    if (!newText.trim()) return;
    if (!currentId) {
      setError("Set a valid id before adding items");
      return;
    }
    try {
      await addItem(newText.trim());
      setNewText("");
    } catch (err) {
      console.error(err);
      setError("Failed to add item");
    }
  };

  const onBreak = async (e?: FormEvent) => {
    e?.preventDefault();
    if (!breakMessage.trim()) return;
    if (!currentId) {
      setError("Set a valid id before sending break message");
      return;
    }
    try {
      await breakTodo(currentId, breakMessage.trim());
      setBreakMessage("");
      setError(null);
    } catch (err) {
      console.error(err);
      setError("Failed to send break message");
    }
  };

  return (
    <div className="p-4 border rounded-md max-w-lg">
      {error ? <div className="text-red-600 mb-2">{error}</div> : null}

      <div className="mb-4">
        <ul className="list-disc pl-5 mt-2">
          {todoItems.length === 0 ? (
            <li className="text-sm text-gray-500">No items</li>
          ) : (
            todoItems.map((it, i) => (
              <li key={i} className="wrap-break-word">
                {it.text}
              </li>
            ))
          )}
        </ul>
      </div>

      <form onSubmit={onAdd} className="flex gap-2">
        <input
          className="flex-1 p-2 border rounded"
          value={newText}
          onChange={(e) => setNewText(e.target.value)}
          placeholder="New todo text"
        />
        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded"
        >
          Add
        </button>
      </form>

      <div className="mt-4 pt-4 border-t">
        <h3 className="text-lg font-semibold mb-2">Break Analysis</h3>
        <form onSubmit={onBreak} className="flex flex-col gap-2">
          <textarea
            className="w-full p-2 border rounded"
            value={breakMessage}
            onChange={(e) => setBreakMessage(e.target.value)}
            placeholder="Enter your question or message..."
            rows={3}
          />
          <button
            type="submit"
            className="px-4 py-2 bg-green-600 text-white rounded self-start"
          >
            Send Break
          </button>
        </form>
      </div>
    </div>
  );
}
