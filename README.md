<-- 🌟 PROJECT BANNER -->
<p align="center">
  <img src="https://raw.githubusercontent.com/Youssef-Elmesedy/Review_Guard/main/the-doctor-shows-the-icon-of-the-protection-of-health.webp" alt="Review Guard Banner" width="100%" />
</p>

<h1 align="center">🛡️ Review Guard</h1>

<p align="center">
  A secure and tamper-resistant review platform built with <b>.NET 8</b> and <b>Clean Architecture</b>.  
  It ensures authentic user reviews by enforcing <b>proof of purchase</b>, <b>trust scoring</b>, and a <b>risk-based moderation system</b>.  
  Designed for scalability, security, and real-world marketplace reliability.
</p>

---

<-- 🌟 HEADER BANNER -->
<p align="center">
  <img src="https://raw.githubusercontent.com/Youssef-Elmesedy/Review_Guard/main/.github/images/banner.webp" width="100%" />
</p>

<h1 align="center">🛡️ Review Guard</h1>

<p align="center">
  <b>Enterprise-Grade Tamper-Resistant Review System</b><br/>
  Built with <b>.NET 8</b>, <b>Clean Architecture</b>, and <b>DDD Lite</b><br/>
  Designed to eliminate fake reviews using proof validation + trust scoring + risk engine
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-blueviolet" />
  <img src="https://img.shields.io/badge/Architecture-Clean-0ea5e9" />
  <img src="https://img.shields.io/badge/CQRS-MediatR-orange" />
  <img src="https://img.shields.io/badge/Security-High-red" />
</p>

---

# ⚡ SYSTEM OVERVIEW

Client → API → Application → Domain ← Infrastructure

<p align="center">
  <a href="https://github.com//Review_Guard/stargazers">
    <img src="https://img.shields.io/github/stars/Youssef-Elmesedy/Review_Guard?color=yellow" alt="Stars Badge"/>
  </a>

  <a href="https://github.com/Youssef-Elmesedy/Review_Guard/network/members">
    <img src="https://img.shields.io/github/forks/Youssef-Elmesedy/Review_Guard?color=blue" alt="Forks Badge"/>
  </a>

  <a href="https://github.com/Youssef-Elmesedy/Review_Guard/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/Youssef-Elmesedy/Review_Guard?color=green" alt="License Badge"/>
  </a>

  <a href="#">
    <img src="https://img.shields.io/badge/.NET-8.0-blueviolet" alt=".NET Version"/>
  </a>
</p>

---

## 🏗️ Architecture Overview

The system is built using **Clean Architecture + DDD (Lite)** to ensure separation of concerns and scalability.
📦 ReviewGuard
┣ 📂 API → Presentation Layer (Controllers, Middleware, Swagger)

┣ 📂 Application → CQRS, Commands, Queries, Validators, DTOs

┣ 📂 Domain → Core Business Logic (Entities, Rules, ValueObjects)

┣ 📂 Infrastructure → Data Access (EF Core, Repositories, External Services)

---

## 🔑 Key Features

- 🧠 **CQRS Architecture** using MediatR  
- 🧾 **Proof of Purchase System** (Invoices, Receipts, Order IDs)  
- 🧑‍💼 **Trust Score System** (Dynamic User Reputation)  
- ⚠️ **Risk Engine** (Spam & Fraud Detection)  
- ⭐ **Weighted Rating System** (Trust-based review influence)  
- 📊 **Admin / Owner / User Dashboards**  
- 📦 **File Storage Abstraction** (Local / Cloud-ready)  
- 📧 **Email Notification System** (Events-based)  
- 🔐 **JWT Authentication + Role-based Authorization**  
- 🚨 **Anti-Spam & Abuse Detection Layer**  

---

## ⚙️ Core Modules

### 👤 User System
- Registration & Email Verification  
- TrustScore-based user leveling  
- Role-based access (User / Admin / Owner)  

### ⭐ Reviews
- Submit review with proof requirement  
- Auto / manual moderation  
- Weighted rating calculation  

### 🏢 Businesses
- Business registration  
- Analytics dashboard  
- Rating aggregation  

### 📎 Proof System
- Upload invoices / receipts  
- Admin verification required  
- Secure file storage abstraction  

---

## 🧠 Design Patterns Used

- Repository Pattern  
- Unit of Work Pattern  
- CQRS Pattern  
- Specification Pattern  
- Strategy Pattern (Risk & Rating logic)  
- Factory Pattern (Infrastructure setup)  

---

## 🧰 Tech Stack

| Layer | Technology |
|------|-----------|
| Backend | ASP.NET Core 8 |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Architecture | Clean Architecture + DDD |
| Auth | JWT Bearer Tokens |
| Validation | FluentValidation |
| Messaging | MediatR (CQRS) |
| Documentation | Swagger / OpenAPI |

---

## 🚀 Getting Started

```bash
dotnet restore
dotnet ef database update
dotnet run --project src/ReviewGuard.API
🔐 Security Features
PBKDF2 Password Hashing
JWT Authentication (Role-based)
IP Tracking for Abuse Detection
Anti-Spam Rules Engine
Global Exception Handling Middleware
🧪 Review Flow
Register → Verify Email → Login
→ Create Business → Upload Proof
→ Admin Verifies Proof
→ Submit Review
→ Risk Engine Evaluation
→ Weighted Rating Update
→ Dashboard Sync
📈 Future Improvements
🌐 Frontend (React / Angular Dashboard)
📱 Mobile App Integration
🤖 AI-based Fraud Detection
📊 Advanced Analytics Dashboard
🔔 Real-time Notifications (SignalR)
👨‍💻 Author

Youssef Elmesedy
.NET Developer | Clean Architecture Enthusiast

🪪 License

This project is licensed under the MIT License.

<p align="center"> ⭐ If you like this project, don't forget to star the repository! </p>

 ```
