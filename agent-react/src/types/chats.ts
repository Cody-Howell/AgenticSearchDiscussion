import { z } from "zod";

export const MessageSchema = z.object({
    id: z.number(),
    chatId: z.number(),
    type: z.string(),
    role: z.string(),
    messageText: z.string()
});

export const ChatSchema = z.object({
    id: z.number(),
    title: z.string()
});

export const ChatWithMessagesSchema = ChatSchema.extend({
    messages: z.array(MessageSchema)
});

export type Message = z.infer<typeof MessageSchema>;
export type Chat = z.infer<typeof ChatSchema>;
export type ChatWithMessages = z.infer<typeof ChatWithMessagesSchema>;
