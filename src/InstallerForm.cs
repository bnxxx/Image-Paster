using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClipboardPaster
{
    public class InstallerForm : Form
    {
        private Label lblStatus;
        private Button btnInstall;
        private Button btnUninstall;

        public InstallerForm()
        {
            InitializeComponent();
            UpdateStatusDisplay();
        }

        private void InitializeComponent()
        {
            this.Text = "Clipboard Image Paster - Setup & Configuration";
            this.Size = new Size(520, 390);
            this.MinimumSize = new Size(520, 390);
            this.MaximumSize = new Size(520, 390);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(28, 30, 36); // Sleek dark slate theme
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            // Header Panel
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 85,
                BackColor = Color.FromArgb(37, 40, 48)
            };

            Label lblTitle = new Label
            {
                Text = "Clipboard Image Paster",
                Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.FromArgb(235, 238, 245),
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label lblSubtitle = new Label
            {
                Text = "Paste clipboard images via right-click on folder icons or open folder backgrounds.",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.FromArgb(160, 166, 180),
                Location = new Point(22, 48),
                AutoSize = true
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);
            this.Controls.Add(headerPanel);

            // Status Card Panel
            Panel statusCard = new Panel
            {
                Location = new Point(25, 105),
                Size = new Size(455, 60),
                BackColor = Color.FromArgb(37, 40, 48)
            };

            Label lblStatusTitle = new Label
            {
                Text = "Context Menu Status:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.FromArgb(200, 205, 215),
                Location = new Point(15, 19),
                AutoSize = true
            };

            lblStatus = new Label
            {
                Text = "Checking...",
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                Location = new Point(175, 18),
                AutoSize = true
            };

            statusCard.Controls.Add(lblStatusTitle);
            statusCard.Controls.Add(lblStatus);
            this.Controls.Add(statusCard);

            // Buttons Panel
            btnInstall = new Button
            {
                Text = "Install to Context Menu",
                Location = new Point(25, 185),
                Size = new Size(220, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(50, 130, 246), // Vibrant blue
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                Cursor = Cursors.Hand
            };
            btnInstall.FlatAppearance.BorderSize = 0;
            btnInstall.Click += new EventHandler(BtnInstall_Click);

            btnUninstall = new Button
            {
                Text = "Uninstall",
                Location = new Point(260, 185),
                Size = new Size(220, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 64, 75), // Subtle gray
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                Cursor = Cursors.Hand
            };
            btnUninstall.FlatAppearance.BorderSize = 0;
            btnUninstall.Click += new EventHandler(BtnUninstall_Click);

            this.Controls.Add(btnInstall);
            this.Controls.Add(btnUninstall);

            // Instructions Box
            Panel tipsCard = new Panel
            {
                Location = new Point(25, 245),
                Size = new Size(455, 95),
                BackColor = Color.FromArgb(33, 36, 43)
            };

            Label lblTipsHeader = new Label
            {
                Text = "Quick Usage Guide:",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.FromArgb(180, 190, 205),
                Location = new Point(12, 10),
                AutoSize = true
            };

            Label lblTipsBody = new Label
            {
                Text = "• Right-click any folder icon OR inside any open folder -> \"Paste clipboard image\".\n" +
                       "• By default, images save instantly and silently as PNG with a timestamp.\n" +
                       "• Hold down SHIFT while clicking the option to open a custom Save As dialog.",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                ForeColor = Color.FromArgb(150, 158, 172),
                Location = new Point(12, 32),
                Size = new Size(430, 55)
            };

            tipsCard.Controls.Add(lblTipsHeader);
            tipsCard.Controls.Add(lblTipsBody);
            this.Controls.Add(tipsCard);
        }

        private void UpdateStatusDisplay()
        {
            bool installed = RegistryManager.IsInstalled();
            if (installed)
            {
                lblStatus.Text = "Installed (Ready to use)";
                lblStatus.ForeColor = Color.FromArgb(74, 222, 128); // Vibrant green
                btnInstall.Enabled = false;
                btnInstall.BackColor = Color.FromArgb(45, 50, 60);
                btnUninstall.Enabled = true;
                btnUninstall.BackColor = Color.FromArgb(239, 68, 68); // Soft red for uninstall
            }
            else
            {
                lblStatus.Text = "Not Installed";
                lblStatus.ForeColor = Color.FromArgb(251, 191, 36); // Warning amber
                btnInstall.Enabled = true;
                btnInstall.BackColor = Color.FromArgb(50, 130, 246);
                btnUninstall.Enabled = false;
                btnUninstall.BackColor = Color.FromArgb(45, 50, 60);
            }
        }

        private void BtnInstall_Click(object sender, EventArgs e)
        {
            string error;
            if (RegistryManager.Install(out error))
            {
                MessageBox.Show(
                    "Successfully installed right-click context menu entry!\n\n" +
                    "You can now right-click on any folder in Windows Explorer and select \"Paste clipboard image\".",
                    "Installation Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                UpdateStatusDisplay();
            }
            else
            {
                MessageBox.Show(
                    "Failed to install context menu entry:\n" + error,
                    "Installation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnUninstall_Click(object sender, EventArgs e)
        {
            string error;
            if (RegistryManager.Uninstall(out error))
            {
                MessageBox.Show(
                    "Successfully uninstalled right-click context menu entry.",
                    "Uninstalled",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                UpdateStatusDisplay();
            }
            else
            {
                MessageBox.Show(
                    "Failed to uninstall context menu entry:\n" + error,
                    "Uninstall Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
