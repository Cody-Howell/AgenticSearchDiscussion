import { useEffect } from "react";
import { Chat } from "../components/Chat";
import { useCurrent } from "../contexts/CurrentContext";

export default function ChatPage() {
    const { currentId, chatMessages, addChatMessage, refreshChat } = useCurrent();

    useEffect(() => {
        void refreshChat();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [currentId]);

    const handleSendMessage = async (text: string) => {
        try {
            await addChatMessage(text);
        } catch (err) {
            console.error("Failed to send message:", err);
            throw err;
        }
    };

    return (
        <div className="h-full border border-emerald-900/70 rounded-2xl bg-emerald-950/40 shadow-2xl shadow-emerald-950/50 overflow-hidden flex flex-col">
            <Chat 
                messages={chatMessages} 
                onSendMessage={handleSendMessage}
            />
        </div>
    );
}
