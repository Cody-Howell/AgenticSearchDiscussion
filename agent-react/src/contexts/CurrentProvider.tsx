import { ReactNode, useEffect, useState, useRef } from "react";
import { StateContext, Message } from "./CurrentContext";
import { getTodos, addTodo, deleteTodo } from "../api/todoApi";
import { TodoItem, TodoItemSchema } from "../types/todos";


export function StateProvider({ children }: { children: ReactNode; }) {
    const [currentId, setCurrentId] = useState<number>(0);
    const [todoItems, setTodoItems] = useState<TodoItem[]>([]);
    const [items, setItems] = useState<Message[]>([]);
    const wsRef = useRef<WebSocket | null>(null);

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
                message: t.Text
            })));
        } catch (e) {
             
            console.error("Failed to load todos", e);
        }
    };

    // WebSocket connection management
    useEffect(() => {
        if (!currentId) {
            // Close existing WebSocket if currentId is cleared
            if (wsRef.current) {
                wsRef.current.close();
                wsRef.current = null;
            }
            return;
        }

        // Determine WebSocket URL (ws or wss based on protocol)
        const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
        const wsUrl = `${protocol}//${window.location.host}/api/ws/${currentId}`;

        // Create WebSocket connection
        const ws = new WebSocket(wsUrl);

        ws.onopen = () => {
            console.log(`WebSocket connected for ID: ${currentId}`);
        };

        ws.onmessage = (event) => {
            console.log('WebSocket message received:', event.data);
            try {
                const data = JSON.parse(event.data);
                if (data.type === 'todo_added') {
                    setTodoItems(items => 
                    [...items, TodoItemSchema.parse(data.content)]
                    )
                }
                // Add more message handlers as needed
            } catch (e) {
                console.error('Failed to parse WebSocket message:', e);
            }
        };

        ws.onerror = (error) => {
            console.error('WebSocket error:', error);
        };

        ws.onclose = () => {
            console.log(`WebSocket closed for ID: ${currentId}`);
        };

        wsRef.current = ws;

        // Cleanup on unmount or currentId change
        return () => {
            if (ws.readyState === WebSocket.OPEN || ws.readyState === WebSocket.CONNECTING) {
                ws.close();
            }
        };
    }, [currentId]);

    useEffect(() => {
        void refresh();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [currentId]);

    const addItem = async (text: string) => {
        if (!currentId) throw new Error("Cannot add item: currentId is not set");
        await addTodo(currentId, text);
        await refresh();
    };

    const deleteItem = async (itemId: string) => {
        if (!currentId) throw new Error("Cannot delete item: currentId is not set");
        await deleteTodo(currentId, itemId);
        await refresh();
    };

    return (
        <StateContext.Provider
            value={{ currentId, setCurrentId, todoItems, items, refresh, addItem, deleteItem }}
        >
            {children}
        </StateContext.Provider>
    );
}
