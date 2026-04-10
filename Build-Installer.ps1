# MomenMedmSys Self-Extracting Installer Builder
# This script creates a single MomenMedmSys-Setup.exe installer

$ErrorActionPreference = "Stop"

Write-Host "Building MomenMedmSys Self-Extracting Installer..." -ForegroundColor Cyan
Write-Host ""

# Paths
$SourceDir = "C:\Users\mom2n\Desktop\MomenSys\Installer-Package"
$OutputDir = "C:\Users\mom2n\Desktop\MomenSys\Output"
$OutputExe = "$OutputDir\MomenMedmSys-Setup.exe"

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Read files as base64
Write-Host "Embedding files..." -ForegroundColor Yellow

$files = @{
    "MomenMedmSys.WPF.exe" = [Convert]::ToBase64String([IO.File]::ReadAllBytes("$SourceDir\MomenMedmSys.WPF.exe"))
    "medmsys.db" = [Convert]::ToBase64String([IO.File]::ReadAllBytes("$SourceDir\medmsys.db"))
    "README.md" = [Convert]::ToBase64String([IO.File]::ReadAllBytes("$SourceDir\README.md"))
    "LICENSE.txt" = [Convert]::ToBase64String([IO.File]::ReadAllBytes("$SourceDir\LICENSE.txt"))
}

# Create installer source code
$installerCode = @"
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace MomenMedmSysInstaller
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallerForm());
        }
    }

    public class InstallerForm : Form
    {
        private TextBox txtLicense;
        private RadioButton rbAccept;
        private RadioButton rbDecline;
        private TextBox txtPath;
        private Button btnBrowse;
        private CheckBox chkDesktop;
        private CheckBox chkStartMenu;
        private Button btnInstall;
        private Button btnCancel;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Panel pnlLicense;
        private Panel pnlOptions;
        private Panel pnlProgress;

        public InstallerForm()
        {
            InitializeComponents();
            SetupLayout();
        }

        private void InitializeComponents()
        {
            Text = "MomenMedmSys Setup - Medical Equipment Management System v1.0.0";
            Size = new Size(600, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // License Panel
            pnlLicense = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            
            var lblTitle = new Label { 
                Text = "LICENSE AGREEMENT", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true 
            };
            pnlLicense.Controls.Add(lblTitle);

            txtLicense = new TextBox {
                Location = new Point(20, 45),
                Size = new Size(540, 280),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9),
                BackColor = Color.White
            };
            pnlLicense.Controls.Add(txtLicense);

            rbAccept = new RadioButton { 
                Text = "I accept the terms of the License Agreement", 
                Location = new Point(20, 340),
                AutoSize = true,
                Checked = true
            };
            pnlLicense.Controls.Add(rbAccept);

            rbDecline = new RadioButton { 
                Text = "I do not accept", 
                Location = new Point(20, 365),
                AutoSize = true 
            };
            pnlLicense.Controls.Add(rbDecline);

            // Options Panel
            pnlOptions = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Visible = false };
            
            var lblInstallTitle = new Label { 
                Text = "Installation Options", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true 
            };
            pnlOptions.Controls.Add(lblInstallTitle);

            var lblPath = new Label { 
                Text = "Install to:", 
                Location = new Point(20, 55),
                AutoSize = true 
            };
            pnlOptions.Controls.Add(lblPath);

            txtPath = new TextBox {
                Location = new Point(100, 52),
                Size = new Size(380, 23),
                Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "MomenMedmSys")
            };
            pnlOptions.Controls.Add(txtPath);

            btnBrowse = new Button {
                Text = "Browse...",
                Location = new Point(490, 50),
                Size = new Size(70, 25)
            };
            btnBrowse.Click += (s, e) => {
                using (var fbd = new FolderBrowserDialog()) {
                    if (fbd.ShowDialog() == DialogResult.OK) {
                        txtPath.Text = fbd.SelectedPath;
                    }
                }
            };
            pnlOptions.Controls.Add(btnBrowse);

            chkDesktop = new CheckBox {
                Text = "Create desktop shortcut",
                Location = new Point(20, 95),
                AutoSize = true,
                Checked = true
            };
            pnlOptions.Controls.Add(chkDesktop);

            chkStartMenu = new CheckBox {
                Text = "Create Start Menu shortcut",
                Location = new Point(20, 120),
                AutoSize = true,
                Checked = true
            };
            pnlOptions.Controls.Add(chkStartMenu);

            // Progress Panel
            pnlProgress = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Visible = false };
            
            lblStatus = new Label {
                Location = new Point(20, 50),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            pnlProgress.Controls.Add(lblStatus);

            progressBar = new ProgressBar {
                Location = new Point(20, 80),
                Size = new Size(540, 25),
                Style = ProgressBarStyle.Continuous
            };
            pnlProgress.Controls.Add(progressBar);

            // Buttons
            btnInstall = new Button {
                Text = "Install",
                Location = new Point(380, 420),
                Size = new Size(90, 30),
                DialogResult = DialogResult.OK
            };
            btnInstall.Click += BtnInstall_Click;

            btnCancel = new Button {
                Text = "Cancel",
                Location = new Point(480, 420),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) => Application.Exit();

            Controls.Add(btnInstall);
            Controls.Add(btnCancel);
            Controls.Add(pnlLicense);
            Controls.Add(pnlOptions);
            Controls.Add(pnlProgress);
        }

        private void SetupLayout()
        {
            // Load license
            txtLicense.Text = @"MomenMedmSys - Medical Equipment Management System
Version 1.0.0
Copyright (C) 2026 MomenMedmSys

LICENSE AGREEMENT

IMPORTANT - READ CAREFULLY BEFORE INSTALLING

By installing, copying, or otherwise using MomenMedmSys (the ""Software""),
you agree to be bound by the terms of this License Agreement.

1. GRANT OF LICENSE
   The Software is provided for internal medical facility use only.

2. RESTRICTIONS
   You may NOT distribute, sell, rent, or lease the Software.

3. DATA AND PRIVACY
   The Software stores data locally using SQLite database.
   No data is transmitted to external servers.

4. DISCLAIMER OF WARRANTY
   THE SOFTWARE IS PROVIDED ""AS IS"" WITHOUT WARRANTY OF ANY KIND.

5. LIMITATION OF LIABILITY
   IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY DAMAGES.

By clicking ""Install"", you agree to these terms.";
        }

        private void BtnInstall_Click(object sender, EventArgs e)
        {
            if (!rbAccept.Checked) {
                MessageBox.Show("You must accept the license agreement to continue.", "License Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string installPath = txtPath.Text;
            if (string.IsNullOrWhiteSpace(installPath)) {
                MessageBox.Show("Please specify an installation path.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create directory
            try {
                if (!Directory.Exists(installPath)) {
                    Directory.CreateDirectory(installPath);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Cannot create directory: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Switch to progress view
            pnlLicense.Visible = false;
            pnlOptions.Visible = false;
            pnlProgress.Visible = true;
            btnInstall.Enabled = false;
            btnCancel.Enabled = false;

            // Extract files
            ExtractFiles(installPath);
        }

        private void ExtractFiles(string installPath)
        {
            var embeddedFiles = GetEmbeddedFiles();
            int total = embeddedFiles.Count;
            int current = 0;

            foreach (var file in embeddedFiles) {
                current++;
                lblStatus.Text = $"Extracting {file.Key}... ({current}/{total})";
                progressBar.Value = (int)((double)current / total * 100);
                Application.DoEvents();

                string filePath = Path.Combine(installPath, file.Key);
                File.WriteAllBytes(filePath, file.Value);
            }

            // Create shortcuts
            lblStatus.Text = "Creating shortcuts...";
            progressBar.Value = 95;
            Application.DoEvents();

            if (chkDesktop.Checked) {
                CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "MomenMedmSys.lnk"), Path.Combine(installPath, "MomenMedmSys.WPF.exe"));
            }

            if (chkStartMenu.Checked) {
                string startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "MomenMedmSys");
                Directory.CreateDirectory(startMenuPath);
                CreateShortcut(Path.Combine(startMenuPath, "MomenMedmSys.lnk"),
                    Path.Combine(installPath, "MomenMedmSys.WPF.exe"));
            }

            progressBar.Value = 100;
            lblStatus.Text = "Installation complete!";

            MessageBox.Show("MomenMedmSys has been successfully installed!\n\n" +
                $"Installation Path: {installPath}\n\n" +
                "Click OK to launch the application.", "Installation Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Launch application
            try {
                Process.Start(Path.Combine(installPath, "MomenMedmSys.WPF.exe"));
            } catch { }

            Application.Exit();
        }

        private System.Collections.Generic.Dictionary<string, byte[]> GetEmbeddedFiles()
        {
            var files = new System.Collections.Generic.Dictionary<string, byte[]>();

            // Embedded files (base64 encoded)
            files.Add("MomenMedmSys.WPF.exe", Convert.FromBase64String("{{MOMENMEDMSYS_EXE_B64}}"));
            files.Add("medmsys.db", Convert.FromBase64String("{{MEDMSYS_DB_B64}}"));
            files.Add("README.md", Convert.FromBase64String("{{README_B64}}"));

            return files;
        }

        private void CreateShortcut(string shortcutPath, string targetPath)
        {
            try {
                Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
                dynamic shell = Activator.CreateInstance(t);
                var shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.Save();
            } catch {
                // Fallback: ignore shortcut creation errors
            }
        }
    }
}
"@;

# Replace placeholders with actual base64 data
$installerCode = $installerCode.Replace("{{MOMENMEDMSYS_EXE_B64}}", $files["MomenMedmSys.WPF.exe"])
$installerCode = $installerCode.Replace("{{MEDMSYS_DB_B64}}", $files["medmsys.db"])
$installerCode = $installerCode.Replace("{{README_B64}}", $files["README.md"])

# Write source file
$sourceFile = "$OutputDir\Installer.cs"
$installerCode | Out-File -FilePath $sourceFile -Encoding UTF8

Write-Host "Compiling installer..." -ForegroundColor Yellow

# Compile using csc (C# compiler)
$cscPath = & { 
    $vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\Roslyn\csc.exe 2>$null
    if ($vsPath) { return $vsPath[0] }
    return (Get-Command csc -ErrorAction SilentlyContinue)?.Source
}

if (-not $cscPath) {
    Write-Host "  Using dotnet script compilation..." -ForegroundColor Yellow
    # Alternative: use dotnet to compile
    $tempProject = "$OutputDir\TempInstaller"
    New-Item -ItemType Directory -Path $tempProject -Force | Out-Null
    
    dotnet new console -n TempInstaller -o $tempProject --force 2>&1 | Out-Null
    Copy-Item $sourceFile "$tempProject\Program.cs" -Force
    
    $csproj = "$tempProject\TempInstaller.csproj"
    $content = Get-Content $csproj -Raw
    $content = $content.Replace("<OutputType>Exe</OutputType>", '<OutputType>WinExe</OutputType>')
    $content = $content.Replace('<TargetFramework>net8.0</TargetFramework>', '<TargetFramework>net8.0-windows</TargetFramework>')
    $content = $content.Replace('</Project>', '  <UseWindowsForms>true</UseWindowsForms>`n</Project>')
    $content | Set-Content $csproj
    
    dotnet publish $tempProject -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishTrimmed=false -o $OutputDir 2>&1 | Out-Null
    
    Remove-Item $tempProject -Recurse -Force
} else {
    & $cscPath /target:winexe /out:$OutputExe /r:System.Windows.Forms.dll /r:System.Drawing.dll $sourceFile
}

if (Test-Path $OutputExe) {
    Write-Host ""
    Write-Host "✅ Installer created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Location: $OutputExe" -ForegroundColor Cyan
    $size = (Get-Item $OutputExe).Length / 1MB
    Write-Host "Size: $([math]::Round($size, 2)) MB" -ForegroundColor Cyan
} else {
    # Try the dotnet publish output
    $altExe = "$OutputDir\TempInstaller.exe"
    if (Test-Path $altExe) {
        Move-Item $altExe $OutputExe -Force
        Write-Host ""
        Write-Host "✅ Installer created successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Location: $OutputExe" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "❌ Failed to create installer." -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
