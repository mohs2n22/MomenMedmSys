# MomenMedmSys — Medical Equipment Management System (MEMS)

A comprehensive **organizational framework** designed to ensure the **efficient and safe operation of medical equipment** throughout its entire lifecycle — from procurement planning to safe disposal.

A **Windows desktop application** built with **.NET 8** and **WPF**, following a **layered architecture** with **MVVM** pattern. Replaces paper records with a fully digital **Computerized Maintenance Management System (CMMS)**.

---

## 🛠️ Key Pillars of the Management System

Modern hospitals manage medical equipment through several technical and administrative phases, all supported by this system:

### 1. 📋 Needs Planning & Procurement
- **Gap Analysis** — Identify required equipment based on medical specialties and patient volume
- **Specification Preparation** — Establish precise technical standards to guarantee equipment quality
- **Technical Evaluation** — Compare technical and financial offers from suppliers
- **Warranty & Contract Tracking** — Track warranty certificates, responsible agents, and service contracts

### 2. 📦 Inventory Control
- **Central Database** — Record serial numbers, purchase dates, locations, and technical specifications
- **Risk-Based Classification** — Categorize equipment by risk level (life support vs. standard diagnostic)
- **Department Assignment** — Track equipment movement and assignment across departments
- **Lifecycle Status** — Monitor equipment from Active → Under Maintenance → Out of Service → Decommissioned → Disposed
- **Digital Device File** — A complete record for each piece of equipment including operation manuals, maintenance history, and warranty certificates

### 3. 🔧 Preventive & Corrective Maintenance
- **Automated Scheduling** — Periodic alerts for routine maintenance checks (daily → annually)
- **Work Order Management** — Track fault reports and ensure rapid repair response
- **Cost Tracking** — Know total expenditure on each device (labor + parts) to assess economic viability
- **Spare Parts Provision** — Inventory management with min/max stock levels to ensure uninterrupted medical service
- **Technician Assignment** — Assign internal technicians or external contractors with findings documentation

### 4. 📏 Calibration & Safety Testing
- **Calibration Scheduling** — Track calibration/verification schedules with as-found/as-left measurements
- **Accuracy Assurance** — Ensure accurate readings for blood pressure monitors, anesthesia machines, etc.
- **Electrical Safety Tests** — Document safety testing to protect patients and medical staff
- **Certificate Management** — Store NIST-traceable standards and external lab accreditations
- **Tolerance Limits** — Track pass/fail with detailed measurement data

---

## 📊 Benefits of Implementing This Digital System

| Benefit | How It's Achieved |
|---------|-------------------|
| **Reduced Downtime** | Early detection of problems through preventive maintenance scheduling |
| **Cost Management** | Track total expenditure per device (purchase + maintenance + parts) to assess economic viability |
| **Standards Compliance** | Easier access to international accreditations — **JCI**, **ISO 13485**, **ISO 14971** |
| **Extended Device Lifespan** | Regular maintenance schedules and proper operation records |
| **Risk Management** | Severity/probability scoring with auto-calculated risk levels per ISO 14971 |
| **Staff Training Records** | Document which nurses or physicians are qualified to use each device |
| **Audit Readiness** | Complete digital trail for inspections and regulatory reviews |

---

## 🏥 Implemented Modules

| Module | Status | Description |
|--------|--------|-------------|
| **Device Asset Management** | ✅ | Full CRUD — manufacturer, model, serial, department/location, purchase price, warranty, technical specs, lifecycle status, risk classification |
| **Preventive & Corrective Maintenance** | ✅ | Schedule recurring tasks, track costs (labor + parts), log findings, assign technicians/contractors, work order management |
| **Calibration Management** | ✅ | Calibration schedules, as-found/as-left measurements, certificates, NIST-traceable standards, tolerance limits, external lab info |
| **Risk & Safety Incident Management** | ✅ | Log incidents per ISO 14971 with severity/probability scoring, root cause analysis, corrective/preventive actions, recall tracking |
| **Service Contract Management** | ✅ | External contracts with SLA tracking, performance metrics, response/resolution times, auto-renewal |
| **Spare Parts Inventory** | ✅ | Stock levels (min/max), suppliers, criticality, obsolescence tracking, linked to maintenance records |
| **Dashboard & Reporting** | ✅ | Overview stats, analytics, and report generation |
| **Digital Device File** | ✅ | Operation/maintenance manuals, repair history, warranty certificates, staff training records — all in one place |

---

## 📦 Domain Model

| Entity | Purpose |
|--------|---------|
| **MedicalDevice** | Core asset register — manufacturer, model, serial, department/location, purchase info, risk classification, lifecycle status, technical specs |
| **MaintenanceRecord** | Preventive, corrective, emergency maintenance, inspections — scheduling, recurrence, costs (labor + parts), findings, contractor info |
| **CalibrationRecord** | Calibration/verification — standards used, as-found/as-left data, tolerance limits, certificates, NIST traceability, external lab accreditation |
| **RiskIncident** | Safety incident management (ISO 14971) — severity/probability/risk level, patient/staff injury tracking, root cause, corrective actions, recall management |
| **ServiceContract** | External maintenance contracts — provider details, coverage types, SLA (response/resolution times), performance metrics, auto-renewal |
| **SparePart** | Inventory management — part numbers, stock levels (min/max), supplier/manufacturer info, lead times, criticality, obsolescence |
| **SparePartUsage** | Links spare parts to maintenance records — quantity used, notes, date |

**Risk classifications:** Low · Medium · High · Critical  
**Maintenance types:** Preventive · Corrective · Emergency · Inspection · Calibration  
**Maintenance status:** Scheduled · In Progress · Completed · Cancelled · Overdue  
**Calibration results:** Pass · Fail · PassWithAdjustment · OutOfTolerance · NotCalibrated  
**Device lifecycle:** Active · Under Maintenance · Out of Service · Pending Calibration · Decommissioned · Disposed

---

## 🖥️ Screens

| View | Description |
|------|-------------|
| **Dashboard** | Overview stats, summaries, risk alerts, and quick actions |
| **Device List** | Browse, search, and filter all medical devices |
| **Device Form** | Add or edit device details — full digital device file |
| **Maintenance** | Manage maintenance records and work orders |
| **Calibration** | Manage calibration records and schedules |
| **Risk Management** | Track safety incidents and risk analysis |
| **Spare Parts** | Inventory management with stock level alerts |
| **Reports** | Generate reports and analytics for compliance |

---

## 🛠️ Technology Stack

| Layer | Technology |
|-------|------------|
| **Framework** | .NET 8.0 |
| **UI** | ASP.NET Core Razor Pages |
| **Database** | MySQL 8.0 |
| **ORM** | Entity Framework Core 8.0 (Pomelo MySQL) |
| **DI** | Microsoft.Extensions.DependencyInjection |

---

## 📐 Architecture

```
MomenMedmSys.slnx
│
├── MomenMedmSys.Core        ← Domain layer (entities, enums — no dependencies)
├── MomenMedmSys.Data        ← Data access layer (EF Core, MySQL, DbContext, migrations)
├── MomenMedmSys.Services    ← Business logic / service layer
└── MomenMedmSys.Web         ← Presentation layer (ASP.NET Core Razor Pages)
```

**Dependency flow:** `Web → Services → Data → Core`

---

## 🚀 Quick Start

### Requirements
- **OS:** Windows 10/11, Linux, or macOS
- **SDK:** .NET 8.0 SDK
- **Database:** MySQL 8.0 (server required)

### Run from Source
```powershell
dotnet run --project MomenMedmSys.Web
```

---

## 📁 Project Structure

```
MomenMedmSys/
├── MomenMedmSys.Core/          Domain entities and enums
│   └── MomenMedmSys.Core.csproj
├── MomenMedmSys.Data/          EF Core DbContext, migrations, and seeders
│   ├── Migrations/
│   └── MomenMedmSys.Data.csproj
├── MomenMedmSys.Services/      Business logic services
│   └── MomenMedmSys.Services.csproj
├── MomenMedmSys.Web/           ASP.NET Core web application
│   ├── Pages/                  Razor Pages
│   ├── wwwroot/                Static files (CSS, JS)
│   └── MomenMedmSys.Web.csproj
├── MomenMedmSys.slnx           Solution file
├── appsettings.json            Application configuration (MySQL connection)
└── README.md                   This file
```

---

## 📋 Demo Data

The `DatabaseSeeder.cs` automatically seeds initial data on first run including:

| Item | Count |
|------|-------|
| Departments | 12 |
| Suppliers | 8 |
| Medical Devices | 74 |
| Admin User | 1 (username: admin, password: Admin@123) |

---

## 🏗️ Build & Distribution

### Build Release
```powershell
.\Build-Release.ps1
```

### Build Installer
```powershell
.\Build-Installer.ps1
```

### Create Inno Setup Installer (optional)
```powershell
iscc MomenMedmSys-Setup.iss
```

---

## 📄 License

See `LICENSE.txt` for full terms.

**Summary:** The software is provided for internal medical facility use only. You may install and use it on any number of computers within your organization. Distribution, sale, or modification is not permitted without authorization.

---

## ⚠️ Disclaimer

This software is a **management tool**, **not a medical device**. It does not replace regulatory compliance requirements. Users remain responsible for **ISO 13485**, **ISO 14971** compliance. Calibration and maintenance records should be verified independently.

---

> **MomenMedmSys** v1.0.0 · Medical Equipment Management System (MEMS) · Copyright © 2026
