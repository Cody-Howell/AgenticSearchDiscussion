import { Dispatch, SetStateAction, useCallback } from "react";
import { addTodo, deleteTodo, getTodos } from "../api/todoApi";
import { useAppToast } from "./useAppToast";
import { TodoItem } from "../types/todos";
import { Message as ChatMessage } from "../types/chats";

interface UseTodoActionsParams {
  currentId: number;
  setTodoItems: Dispatch<SetStateAction<TodoItem[]>>;
  setChatMessages: Dispatch<SetStateAction<ChatMessage[]>>;
}

export function useTodoActions({ currentId, setTodoItems, setChatMessages }: UseTodoActionsParams) {
  const { showToast } = useAppToast();

  const refreshTodos = useCallback(async () => {
    if (!currentId) {
      setTodoItems([]);
      setChatMessages([]);
      return;
    }

    try {
      const todos = await getTodos(currentId);
      setTodoItems(todos);
    } catch (e) {
      console.error("Failed to load todos", e);
      const message = e instanceof Error ? e.message : "Failed to load todos";
      showToast(message, "error");
    }
  }, [currentId, setChatMessages, setTodoItems, showToast]);

  const addItem = useCallback(
    async (text: string) => {
      if (!currentId) throw new Error("Cannot add item: currentId is not set");
      await addTodo(currentId, text);
      await refreshTodos();
    },
    [currentId, refreshTodos]
  );

  const deleteItem = useCallback(
    async (itemId: string) => {
      if (!currentId) throw new Error("Cannot delete item: currentId is not set");
      await deleteTodo(currentId, itemId);
      await refreshTodos();
    },
    [currentId, refreshTodos]
  );

  return { refreshTodos, addItem, deleteItem };
}
