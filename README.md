Alex (or other grader), this template is my own creation that has a React frontend, C# API, and Postgres database all hooked together already; since I know this will be my tech stack, I'm making it faster for myself. Please don't dock points :)

---

## Elevator Pitch
This project has a C# API that abstracts information sources (files, external APIs, etc.) behind a shared interface, which the AI can function call through a Table of Contents for different project providers. 

Along with that information, you can put the AI into a more visible (and interactive) Research box, as you can with ChatGPT, which is intended for more deep thinking processes. Such as, you can assign it a title and give it a short description, and watch it think through items in real time. (this is up for consideration)

## Contributors
Me :D (Cody Howell)

## List of Features
AI can read the following functions: 
- Can see list of defined projects (user request)
- Can see list of approved sources/Table of Contents (as desired)
- Can get a text value from approved source (as desired)
- Can query user for research re-alignment (user request)

It might also be cool to implement an API-level to-do list that the AI can call on. That might be excellent for research tasks.

I'll be utilizing a WebSocket connection to get info from the API. 

## Project Risks
1. For files, I'll need to worry about the path and recursively finding all files (while excluding some folders like `node_modules`) for the table of contents.
2. If you require me to, I'll need to worry about sending info from the WebSockets to the API (so far, I've mainly gone only one direction)

(i know it's not done, I'm tired, I'm sorry)

---

## Project Schedule

### Oct 29

#### Estimates:

Rubric items:
- Network calls

Features:
- Network call to API for AI response passthrough

#### Delivered

Rubric Items:
- Network calls

Features:
- Got a response from the AI

### Nov 1

#### Estimates:

Rubric items:
- CI/CD (to DigitalOcean)
- KeyCloak authentication

Features:
- Make a working Function call in C#

#### Delivered

Rubric Items:
- CI/CD makes a docker image to import in DigitalOcean
- KeyCloak

Features:
- SignIn button and GUID of user is known
- Separate "Client" for KeyCloak

### Nov 5

#### Estimates:

Rubric items:
--

Features:
--

### Nov 8

#### Estimates:

Rubric items:
- Typescript (frontend and backend)
- Pages

Features:
- 

#### Delivered

Rubric Items:
- Types for all AI parts (conversation, function calls)
- One more page (Home)

Features:
- Created classes and a default function to call
- Worked on the project (!)

### Nov 12

#### Estimates:

Rubric items:
- Agent function calls

Features:
- To-do lists are added per global conversation

#### Delivered

Rubric Items:

Features:
- To-do lists are stored in the API (in memory)

### Nov 15

#### Estimates:

Rubric items:
- More views 

Features:
- Todo list features
- File endpoints

#### Delivered

Rubric items:
- More views 

Features:
- Todo list features
- File endpoints started (not quite working)


### Nov 19

#### Estimates:

Rubric items:
- Tests in pipeline
- Linting in pipeline

Features:
- feature 4 with description
- feature 5 with description

#### Delivered

Rubric Items:

Features:


### Nov 22

#### Estimates:

Rubric items:
- item 1 with description
- item 2 with description

Features:
- feature 4 with description
- feature 5 with description

#### Delivered

Rubric Items:

Features:


### Nov 25

#### Estimates:

Rubric items:
- item 1 with description
- item 2 with description

Features:
- feature 4 with description
- feature 5 with description

#### Delivered

Rubric Items:

Features:

### Dec 3

#### Estimates:

Rubric items:
- item 1 with description
- item 2 with description

Features:
- feature 4 with description
- feature 5 with description

#### Delivered

Rubric Items:

Features:

### Dec 6

#### Estimates:

Rubric items:
- item 1 with description
- item 2 with description

Features:
- feature 4 with description
- feature 5 with description

#### Delivered

Rubric Items:

Features:
