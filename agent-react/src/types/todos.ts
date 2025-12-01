import {z} from "zod";


export const TodoItemSchema = z.object({
    Id: z.string(), 
    Text: z.string()
});

export type TodoItem = z.infer<typeof TodoItemSchema>;