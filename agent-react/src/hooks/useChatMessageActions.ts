import { Dispatch, SetStateAction, useCallback } from "react";
import { addMessage, getMessages } from "../api/chatApi";
import { Message as ChatMessage } from "../types/chats";
import { useAppToast } from "./useAppToast";

interface UseChatMessageActionsParams {
  currentId: number;
  setChatMessages: Dispatch<SetStateAction<ChatMessage[]>>;
}

export function useChatMessageActions({ currentId, setChatMessages }: UseChatMessageActionsParams) {
  const { showToast } = useAppToast();

  const refreshChat = useCallback(async () => {
    if (!currentId) {
      setChatMessages([]);
      return;
    }

    try {
      const messages = await getMessages(currentId);
      setChatMessages(messages);
    } catch (e) {
      console.error("Failed to load chat messages", e);
      const message = e instanceof Error ? e.message : "Failed to load chat messages";
      showToast(message, "error");
    }
  }, [currentId, setChatMessages, showToast]);

  const addChatMessage = useCallback(
    async (text: string) => {
      if (!currentId) throw new Error("Cannot add message: currentId is not set");
      await addMessage(currentId, text);
    },
    [currentId]
  );

  return { refreshChat, addChatMessage };
}
