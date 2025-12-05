import { useState, useRef, useEffect } from "react";

interface EditableItemProps {
  id: number;
  title: string;
  isActive: boolean;
  onClick: () => void;
  onUpdate: (id: number, newTitle: string) => void;
}

export default function EditableItem({
  id,
  title,
  isActive,
  onClick,
  onUpdate,
}: EditableItemProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [editValue, setEditValue] = useState(title);
  const inputRef = useRef<HTMLTextAreaElement>(null);

  useEffect(() => {
    if (isEditing && inputRef.current) {
      inputRef.current.focus();
      inputRef.current.select();
    }
  }, [isEditing]);

  const handleDoubleClick = () => {
    setIsEditing(true);
    setEditValue(title);
  };

  const handleBlur = () => {
    if (editValue.trim() && editValue !== title) {
      onUpdate(id, editValue.trim());
    } else {
      setEditValue(title);
    }
    setIsEditing(false);
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
    if (e.key === "Enter") {
      inputRef.current?.blur();
    } else if (e.key === "Escape") {
      setEditValue(title);
      setIsEditing(false);
    }
  };

  if (isEditing) {
    return (
      <li>
        <textarea
          ref={inputRef}
          value={editValue}
          onChange={(e) => setEditValue(e.target.value)}
          onBlur={handleBlur}
          onKeyDown={handleKeyDown}
          className="w-full px-4 py-2 rounded-lg border-2 border-emerald-500 bg-emerald-950/60 text-emerald-50 focus:outline-none focus:ring-2 focus:ring-emerald-400"
        />
      </li>
    );
  }

  return (
    <li>
      <button
        onClick={onClick}
        onDoubleClick={handleDoubleClick}
        className={`w-full text-left px-4 py-2 rounded-lg transition-all duration-200 cursor-pointer ${
          isActive
            ? "bg-emerald-700 text-emerald-50 font-semibold shadow-lg shadow-emerald-900/50"
            : "bg-emerald-900/50 text-emerald-100 hover:bg-emerald-800/70 hover:-translate-y-0.5 hover:shadow-lg hover:shadow-emerald-950/50"
        }`}
      >
        {title}
      </button>
    </li>
  );
}
