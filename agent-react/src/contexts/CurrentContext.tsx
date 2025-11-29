import { createContext, useContext } from "react";

export type Message = {
  type: string;
  role: string;
  message: string;
};

export type TodoItem = {
  id: string;
  text: string;
};

type CurrentContextType = {
  currentId: number;
  setCurrentId: (id: number) => void;
  todoItems: TodoItem[];
  items: Message[];
  refresh: () => Promise<void>;
  addItem: (text: string) => Promise<void>;
};

export const StateContext = createContext<CurrentContextType | undefined>(undefined);

export function useCurrent() {
  const ctx = useContext(StateContext);
  if (!ctx) throw new Error("useCurrent must be used within CurrentProvider");
  return ctx;
}

export default StateContext;
