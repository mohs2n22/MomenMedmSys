; ============================================
; MomenMedmSys - Medical Equipment Management System
; Inno Setup Installation Script
; ============================================
; Download Inno Setup: https://jrsoftware.org/isinfo.php
; Compile with: ISCC.exe MomenMedmSys-Setup.iss

#define MyAppName "MomenMedmSys"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "MomenMedmSys"
#define MyAppURL "https://medmsys.local"
#define MyAppExeName "MomenMedmSys.WPF.exe"
#define MyAppDatabase "medmsys.db"

[Setup]
; App identity
AppId={{A8F3E2D1-4B5C-6D7E-8F9A-0B1C2D3E4F5A}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; Installation settings
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE.txt
OutputDir=Output
OutputBaseFilename=MomenMedmSys-Setup-v{#MyAppVersion}
SetupIconFile=icon.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardSizePercent=100,100
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

; Privileges and restart
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
RestartIfNeededByRun=no

; Version info
VersionInfoVersion={#MyAppVersion}
VersionInfoCopyright=Copyright (C) 2026 {#MyAppPublisher}
VersionInfoDescription=Medical Equipment Management System Installer

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode
Name: "firewallrule"; Description: "Add firewall exception (if needed)"; GroupDescription: "Network:"; Flags: unchecked

[Files]
; Main application files
Source: "Distribution\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "Distribution\{#MyAppDatabase}"; DestDir: "{app}"; Flags: ignoreversion

; Documentation
Source: "Distribution\README.md"; DestDir: "{app}"; DestName: "README.txt"; Flags: ignoreversion

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
; Start Menu
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{group}\README"; Filename: "{app}\README.txt"

; Desktop
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

; Quick Launch
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Launch application after installation
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[UninstallDelete]
; Clean up database on uninstall (optional - comment out to keep data)
Type: files; Name: "{app}\{#MyAppDatabase}"
Type: files; Name: "{app}\{#MyAppDatabase}-journal"
Type: files; Name: "{app}\{#MyAppDatabase}-wal"
Type: files; Name: "{app}\{#MyAppDatabase}-shm"

[Registry]
; File associations (optional)
; Root: HKCU; Subkey: "Software\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"

[Code]
// Check if database already exists and ask user
function InitializeSetup(): Boolean;
var
  MsgResult: Integer;
  ExistingDB: String;
begin
  Result := True;
  
  // Check for existing installation
  ExistingDB := ExpandConstant('{autopf}\{#MyAppName}\{#MyAppDatabase}');
  if FileExists(ExistingDB) then
  begin
    MsgResult := MsgBox('An existing MomenMedmSys installation was detected.' + #13 + #13 +
      'Do you want to:' + #13 +
      '  Yes  - Keep existing data (upgrade)' + #13 +
      '  No   - Replace with fresh database' + #13 + #13 +
      'Click Cancel to abort installation.',
      mbConfirmation, MB_YESNOCANCEL);
      
    if MsgResult = IDCANCEL then
      Result := False
    else if MsgResult = IDYES then
      // Keep existing - we'll handle this in CurStepChanged
      Result := True
    else
      // Replace - delete old database
      DeleteFile(ExistingDB);
  end;
end;

// Post-installation actions
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Set proper permissions on database file
    // (SQLite needs write access)
  end;
end;

// Custom welcome page message
procedure InitializeWizard;
begin
  WizardForm.WelcomeLabel2.Caption :=
    'This will install MomenMedmSys v{#MyAppVersion} on your computer.' + #13 + #13 +
    'Medical Equipment Management System - A comprehensive solution for managing' + #13 +
    'the entire medical device lifecycle from purchase to disposal.' + #13 + #13 +
    'It is recommended that you close all other applications before continuing.';
end;

// Custom finish page message
procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpFinished then
  begin
    WizardForm.FinishedLabel.Caption :=
      'MomenMedmSys has been successfully installed on your computer.' + #13 + #13 +
      'Click Finish to exit Setup and launch the application.' + #13 + #13 +
      'The database (medmsys.db) is located in the installation folder.';
  end;
end;
