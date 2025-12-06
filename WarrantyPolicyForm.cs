using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class WarrantyPolicyForm : Form
    {
        private ISession _db => Program.DbSession;

        // cache để lọc tại client
        private List<PolicyRow> _cache = new List<PolicyRow>();

        public WarrantyPolicyForm()
        {
            InitializeComponent();
        }

        private void WarrantyPolicyForm_Load(object sender, EventArgs e)
        {
            LoadProductDropdown();
            TryLoadAll();
        }

        private void LoadProductDropdown()
        {
            try
            {
                if (_db == null) { SetStatus("Chưa kết nối cơ sở dữ liệu."); return; }

                // Lấy danh sách model sản phẩm
                var rs = _db.Execute("SELECT id, name FROM products;");
                var items = rs.Select(r => new
                {
                    Id = r.GetValue<string>("id"),
                    Name = r.IsNull("name") ? "" : r.GetValue<string>("name")
                })
                .OrderBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
                .ToList();

                // Hiển thị dạng "ID — Name"
                cboProductId.DataSource = items;
                cboProductId.DisplayMember = "Name";
                cboProductId.ValueMember = "Id";

                cboProductId.DrawMode = DrawMode.OwnerDrawFixed;
                cboProductId.DropDownStyle = ComboBoxStyle.DropDownList;

                // QUAN TRỌNG: đặt Source trước rồi mới đặt Mode
                cboProductId.AutoCompleteSource = AutoCompleteSource.ListItems;
                cboProductId.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                // tránh gắn trùng handler
                cboProductId.DrawItem -= Cbo_DrawItem;
                cboProductId.DrawItem += Cbo_DrawItem;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách sản phẩm: " + ex.Message, "Chính sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Cbo_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                dynamic item = cboProductId.Items[e.Index];
                string id = item.Id;
                string name = item.Name;
                string text = string.IsNullOrWhiteSpace(name) ? id : $"{id} — {name}";
                using (var br = new System.Drawing.SolidBrush(e.ForeColor))
                    e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        private void TryLoadAll()
        {
            try
            {
                if (_db == null) { SetStatus("Chưa kết nối cơ sở dữ liệu."); return; }

                var rs = _db.Execute("SELECT product_id, warranty_months, conditions FROM warranty_policy_by_product;");
                _cache = rs.Select(r => new PolicyRow
                {
                    ProductId = r.GetValue<string>("product_id"),
                    WarrantyMonths = r.IsNull("warranty_months") ? (int?)null : r.GetValue<int>("warranty_months"),
                    Conditions = r.IsNull("conditions") ? null : r.GetValue<string>("conditions")
                })
                .OrderBy(p => p.ProductId, StringComparer.OrdinalIgnoreCase)
                .ToList();

                BindGrid(_cache);
                SetStatus($"Đã nạp {_cache.Count} chính sách.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Chính sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi tải dữ liệu.");
            }
        }

        private void BindGrid(IEnumerable<PolicyRow> data)
        {
            var dt = new DataTable();
            dt.Columns.Add("ProductId");
            dt.Columns.Add("WarrantyMonths");
            dt.Columns.Add("Conditions");

            foreach (var p in data)
                dt.Rows.Add(p.ProductId, p.WarrantyMonths?.ToString() ?? "", p.Conditions);

            grid.DataSource = dt;

            // Việt hoá header
            if (grid.Columns.Contains("ProductId")) grid.Columns["ProductId"].HeaderText = "Model";
            if (grid.Columns.Contains("WarrantyMonths")) grid.Columns["WarrantyMonths"].HeaderText = "Tháng BH";
            if (grid.Columns.Contains("Conditions")) grid.Columns["Conditions"].HeaderText = "Điều kiện/Ghi chú";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadProductDropdown();
            TryLoadAll();
            ClearInputs();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch.Text ?? "").Trim();
            if (string.IsNullOrEmpty(kw)) { BindGrid(_cache); SetStatus($"Đang hiển thị {_cache.Count} chính sách."); return; }

            var q = kw.ToLowerInvariant();
            var filtered = _cache.Where(p =>
                (p.ProductId ?? "").ToLowerInvariant().Contains(q) ||
                (p.Conditions ?? "").ToLowerInvariant().Contains(q)
            ).ToList();

            BindGrid(filtered);
            SetStatus($"Tìm thấy {filtered.Count}/{_cache.Count} chính sách khớp \"{kw}\".");
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

            var productId = row.Cells["ProductId"].Value?.ToString();
            SetComboProduct(productId);

            if (int.TryParse(row.Cells["WarrantyMonths"].Value?.ToString(), out var wm))
                numMonths.Value = Math.Max(numMonths.Minimum, Math.Min(numMonths.Maximum, wm));
            else
                numMonths.Value = 12;

            txtConditions.Text = row.Cells["Conditions"].Value?.ToString();
        }

        private void SetComboProduct(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId)) { cboProductId.SelectedIndex = -1; return; }
            for (int i = 0; i < cboProductId.Items.Count; i++)
            {
                dynamic item = cboProductId.Items[i];
                if (string.Equals(item.Id as string, productId, StringComparison.OrdinalIgnoreCase))
                {
                    cboProductId.SelectedIndex = i;
                    return;
                }
            }
            // nếu model không còn trong danh mục -> bỏ chọn
            cboProductId.SelectedIndex = -1;
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void ClearInputs()
        {
            cboProductId.SelectedIndex = -1;
            numMonths.Value = 12;
            txtConditions.Clear();
            cboProductId.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var productId = cboProductId.SelectedValue as string;
            var months = (int)numMonths.Value;
            var conditions = (txtConditions.Text ?? "").Trim();

            if (string.IsNullOrEmpty(productId))
            {
                MessageBox.Show("Vui lòng chọn model sản phẩm.", "Chính sách", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Kiểm tra trùng
                var check = _db.Execute(_db.Prepare(
                    "SELECT product_id FROM warranty_policy_by_product WHERE product_id = ?;").Bind(productId))
                    .FirstOrDefault();
                if (check != null)
                {
                    MessageBox.Show($"Chính sách cho '{productId}' đã tồn tại.", "Chính sách",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var ps = _db.Prepare(
                    "INSERT INTO warranty_policy_by_product (product_id, warranty_months, conditions) VALUES (?, ?, ?);");
                _db.Execute(ps.Bind(productId, months, string.IsNullOrWhiteSpace(conditions) ? (object)null : conditions));

                SetStatus($"Đã thêm chính sách cho {productId}.");
                TryLoadAll();
                SelectRowByProduct(productId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message, "Chính sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi thêm.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var productId = cboProductId.SelectedValue as string;
            if (string.IsNullOrEmpty(productId))
            {
                MessageBox.Show("Chọn một model cần sửa.", "Chính sách",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var months = (int)numMonths.Value;
            var conditions = (txtConditions.Text ?? "").Trim();

            try
            {
                var ps = _db.Prepare(
                    "UPDATE warranty_policy_by_product SET warranty_months = ?, conditions = ? WHERE product_id = ?;");
                _db.Execute(ps.Bind(months,
                                    string.IsNullOrWhiteSpace(conditions) ? (object)null : conditions,
                                    productId));

                SetStatus($"Đã cập nhật chính sách cho {productId}.");
                TryLoadAll();
                SelectRowByProduct(productId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Chính sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi cập nhật.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var productId = cboProductId.SelectedValue as string;
            if (string.IsNullOrEmpty(productId))
            {
                MessageBox.Show("Chọn model cần xoá.", "Chính sách",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận xoá chính sách cho '{productId}'?",
                                "Xoá chính sách", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var ps = _db.Prepare("DELETE FROM warranty_policy_by_product WHERE product_id = ?;");
                _db.Execute(ps.Bind(productId));

                SetStatus($"Đã xoá chính sách cho {productId}.");
                TryLoadAll();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xoá: " + ex.Message, "Chính sách", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi xoá.");
            }
        }

        private void SelectRowByProduct(string productId)
        {
            if (grid.DataSource is DataTable)
            {
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if ((r.Cells["ProductId"].Value?.ToString() ?? "") == productId)
                    {
                        r.Selected = true;
                        grid.FirstDisplayedScrollingRowIndex = r.Index;
                        FillFromSelectedRow(r.Index);
                        break;
                    }
                }
            }
        }

        private void SetStatus(string text) => lblStatus.Text = text;

        private class PolicyRow
        {
            public string ProductId { get; set; }
            public int? WarrantyMonths { get; set; }
            public string Conditions { get; set; }
        }
    }
}
