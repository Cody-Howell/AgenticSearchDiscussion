import { useState, FormEvent } from "react";
import { useCurrent } from "../contexts/CurrentContext";

export default function TodoManager() {
  const { currentId, todoItems, addItem } = useCurrent();
  const [newText, setNewText] = useState("");
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
    </div>
  );
}
