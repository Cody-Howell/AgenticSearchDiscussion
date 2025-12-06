import { ReactNode, useEffect, useState } from "react";
import { StateContext } from "./CurrentContext";
import { TodoItem } from "../types/todos";
import { Message as ChatMessage, Chat } from "../types/chats";
import { useTodoActions } from "../hooks/useTodoActions";
import { useChatMessageActions } from "../hooks/useChatMessageActions";
import { useChatListActions } from "../hooks/useChatListActions";
import { useChatWebSocket } from "../hooks/useChatWebSocket";


export function StateProvider({ children }: { children: ReactNode; }) {
    const [currentId, setCurrentId] = useState<number>(0);
    const [todoItems, setTodoItems] = useState<TodoItem[]>([]);
    const [chatMessages, setChatMessages] = useState<ChatMessage[]>([]);
    const [chats, setChats] = useState<Chat[]>([]);
    const { refreshTodos, addItem, deleteItem } = useTodoActions({ currentId, setTodoItems, setChatMessages });
    const { refreshChat, addChatMessage } = useChatMessageActions({ currentId, setChatMessages });
    const { refreshChats, createChat, updateChatTitle } = useChatListActions({ setCurrentId, setChats });

    useChatWebSocket({ currentId, setTodoItems, setChatMessages });

    useEffect(() => {
        void refreshTodos();
    }, [currentId, refreshTodos]);

    useEffect(() => {
        void refreshChats();
    }, [refreshChats]);

    return (
        <StateContext.Provider
            value={{ currentId, setCurrentId, todoItems, chatMessages, chats, refresh: refreshTodos, addItem, deleteItem, addChatMessage, refreshChat, createChat, refreshChats, updateChatTitle }}
        >
            {children}
        </StateContext.Provider>
    );
}
