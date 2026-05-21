# SetPoint - Backend API

Core services and RESTful API engine for **SetPoint**, an advanced strength training tracking and optimization platform.

*Intellectual Property Note: The native Android client (Kotlin, Jetpack Compose, Room DB) is maintained in a private repository for commercial monetization purposes. This repository showcases 100% of the production-ready backend engineering.*

## 🛠️ Tech Stack
* **Core Framework:** .NET 8 (ASP.NET Core)
* **Language:** C# 12
* **Data Access (ORM):** Entity Framework Core (Code-First)
* **Relational Database:** PostgreSQL
* **Security:** JWT (JSON Web Tokens) Authentication & Custom Encryption Services

---

## 📐 System Architecture & Design Patterns

The backend follows a strict **N-Tier Architecture** pattern, enforcing a clean separation of concerns, decoupling, and dependency isolation:

1. **Presentation Layer (API Layer / Program.cs):**
   * Configures the HTTP request pipeline middleware execution order explicitly.
   * Standardizes CORS infrastructure with isolated environment policies (`AllowAll` for local mobile testing / `ProductionPolicy` for secure domains).
   * Implements Reverse Proxy compatibility by overriding and mapping headers (`X-Forwarded-For`, `X-Forwarded-Proto`) to safely handle hosting under upstream gateway topologies.
2. **Business Logic Layer (BLL):**
   * Services are decoupled using strict Dependency Injection lifetimes (`AddScoped`) mapping abstractions to implementations (`IUserBll` ➔ `UserBll`, etc.).
   * In-memory object mapping via AutoMapper isolates database entity states from external-facing Data Transfer Objects (DTOs).
3. **Data Access Layer (DAL):**
   * Database session scoping manages transactions using a centralized context (`SetPointDbContext`) over PostgreSQL.

---

## 🔒 Advanced Security & Production-Grade Features

Unlike standard academic projects, this core backend integrates specific production-level guardrails:

### 1. Robust Account-Lockout & Brute-Force Protection
The `AuthService` tracks security events through an asynchronous log persistence layer. It actively mitigates brute-force authentication attempts using a sliding execution window:
* Captures failed login state metrics (`Access Failed - Invalid Password`).
* Implements a **3-strike lockout threshold within a 1-minute window**, throwing explicit `UnauthorizedAccessException` and blocking upstream authentication calls at the database level to optimize computing cycles.
* Transparent runtime password maintenance checks for credential rehashing algorithms automatically.

### 2. Network Rate-Limiting Policy
Protects endpoints against automation scripting and API abuse by registering a native `RateLimiter` middleware layer:
* **Policy `SincronizacionLenta`:** Implements a stateful `FixedWindowLimiter` bound to the client's authenticated identity or remote IP address.
* Restricts synchronization throughput traffic to **10 requests per minute with a strict zero-queue policy**, issuing standard `429 TooManyRequests` HTTP status codes upon violation.

---

## 🔄 Two-Way Synchronization Engine (Offline-First Support)

The `SyncService` coordinates an atomic replication protocol designed to ingest, process, and reconcile decoupled mobile device datasets:

* **Push Pipeline (`ProcessPush`):** Higher-order functional mapping leverages structured delegate actions (`Func<T, Task<bool>>`) and state-mutators (`Action<T>`) to genericize batch collection synchronizations across 12 distinct relational entity fields simultaneously, capturing internal state exceptions gracefully without disrupting the sync stack execution.
* **Pull Pipeline (`ProcessPull`):** Resolves delta updates utilizing timestamp lookups (`LastSync`) combined with complex explicit memory-join lookups (`HashSet<Guid>`) to batch-replicate domain entity states belonging exclusively to the user and their explicit non-rejected social contact associations.
