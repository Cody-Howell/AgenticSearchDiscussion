import { Message, Chat, MessageSchema } from "../types/chats";

export async function getAllChats(): Promise<Chat[]> {
    const response = await fetch('/api/chats');
    if (!response.ok) throw new Error(`Failed to fetch chats: ${response.statusText}`);
    return response.json();
}

export async function getMessages(chatId: number): Promise<Message[]> {
    const response = await fetch(`/api/chats/${chatId}/messages`);
    if (!response.ok) throw new Error(`Failed to fetch messages: ${response.statusText}`);
    return MessageSchema.array().parse(await response.json());
}

export async function createChat(): Promise<{ id: number }> {
    const response = await fetch('/api/chats/create', {
        method: 'POST'
    });
    if (!response.ok) throw new Error(`Failed to create chat: ${response.statusText}`);
    return response.json();
}

export async function addMessage(chatId: number, message: string): Promise<void> {
    const response = await fetch(`/api/chats/${chatId}/messages`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(message)
    });
    if (!response.ok) throw new Error(`Failed to add message: ${response.statusText}`);
}

export async function deleteMessage(chatId: number, messageId: number): Promise<void> {
    const response = await fetch(`/api/chats/${chatId}/messages/${messageId}`, {
        method: 'DELETE'
    });
    if (!response.ok) throw new Error(`Failed to delete message: ${response.statusText}`);
}

export async function updateChatTitle(chatId: number, title: string): Promise<void> {
    const response = await fetch(`/api/chats/${chatId}/title`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(title)
    });
    if (!response.ok) throw new Error(`Failed to update chat title: ${response.statusText}`);
}
