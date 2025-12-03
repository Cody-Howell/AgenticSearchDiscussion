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
          className="w-full px-4 py-2 rounded-lg border-2 border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
      </li>
    );
  }

  return (
    <li>
      <button
        onClick={onClick}
        onDoubleClick={handleDoubleClick}
        className={`w-full text-left px-4 py-2 rounded-lg transition-colors ${
          isActive
            ? "bg-blue-600 text-white font-semibold"
            : "bg-gray-100 text-gray-700 hover:bg-gray-200"
        }`}
      >
        {title}
      </button>
    </li>
  );
}
