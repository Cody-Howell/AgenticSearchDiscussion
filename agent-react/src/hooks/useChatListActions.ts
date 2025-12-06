import { Dispatch, SetStateAction, useCallback } from "react";
import { Chat } from "../types/chats";
import { createChat as apiCreateChat, getAllChats, updateChatTitle as apiUpdateChatTitle } from "../api/chatApi";
import { useAppToast } from "./useAppToast";

interface UseChatListActionsParams {
  setChats: Dispatch<SetStateAction<Chat[]>>;
  setCurrentId: (id: number) => void;
}

export function useChatListActions({ setChats, setCurrentId }: UseChatListActionsParams) {
  const { showToast } = useAppToast();

  const refreshChats = useCallback(async () => {
    try {
      const allChats = await getAllChats();
      setChats(allChats);
    } catch (e) {
      console.error("Failed to load chats", e);
      const message = e instanceof Error ? e.message : "Failed to load chats";
      showToast(message, "error");
    }
  }, [setChats, showToast]);

  const createChat = useCallback(async () => {
    try {
      const result = await apiCreateChat();
      await refreshChats();
      setCurrentId(result.id);
    } catch (e) {
      console.error("Failed to create chat", e);
      const message = e instanceof Error ? e.message : "Failed to create chat";
      showToast(message, "error");
    }
  }, [refreshChats, setCurrentId, showToast]);

  const updateChatTitle = useCallback(
    async (chatId: number, title: string) => {
      try {
        await apiUpdateChatTitle(chatId, title);
        await refreshChats();
      } catch (e) {
        console.error("Failed to update chat title", e);
        const message = e instanceof Error ? e.message : "Failed to update chat title";
        showToast(message, "error");
      }
    },
    [refreshChats, showToast]
  );

  return { refreshChats, createChat, updateChatTitle };
}
