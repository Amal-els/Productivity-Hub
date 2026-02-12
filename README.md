# THE DAG HUB

THE DAG HUB is a smart  ASP.NET Core MVC web application that helps users manage their work , activities and meetings and track their productivity level.

---

 ## Features
- User authentification
- Visualize an interactive dashboard that tracks your activity
- Book activities within a calendar
- Receive reminders and notifications on Email
- Set a pomodoro timer and track you pomodoro achievements
- Create and manage notes
- Add and manage tasks in a to-do-list
- Organize meetings and invite people to participate
- Integrate your email and access your Inbox
- Compose emails using natural speech through an AI Agent assitant
- AI chatbot integration (OpenAI API)
- Clean MVC architecture with service layer separation

---

## Technologies Used

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server / LocalDB
- OpenAI API / fastApi
- Bootstrap (UI)

---

## Requirements

Make sure you have the following installed:

- .NET 8 SDK (or your project’s target version)
- SQL Server or LocalDB
- Node.js (if frontend dependencies are used)
- An OpenAI API key
- Uvicorn fastApi

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/your-username/Productivity-Hub.git
cd Productivity-Hub
````
### 2. Configure OpenAI API Key
``` json
"OpenAI": {
  "ApiKey": "your-api-key"
}
```
### 3. set your evironement variable
```bash
setx OPENAI_API_KEY "your-api-key"
```
### 4. Apply Database migrations
```bash
dotnet ef database update
```
### 5. Restore dependencies
```bash
dotnet restore
```
### 6. Run the application
``` bash
dotnet run
```
## Project structure
Controllers/                 → MVC controllers

Services/
Interfaces/               → Service abstractions
Implementations/          → Business logic
Data/                     
Migrations/               → Migrations of the database
Models/
Dtos/                     → Data transfer objects


Views/                       → Razor views
Calendar/                    → Views for the Calendar
Chat/                        → views for the ChatBot
Meetings/                    → Views for the meeting
Note/                        → Views for the notes
Pomodoro/                    → Views for the Pomodoro
Shared/                      → shared views between all the views
toDoList/                    → Views for the to-do-list
toDoTask/                    → Views for the tasks

Program.cs                   → Application configuration

## ChatBot Architecture
The chatbot follows a service-based architecture:

IChatService → Defines chatbot contract

ChatService → Implements OpenAI API communication

ChatController → Handles HTTP requests

ChatRequest / ChatResponse → DTOs
## Flow
1- User sends message from UI.

2- ChatController receives request.

3- Controller calls _chatService.SendMessageAsync().

4- Service sends request to OpenAI.

5- Response is returned as JSON.

6- UI displays chatbot reply.
## Voice assistant Architecture
The voice assistant feature follows a distributed, event-driven architecture combining the .NET Email Service, Vapi Voice Interface, n8n Automation Workflows, and a Google Sheets Contacts Database.
## Flow
1- The user activates the voice assistant widget embedded in the Email View.

2- The user provides voice instructions (recipient and subject).

3- Vapi processes speech-to-text and sends structured conversation data to n8n.

4- n8n orchestrates the workflow:

5- Retrieves contact details from Google Sheets.

6- Generates email content using AI services.

7- Returns structured email data.

8- The .NET application receives the generated content and automatically populates the email composition form.
## Future Improvements
- Persistant Chat history
- Admin dashboard
- Automated workflows and more AI assistants
- Cloud deployment (Azure / Railway / Render)
# Authors
- Amal Bahri -> GitHub: https://github.com/Amal-els
- Ghaida Ben Salah -> GitHub: https://github.com/ghaidabs
- Dorra Ourabi -> GitHub: https://github.com/dorra-ourabi 