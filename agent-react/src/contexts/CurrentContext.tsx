import { createContext, useContext } from "react";
import { TodoItem } from "../types/todos";
import { Message as ChatMessage, Chat } from "../types/chats";

export type Message = {
  type: string;
  role: string;
  message: string;
};

type CurrentContextType = {
  currentId: number;
  setCurrentId: (id: number) => void;
  todoItems: TodoItem[];
  chatMessages: ChatMessage[];
  chats: Chat[];
  refresh: () => Promise<void>;
  addItem: (text: string) => Promise<void>;
  deleteItem: (itemId: string) => Promise<void>;
  addChatMessage: (text: string) => Promise<void>;
  refreshChat: () => Promise<void>;
  createChat: () => Promise<void>;
  refreshChats: () => Promise<void>;
  updateChatTitle: (chatId: number, title: string) => Promise<void>;
};


export const StateContext = createContext<CurrentContextType | undefined>(undefined);

export function useCurrent() {
  const ctx = useContext(StateContext);
  if (!ctx) throw new Error("useCurrent must be used within CurrentProvider");
  return ctx;
}

export default StateContext;
