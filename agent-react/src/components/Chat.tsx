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
            <div className="flex-1 overflow-y-auto p-4 flex justify-center">
                <div className="w-full max-w-[800px] space-y-4">
                {messages.length === 0 ? (
                        <div className="text-emerald-200/70 text-center">No messages yet</div>
                ) : (
                    messages.map((message) => (
                        <div 
                            key={message.Id} 
                                className={`p-4 rounded-xl wrap-break-word whitespace-pre-wrap cursor-pointer border transition-all duration-200 hover:border-emerald-400/60 hover:-translate-y-0.5 shadow-lg shadow-emerald-950/30 ${
                                    message.Role.toLowerCase() === 'user' 
                                        ? 'bg-emerald-900/60 border-emerald-800/70 ml-8 text-emerald-50' 
                                        : 'bg-slate-900/70 border-slate-800 mr-8 text-emerald-50'
                                }`}
                            onDoubleClick={() => handleDoubleClick(message.Id)}
                        >
                                <div className="flex gap-2 mb-2 text-xs text-emerald-200/70">
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
            <form onSubmit={handleSubmit} className="border-t border-emerald-900/70 p-4 bg-slate-900/70 rounded-b-xl">
                <div className="flex gap-2">
                    <input
                        type="text"
                        value={inputText}
                        onChange={(e) => setInputText(e.target.value)}
                        placeholder="Type your message..."
                        className="flex-1 p-3 border border-emerald-800/70 rounded-lg bg-emerald-950/50 text-emerald-50 placeholder:text-emerald-200/50 focus:outline-none focus:ring-2 focus:ring-emerald-500"
                    />
                    <button 
                        type="submit" 
                        className="px-6 py-2 bg-emerald-600 text-emerald-50 rounded-lg hover:bg-emerald-500 transition-all duration-200 cursor-pointer hover:-translate-y-0.5 hover:shadow-lg shadow-emerald-950/50 focus:outline-none focus:ring-2 focus:ring-emerald-400"
                    >
                        Send
                    </button>
                </div>
            </form>
        </div>
    );
};
