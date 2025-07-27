# 🎓 StudentPortal.System

**StudentPortal.System** is a modern web application built with **ASP.NET Core MVC** and **mySQL**, designed to streamline management of educational entities such as Students, Tutors, Subjects, and Grades — with real-time chat support between users.

> 🔐 Secrets like database credentials are stored in a `.env` file and excluded from version control for security.

---

## 🚀 Features

- Full CRUD operations for:
  - ✅ User Profiles
  - ✅ Students
  - ✅ Subjects
  - ✅ Student–Subject relationships (many-to-many)
  - ✅ Tutors
  - ✅ Tutor–Subject relationships (many-to-many)
  - ✅ Tutor–Material relationships (many-to-many)
  - ✅ Real-time Chat between Students and Tutors
- Clean MVC architecture
- MongoDB integration using Entity Framework Core with a MongoDB provider
- Environment-based configuration via `.env`

---

## 🏗️ Tech Stack

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- ASP.NET Core MVC
- MongoDB
- SignalR (for chat functionality)
- [DotNetEnv](https://www.nuget.org/packages/DotNetEnv) for `.env` support

---

## ⚙️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/KhulisoJohn/studentportal.System.git
cd studentportal.System
```
### 2. Setup Environment Variables
Create a .env file in the root directory:

```bash
MONGO_URI=mongodb://localhost:27017
MONGO_DB_NAME=StudentPortalDB
```
### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Run the App

```bash
dotnet run
```
---

## Project Structure

- Controllers/
- Models/
- Views/
- Data/
   └── MongoContext.cs
- Hubs/
    └── ChatHub.cs
- wwwroot/
-Program.cs
- .env
- .gitignore
- StudentPortal.System.csproj

---
## 💬 Real-Time Chat

- Tutors and students can send and receive messages instantly.

- Powered by SignalR and stored in MongoDB for history tracking.

- Secure and scalable for future enhancements like attachments or group chats.

## ☁️ Deployment Notes

This app runs on any platform supporting .NET 8. For production deployment:

- Use a reverse proxy like NGINX or IIS

- Enable HTTPS

- Use a hosted MongoDB instance (e.g., MongoDB Atlas)

- Set up a firewall and environment-specific secrets

 ## 🔒 Security Notes
 
- Never commit .env or secrets to version control.

- Ensure passwords and tokens are encrypted in transit and at rest.

- Regularly audit and update dependencies.

  

## 📌 To Do
 - Authentication & Authorization

-  Unit Testing

-  REST API support

 - Docker containerization (optional)

 - Chat Notifications (push/email)

## 📄 License
This project is open-source. You’re welcome to use, extend, or contribute to it.

## 🤝 Contributing
Pull requests are welcome. For large changes, please open an issue to discuss your idea first.

## 📬 Contact
- Developer: Khulyso John
- Twitter: @khulysojohn
  - Linkedin : LinkedIn Profile


