# 🌱 FarmersHub

A modern, secure, and user-friendly platform for managing farmers, employees, and products, built with ASP.NET Core, Entity Framework, and Identity.

---

## 🚨 Emergency Test Accounts

- 👑 **Admin:**  
  - Username: `admin@farmhub.com`  
  - Password: `Admin123!`
- 🧑‍💼 **Employee:**  
  - Username: `employee@mail.com`  
  - Password: `Emp12345!`
- 🚜 **Farmer:**  
  - Username: `mack@farmerhub`  
  - Password: `Farmer123!`

---

## 📑 Table of Contents

- [🌱 FarmersHub](#-farmershub)
  - [🚨 Emergency Test Accounts](#-emergency-test-accounts)
  - [📑 Table of Contents](#-table-of-contents)
  - [🌟 Project Vision](#-project-vision)
  - [✨ Features](#-features)
  - [⚡️ Setup \& Prerequisites](#️-setup--prerequisites)
    - [🗄️ Database Setup](#️-database-setup)
  - [🏗️ Project Structure \& Roles](#️-project-structure--roles)
    - [🔄 Key Flows](#-key-flows)
  - [🚀 How to Use FarmersHub](#-how-to-use-farmershub)
  - [🛠️ Tech Stack](#️-tech-stack)
  - [⚠️ Limitations & Future Improvements](#️-limitations--future-improvements)
  - [🔗 References](#-references)

---

## 🌟 Project Vision

FarmersHub is designed to streamline the management of agricultural products and users (farmers, employees, admins) with a focus on security, usability, and scalability. The app leverages ASP.NET Core Identity for robust authentication and role management, and features a clean, modern UI using the Bootswatch Minty theme.

---

## ✨ Features

- 🔐 **Role-Based Access Control:**  
  - 👑 Admin, 🧑‍💼 Employee, and 🚜 Farmer roles with tailored permissions.
- 🖥️ **Modern Admin Dashboard:**  
  - Admins can view, edit, and delete all farmers, employees, and products.
  - Quick navigation to all management areas.
- 👨‍🌾 **Farmer Management:**  
  - Employees and admins can create, view, edit, and delete farmers.
  - Credentials are generated and displayed securely for new farmers.
- 🥕 **Product Management:**  
  - Farmers can create, view, edit, and delete their own products.
  - Admins can manage all products.
- 💎 **User Experience:**  
  - Clean, responsive UI with Bootstrap cards, tables, and icons.
  - Copy-to-clipboard for credentials, inline validation, and helpful tooltips.
  - All key pages accessible from the navbar for all roles.
- 🛡️ **Security:**  
  - Uses ASP.NET Core Identity for authentication and role management.
  - Follows best practices for password handling and user onboarding.

---

## ⚡️ Setup & Prerequisites

1. **.NET 8.0 SDK**  
   [Download .NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. **Entity Framework Core 9.0.4** (via NuGet)
3. **SQLite** (default, or SQL Server if configured)
4. **EF Core CLI Tools**  
   ```bash
   dotnet tool install --global dotnet-aspnet-codegenerator
   dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
   dotnet add package Microsoft.EntityFrameworkCore.Design
   ```
5. **ASP.NET Core Identity UI Packages**  
   ```bash
   dotnet add package Microsoft.AspNetCore.Identity.UI
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   ```
6. **IDE:** Visual Studio 2022+ or VS Code with C# extension

### 🗄️ Database Setup

```bash
dotnet ef migrations add CreateIdentitySchema
dotnet ef database update
```

---

## 🏗️ Project Structure & Roles

- 👑 **Admin:**  
  - Full access to all users, products, and farmers.
  - Can assign roles, edit/delete any user, and manage all data.
- 🧑‍💼 **Employee:**  
  - Can create and manage farmers and view all products.
- 🚜 **Farmer:**  
  - Can manage their own products and view their profile.

### 🔄 Key Flows

1. 👑 Admin seeds and manages employees.
2. 🧑‍💼 Employees create and manage farmers.
3. 🚜 Farmers manage their own products.
4. 👑 Admins have full oversight and management via the dashboard.

---

## 🚀 How to Use FarmersHub

1. 📝 **Register a new user** via the registration page (default role: Farmer).
2. 👑 **Login as Admin** (see credentials above) to assign Employee roles as needed.
3. 🧑‍💼 **Employees** can create new farmers and view/manage all farmers and products.
4. 🚜 **Farmers** can log in with their credentials to manage their own products.
5. 👑 **Admins** can access the Admin Panel, "Our Farmers", and all management features from the navbar.

---

## 🛠️ Tech Stack

- ⚙️ ASP.NET Core 8.0
- 🗃️ Entity Framework Core 9.0.4
- 🔐 ASP.NET Core Identity
- 🗄️ SQLite (default, can be swapped for SQL Server)
- 🎨 Bootstrap (Bootswatch Minty theme)
- 🎉 Font Awesome & Bootstrap Icons

---

## ⚠️ Limitations & Future Improvements

- **User Profile Fields:**
  - The registration form currently only collects email and password. Full name and contact number are set to the email by default for new farmers. For a production system, extend the registration form to collect real names and contact numbers.
- **Credential Handling:**
  - Credentials for new farmers are displayed to employees for copy-paste. In a real deployment, implement secure email delivery and password reset flows.
- **Email Sending:**
  - Email sending is stubbed for demo purposes. Integrate a real email service (e.g., SendGrid, SMTP) for production.
- **Identity UI:**
  - The default ASP.NET Core Identity UI is used. For a seamless experience, customize these pages to match the rest of the app.
- **Validation & Error Handling:**
  - While improved, some forms could benefit from more advanced validation and user feedback.
- **Scalability:**
  - The app is designed for demo and prototype use. For large-scale deployment, consider performance, security hardening, and cloud deployment best practices.

---

## 🔗 References

- [Microsoft Docs - ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0)
- [Bootswatch Themes](https://bootswatch.com/minty/)
- [CLI Tools for ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-9.0)
- [CSS Tricks](https://css-tricks.com/)
- [Pixelbay](https://pixabay.com/)