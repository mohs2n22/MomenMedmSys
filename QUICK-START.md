# MEDMsys - Quick Start Guide

## 📦 Distribution Files

### 1. Standalone (No Install) - **Recommended for testing**
Location: `Distribution\`
- `MEDMsys.WPF.exe` (85 MB) - Just double-click to run
- `medmsys.db` - SQLite database with demo data
- No installation needed, no MySQL required!

### 2. Installer with Wizard - **For end users**
Location: `Install-MEDMsys.bat`
- Right-click → **Run as Administrator**
- Shows license agreement (must accept)
- Choose install location (User or Program Files)
- Creates desktop/Start Menu shortcuts
- Registers in Windows Add/Remove Programs
- Includes uninstaller

### 3. Professional Installer (Optional)
If you have **Inno Setup** installed:
```cmd
iscc MEDMsys-Setup.iss
```
Creates `Output\MEDMsys-Setup-v1.0.0.exe` with:
- Welcome wizard
- License page
- Install location picker
- Shortcut options
- Progress bar
- Finish page with "Launch now" option

---

## 🚀 Two Ways to Run

### Option A: Portable (No Install)
```
1. Open: C:\Users\mom2n\Desktop\MEDMsys\Distribution\
2. Double-click: MEDMsys.WPF.exe
```

### Option B: Install First
```
1. Right-click: Install-MEDMsys.bat → Run as Administrator
2. Accept license agreement
3. Choose install location
4. Choose shortcuts
5. Launch when done
```

---

## 📋 What's Included

| Item | Details |
|------|---------|
| **Application** | MEDMsys.WPF.exe (85 MB, standalone) |
| **Database** | medmsys.db (SQLite, 119 KB) |
| **Devices** | 10 pre-loaded medical devices |
| **Maintenance** | 7 records with costs |
| **Calibrations** | 6 records with certificates |
| **Incidents** | 3 risk/safety incidents |
| **Contracts** | 3 service contracts |
| **Spare Parts** | 10 parts with inventory levels |

---

## 🔧 Requirements

- **OS**: Windows 10/11 (64-bit)
- **Database**: None! (SQLite embedded)
- **Runtime**: None! (self-contained .NET 8)
- **Permissions**: Standard user (install to AppData) or Admin (Program Files)

---

## 📁 Project Structure

```
MEDMsys/
├── Distribution/          ← Ready-to-distribute files
│   ├── MEDMsys.WPF.exe   ← Standalone app (85 MB)
│   ├── medmsys.db        ← SQLite database
│   └── README.md
│
├── Install-MEDMsys.bat   ← Windows installer script
├── LICENSE.txt           ← License agreement
├── MEDMsys-Setup.iss     ← Inno Setup script (optional)
├── MEDMsys-NSIS.nsi      ← NSIS script (optional)
│
├── MEDMsys.Core/         ← Source code
├── MEDMsys.Data/
├── MEDMsys.Services/
├── MEDMsys.WPF/
└── MEDMsys.slnx          ← Solution file
```

---

## ✅ Ready to Ship!

Your MEDMsys application is complete and ready for distribution.
