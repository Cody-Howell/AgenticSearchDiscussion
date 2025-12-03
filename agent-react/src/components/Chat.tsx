import React, { useState, useEffect, FC } from 'react';
import { MarkdownDisplay } from './MarkdownDisplay';
import { Message } from '../types/chats';

interface ChatProps {
    messages: Message[];
    onSendMessage: (text: string) => void;
    className?: string;
}

export const Chat: FC<ChatProps> = ({ messages, onSendMessage, className = '' }) => {
    const [inputText, setInputText] = useState('');
    const [collapsedMessages, setCollapsedMessages] = useState<Set<number>>(new Set());
    const [processedMessageIds, setProcessedMessageIds] = useState<Set<number>>(new Set());

    // Auto-collapse tool calls and tool results when new messages arrive
    useEffect(() => {
        const currentMessageIds = new Set(messages.map(m => m.Id));
        const newMessageIds = messages.filter(m => !processedMessageIds.has(m.Id));
        
        if (newMessageIds.length > 0) {
            setCollapsedMessages((prevCollapsed) => {
                const newCollapsed = new Set(prevCollapsed);
                newMessageIds.forEach((message) => {
                    const messageType = message.Type.toLowerCase();
                    if (messageType.includes('tool') || messageType === 'tool_use' || messageType === 'tool_result') {
                        newCollapsed.add(message.Id);
                    }
                });
                return newCollapsed;
            });
            
            setProcessedMessageIds(currentMessageIds);
        }
    }, [messages, processedMessageIds]);

    const handleDoubleClick = (messageId: number) => {
        setCollapsedMessages((prev) => {
            const newSet = new Set(prev);
            if (newSet.has(messageId)) {
                newSet.delete(messageId);
            } else {
                newSet.add(messageId);
            }
            return newSet;
        });
    };

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (inputText.trim()) {
            onSendMessage(inputText);
            setInputText('');
        }
    };

    console.log(collapsedMessages);

    return (
        <div className={`flex flex-col h-full ${className}`}>
            {/* Scrollable area with centered fixed-width column */}
            <div className="flex-1 overflow-y-auto p-4 flex justify-center">
                <div className="w-full max-w-[800px] space-y-4">
                {messages.length === 0 ? (
                    <div className="text-gray-500 text-center">No messages yet</div>
                ) : (
                    messages.map((message) => (
                        <div 
                            key={message.Id} 
                            className={`p-4 rounded-lg wrap-break-word whitespace-pre-wrap cursor-pointer ${
                                message.Role.toLowerCase() === 'user' 
                                    ? 'bg-blue-50 ml-8' 
                                    : 'bg-gray-50 mr-8'
                            }`}
                            onDoubleClick={() => handleDoubleClick(message.Id)}
                        >
                            <div className="flex gap-2 mb-2 text-xs text-gray-600">
                                <span className="font-semibold">{message.Role}</span>
                                <span>•</span>
                                <span>{message.Type}</span>
                                {collapsedMessages.has(message.Id) && (
                                    <>
                                        <span>•</span>
                                        <span className="italic">collapsed</span>
                                    </>
                                )}
                            </div>
                            {!collapsedMessages.has(message.Id) && (
                                <MarkdownDisplay 
                                    value={message.MessageText} 
                                    className="prose prose-sm max-w-none wrap-break-word"
                                />
                            )}
                        </div>
                    ))
                )}
                </div>
            </div>
            <form onSubmit={handleSubmit} className="border-t p-4 bg-white">
                <div className="flex gap-2">
                    <input
                        type="text"
                        value={inputText}
                        onChange={(e) => setInputText(e.target.value)}
                        placeholder="Type your message..."
                        className="flex-1 p-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                    <button 
                        type="submit" 
                        className="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                        Send
                    </button>
                </div>
            </form>
        </div>
    );
};
