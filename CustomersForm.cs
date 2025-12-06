using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class CustomersForm : Form
    {
        private ISession _db => Program.DbSession;

        // Cache để lọc tìm kiếm cục bộ (Cassandra không hỗ trợ LIKE)
        private List<CustomerRow> _cache = new List<CustomerRow>();

        public CustomersForm()
        {
            InitializeComponent();
        }

        private void CustomersForm_Load(object sender, EventArgs e)
        {
            TryLoadAll();
        }

        private void TryLoadAll()
        {
            try
            {
                if (_db == null) { SetStatus("Chưa kết nối cơ sở dữ liệu."); return; }

                var rs = _db.Execute("SELECT id, name, address, phone, email FROM customers;");
                _cache = rs.Select(r => new CustomerRow
                {
                    Id = r.GetValue<string>("id"),
                    Name = r.GetValue<string>("name"),
                    Address = r.IsNull("address") ? null : r.GetValue<string>("address"),
                    Phone = r.IsNull("phone") ? null : r.GetValue<string>("phone"),
                    Email = r.IsNull("email") ? null : r.GetValue<string>("email")
                })
                .OrderBy(c => c.Id, StringComparer.OrdinalIgnoreCase)
                .ToList();

                BindGrid(_cache);
                SetStatus($"Đã nạp {_cache.Count} khách hàng.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi tải dữ liệu.");
            }
        }

        private void BindGrid(IEnumerable<CustomerRow> data)
        {
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Address");
            dt.Columns.Add("Phone");
            dt.Columns.Add("Email");

            foreach (var c in data)
                dt.Rows.Add(c.Id, c.Name, c.Address, c.Phone, c.Email);

            grid.DataSource = dt;

            // Việt hoá tiêu đề (nếu muốn hiển thị tiếng Việt trên header)
            if (grid.Columns.Contains("ID")) grid.Columns["ID"].HeaderText = "Mã KH";
            if (grid.Columns.Contains("Name")) grid.Columns["Name"].HeaderText = "Tên";
            if (grid.Columns.Contains("Address")) grid.Columns["Address"].HeaderText = "Địa chỉ";
            if (grid.Columns.Contains("Phone")) grid.Columns["Phone"].HeaderText = "Điện thoại";
            if (grid.Columns.Contains("Email")) grid.Columns["Email"].HeaderText = "Email";
        }

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
                SetStatus($"Đang hiển thị {_cache.Count} khách hàng.");
                return;
            }

            var q = kw.ToLowerInvariant();
            var filtered = _cache.Where(c =>
                (c.Id ?? "").ToLowerInvariant().Contains(q) ||
                (c.Name ?? "").ToLowerInvariant().Contains(q) ||
                (c.Phone ?? "").ToLowerInvariant().Contains(q) ||
                (c.Email ?? "").ToLowerInvariant().Contains(q)
            ).ToList();

            BindGrid(filtered);
            SetStatus($"Tìm thấy {filtered.Count}/{_cache.Count} khách hàng khớp \"{kw}\".");
        }

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
            txtAddress.Text = row.Cells["Address"].Value?.ToString();
            txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            txtEmail.Text = row.Cells["Email"].Value?.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtId.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            var name = (txtName.Text ?? "").Trim();
            var address = (txtAddress.Text ?? "").Trim();
            var phone = (txtPhone.Text ?? "").Trim();
            var email = (txtEmail.Text ?? "").Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tối thiểu ID và Tên.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ (chỉ gồm số, dấu +, khoảng trắng, -).", "Khách hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra trùng ID
                var check = _db.Execute(_db.Prepare("SELECT id FROM customers WHERE id = ?;").Bind(id)).FirstOrDefault();
                if (check != null)
                {
                    MessageBox.Show($"ID '{id}' đã tồn tại.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var ps = _db.Prepare("INSERT INTO customers (id, name, address, phone, email) VALUES (?, ?, ?, ?, ?);");
                _db.Execute(ps.Bind(id,
                                    name,
                                    string.IsNullOrWhiteSpace(address) ? null : address,
                                    string.IsNullOrWhiteSpace(phone) ? null : phone,
                                    string.IsNullOrWhiteSpace(email) ? null : email));

                SetStatus($"Đã thêm khách hàng {id}.");
                TryLoadAll();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message, "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi thêm.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Chọn 1 dòng hoặc nhập ID cần sửa.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var name = (txtName.Text ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Tên không được rỗng.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var address = (txtAddress.Text ?? "").Trim();
            var phone = (txtPhone.Text ?? "").Trim();
            var email = (txtEmail.Text ?? "").Trim();

            if (!IsValidPhone(phone))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Email không hợp lệ.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var ps = _db.Prepare("UPDATE customers SET name = ?, address = ?, phone = ?, email = ? WHERE id = ?;");
                _db.Execute(ps.Bind(name,
                                    string.IsNullOrWhiteSpace(address) ? null : address,
                                    string.IsNullOrWhiteSpace(phone) ? null : phone,
                                    string.IsNullOrWhiteSpace(email) ? null : email,
                                    id));

                SetStatus($"Đã cập nhật khách hàng {id}.");
                TryLoadAll();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi cập nhật.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Nhập ID cần xoá hoặc chọn 1 dòng.", "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận xoá khách hàng '{id}'?",
                                "Xoá khách hàng", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var ps = _db.Prepare("DELETE FROM customers WHERE id = ?;");
                _db.Execute(ps.Bind(id));

                SetStatus($"Đã xoá khách hàng {id}.");
                TryLoadAll();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xoá: " + ex.Message, "Khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true; // cho phép trống
            // Chỉ cho phép số, khoảng trắng, + và -
            return Regex.IsMatch(phone, @"^[0-9+\-\s]+$");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; // cho phép trống
            return email.Contains("@") && email.Contains(".");
        }

        private void SetStatus(string text) => lblStatus.Text = text;

        private class CustomerRow
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }
    }
}
