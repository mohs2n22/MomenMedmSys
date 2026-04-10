using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MomenMedmSys.Installer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
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
            LoadLicense();
        }

        private void InitializeComponents()
        {
            Text = "MomenMedmSys Setup - Medical Equipment Management System v1.0.0";
            Size = new Size(620, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 240);
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // License Panel
            pnlLicense = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };

            var lblTitle = CreateLabel("LICENSE AGREEMENT", 0, 0, 14, FontStyle.Bold);
            pnlLicense.Controls.Add(lblTitle);

            txtLicense = new TextBox
            {
                Location = new Point(0, 30),
                Size = new Size(560, 300),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9),
                BackColor = Color.White
            };
            pnlLicense.Controls.Add(txtLicense);

            rbAccept = new RadioButton
            {
                Text = "I &accept the terms of the License Agreement",
                Location = new Point(0, 340),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlLicense.Controls.Add(rbAccept);

            rbDecline = new RadioButton
            {
                Text = "I do &not accept",
                Location = new Point(0, 365),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlLicense.Controls.Add(rbDecline);

            // Options Panel
            pnlOptions = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), Visible = false };

            var lblInstallTitle = CreateLabel("Installation Options", 0, 0, 14, FontStyle.Bold);
            pnlOptions.Controls.Add(lblInstallTitle);

            var lblPath = CreateLabel("Install &to:", 0, 40, 9.5f, FontStyle.Regular);
            pnlOptions.Controls.Add(lblPath);

            txtPath = new TextBox
            {
                Location = new Point(80, 38),
                Size = new Size(390, 23),
                Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MomenMedmSys"),
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlOptions.Controls.Add(txtPath);

            btnBrowse = new Button
            {
                Text = "Browse...",
                Location = new Point(480, 36),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 9f)
            };
            btnBrowse.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog
                {
                    Description = "Select installation folder",
                    SelectedPath = txtPath.Text,
                    UseDescriptionForTitle = true
                };
                if (fbd.ShowDialog() == DialogResult.OK) txtPath.Text = fbd.SelectedPath;
            };
            pnlOptions.Controls.Add(btnBrowse);

            chkDesktop = new CheckBox
            {
                Text = "Create &desktop shortcut",
                Location = new Point(0, 80),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlOptions.Controls.Add(chkDesktop);

            chkStartMenu = new CheckBox
            {
                Text = "Create &Start Menu shortcut",
                Location = new Point(0, 105),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlOptions.Controls.Add(chkStartMenu);

            // Progress Panel
            pnlProgress = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20), Visible = false };

            lblStatus = new Label
            {
                Location = new Point(0, 50),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f)
            };
            pnlProgress.Controls.Add(lblStatus);

            progressBar = new ProgressBar
            {
                Location = new Point(0, 85),
                Size = new Size(560, 25),
                Style = ProgressBarStyle.Continuous
            };
            pnlProgress.Controls.Add(progressBar);

            // Buttons
            btnInstall = new Button
            {
                Text = "&Install",
                Location = new Point(390, 440),
                Size = new Size(100, 32),
                DialogResult = DialogResult.None,
                FlatStyle = FlatStyle.Standard,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            btnInstall.Click += BtnInstall_Click;
            Controls.Add(btnInstall);

            btnCancel = new Button
            {
                Text = "&Cancel",
                Location = new Point(500, 440),
                Size = new Size(100, 32),
                DialogResult = DialogResult.Cancel,
                FlatStyle = FlatStyle.Standard,
                Font = new Font("Segoe UI", 9.5f)
            };
            btnCancel.Click += (s, e) => Application.Exit();
            Controls.Add(btnCancel);

            Controls.Add(pnlLicense);
            Controls.Add(pnlOptions);
            Controls.Add(pnlProgress);
        }

        private Label CreateLabel(string text, int x, int y, float fontSize, FontStyle style)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", fontSize, style)
            };
        }

        private void LoadLicense()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MomenMedmSys.Installer.LICENSE.txt");
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                txtLicense.Text = reader.ReadToEnd();
            }
        }

        private async void BtnInstall_Click(object? sender, EventArgs e)
        {
            if (!rbAccept.Checked)
            {
                MessageBox.Show("You must accept the license agreement to continue.", "License Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string installPath = txtPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(installPath))
            {
                MessageBox.Show("Please specify an installation path.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (!Directory.Exists(installPath)) Directory.CreateDirectory(installPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot create directory:\n{ex.Message}", "Error",
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
            await ExtractFilesAsync(installPath);
        }

        private async Task ExtractFilesAsync(string installPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = new[]
            {
                ("MomenMedmSys.Installer.MomenMedmSys.WPF.exe", "MomenMedmSys.WPF.exe"),
                ("MomenMedmSys.Installer.MomenMedmSys.db", "MomenMedmSys.db"),
                ("MomenMedmSys.Installer.README.md", "README.md")
            };

            for (int i = 0; i < resources.Length; i++)
            {
                var (resName, fileName) = resources[i];
                lblStatus.Text = $"Extracting {fileName}... ({i + 1}/{resources.Length})";
                progressBar.Value = (i + 1) * 30;
                await Task.Delay(100);

                using var stream = assembly.GetManifestResourceStream(resName);
                if (stream != null)
                {
                    using var fileStream = new FileStream(Path.Combine(installPath, fileName), FileMode.Create);
                    await stream.CopyToAsync(fileStream);
                }
            }

            // Create shortcuts
            lblStatus.Text = "Creating shortcuts...";
            progressBar.Value = 90;
            await Task.Delay(100);

            string exePath = Path.Combine(installPath, "MomenMedmSys.WPF.exe");

            if (chkDesktop.Checked)
            {
                CreateShortcut(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MomenMedmSys.lnk"),
                    exePath, installPath);
            }

            if (chkStartMenu.Checked)
            {
                string startMenuPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs", "MomenMedmSys");
                Directory.CreateDirectory(startMenuPath);
                CreateShortcut(Path.Combine(startMenuPath, "MomenMedmSys.lnk"), exePath, installPath);
            }

            // Create uninstaller
            lblStatus.Text = "Creating uninstaller...";
            progressBar.Value = 95;
            await Task.Delay(100);

            string uninstallBat = $@"@echo off
echo Uninstalling MomenMedmSys...
set /p KEEP_DB=Keep database file? (Y/N): 
if /I not ""%KEEP_DB%""==""Y"" (
  del /f /q ""{installPath}\MomenMedmSys.db*"" 2>nul
)
del /f /q ""{installPath}\MomenMedmSys.WPF.exe""
del /f /q ""{installPath}\README.md""
del /f /q ""%~dp0%~nx0""
for /f ""tokens=*"" %%a in ('powershell -Command ""[Environment]::GetFolderPath('Desktop')""') do (
  if exist ""%%a\MomenMedmSys.lnk"" del /f /q ""%%a\MomenMedmSys.lnk""
)
if exist ""%APPDATA%\Microsoft\Windows\Start Menu\Programs\MomenMedmSys"" rmdir /s /q ""%APPDATA%\Microsoft\Windows\Start Menu\Programs\MomenMedmSys""
rmdir /s /q ""{installPath}"" 2>nul
echo MomenMedmSys has been uninstalled.
pause
";
            File.WriteAllText(Path.Combine(installPath, "uninstall.bat"), uninstallBat);

            // Registry entry
            try
            {
                Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys")
                    ?.SetValue("DisplayName", "MomenMedmSys");
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys", true)
                    ?.SetValue("UninstallString", $@"""{installPath}\uninstall.bat""");
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys", true)
                    ?.SetValue("DisplayIcon", $@"""{exePath}""");
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\MomenMedmSys", true)
                    ?.SetValue("DisplayVersion", "1.0.0");
            }
            catch { }

            progressBar.Value = 100;
            lblStatus.Text = "Installation complete!";

            MessageBox.Show($"MomenMedmSys has been successfully installed!\n\nInstallation Path: {installPath}\n\nClick OK to launch the application.",
                "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Launch application
            try { Process.Start(new ProcessStartInfo(exePath) { UseShellExecute = true }); } catch { }

            Application.Exit();
        }

        private void CreateShortcut(string shortcutPath, string targetPath, string workingDir)
        {
            try
            {
                Type? t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
                if (t == null) return;
                dynamic? shell = Activator.CreateInstance(t);
                if (shell == null) return;
                var shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = workingDir;
                shortcut.Save();
            }
            catch { }
        }
    }
}
