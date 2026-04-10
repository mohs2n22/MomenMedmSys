; ============================================
; MomenMedmSys - Medical Equipment Management System
; NSIS Installation Script
; ============================================
; Download NSIS: https://nsis.sourceforge.io/Download
; Compile with: makensis MomenMedmSys-NSIS.nsi

!define APP_NAME "MomenMedmSys"
!define APP_VERSION "1.0.0"
!define APP_PUBLISHER "MomenMedmSys"
!define APP_EXE "MomenMedmSys.WPF.exe"
!define APP_DB "medmsys.db"

; Include modern UI
!include "MUI2.nsh"

; General settings
Name "${APP_NAME} ${APP_VERSION}"
OutFile "Output\MomenMedmSys-Setup-v${APP_VERSION}.exe"
InstallDir "$PROGRAMFILES64\${APP_NAME}"
InstallDirRegKey HKLM "Software\${APP_NAME}" "InstallDir"
RequestExecutionLevel admin
ShowInstDetails show
ShowUnInstDetails show

; Modern UI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\win.bmp"

; Pages
!insertmacro MUI_PAGE_LICENSE "LICENSE.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language
!insertmacro MUI_LANGUAGE "English"

; Installer
Section "MomenMedmSys Application" SecApp
  SectionIn RO
  
  ; Set output path
  SetOutPath "$INSTDIR"
  
  ; Install files
  File "Distribution\${APP_EXE}"
  File "Distribution\${APP_DB}"
  File "Distribution\README.md"
  
  ; Write uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
  ; Create registry entries
  WriteRegStr HKLM "Software\${APP_NAME}" "InstallDir" "$INSTDIR"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayName" "${APP_NAME}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayIcon" '"$INSTDIR\${APP_EXE}"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "Publisher" "${APP_PUBLISHER}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayVersion" "${APP_VERSION}"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "NoRepair" 1
SectionEnd

Section "Start Menu Shortcuts" SecShortcuts
  CreateDirectory "$SMPROGRAMS\${APP_NAME}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"
  CreateShortCut "$SMPROGRAMS\${APP_NAME}\README.lnk" "$INSTDIR\README.md"
SectionEnd

Section "Desktop Shortcut" SecDesktop
  CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}"
SectionEnd

Section "Launch Application" SecRun
  ExecShell "" "$INSTDIR\${APP_EXE}"
SectionEnd

; Uninstaller
Section "Uninstall"
  ; Remove registry entries
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
  DeleteRegKey HKLM "Software\${APP_NAME}"
  
  ; Remove files
  Delete "$INSTDIR\${APP_EXE}"
  Delete "$INSTDIR\${APP_DB}"
  Delete "$INSTDIR\README.md"
  Delete "$INSTDIR\uninstall.exe"
  
  ; Remove shortcuts
  Delete "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\Uninstall ${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\README.lnk"
  RMDir "$SMPROGRAMS\${APP_NAME}"
  Delete "$DESKTOP\${APP_NAME}.lnk"
  
  ; Remove directory
  RMDir "$INSTDIR"
  
  ; Ask if user wants to keep database
  MessageBox MB_YESNO "Do you want to keep the database file (medmsys.db)?" IDYES KeepDB
    Delete "$INSTDIR\${APP_DB}"
  KeepDB:
SectionEnd

; Descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecApp} "Core application files and database"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecShortcuts} "Create Start Menu shortcuts"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Create desktop shortcut"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecRun} "Launch MomenMedmSys after installation"
!insertmacro MUI_FUNCTION_DESCRIPTION_END
