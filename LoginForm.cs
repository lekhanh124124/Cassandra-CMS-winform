// File: LoginForm.cs
using Cassandra;
using System;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class LoginForm : Form
    {
        public string LoggedUsername { get; private set; } = "";
        public string LoggedRole { get; private set; } = "-";
        public string LoggedUserId { get; private set; } = "";
        public string LoggedEmployeeId { get; private set; } = "";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            TryLogin();
        }

        private void TryLogin()
        {
            lblError.Visible = false;

            var u = txtUsername.Text.Trim();
            var p = txtPassword.Text;

            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
            {
                lblError.Text = "Vui lòng nhập đầy đủ username và password.";
                lblError.Visible = true;
                return;
            }

            try
            {
                var db = Program.DbSession;
                if (db == null)
                {
                    lblError.Text = "Chưa kết nối được cơ sở dữ liệu.";
                    lblError.Visible = true;
                    return;
                }

                Row row = null;

                try
                {
                    // Cách tốt nhất: có secondary index username (users_username_idx)
                    var ps = db.Prepare("SELECT user_id, password_hash, role, employee_id FROM users WHERE username = ? LIMIT 1;");
                    var rs = db.Execute(ps.Bind(u));
                    row = rs.SingleOrDefault();
                }
                catch (InvalidQueryException)
                {
                    // Fallback nếu không có index: dùng ALLOW FILTERING (chậm nhưng đảm bảo đăng nhập được)
                    var ps = db.Prepare(@"SELECT user_id, password_hash, role, employee_id 
                                          FROM users WHERE username = ? LIMIT 1 ALLOW FILTERING;");
                    var rs = db.Execute(ps.Bind(u));
                    row = rs.SingleOrDefault();
                }

                if (row == null)
                {
                    lblError.Text = "Sai username hoặc password.";
                    lblError.Visible = true;
                    return;
                }

                var hash = row.GetValue<string>("password_hash");
                var ok = BCrypt.Net.BCrypt.Verify(p, hash);
                if (!ok)
                {
                    lblError.Text = "Sai username hoặc password.";
                    lblError.Visible = true;
                    return;
                }

                LoggedUsername = u;
                LoggedRole = row.GetValue<string>("role") ?? "-";
                LoggedUserId = row.GetValue<string>("user_id");
                LoggedEmployeeId = row.GetValue<string>("employee_id");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = "Đăng nhập lỗi: " + ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
