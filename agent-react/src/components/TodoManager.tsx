import { useState, FormEvent } from "react";
import { useCurrent } from "../contexts/CurrentContext";

export default function TodoManager() {
  const { currentId, setCurrentId, todoItems, addItem } = useCurrent();
  const [idInput, setIdInput] = useState<string>(
    currentId ? String(currentId) : ""
  );
  const [newText, setNewText] = useState("");
  const [error, setError] = useState<string | null>(null);

  const onIdBlur = () => {
    if (idInput.trim() === "") {
      setCurrentId(0);
      setError(null);
      return;
    }
    const parsed = Number(idInput);
    if (Number.isNaN(parsed) || !Number.isInteger(parsed)) {
      setError("Please enter a valid integer id");
      return;
    }
    setError(null);
    setCurrentId(parsed);
  };

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
      // eslint-disable-next-line no-console
      console.error(err);
      setError("Failed to add item");
    }
  };

  return (
    <div className="p-4 border rounded-md max-w-lg">
      <label className="block mb-2">
        <div className="text-sm font-medium">Current ID</div>
        <input
          className="mt-1 p-2 border rounded w-full"
          value={idInput}
          onChange={(e) => setIdInput(e.target.value)}
          onBlur={onIdBlur}
          placeholder="Enter numeric id and tab out"
        />
      </label>
      {error ? <div className="text-red-600 mb-2">{error}</div> : null}

      <div className="mb-4">
        <div className="text-sm font-medium">Todos for ID: {currentId || "(none)"}</div>
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
