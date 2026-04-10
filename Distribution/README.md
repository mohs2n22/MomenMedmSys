# MomenMedmSys v1.0.0 - Medical Equipment Management System

## 🚀 How to Run

**No MySQL needed!** Just double-click:

```
MomenMedmSys.WPF.exe
```

The app uses SQLite - a single file database included in this folder.

---

## 📦 Contents

| File | Size | Description |
|------|------|-------------|
| `MomenMedmSys.WPF.exe` | ~85 MB | Main application (standalone) |
| `medmsys.db` | - | SQLite database with demo data |
| `README.md` | - | This file |

---

## 📊 Demo Data Included

The database comes pre-loaded with:
- **10 Medical Devices** (X-Ray, Ventilators, Monitors, etc.)
- **7 Maintenance Records**
- **6 Calibration Records**
- **3 Risk Incidents**
- **3 Service Contracts**
- **10 Spare Parts**

---

## 🔧 Reset Demo Data

To reset the database to default demo data:

```bash
sqlite3 medmsys.db < seed-data-sqlite.sql
```

---

## 🔍 Troubleshooting

**"Application won't start"**
- Check Windows Defender isn't blocking the EXE
- Right-click → Properties → Check "Unblock" if present

**"Database error"**
- Make sure `medmsys.db` is in the same folder as the EXE
- Delete `medmsys.db` and re-copy from backup to reset

---

**Version**: 1.0.0  
**Build**: April 2026  
**Platform**: Windows 10/11 (x64)  
**Database**: SQLite (embedded, no server needed)
