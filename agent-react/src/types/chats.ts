import { z } from "zod";

export const MessageSchema = z.object({
    Id: z.number(),
    ChatId: z.number(),
    Type: z.string(),
    Role: z.string(),
    MessageText: z.string()
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
