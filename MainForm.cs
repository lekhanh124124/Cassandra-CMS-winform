// File: MainForm.cs
using Cassandra;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Collections.Generic;

namespace QuanLyBaoHanhSanPham
{
    public partial class MainForm : Form
    {
        public string CurrentUsername { get; set; } = "(chưa đăng nhập)";
        public string CurrentRole { get; set; } = "-";

        // Keep track of currently selected button
        private Button currentSelectedButton;

        // Ensure first-time init only happens once
        private bool _initialized = false;

        // Role-based access control
        private readonly Dictionary<string, HashSet<string>> rolePermissions = new Dictionary<string, HashSet<string>>
        {
            ["manager"] = new HashSet<string> { "dashboard", "tickets", "products", "productunits", "customers", "policies", "employees", "reports" },
            ["dispatcher"] = new HashSet<string> { "dashboard", "tickets", "products", "productunits", "customers", "policies", "reports" },
            ["receiver"] = new HashSet<string> { "dashboard", "tickets", "customers", "reports" },
            ["technician"] = new HashSet<string> { "dashboard", "tickets", "products", "productunits", "reports" },
            ["employee"] = new HashSet<string> { "dashboard", "tickets", "customers", "reports" }
        };

        public MainForm()
        {
            InitializeComponent();
            // Defer login and role-based initialization to OnShown()
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (_initialized) return;
            _initialized = true;

            // If role is not recognized (first app open), prompt for login
            if (!IsRoleRecognized(CurrentRole))
            {
                using (var login = new LoginForm())
                {
                    var result = login.ShowDialog(this);
                    if (result != DialogResult.OK)
                    {
                        // Exit app if user cancels at startup
                        this.Close();
                        return;
                    }

                    this.CurrentUsername = login.LoggedUsername;
                    this.CurrentRole = login.LoggedRole;
                }
            }

            // Now we have a role; apply UI and show dashboard
            ApplyUserInfoToHeader();
            ApplyRoleBasedAccess();
            ShowDashboard();
        }

        private bool IsRoleRecognized(string role)
        {
            var r = role?.ToLower();
            return !string.IsNullOrEmpty(r) && rolePermissions.ContainsKey(r);
        }

        private void ApplyUserInfoToHeader()
        {
            lblUser.Text = $"User: {CurrentUsername}";
            lblRole.Text = $"Role: {GetRoleDisplayName(CurrentRole)}";
        }

        private string GetRoleDisplayName(string role)
        {
            if (role == null)
                return "-";
            var r = role.ToLower();
            if (r == "manager") return "Quản lý";
            if (r == "dispatcher") return "Điều phối viên";
            if (r == "receiver") return "Nhân viên tiếp nhận";
            if (r == "technician") return "Kỹ thuật viên";
            if (r == "employee") return "Nhân viên";
            return role;
        }

        private void ApplyRoleBasedAccess()
        {
            var userRole = CurrentRole?.ToLower();
            if (string.IsNullOrEmpty(userRole) || !rolePermissions.ContainsKey(userRole))
            {
                // Hide all navigation buttons if role is not recognized
                HideAllNavigationButtons();
                return;
            }

            var allowedFeatures = rolePermissions[userRole];

            // Apply visibility and styling based on role permissions
            SetButtonAccess(btnDashboard, "dashboard", allowedFeatures);
            SetButtonAccess(btnTickets, "tickets", allowedFeatures);
            SetButtonAccess(btnProducts, "products", allowedFeatures);
            //SetButtonAccess(btnProductUnits, "productunits", allowedFeatures);
            SetButtonAccess(btnCustomers, "customers", allowedFeatures);
            SetButtonAccess(btnPolicies, "policies", allowedFeatures);
            SetButtonAccess(btnEmployees, "employees", allowedFeatures);
            SetButtonAccess(btnReports, "reports", allowedFeatures);

            // Update status with role-specific message
            statusText.Text = $"Hệ thống bảo hành - Vai trò: {GetRoleDisplayName(CurrentRole)}";
        }

        private void SetButtonAccess(Button btn, string feature, HashSet<string> allowedFeatures)
        {
            if (allowedFeatures.Contains(feature))
            {
                btn.Visible = true;
                btn.Enabled = true;
                // Add role-specific styling hints
                AddRoleSpecificStyling(btn, feature);
            }
            else
            {
                btn.Visible = false;
                btn.Enabled = false;
            }
        }

        private void AddRoleSpecificStyling(Button btn, string feature)
        {
            var userRole = CurrentRole?.ToLower();

            // Add visual indicators for primary features per role
            switch (userRole)
            {
                case "technician":
                    if (feature == "tickets" || feature == "products" || feature == "productunits")
                        btn.Font = new Font(btn.Font, FontStyle.Bold);
                    break;

                case "receiver":
                    if (feature == "tickets" || feature == "customers")
                        btn.Font = new Font(btn.Font, FontStyle.Bold);
                    break;

                case "dispatcher":
                    if (feature == "tickets" || feature == "policies")
                        btn.Font = new Font(btn.Font, FontStyle.Bold);
                    break;

                case "manager":
                    if (feature == "dashboard" || feature == "reports" || feature == "employees")
                        btn.Font = new Font(btn.Font, FontStyle.Bold);
                    break;
            }
        }

        private void HideAllNavigationButtons()
        {
            var buttons = new[] { btnDashboard, btnTickets, btnProducts,
                                  btnCustomers, btnPolicies, btnEmployees, btnReports };

            foreach (var btn in buttons)
            {
                btn.Visible = false;
                btn.Enabled = false;
            }
        }

        public bool HasPermission(string feature)
        {
            var userRole = CurrentRole?.ToLower();
            if (string.IsNullOrEmpty(userRole) || !rolePermissions.ContainsKey(userRole))
                return false;

            return rolePermissions[userRole].Contains(feature.ToLower());
        }

        private void ShowDashboard()
        {
            if (!HasPermission("dashboard"))
            {
                ShowAccessDenied("Dashboard");
                return;
            }

            SetSelectedButton(btnDashboard);
            lblAppTitle.Text = "Dashboard";
            //var f = new DashboardForm { CurrentUsername = this.CurrentUsername };
            var f = new DashboardForm ();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Dashboard";
        }

        private void ShowAccessDenied(string featureName)
        {
            contentHost.Controls.Clear();
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 249, 250) };

            var lblIcon = new Label
            {
                Text = "🚫",
                Font = new Font("Segoe UI", 48F),
                AutoSize = true,
                ForeColor = Color.FromArgb(220, 53, 69)
            };

            var lblMessage = new Label
            {
                Text = $"Không có quyền truy cập {featureName}",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(73, 80, 87)
            };

            var lblRole = new Label
            {
                Text = $"Vai trò hiện tại: {GetRoleDisplayName(CurrentRole)}",
                Font = new Font("Segoe UI", 12F),
                AutoSize = true,
                ForeColor = Color.FromArgb(108, 117, 125)
            };

            // Center the controls
            panel.Paint += (s, e) =>
            {
                int centerX = panel.Width / 2;
                int centerY = panel.Height / 2;

                lblIcon.Location = new Point(centerX - lblIcon.Width / 2, centerY - 80);
                lblMessage.Location = new Point(centerX - lblMessage.Width / 2, centerY - 20);
                lblRole.Location = new Point(centerX - lblRole.Width / 2, centerY + 20);
            };

            panel.Controls.Add(lblIcon);
            panel.Controls.Add(lblMessage);
            panel.Controls.Add(lblRole);
            contentHost.Controls.Add(panel);

            statusText.Text = $"Truy cập bị từ chối: {featureName}";
        }

        private void SetSelectedButton(Button selectedButton)
        {
            // Reset all buttons to default state
            ResetAllButtons();

            // Set the selected button appearance
            selectedButton.BackColor = Color.FromArgb(0, 123, 255);
            selectedButton.ForeColor = Color.White;

            // Update current selected button
            currentSelectedButton = selectedButton;
        }

        private void ResetAllButtons()
        {
            var buttons = new[] { btnDashboard, btnTickets, btnProducts,
                                  btnCustomers, btnPolicies, btnEmployees, btnReports };

            foreach (var btn in buttons)
            {
                if (btn.Visible && btn.Enabled)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.FromArgb(173, 181, 189);
                }
            }
        }

        // Nav handlers with permission checks
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            if (!HasPermission("dashboard")) { ShowAccessDenied("Dashboard"); return; }

            SetSelectedButton(btnDashboard);
            lblAppTitle.Text = "Dashboard";
            //var f = new DashboardForm { CurrentUsername = this.CurrentUsername };
            var f = new DashboardForm ();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Dashboard";
        }

        private void btnTickets_Click(object sender, EventArgs e)
        {
            if (!HasPermission("tickets")) { ShowAccessDenied("Support Tickets"); return; }

            SetSelectedButton(btnTickets);
            lblAppTitle.Text = "Support Tickets";
            var f = new TicketsForm();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Support Tickets";
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            if (!HasPermission("products")) { ShowAccessDenied("Products"); return; }

            SetSelectedButton(btnProducts);
            lblAppTitle.Text = "Products";
            var f = new ProductsForm();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Products";
        }

        //private void btnProductUnits_Click(object sender, EventArgs e)
        //{
        //    if (!HasPermission("productunits")) { ShowAccessDenied("Product Units"); return; }

        //    SetSelectedButton(btnProductUnits);
        //    lblAppTitle.Text = "Product Units";
        //    var f = new ProductUnitsForm();
        //    ShowInContent(f);
        //    statusText.Text = "Đang hiển thị Product Units";
        //}

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            if (!HasPermission("customers")) { ShowAccessDenied("Customers"); return; }

            SetSelectedButton(btnCustomers);
            lblAppTitle.Text = "Customers";
            var f = new CustomersForm();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Customers";
        }

        private void btnPolicies_Click(object sender, EventArgs e)
        {
            if (!HasPermission("policies")) { ShowAccessDenied("Warranty Policies"); return; }

            SetSelectedButton(btnPolicies);
            lblAppTitle.Text = "Warranty Policies";
            var f = new WarrantyPolicyForm();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Warranty Policies";
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            if (!HasPermission("employees")) { ShowAccessDenied("Employees"); return; }

            SetSelectedButton(btnEmployees);
            lblAppTitle.Text = "Employees";
            var f = new EmployeesForm();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Employees";
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            if (!HasPermission("reports")) { ShowAccessDenied("Reports"); return; }

            SetSelectedButton(btnReports);
            lblAppTitle.Text = "Reports";
            var f = new ReportsForm();
            ShowInContent(f);
            statusText.Text = "Đang hiển thị Reports";
        }

        private void LoadFeatureStub(string name)
        {
            contentHost.Controls.Clear();
            var lbl = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11F),
                Text = $"[{name}] đang phát triển. Nhúng UserControl tương ứng tại đây.",
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(16, 16)
            };
            contentHost.Controls.Add(lbl);
            statusText.Text = $"Đã chọn: {name}";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var login = new LoginForm())
            {
                var result = login.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.CurrentUsername = login.LoggedUsername;
                    this.CurrentRole = login.LoggedRole;
                    ApplyUserInfoToHeader();
                    ApplyRoleBasedAccess(); // Apply new role permissions
                    this.Show();
                    // Show Dashboard instead of welcome message after login
                    ShowDashboard();
                }
                else
                {
                    this.Close();
                }
            }
        }

        // ========= NEW: Đổi mật khẩu =========
        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentUsername) || CurrentUsername == "(chưa đăng nhập)")
            {
                MessageBox.Show("Bạn chưa đăng nhập.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new ChangePasswordDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var oldPwd = dlg.OldPassword;
                var newPwd = dlg.NewPassword;
                var newPwd2 = dlg.ConfirmPassword;

                if (string.IsNullOrWhiteSpace(oldPwd) || string.IsNullOrWhiteSpace(newPwd))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ mật khẩu.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newPwd.Length < 6)
                {
                    MessageBox.Show("Mật khẩu mới tối thiểu 6 ký tự.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (newPwd != newPwd2)
                {
                    MessageBox.Show("Xác nhận mật khẩu không khớp.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var db = Program.DbSession;
                    if (db == null) { MessageBox.Show("Chưa kết nối cơ sở dữ liệu."); return; }

                    // Lấy hash hiện tại theo username từ bảng users (PK là user_id => cần ALLOW FILTERING)
                    var psGet = db.Prepare("SELECT user_id, password_hash FROM users WHERE username = ? ALLOW FILTERING;");
                    var row = db.Execute(psGet.Bind(CurrentUsername)).FirstOrDefault();
                    if (row == null)
                    {
                        MessageBox.Show("Không tìm thấy tài khoản người dùng.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var userId = row.GetValue<string>("user_id");
                    var currentHash = row.GetValue<string>("password_hash");

                    // Xác thực mật khẩu cũ
                    if (!BCryptNet.Verify(oldPwd, currentHash))
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Tạo hash mới & cập nhật ở bảng users
                    var newHash = BCryptNet.HashPassword(newPwd);
                    var psU1 = db.Prepare("UPDATE users SET password_hash = ? WHERE user_id = ?;");
                    db.Execute(psU1.Bind(newHash, userId));

                    MessageBox.Show("Đổi mật khẩu thành công.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đổi mật khẩu: " + ex.Message, "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowInContent(Form child)
        {
            foreach (Control c in contentHost.Controls) c.Dispose();
            contentHost.Controls.Clear();

            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Dock = DockStyle.Fill;

            contentHost.Controls.Add(child);
            child.Show();
        }

        // ========= NEW: Dialog nhỏ nhập mật khẩu =========
        private sealed class ChangePasswordDialog : Form
        {
            private TextBox txtOld;
            private TextBox txtNew;
            private TextBox txtConfirm;
            private Button btnOk;
            private Button btnCancel;

            public string OldPassword => txtOld.Text;
            public string NewPassword => txtNew.Text;
            public string ConfirmPassword => txtConfirm.Text;

            public ChangePasswordDialog()
            {
                this.Text = "Đổi mật khẩu";
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ClientSize = new Size(380, 200);
                this.Font = new Font("Segoe UI", 9.5F);

                var lblOld = new Label { Text = "Mật khẩu cũ", AutoSize = true, Location = new Point(16, 20) };
                var lblNew = new Label { Text = "Mật khẩu mới", AutoSize = true, Location = new Point(16, 60) };
                var lblConfirm = new Label { Text = "Xác nhận", AutoSize = true, Location = new Point(16, 100) };

                txtOld = new TextBox { Location = new Point(120, 16), Width = 230, UseSystemPasswordChar = true };
                txtNew = new TextBox { Location = new Point(120, 56), Width = 230, UseSystemPasswordChar = true };
                txtConfirm = new TextBox { Location = new Point(120, 96), Width = 230, UseSystemPasswordChar = true };

                btnOk = new Button { Text = "OK", Width = 90, Location = new Point(160, 140) };
                btnCancel = new Button { Text = "Hủy", Width = 90, Location = new Point(260, 140) };

                btnOk.Click += (s, e) => { this.DialogResult = DialogResult.OK; };
                btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

                this.Controls.Add(lblOld);
                this.Controls.Add(lblNew);
                this.Controls.Add(lblConfirm);
                this.Controls.Add(txtOld);
                this.Controls.Add(txtNew);
                this.Controls.Add(txtConfirm);
                this.Controls.Add(btnOk);
                this.Controls.Add(btnCancel);
            }
        }
    }
}