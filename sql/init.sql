create table chats (
    id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    title varchar(80) NOT NULL DEFAULT 'Untitled'
);

create table messages (
    id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY NOT NULL,
    chatId int references chats (id) NOT NULL,
    chatType varchar(80) NOT NULL, 
    chatRole varchar(80) NOT NULL, 
    MessageText text NOT NULL
);