@echo off
echo Building MomenMedmSys and updating installer...
powershell -ExecutionPolicy Bypass -File "%~dp0Build-Release.ps1"
pause
