# SetPoint Backend API

Backend engine powering **SetPoint**, an offline-first strength training platform designed around synchronization, workout tracking and social interactions.

> **Note:** The native Android application *(Kotlin + Jetpack Compose + Room DB)* remains private for commercial monetization purposes. This repository exposes the complete production backend used by the mobile client.

---

## Contact

| | |
|---|---|
| **Developer** | Piotrek Budzowski |
| **Role** | Backend / Android Developer |
| **Email** | piobudzows@gmail.com |
| **LinkedIn** | [linkedin.com/in/piobudzows/](https://www.linkedin.com/in/piobudzows/) |
| **GitHub** | [github.com/piobudzows](https://github.com/budzowskipiotrek) |
---

## Tech Stack

<div align="left">

![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23_12-239120?style=flat-square&logo=csharp&logoColor=white)
![ASP.NET](https://img.shields.io/badge/ASP.NET_Core-5C2D91?style=flat-square)
![EF](https://img.shields.io/badge/EF_Core-68217A?style=flat-square)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=flat-square&logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?style=flat-square)
![AutoMapper](https://img.shields.io/badge/AutoMapper-red?style=flat-square)

</div>

---

# Project Overview

SetPoint was designed around a common mobile constraint:

**Users may train without connectivity while still expecting local persistence, instant writes and later synchronization across devices.**

The backend implements an **offline-first synchronization engine** capable of reconciling local and remote datasets through timestamp-based delta replication.

Core domains:

| Module | Description |
|---|---|
| Authentication | JWT authentication + brute-force protection |
| Synchronization | Bidirectional Push / Pull sync engine |
| Workout Tracking | Sessions, exercises and sets |
| Routines | Training template system |
| Social Layer | User relations and sharing |
| Measurements | Body composition tracking |
| Logging | Operational event tracking |

---

# Main Technical Challenge Solved

The core engineering challenge behind SetPoint was implementing synchronization between:

```text
Android (Room Database)
            ⇅
Offline Storage Layer
            ⇅
Synchronization Engine
            ⇅
ASP.NET Core Backend
            ⇅
PostgreSQL

The synchronization engine was built to support **offline training sessions with eventual consistency**,
transferring only modified entities through timestamp-based delta replication instead of full dataset replacement.
```
# Engineering Notes

Some implementation details intentionally exposed in this repository:

- Generic synchronization handlers using delegates (`Func<T, Task<bool>>`)
- Delta replication based on `LastSync`
- JWT refresh during synchronization
- Login lockout mechanism
- Rate limited sync endpoints
- Explicit middleware execution order
- Soft delete persistence model
- Social graph synchronization logic

# Current Work

Ongoing improvements:

- Push pipeline optimization through batched synchronization processing
- Synchronization performance improvements for large payloads
- User relation retrieval optimization
- Delta propagation refinements for social entities

Current synchronization pull path already uses delta replication.
Push path optimization is currently under implementation to reduce sequential processing overhead across multiple entities.
