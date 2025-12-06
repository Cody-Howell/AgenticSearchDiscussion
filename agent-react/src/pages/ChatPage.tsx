import { useEffect } from "react";
import { Chat } from "../components/Chat";
import { useCurrent } from "../contexts/CurrentContext";
import { useAppToast } from "../hooks/useAppToast";

export default function ChatPage() {
    const { currentId, chatMessages, addChatMessage, refreshChat } = useCurrent();
    const { showToast } = useAppToast();

    useEffect(() => {
        void refreshChat();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [currentId]);

    const handleSendMessage = async (text: string) => {
        try {
            await addChatMessage(text);
        } catch (err) {
            console.error("Failed to send message:", err);
            const message = err instanceof Error ? err.message : "Failed to send message";
            showToast(message, "error");
            throw err;
        }
    };

    if (!currentId || currentId <= 0) {
        return (
            <div className="h-full border border-emerald-900/70 rounded-2xl bg-emerald-950/50 shadow-lg shadow-emerald-950/40 overflow-hidden flex flex-col items-center justify-center">
                <div className="text-emerald-200/70 text-center">Please select a valid ID to view chat</div>
            </div>
        );
    }

    return (
        <div className="h-full border border-emerald-900/70 rounded-2xl bg-emerald-950/50 shadow-lg shadow-emerald-950/40 overflow-hidden flex flex-col">
            <Chat 
                messages={chatMessages} 
                onSendMessage={handleSendMessage}
            />
        </div>
    );
}
