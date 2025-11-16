import { createContext, useContext } from "react";

type CurrentContextType = {
  currentId: number;
  setCurrentId: (id: number) => void;
  items: string[];
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
