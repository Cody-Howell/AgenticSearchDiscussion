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
        }
    };

    return (
        <div className="h-[calc(100vh-8rem)] border rounded-lg bg-white shadow-sm">
            <Chat 
                messages={chatMessages} 
                onSendMessage={handleSendMessage}
            />
        </div>
    );
}
