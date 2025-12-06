// File: LoginForm.Designer.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelRoot;
        private Panel panelCard;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private CheckBox chkRemember;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblError;
        private PictureBox picLogo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            { components.Dispose(); }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.panelRoot = new Panel();
            this.panelCard = new Panel();
            this.picLogo = new PictureBox();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.lblUsername = new Label();
            this.lblPassword = new Label();
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.chkShowPassword = new CheckBox();
            this.chkRemember = new CheckBox();
            this.btnLogin = new Button();
            this.btnCancel = new Button();
            this.lblError = new Label();

            this.SuspendLayout();

            // LoginForm
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Text = "Login - Warranty Management System";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(520, 440);
            this.BackColor = Color.FromArgb(0, 123, 255);

            // panelRoot
            this.panelRoot.Dock = DockStyle.Fill;
            this.panelRoot.Padding = new Padding(40);
            this.panelRoot.BackColor = Color.FromArgb(0, 123, 255);
            this.Controls.Add(this.panelRoot);

            // panelCard
            this.panelCard.BackColor = Color.White;
            this.panelCard.Dock = DockStyle.Fill;
            this.panelCard.Padding = new Padding(40);
            this.panelCard.Location = new Point(40, 40);
            this.panelCard.Size = new Size(440, 360);

            // picLogo
            this.picLogo.BackColor = Color.FromArgb(0, 123, 255);
            this.picLogo.Location = new Point(190, 20);
            this.picLogo.Size = new Size(60, 60);
            this.picLogo.SizeMode = PictureBoxSizeMode.StretchImage;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Text = "Welcome Back";
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(33, 37, 41);
            this.lblTitle.Location = new Point(40, 100);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // lblSubtitle
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Text = "Sign in to your warranty management account";
            this.lblSubtitle.Font = new Font("Segoe UI", 10F);
            this.lblSubtitle.ForeColor = Color.FromArgb(108, 117, 125);
            this.lblSubtitle.Location = new Point(40, 135);

            // lblUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Text = "Username";
            this.lblUsername.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblUsername.ForeColor = Color.FromArgb(73, 80, 87);
            this.lblUsername.Location = new Point(40, 175);

            // txtUsername
            this.txtUsername.BorderStyle = BorderStyle.None;
            this.txtUsername.Font = new Font("Segoe UI", 12F);
            this.txtUsername.ForeColor = Color.FromArgb(73, 80, 87);
            this.txtUsername.Location = new Point(40, 200);
            this.txtUsername.Width = 360;
            this.txtUsername.Height = 28;
            this.txtUsername.TabIndex = 0;
            this.txtUsername.BackColor = Color.FromArgb(248, 249, 250);

            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Text = "Password";
            this.lblPassword.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblPassword.ForeColor = Color.FromArgb(73, 80, 87);
            this.lblPassword.Location = new Point(40, 240);

            // txtPassword
            this.txtPassword.BorderStyle = BorderStyle.None;
            this.txtPassword.Font = new Font("Segoe UI", 12F);
            this.txtPassword.ForeColor = Color.FromArgb(73, 80, 87);
            this.txtPassword.Location = new Point(40, 265);
            this.txtPassword.Width = 360;
            this.txtPassword.Height = 28;
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.TabIndex = 1;
            this.txtPassword.BackColor = Color.FromArgb(248, 249, 250);

            // chkShowPassword
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Text = "Show password";
            this.chkShowPassword.Font = new Font("Segoe UI", 9F);
            this.chkShowPassword.ForeColor = Color.FromArgb(108, 117, 125);
            this.chkShowPassword.Location = new Point(40, 305);
            this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);

            // chkRemember
            this.chkRemember.AutoSize = true;
            this.chkRemember.Text = "Remember me";
            this.chkRemember.Font = new Font("Segoe UI", 9F);
            this.chkRemember.ForeColor = Color.FromArgb(108, 117, 125);
            this.chkRemember.Location = new Point(220, 305);

            // lblError
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = Color.FromArgb(220, 53, 69);
            this.lblError.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            this.lblError.Location = new Point(40, 330);
            this.lblError.Text = "";
            this.lblError.Visible = false;

            // btnLogin
            this.btnLogin.Text = "Sign In";
            this.btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.BackColor = Color.FromArgb(0, 123, 255);
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Size = new Size(180, 44);
            this.btnLogin.Location = new Point(220, 360);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // btnCancel
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Font = new Font("Segoe UI", 12F);
            this.btnCancel.ForeColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.BackColor = Color.Transparent;
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderColor = Color.FromArgb(108, 117, 125);
            this.btnCancel.Size = new Size(120, 44);
            this.btnCancel.Location = new Point(40, 360);
            this.btnCancel.DialogResult = DialogResult.Cancel;

            // Add to panels
            this.panelCard.Controls.Add(this.picLogo);
            this.panelCard.Controls.Add(this.lblTitle);
            this.panelCard.Controls.Add(this.lblSubtitle);
            this.panelCard.Controls.Add(this.lblUsername);
            this.panelCard.Controls.Add(this.txtUsername);
            this.panelCard.Controls.Add(this.lblPassword);
            this.panelCard.Controls.Add(this.txtPassword);
            this.panelCard.Controls.Add(this.chkShowPassword);
            this.panelCard.Controls.Add(this.chkRemember);
            this.panelCard.Controls.Add(this.lblError);
            this.panelCard.Controls.Add(this.btnCancel);
            this.panelCard.Controls.Add(this.btnLogin);

            this.panelRoot.Controls.Add(this.panelCard);

            // Accept/Cancel buttons
            this.AcceptButton = this.btnLogin;
            this.CancelButton = this.btnCancel;

            this.ResumeLayout(false);
        }
        #endregion
    }
}
