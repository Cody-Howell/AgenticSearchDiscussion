import { Dispatch, SetStateAction } from "react";
import { getTodos } from "../api/todoApi";
import { TodoItem } from "../types/todos";
import { Message as ChatMessage, Chat } from "../types/chats";
import { getMessages, getAllChats } from "../api/chatApi";

export async function refresh(
  currentId: number,
  setTodoItems: Dispatch<SetStateAction<TodoItem[]>>,
  setChatMessages: Dispatch<SetStateAction<ChatMessage[]>>
): Promise<void> {
  if (!currentId) {
    setTodoItems([]);
    setChatMessages([]);
    return;
  }
  try {
    const todos = await getTodos(currentId);
    console.log(todos);
    setTodoItems(todos);
  } catch (e) {
    console.error("Failed to load todos", e);
  }
}

export async function refreshChat(
  currentId: number,
  setChatMessages: Dispatch<SetStateAction<ChatMessage[]>>
): Promise<void> {
  if (!currentId) {
    setChatMessages([]);
    return;
  }
  try {
    const messages = await getMessages(currentId);
    setChatMessages(messages);
  } catch (e) {
    console.error("Failed to load chat messages", e);
  }
}

export async function refreshChats(
  setChats: Dispatch<SetStateAction<Chat[]>>
): Promise<void> {
  try {
    const allChats = await getAllChats();
    setChats(allChats);
  } catch (e) {
    console.error("Failed to load chats", e);
  }
}
