🛍️ Sales Management System (ASP.NET Core MVC + Entity Framework Core)
This is a complete Sales Management System built using ASP.NET Core MVC, Entity Framework Core, and Identity for authentication. The application manages Users, Products, Customers, Orders, and Sales Items, featuring robust functionalities such as CRUD operations, searching, paging, sorting, session handling, and user authentication.

🚀 Key Features
✅ User Authentication (via Identity + custom AppUser support)

🔍 Search & Filter capabilities for Products, Customers, and Orders

↕️ Paging & Sorting with server-side implementation

🛒 Full Sales Workflow: Customer → Order → Sales Items

🧾 Product Management with photo upload and price tracking

👤 Customer Profiles with address and mobile information

🗂 Entity Relationships: One-to-Many (Customer → Orders, Product → SalesItems)

📦 Sales Order Module with dynamic item management

💾 Session Management for temporary data handling

🛡️ Role-Based Access Control with ASP.NET Identity

🧪 Data Validation using Data Annotations

🧩 Tech Stack
ASP.NET Core MVC

Entity Framework Core (Code First)

SQL Server

Identity (Authentication & Roles)

Bootstrap (UI Design)

LINQ, Sessions, ViewModels

Razor Views with TagHelpers

📦 Core Domain Models
AppUser – Custom user table with role & active status

ApplicationUser – Identity-based user extension

Product, Customer, SalesOrder, SalesItem – Fully linked relational models

This project demonstrates a real-world structure of a sales and inventory system, ideal for learning clean architecture, data management, and MVC best practices in ASP.NET Core.

