import { ReactNode, useEffect, useState } from "react";
import { StateContext } from "./CurrentContext";
import { getTodos, addTodo } from "../api/todoApi";


export function StateProvider({ children }: { children: ReactNode; }) {
    const [currentId, setCurrentId] = useState<number>(0);
    const [items, setItems] = useState<string[]>([]);

    const refresh = async () => {
        if (!currentId) {
            setItems([]);
            return;
        }
        try {
            const todos = await getTodos(currentId);
            console.log(todos)
            setItems(todos.map((t) => t.text));
        } catch (e) {
            // eslint-disable-next-line no-console
            console.error("Failed to load todos", e);
        }
    };

    useEffect(() => {
        void refresh();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [currentId]);

    const addItem = async (text: string) => {
        if (!currentId) throw new Error("Cannot add item: currentId is not set");
        await addTodo(currentId, text);
        await refresh();
    };

    return (
        <StateContext.Provider
            value={{ currentId, setCurrentId, items, refresh, addItem }}
        >
            {children}
        </StateContext.Provider>
    );
}
