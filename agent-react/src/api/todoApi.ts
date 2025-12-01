export type ApiTodoItem = {
    Id?: string;
    id?: string;
    Text?: string;
    text?: string;
};

export type Todo = { id: string; text: string };

async function handleResponse(res: Response) {
    if (!res.ok) {
        const text = await res.text().catch(() => "");
        throw new Error(`API error ${res.status}: ${text}`);
    }
    // Try parse JSON, if empty return null
    const txt = await res.text();
    if (!txt) return null;
    try {
        return JSON.parse(txt);
    } catch (e) {
        // not JSON
        return txt;
    }
}

export async function getTodos(id: number): Promise<Todo[]> {
    const res = await fetch(`/api/todo/${id}`);
    const data = await handleResponse(res);
    if (!Array.isArray(data)) return [];
    return data.map((d: ApiTodoItem) => ({ id: d.Id ?? d.id ?? "", text: d.Text ?? d.text ?? "" }));
}

export async function getOldest(id: number): Promise<Todo | null> {
    const res = await fetch(`/api/todo/${id}/oldest`);
    const data = await handleResponse(res);
    if (!data) return null;
    const d: ApiTodoItem = data as ApiTodoItem;
    return { id: d.Id ?? d.id ?? "", text: d.Text ?? d.text ?? "" };
}

export async function addTodo(id: number, item: string): Promise<void> {
    const res = await fetch(`/api/todo/${id}?item=${item}`, {
        method: "POST"
    });
    await handleResponse(res);
}

export async function deleteTodo(id: number, itemId?: string): Promise<void> {
    const url = itemId ? `/api/todo/${id}?item=${itemId}` : `/api/todo/${id}`;
    const res = await fetch(url, { method: "DELETE" });
    await handleResponse(res);
}

export async function breakTodo(id: number, message: string): Promise<void> {
    await fetch(`/api/todo/${id}/break`, {
        method: "POST",
        headers: {
            "Content-Type": "text/plain"
        },
        body: message
    });
}

export default {
    getTodos,
    getOldest,
    addTodo,
    deleteTodo,
    breakTodo,
};
