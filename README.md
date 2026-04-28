<h1 align="center">🖥️ C# .NET 8 WPF POS Architecture Showcase</h1>

<p align="center">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" />
  <img src="https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/WPF-0089D6?style=for-the-badge&logo=windows&logoColor=white" />
  <img src="https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white" />
</p>

### 📌 About This Showcase
This repository contains selected core architectural components of **Lavira.AkyaPOS**, a commercial Point of Sale and Restaurant Management software I developed from scratch. 

It demonstrates my ability to build robust, scalable, and maintainable desktop applications using modern **C# .NET 8** standards, strict **MVVM** design patterns, and micro-ORMs.

> **⚠️ Note:** This is a partial repository. Full source code, XAML UI designs, licensing algorithms, and commercial API keys have been excluded to protect intellectual property.

### ⚙️ Core Architecture & Tech Stack

#### 1. Strict MVVM Pattern (`CommunityToolkit.Mvvm`)
- Clean separation of UI (`Views`) and business logic (`ViewModels`).
- Extensive use of `IValueConverter` for dynamic UI state changes without code-behind.
- Reactive properties and RelayCommands for seamless user interactions.

#### 2. High-Performance Data Access (`Dapper` + `SQLite`)
- Moved away from heavy ORMs (like EF Core) in favor of **Dapper** to ensure lightning-fast local database queries.
- Repository Pattern implementation (`IRepository`) for clean data abstraction.

#### 3. 3rd Party API Integrations
- Asynchronous integration with external food delivery APIs (e.g., TrendyolGo).
- Robust JSON serialization/deserialization (`Newtonsoft.Json`) and HTTP request handling (`RestSharp`).

#### 4. Security & Role Management
- Password hashing utilizing `BCrypt.Net-Next`.
- Custom `UserSession` and `PermissionChecker` for role-based UI rendering and access control.

---
<p align="center">
  <i>Copyright &copy; 2026 Furkan. All Rights Reserved.</i><br>
  <i>Showcase intended for technical evaluation only.</i>
</p>
