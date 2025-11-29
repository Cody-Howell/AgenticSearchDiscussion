import { ReactNode, useEffect, useState } from "react";
import { StateContext, Message, TodoItem } from "./CurrentContext";
import { getTodos, addTodo } from "../api/todoApi";


export function StateProvider({ children }: { children: ReactNode; }) {
    const [currentId, setCurrentId] = useState<number>(0);
    const [todoItems, setTodoItems] = useState<TodoItem[]>([]);
    const [items, setItems] = useState<Message[]>([]);

    const refresh = async () => {
        if (!currentId) {
            setTodoItems([]);
            setItems([]);
            return;
        }
        try {
            const todos = await getTodos(currentId);
            console.log(todos);
            setTodoItems(todos);
            setItems(todos.map((t) => ({
                type: "todo",
                role: "user",
                message: t.text
            })));
        } catch (e) {
             
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
            value={{ currentId, setCurrentId, todoItems, items, refresh, addItem }}
        >
            {children}
        </StateContext.Provider>
    );
}
