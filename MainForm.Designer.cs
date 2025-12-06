// File: MainForm.Designer.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel sidebar;
        private Panel header;
        private Panel panelHeaderRight;
        private Panel contentHost;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusText;

        private Button btnDashboard;
        private Button btnTickets;
        private Button btnProducts;
        //private Button btnProductUnits;
        private Button btnCustomers;
        private Button btnPolicies;
        private Button btnEmployees;
        private Button btnReports;

        private Label lblAppTitle;
        private Label lblUser;
        private Label lblRole;
        private Button btnLogout;
        private Button btnChangePassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            { components.Dispose(); }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.sidebar = new System.Windows.Forms.Panel();
            this.btnReports = new System.Windows.Forms.Button();
            this.btnEmployees = new System.Windows.Forms.Button();
            this.btnPolicies = new System.Windows.Forms.Button();
            this.btnCustomers = new System.Windows.Forms.Button();
            //this.btnProductUnits = new System.Windows.Forms.Button();
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnTickets = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.header = new System.Windows.Forms.Panel();
            this.panelHeaderRight = new System.Windows.Forms.Panel();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblRole = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.contentHost = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.sidebar.SuspendLayout();
            this.header.SuspendLayout();
            this.panelHeaderRight.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // sidebar
            // 
            this.sidebar.BackColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.sidebar.Controls.Add(this.btnReports);
            this.sidebar.Controls.Add(this.btnEmployees);
            this.sidebar.Controls.Add(this.btnPolicies);
            this.sidebar.Controls.Add(this.btnCustomers);
            //this.sidebar.Controls.Add(this.btnProductUnits);
            this.sidebar.Controls.Add(this.btnProducts);
            this.sidebar.Controls.Add(this.btnTickets);
            this.sidebar.Controls.Add(this.btnDashboard);
            this.sidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.sidebar.Location = new System.Drawing.Point(0, 0);
            this.sidebar.Name = "sidebar";
            this.sidebar.Padding = new System.Windows.Forms.Padding(0, 20, 0, 20);
            this.sidebar.Size = new System.Drawing.Size(260, 627);
            this.sidebar.TabIndex = 2;

            // Sidebar buttons with modern styling
            var buttonFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            var buttonHeight = 48;
            var buttonPadding = new Padding(20, 0, 20, 0);

            // 
            // btnDashboard
            // 
            this.btnDashboard.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnDashboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 105, 217);
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.Font = buttonFont;
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Location = new System.Drawing.Point(0, 20);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Padding = buttonPadding;
            this.btnDashboard.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnDashboard.TabIndex = 7;
            this.btnDashboard.Text = "📊 Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = false;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);

            // 
            // btnTickets
            // 
            this.btnTickets.BackColor = System.Drawing.Color.Transparent;
            this.btnTickets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTickets.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTickets.FlatAppearance.BorderSize = 0;
            this.btnTickets.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.btnTickets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTickets.Font = buttonFont;
            this.btnTickets.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            this.btnTickets.Location = new System.Drawing.Point(0, 68);
            this.btnTickets.Name = "btnTickets";
            this.btnTickets.Padding = buttonPadding;
            this.btnTickets.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnTickets.TabIndex = 6;
            this.btnTickets.Text = "🎫 Support Tickets";
            this.btnTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTickets.UseVisualStyleBackColor = false;
            this.btnTickets.Click += new System.EventHandler(this.btnTickets_Click);

            // 
            // btnProducts
            // 
            this.btnProducts.BackColor = System.Drawing.Color.Transparent;
            this.btnProducts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProducts.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProducts.FlatAppearance.BorderSize = 0;
            this.btnProducts.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.btnProducts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProducts.Font = buttonFont;
            this.btnProducts.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            this.btnProducts.Location = new System.Drawing.Point(0, 116);
            this.btnProducts.Name = "btnProducts";
            this.btnProducts.Padding = buttonPadding;
            this.btnProducts.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnProducts.TabIndex = 5;
            this.btnProducts.Text = "📱 Products";
            this.btnProducts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProducts.UseVisualStyleBackColor = false;
            this.btnProducts.Click += new System.EventHandler(this.btnProducts_Click);

            // 
            // btnProductUnits
            // 
            //this.btnProductUnits.BackColor = System.Drawing.Color.Transparent;
            //this.btnProductUnits.Cursor = System.Windows.Forms.Cursors.Hand;
            //this.btnProductUnits.Dock = System.Windows.Forms.DockStyle.Top;
            //this.btnProductUnits.FlatAppearance.BorderSize = 0;
            //this.btnProductUnits.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            //this.btnProductUnits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            //this.btnProductUnits.Font = buttonFont;
            //this.btnProductUnits.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            //this.btnProductUnits.Location = new System.Drawing.Point(0, 164);
            //this.btnProductUnits.Name = "btnProductUnits";
            //this.btnProductUnits.Padding = buttonPadding;
            //this.btnProductUnits.Size = new System.Drawing.Size(260, buttonHeight);
            //this.btnProductUnits.TabIndex = 4;
            //this.btnProductUnits.Text = "🔢 Product Units";
            //this.btnProductUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //this.btnProductUnits.UseVisualStyleBackColor = false;
            //this.btnProductUnits.Click += new System.EventHandler(this.btnProductUnits_Click);

            // 
            // btnCustomers
            // 
            this.btnCustomers.BackColor = System.Drawing.Color.Transparent;
            this.btnCustomers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustomers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCustomers.FlatAppearance.BorderSize = 0;
            this.btnCustomers.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.btnCustomers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomers.Font = buttonFont;
            this.btnCustomers.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            this.btnCustomers.Location = new System.Drawing.Point(0, 212);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Padding = buttonPadding;
            this.btnCustomers.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnCustomers.TabIndex = 3;
            this.btnCustomers.Text = "👥 Customers";
            this.btnCustomers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCustomers.UseVisualStyleBackColor = false;
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);

            // 
            // btnPolicies
            // 
            this.btnPolicies.BackColor = System.Drawing.Color.Transparent;
            this.btnPolicies.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPolicies.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPolicies.FlatAppearance.BorderSize = 0;
            this.btnPolicies.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.btnPolicies.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPolicies.Font = buttonFont;
            this.btnPolicies.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            this.btnPolicies.Location = new System.Drawing.Point(0, 260);
            this.btnPolicies.Name = "btnPolicies";
            this.btnPolicies.Padding = buttonPadding;
            this.btnPolicies.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnPolicies.TabIndex = 2;
            this.btnPolicies.Text = "📋 Warranty Policies";
            this.btnPolicies.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPolicies.UseVisualStyleBackColor = false;
            this.btnPolicies.Click += new System.EventHandler(this.btnPolicies_Click);

            // 
            // btnEmployees
            // 
            this.btnEmployees.BackColor = System.Drawing.Color.Transparent;
            this.btnEmployees.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEmployees.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEmployees.FlatAppearance.BorderSize = 0;
            this.btnEmployees.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.btnEmployees.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEmployees.Font = buttonFont;
            this.btnEmployees.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            this.btnEmployees.Location = new System.Drawing.Point(0, 308);
            this.btnEmployees.Name = "btnEmployees";
            this.btnEmployees.Padding = buttonPadding;
            this.btnEmployees.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnEmployees.TabIndex = 1;
            this.btnEmployees.Text = "👨‍💼 Employees";
            this.btnEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEmployees.UseVisualStyleBackColor = false;
            this.btnEmployees.Click += new System.EventHandler(this.btnEmployees_Click);

            // 
            // btnReports
            // 
            this.btnReports.BackColor = System.Drawing.Color.Transparent;
            this.btnReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReports.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnReports.FlatAppearance.BorderSize = 0;
            this.btnReports.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(52, 58, 64);
            this.btnReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReports.Font = buttonFont;
            this.btnReports.ForeColor = System.Drawing.Color.FromArgb(173, 181, 189);
            this.btnReports.Location = new System.Drawing.Point(0, 356);
            this.btnReports.Name = "btnReports";
            this.btnReports.Padding = buttonPadding;
            this.btnReports.Size = new System.Drawing.Size(260, buttonHeight);
            this.btnReports.TabIndex = 0;
            this.btnReports.Text = "📊 Reports";
            this.btnReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReports.UseVisualStyleBackColor = false;
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);

            // 
            // header
            // 
            this.header.BackColor = System.Drawing.Color.White;
            this.header.Controls.Add(this.panelHeaderRight);
            this.header.Controls.Add(this.lblAppTitle);
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Location = new System.Drawing.Point(260, 0);
            this.header.Name = "header";
            this.header.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.header.Size = new System.Drawing.Size(822, 80);
            this.header.TabIndex = 1;

            // 
            // panelHeaderRight
            // 
            this.panelHeaderRight.Controls.Add(this.lblUser);
            this.panelHeaderRight.Controls.Add(this.lblRole);
            this.panelHeaderRight.Controls.Add(this.btnLogout);
            this.panelHeaderRight.Controls.Add(this.btnChangePassword);
            this.panelHeaderRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelHeaderRight.Location = new System.Drawing.Point(298, 16);
            this.panelHeaderRight.Name = "panelHeaderRight";
            this.panelHeaderRight.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.panelHeaderRight.Size = new System.Drawing.Size(500, 48);
            this.panelHeaderRight.TabIndex = 0;

            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.lblUser.Location = new System.Drawing.Point(12, 8);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(130, 20);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "User: Not logged in";

            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblRole.ForeColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.lblRole.Location = new System.Drawing.Point(12, 28);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(45, 15);
            this.lblRole.TabIndex = 1;
            this.lblRole.Text = "Role: -";

            // 
            // btnChangePassword
            // 
            this.btnChangePassword.BackColor = System.Drawing.Color.FromArgb(255, 193, 7);
            this.btnChangePassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangePassword.FlatAppearance.BorderSize = 0;
            this.btnChangePassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangePassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnChangePassword.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.btnChangePassword.Location = new System.Drawing.Point(260, 8);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(120, 32);
            this.btnChangePassword.TabIndex = 3;
            this.btnChangePassword.Text = "Change Password";
            this.btnChangePassword.UseVisualStyleBackColor = false;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);

            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(388, 8);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(100, 32);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);

            // 
            // lblAppTitle
            // 
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.FromArgb(33, 37, 41);
            this.lblAppTitle.Location = new System.Drawing.Point(24, 24);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(128, 32);
            this.lblAppTitle.TabIndex = 1;
            this.lblAppTitle.Text = "Dashboard";

            // 
            // contentHost
            // 
            this.contentHost.BackColor = System.Drawing.Color.White;
            this.contentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentHost.Location = new System.Drawing.Point(260, 80);
            this.contentHost.Name = "contentHost";
            this.contentHost.Padding = new System.Windows.Forms.Padding(0);
            this.contentHost.Size = new System.Drawing.Size(822, 521);
            this.contentHost.TabIndex = 0;

            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText});
            this.statusStrip.Location = new System.Drawing.Point(0, 601);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1082, 26);
            this.statusStrip.TabIndex = 3;

            // 
            // statusText
            // 
            this.statusText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusText.ForeColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(1067, 21);
            this.statusText.Spring = true;
            this.statusText.Text = "Ready - Warranty Management System";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1082, 627);
            this.Controls.Add(this.contentHost);
            this.Controls.Add(this.header);
            this.Controls.Add(this.sidebar);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Warranty Management System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.sidebar.ResumeLayout(false);
            this.header.ResumeLayout(false);
            this.header.PerformLayout();
            this.panelHeaderRight.ResumeLayout(false);
            this.panelHeaderRight.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
    }
}