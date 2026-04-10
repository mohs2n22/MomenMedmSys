@echo off
setlocal EnableDelayedExpansion

REM ============================================
REM MomenMedmSys - Medical Equipment Management System
REM Windows Installer
REM ============================================

title MomenMedmSys Installer v1.0.0

REM Get script directory
set "SCRIPT_DIR=%~dp0"
set "SCRIPT_DIR=%SCRIPT_DIR:~0,-1%"

REM ============================================
REM WELCOME SCREEN
REM ============================================
cls
echo.
echo        ============================================
echo         MomenMedmSys - Medical Equipment Management System
echo         Version 1.0.0
echo        ============================================
echo.
echo   This installer will set up MomenMedmSys on your computer.
echo.
echo   Press any key to continue...
pause >nul

REM ============================================
REM LICENSE AGREEMENT
REM ============================================
cls
echo.
echo        ============================================
echo         LICENSE AGREEMENT
echo        ============================================
echo.
type "%SCRIPT_DIR%\LICENSE.txt"
echo.
echo        ============================================
echo.
set /p "ACCEPT=Do you accept the terms of the License Agreement? (Y/N): "
if /I not "%ACCEPT%"=="Y" (
    echo.
    echo Installation cancelled.
    pause
    exit /b 0
)

REM ============================================
REM INSTALL LOCATION
REM ============================================
cls
echo.
echo        ============================================
echo         Installation Location
echo        ============================================
echo.
echo   Select installation type:
echo.
echo   1. Current User  - Installs to your user folder (no admin needed)
echo   2. All Users     - Installs to Program Files (requires admin)
echo.
set /p "INST_TYPE=Choose (1 or 2): "

if "%INST_TYPE%"=="2" (
    set "INSTALL_DIR=C:\Program Files\MomenMedmSys"
    echo.
    echo Installing to: %INSTALL_DIR%
    echo.
    echo NOTE: Administrator privileges will be required.
    echo If you don't have admin rights, press Ctrl+C to cancel and choose option 1.
    pause

    REM Try to elevate
    net session >nul 2>&1
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo Requesting administrator privileges...
        powershell -Command "Start-Process '%~f0' -Verb RunAs" 2>nul
        exit /b
    )
) else (
    set "INSTALL_DIR=%LOCALAPPDATA%\MomenMedmSys"
    echo.
    echo Installing to: %INSTALL_DIR%
)

REM Create installation directory
if not exist "%INSTALL_DIR%" (
    echo.
    echo Creating directory...
    mkdir "%INSTALL_DIR%" 2>nul
    if %ERRORLEVEL% NEQ 0 (
        echo.
        echo [ERROR] Failed to create directory.
        pause
        exit /b 1
    )
)

REM ============================================
REM SHORTCUTS
REM ============================================
cls
echo.
echo        ============================================
echo         Shortcuts
echo        ============================================
echo.
set /p "DESKTOP_SHORTCUT=Create desktop shortcut? (Y/N): "
set /p "START_SHORTCUT=Create Start Menu shortcut? (Y/N): "

REM ============================================
REM INSTALLATION
REM ============================================
cls
echo.
echo        ============================================
echo         Installing MomenMedmSys
echo        ============================================
echo.

echo [1/4] Copying application files...
copy /Y "%SCRIPT_DIR%\Distribution\MomenMedmSys.WPF.exe" "%INSTALL_DIR%\" >nul
echo        Done.

echo [2/4] Copying database...
copy /Y "%SCRIPT_DIR%\Distribution\medmsys.db" "%INSTALL_DIR%\" >nul
echo        Done.

echo [3/4] Copying documentation...
copy /Y "%SCRIPT_DIR%\Distribution\README.md" "%INSTALL_DIR%\" >nul
echo        Done.

echo [4/4] Creating shortcuts...

REM Start Menu shortcut
if /I "%START_SHORTCUT%"=="Y" (
    set "START_DIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\MomenMedmSys"
    if not exist "%START_DIR%" mkdir "%START_DIR%"

    powershell -Command "$W = New-Object -com WScript.Shell; $S = $W.CreateShortcut('%START_DIR%\MomenMedmSys.lnk'); $S.TargetPath = '%INSTALL_DIR%\MomenMedmSys.WPF.exe'; $S.WorkingDirectory = '%INSTALL_DIR%'; $S.Save()" 2>nul

    echo        Start Menu shortcut created.
)

REM Desktop shortcut
if /I "%DESKTOP_SHORTCUT%"=="Y" (
    for /f "tokens=*" %%a in ('powershell -Command "[Environment]::GetFolderPath('Desktop')"') do set "DESKTOP_PATH=%%a"

    powershell -Command "$W = New-Object -com WScript.Shell; $S = $W.CreateShortcut('%DESKTOP_PATH%\MomenMedmSys.lnk'); $S.TargetPath = '%INSTALL_DIR%\MomenMedmSys.WPF.exe'; $S.WorkingDirectory = '%INSTALL_DIR%'; $S.Save()" 2>nul

    echo        Desktop shortcut created.
)

echo.

REM ============================================
REM UNINSTALLER
REM ============================================
(
echo @echo off
echo setlocal
echo echo.
echo echo Uninstalling MomenMedmSys...
echo echo.
echo set /p "KEEP_DB=Keep database file? (Y/N): "
echo if /I not "%%KEEP_DB%%"=="Y" (
echo   del /f /q "%INSTALL_DIR%\medmsys.db*" 2^>nul
echo )
echo del /f /q "%INSTALL_DIR%\MomenMedmSys.WPF.exe"
echo del /f /q "%INSTALL_DIR%\README.md"
echo for /f "tokens=*" %%%%a in ^('powershell -Command "[Environment]::GetFolderPath^('Desktop^')"'^) do (
echo   if exist "%%%%a\MomenMedmSys.lnk" del /f /q "%%%%a\MomenMedmSys.lnk"
echo )
echo if exist "%%APPDATA%%\Microsoft\Windows\Start Menu\Programs\MomenMedmSys" rmdir /s /q "%%APPDATA%%\Microsoft\Windows\Start Menu\Programs\MomenMedmSys"
echo del /f /q "%%~dp0%%~nx0"
echo rmdir /s /q "%INSTALL_DIR%" 2^>nul
echo echo.
echo echo MomenMedmSys has been uninstalled.
echo pause
) > "%INSTALL_DIR%\uninstall.bat"

REM Registry entry
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys" /v DisplayName /t REG_SZ /d "MomenMedmSys" /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys" /v DisplayVersion /t REG_SZ /d "1.0.0" /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys" /v UninstallString /t REG_SZ /d "%INSTALL_DIR%\uninstall.bat" /f >nul 2>&1
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys" /v DisplayIcon /t REG_SZ /d "%INSTALL_DIR%\MomenMedmSys.WPF.exe" /f >nul 2>&1

REM ============================================
REM COMPLETE
REM ============================================
cls
echo.
echo        ============================================
echo         Installation Complete!
echo        ============================================
echo.
echo   Installation Path: %INSTALL_DIR%
echo.
echo   Installed:
echo     - MomenMedmSys.WPF.exe
echo     - medmsys.db (with demo data)
echo     - README.md
echo     - Uninstaller
echo.

set /p "LAUNCH=Launch MomenMedmSys now? (Y/N): "
if /I "%LAUNCH%"=="Y" (
    start "" "%INSTALL_DIR%\MomenMedmSys.WPF.exe"
)

echo.
echo Done. Press any key to exit...
pause >nul
exit /b 0
