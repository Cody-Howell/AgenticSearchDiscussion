import React, { useState } from 'react';
import { MarkdownDisplay } from './MarkdownDisplay';
import { Message } from '../types/chats';

interface ChatProps {
    messages: Message[];
    onSendMessage: (text: string) => void;
    className?: string;
}

export const Chat: React.FC<ChatProps> = ({ messages, onSendMessage, className = '' }) => {
    const [inputText, setInputText] = useState('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (inputText.trim()) {
            onSendMessage(inputText);
            setInputText('');
        }
    };

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
                            className={`p-4 rounded-lg break-words whitespace-pre-wrap ${
                                message.Role.toLowerCase() === 'user' 
                                    ? 'bg-blue-50 ml-8' 
                                    : 'bg-gray-50 mr-8'
                            }`}
                        >
                            <div className="flex gap-2 mb-2 text-xs text-gray-600">
                                <span className="font-semibold">{message.Role}</span>
                                <span>•</span>
                                <span>{message.Type}</span>
                            </div>
                            <MarkdownDisplay 
                                value={message.MessageText} 
                                className="prose prose-sm max-w-none break-words"
                            />
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
