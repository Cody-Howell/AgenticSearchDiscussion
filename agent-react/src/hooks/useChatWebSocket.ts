import { Dispatch, SetStateAction, useEffect, useRef } from "react";
import { TodoItem, TodoItemSchema } from "../types/todos";
import { Message as ChatMessage, MessageSchema } from "../types/chats";
import { useAppToast } from "./useAppToast";

interface UseChatWebSocketParams {
  currentId: number;
  setTodoItems: Dispatch<SetStateAction<TodoItem[]>>;
  setChatMessages: Dispatch<SetStateAction<ChatMessage[]>>;
}

export function useChatWebSocket({ currentId, setTodoItems, setChatMessages }: UseChatWebSocketParams) {
  const { showToast } = useAppToast();
  const wsRef = useRef<WebSocket | null>(null);

  useEffect(() => {
    if (!currentId) {
      if (wsRef.current) {
        wsRef.current.close();
        wsRef.current = null;
      }
      return;
    }

    const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
    const wsUrl = `${protocol}//${window.location.host}/api/ws/${currentId}`;
    const ws = new WebSocket(wsUrl);

    ws.onopen = () => {
      console.log(`WebSocket connected for ID: ${currentId}`);
    };

    ws.onmessage = (event) => {
      console.log("WebSocket message received:", event.data);
      try {
        const data = JSON.parse(event.data);
        if (data.type === "todo_added") {
          setTodoItems((items) => [...items, TodoItemSchema.parse(data.content)]);
        } else if (data.type === "chat_message") {
          const message = MessageSchema.parse(data.content);
          setChatMessages((messages) => [...messages, message]);
        }
      } catch (e) {
        console.error("Failed to parse WebSocket message:", e);
        const message = e instanceof Error ? e.message : "Failed to parse WebSocket message";
        showToast(message, "error");
      }
    };

    ws.onerror = (error) => {
      console.error("WebSocket error:", error);
    };

    ws.onclose = () => {
      console.log(`WebSocket closed for ID: ${currentId}`);
    };

    wsRef.current = ws;

    return () => {
      if (ws.readyState === WebSocket.OPEN || ws.readyState === WebSocket.CONNECTING) {
        ws.close();
      }
    };
  }, [currentId, setChatMessages, setTodoItems, showToast]);
}
