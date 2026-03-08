# Restaurant Performance Dashboard

**A Full-Stack Project by Geison Herrera & Daniel Vega**
BSc in Computing — Dorset College Dublin

---

## Overview

A centralized POS & Performance Dashboard that solves a real restaurant management problem: managers spending hours collecting data from disparate systems (Stripe, Toast, Excel) to understand business performance.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 8 (C#) |
| Frontend | Blazor Server |
| Database | PostgreSQL |
| ORM | Entity Framework Core 8 |
| Reporting | QuestPDF |
| Authentication | ASP.NET Identity (Role-based) |
| Architecture | Clean Architecture |

## Features

- POS order management (open, add items, close, void)
- Sales tracking with payment method breakdown
- Staff shift management & tips tracking
- Expense recording & approval workflow
- Weekly & monthly PDF report generation
- CSV data import
- Role-based access (Admin, Manager, Staff)
- Real-time dashboard updates (SignalR)

## Architecture

```
RestaurantDashboard/
├── RestaurantDashboard.Domain/          # Entities, Value Objects, Domain Events
├── RestaurantDashboard.Application/     # CQRS (MediatR), DTOs, Validators
├── RestaurantDashboard.Infrastructure/  # EF Core, Identity, Repositories
└── RestaurantDashboard.Web/             # Blazor Server UI
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

### Run locally

1. Clone the repository
   ```bash
   git clone https://github.com/geisonhg/restaurant-performance-dashboard.git
   cd restaurant-performance-dashboard
   ```

2. Update the connection string in `backend/RestaurantDashboard/RestaurantDashboard.Web/appsettings.Development.json`
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=restaurant_dashboard_dev;Username=postgres;Password=YOUR_PASSWORD"
     }
   }
   ```

3. Run the application
   ```bash
   dotnet run --project backend/RestaurantDashboard/RestaurantDashboard.Web
   ```

4. Open `http://localhost:5000` in your browser

### Default Credentials (Development Seed)

| Email | Password | Role |
|---|---|---|
| admin@restaurant.com | Admin@12345! | Admin |
| manager@restaurant.com | Manager@12345! | Manager |
| staff@restaurant.com | Staff@12345! | Staff |

## Implementation Roadmap

- [x] **Phase 1** — Foundation: Solution structure, Domain, Application, Infrastructure, EF Core migrations
- [ ] **Phase 2** — Core Domain: Full CQRS handlers, unit tests
- [ ] **Phase 3** — Blazor Frontend: POS UI, Dashboard with charts
- [ ] **Phase 4** — Reporting & CSV Import: QuestPDF, background jobs
- [ ] **Phase 5** — Real-time & CI/CD: SignalR, GitHub Actions

---

*BSc Computing Final Project — Dorset College Dublin, 2025/2026*
