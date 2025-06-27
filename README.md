# 🎓 StudentPortal

StudentPortalSystem is a web application built using **ASP.NET Core MVC** and **Entity Framework Core**, designed to manage educational entities such as students, Tutors, Subjects, and Grades.

> 🔐 Secure-by-default: Secrets like database passwords are stored in a `.env` file and excluded from source control.

---

## 🚀 Features

- Full CRUD operations for:
  - ✅ User Profiles
  - ✅ Students
  - ✅ Subjects
  - ✅ StudentSubjects relationships (many-to-many)
  - ✅ Tutors
  - ✅ TutorSubjects relationship (many-to-many)
  - ✅ TutorMaterial relationships (many-to-many)
  - ✅ 
  - ✅ 
- Clean MVC architecture
- mySQL Server integration with Entity Framework
- Environment-based configuration using `.env` file

---

## 🏗️ Technologies Used

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- ASP.NET Core MVC
- Entity Framework Core
- mySQL Server
- [DotNetEnv](https://www.nuget.org/packages/DotNetEnv) for `.env` support

---

## ⚙️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/KhulisoJohn/studentportal.git
cd studentportal
```
### 2. Setup Environment Variables
Create a `.env` file in the root directory:

```bash
DB_SERVER=localhost
DB_NAME=StudentPortalDB
DB_USER=your_sql_user
DB_PASSWORD=your_secure_password
```
### 3. Install Dependencies

```bash
dotnet restore
```
### 4. Apply Migrations & Update the Database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
### 5. Run the App

```bash
dotnet run
```
## 📝 Project Structure
```
├── Controllers/
├── Models/
├── Views/
├── Data/
│   └── StudentPortalDbContext.cs
├── wwwroot/
├── Program.cs
├── .env
├── .gitignore
└── StudentPortal.csproj
```

## ☁️ Hosting & Deployment

This project is designed to run on a Windows server. If your public IP changes often, consider:

- Setting up Dynamic DNS with tools like DuckDNS or No-IP

- Hosting with IIS or using Kestrel + reverse proxy (like nginx or Apache)

- Use HTTPS and secure firewall rules

##   🔒 Security Notes
Secrets like DB credentials must never be committed to GitHub.

The `.env` file is already ignored via `.gitignore.`

## ✅ To Do

 Authentication & Authorization
- Unit Testing
- API support (future-proof)
- Docker containerization (optional)

## 📄 License
This project is open-source. Feel free to use, modify, or contribute.

## 🤝 Contributing
Pull requests are welcome. Open an issue first for major changes.

## 📬 Contact
Developer: Khulyso John
Twitter: @khulysojohn
LinkedIn: LinkedIn Profile










