// File: EmployeesForm.cs
using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class EmployeesForm : Form
    {
        private ISession _db => Program.DbSession;

        // cache để tìm kiếm nội bộ
        private List<EmployeeRow> _cache = new List<EmployeeRow>();

        public EmployeesForm()
        {
            InitializeComponent();
        }

        private void EmployeesForm_Load(object sender, EventArgs e)
        {
            // gợi ý role (có thể sửa/nhập mới tùy ý)
            cmbRole.Items.Clear();
            cmbRole.Items.AddRange(new object[] { "technician", "receiver", "dispatcher", "manager", "employee" });

            // BẬT gợi ý cho dropdown theo đúng thứ tự để tránh lỗi
            cmbRole.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbRole.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            TryLoadAll();
        }

        // ========================= CORE LOAD/BIND =========================
        private void TryLoadAll()
        {
            try
            {
                if (_db == null) { SetStatus("Chưa kết nối cơ sở dữ liệu."); return; }

                var rs = _db.Execute("SELECT id, name, phone, email, role FROM employees;");
                _cache = rs.Select(r => new EmployeeRow
                {
                    Id = r.GetValue<string>("id"),
                    Name = r.GetValue<string>("name"),
                    Phone = r.GetValue<string>("phone"),
                    Email = r.GetValue<string>("email"),
                    Role = r.GetValue<string>("role")
                })
                .OrderBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                .ToList();

                BindGrid(_cache);
                SetStatus($"Đã nạp {_cache.Count} nhân viên.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi tải dữ liệu.");
            }
        }

        private void BindGrid(IEnumerable<EmployeeRow> data)
        {
            // Dùng cột kỹ thuật (EN) để code tra cứu theo tên cột ổn định
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Phone");
            dt.Columns.Add("Email");
            dt.Columns.Add("Role");

            foreach (var e in data)
                dt.Rows.Add(e.Id, e.Name, e.Phone, e.Email, e.Role);

            grid.DataSource = dt;

            // Đổi header sang tiếng Việt cho đẹp
            if (grid.Columns["ID"] != null) grid.Columns["ID"].HeaderText = "Mã NV";
            if (grid.Columns["Name"] != null) grid.Columns["Name"].HeaderText = "Họ tên";
            if (grid.Columns["Phone"] != null) grid.Columns["Phone"].HeaderText = "Điện thoại";
            if (grid.Columns["Email"] != null) grid.Columns["Email"].HeaderText = "Email";
            if (grid.Columns["Role"] != null) grid.Columns["Role"].HeaderText = "Vai trò";
        }

        // ========================= TOP BAR =========================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            TryLoadAll();
            ClearInputs();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch.Text ?? "").Trim();
            if (string.IsNullOrEmpty(kw))
            {
                BindGrid(_cache);
                SetStatus($"Đang hiển thị {_cache.Count} nhân viên.");
                return;
            }

            var q = kw.ToLowerInvariant();
            var filtered = _cache.Where(emp =>
                (emp.Id ?? "").ToLowerInvariant().Contains(q) ||
                (emp.Name ?? "").ToLowerInvariant().Contains(q) ||
                (emp.Phone ?? "").ToLowerInvariant().Contains(q) ||
                (emp.Email ?? "").ToLowerInvariant().Contains(q) ||
                (emp.Role ?? "").ToLowerInvariant().Contains(q)
            ).ToList();

            BindGrid(filtered);
            SetStatus($"Tìm thấy {filtered.Count}/{_cache.Count} nhân viên khớp \"{kw}\".");
        }

        // ========================= GRID EVENTS =========================
        private void grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            FillFromSelectedRow(e.RowIndex);
        }

        private void grid_DoubleClick(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;
            FillFromSelectedRow(grid.CurrentRow.Index);
        }

        private void FillFromSelectedRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;
            var row = grid.Rows[rowIndex];
            txtId.Text = row.Cells["ID"].Value?.ToString();
            txtName.Text = row.Cells["Name"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();

            var role = row.Cells["Role"].Value?.ToString();
            if (!string.IsNullOrWhiteSpace(role) && !cmbRole.Items.Contains(role))
                cmbRole.Items.Add(role);
            cmbRole.Text = role ?? "";

            // khuyến nghị không đổi ID khi update
        }

        // ========================= EDITOR BUTTONS =========================
        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            cmbRole.Text = "";
            txtId.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            var name = (txtName.Text ?? "").Trim();
            var phone = (txtPhone.Text ?? "").Trim();
            var email = (txtEmail.Text ?? "").Trim();
            var role = NormalizeRole((cmbRole.Text ?? "").Trim());

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tối thiểu Mã NV và Họ tên.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ (chỉ chứa số, dấu +, khoảng trắng, -).", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_db == null) { MessageBox.Show("Chưa kết nối CSDL"); return; }

                // check trùng Employee ID
                var checkEmp = _db.Execute(_db.Prepare("SELECT id FROM employees WHERE id = ?;").Bind(id)).FirstOrDefault();
                if (checkEmp != null)
                {
                    MessageBox.Show($"Mã NV '{id}' đã tồn tại.", "Nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // check trùng username (username = id) trong bảng users
                var checkUser = _db.Execute(_db.Prepare("SELECT user_id FROM users WHERE username = ? ALLOW FILTERING;").Bind(id)).FirstOrDefault();
                if (checkUser != null)
                {
                    MessageBox.Show($"Tài khoản với username '{id}' đã tồn tại.", "Nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 1) Tạo employee
                var psEmp = _db.Prepare("INSERT INTO employees (id, name, phone, email, role) VALUES (?, ?, ?, ?, ?);");

                // 2) Tạo user tự động: username = password = id (hash)
                var userId = Guid.NewGuid().ToString("N");
                var pwdHash = BCrypt.Net.BCrypt.HashPassword(id);
                var effectiveRole = string.IsNullOrWhiteSpace(role) ? "employee" : role;

                var psUser1 = _db.Prepare("INSERT INTO users (user_id, username, password_hash, role, employee_id) VALUES (?, ?, ?, ?, ?);");

                var batch = new BatchStatement()
                    .Add(psEmp.Bind(
                        id,
                        name,
                        string.IsNullOrWhiteSpace(phone) ? null : phone,
                        string.IsNullOrWhiteSpace(email) ? null : email,
                        string.IsNullOrWhiteSpace(role) ? null : role
                    ))
                    .Add(psUser1.Bind(userId, id, pwdHash, effectiveRole, id));

                _db.Execute(batch);

                SetStatus($"Đã thêm nhân viên {id} và tạo tài khoản đăng nhập (username=password=id).");
                TryLoadAll();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message, "Nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi thêm.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Chọn 1 dòng hoặc nhập Mã NV cần sửa.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var name = (txtName.Text ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Họ tên không được rỗng.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var phone = (txtPhone.Text ?? "").Trim();
            var email = (txtEmail.Text ?? "").Trim();
            var role = NormalizeRole((cmbRole.Text ?? "").Trim());

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_db == null) { MessageBox.Show("Chưa kết nối CSDL"); return; }

                // Update employee
                var ps = _db.Prepare("UPDATE employees SET name = ?, phone = ?, email = ?, role = ? WHERE id = ?;");
                _db.Execute(ps.Bind(
                    name,
                    string.IsNullOrWhiteSpace(phone) ? null : phone,
                    string.IsNullOrWhiteSpace(email) ? null : email,
                    string.IsNullOrWhiteSpace(role) ? null : role,
                    id
                ));

                // Đồng bộ role của user (username = id)
                SyncUserRoleForEmployee(id, role);

                SetStatus($"Đã cập nhật nhân viên {id} (đồng bộ vai trò tài khoản).");
                TryLoadAll();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi cập nhật.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Nhập Mã NV cần xoá hoặc chọn 1 dòng.", "Nhân viên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận xoá nhân viên '{id}' (sẽ xoá cả tài khoản liên quan)?",
                                "Xoá nhân viên", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (_db == null) { MessageBox.Show("Chưa kết nối CSDL"); return; }
                            
                // Lấy user_id từ users (username = id)
                var rowUser = _db.Execute(_db.Prepare("SELECT user_id FROM users WHERE username = ? ALLOW FILTERING;").Bind(id)).FirstOrDefault();
                var psDelEmp = _db.Prepare("DELETE FROM employees WHERE id = ?;");

                if (rowUser == null)
                {
                    // Không có user -> xoá riêng employee
                    _db.Execute(psDelEmp.Bind(id));
                }
                else
                {
                    var userId = rowUser.GetValue<string>("user_id");

                    var psDelU1 = _db.Prepare("DELETE FROM users WHERE user_id = ?;");

                    var batch = new BatchStatement()
                        .Add(psDelEmp.Bind(id))
                        .Add(psDelU1.Bind(userId));

                    _db.Execute(batch);
                }

                SetStatus($"Đã xoá nhân viên {id} và tài khoản liên quan (nếu có).");
                TryLoadAll();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xoá: " + ex.Message, "Nhân viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi xoá.");
            }
        }

        private void SelectRowById(string id)
        {
            if (grid.DataSource is DataTable)
            {
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if ((r.Cells["ID"].Value?.ToString() ?? "") == id)
                    {
                        r.Selected = true;
                        grid.FirstDisplayedScrollingRowIndex = r.Index;
                        FillFromSelectedRow(r.Index);
                        break;
                    }
                }
            }
        }

        // ========================= HELPERS =========================
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true;
            return Regex.IsMatch(phone, @"^[0-9+\-\s]+$");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true;
            return email.Contains("@") && email.Contains(".");
        }

        private string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "employee";
            return role.Trim();
        }

        private void SyncUserRoleForEmployee(string employeeId, string newRole)
        {
            try
            {
                var effectiveRole = NormalizeRole(newRole);

                // user có username = employeeId
                var rs = _db.Execute(_db.Prepare("SELECT user_id FROM users WHERE username = ? ALLOW FILTERING;").Bind(employeeId))
                            .FirstOrDefault();
                if (rs == null)
                {
                    // chưa có user -> tạo mới cho đúng nguyên tắc đồng bộ
                    var userId = Guid.NewGuid().ToString("N");
                    var pwdHash = BCrypt.Net.BCrypt.HashPassword(employeeId);

                    var ps1 = _db.Prepare("INSERT INTO users (user_id, username, password_hash, role, employee_id) VALUES (?, ?, ?, ?, ?);");

                    var batchCreate = new BatchStatement()
                        .Add(ps1.Bind(userId, employeeId, pwdHash, effectiveRole, employeeId));

                    _db.Execute(batchCreate);
                    return;
                }

                var uid = rs.GetValue<string>("user_id");

                var psu1 = _db.Prepare("UPDATE users SET role = ? WHERE user_id = ?;");

                var batch = new BatchStatement()
                    .Add(psu1.Bind(effectiveRole, uid));

                _db.Execute(batch);
            }
            catch (Exception ex)
            {
                // không chặn UI flow, chỉ báo trạng thái
                SetStatus("Cảnh báo: đồng bộ vai trò tài khoản lỗi - " + ex.Message);
            }
        }

        private void SetStatus(string s) => lblStatus.Text = s;

        private class EmployeeRow
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
        }
    }
}
