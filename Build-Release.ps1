# MomenMedmSys - Automated Build & Installer Script
# Run this to rebuild the app and update MomenMedmSys-Setup.exe automatically

$ErrorActionPreference = "Stop"
$RootDir = $PSScriptRoot

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MomenMedmSys Automated Build System" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Build & Publish Main Application
Write-Host "[1/4] Building Application..." -ForegroundColor Yellow
dotnet build "$RootDir\MomenMedmSys.WPF\MomenMedmSys.WPF.csproj" -c Release --verbosity quiet
if ($LASTEXITCODE -ne 0) { Write-Host "❌ App build failed." -ForegroundColor Red; exit 1 }

Write-Host "[2/4] Publishing Application to Distribution..." -ForegroundColor Yellow
$DistDir = "$RootDir\Distribution"
if (-not (Test-Path $DistDir)) { New-Item -ItemType Directory -Path $DistDir | Out-Null }

dotnet publish "$RootDir\MomenMedmSys.WPF\MomenMedmSys.WPF.csproj" -c Release -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -o $DistDir --verbosity quiet

if ($LASTEXITCODE -ne 0) { Write-Host "❌ App publish failed." -ForegroundColor Red; exit 1 }

# 2. Copy Database
Write-Host "[3/4] Updating Database..." -ForegroundColor Yellow
$DbSource = "$RootDir\MomenMedmSys.Data\medmsys.db"
if (Test-Path $DbSource) {
    Copy-Item $DbSource "$DistDir\medmsys.db" -Force
    Write-Host "    Database copied." -ForegroundColor Green
} else {
    Write-Host "    ⚠️ Database not found. Skipping." -ForegroundColor Yellow
}

# 3. Build Installer
Write-Host "[4/4] Building Installer..." -ForegroundColor Yellow
$OutputDir = "$RootDir\Output"
if (-not (Test-Path $OutputDir)) { New-Item -ItemType Directory -Path $OutputDir | Out-Null }

dotnet publish "$RootDir\MomenMedmSys.Installer\MomenMedmSys.Installer.csproj" -c Release -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:PublishTrimmed=false `
    -o $OutputDir --verbosity quiet

if ($LASTEXITCODE -ne 0) { Write-Host "❌ Installer build failed." -ForegroundColor Red; exit 1 }

# 4. Rename & Finalize
if (Test-Path "$OutputDir\MomenMedmSys.Installer.exe") {
    Move-Item "$OutputDir\MomenMedmSys.Installer.exe" "$OutputDir\MomenMedmSys-Setup.exe" -Force
}

# Summary
$ExePath = "$OutputDir\MomenMedmSys-Setup.exe"
if (Test-Path $ExePath) {
    $SizeMB = [math]::Round((Get-Item $ExePath).Length / 1MB, 2)
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ✅ Build Complete!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📦 Installer: $ExePath" -ForegroundColor Cyan
    Write-Host "📏 Size:      $SizeMB MB" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Double-click MomenMedmSys-Setup.exe to test." -ForegroundColor Gray
} else {
    Write-Host "❌ Failed to generate installer." -ForegroundColor Red
}
