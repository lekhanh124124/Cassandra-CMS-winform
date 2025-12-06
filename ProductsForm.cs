using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class ProductsForm : Form
    {
        private ISession _db => Program.DbSession;

        // Cache để lọc
        private List<ProductRow> _cache = new List<ProductRow>();

        // Danh mục loại (khoá ngoại) – chỉ lưu id/code hiển thị
        private List<string> _typeList = new List<string>();

        public ProductsForm()
        {
            InitializeComponent();
        }

        private void ProductsForm_Load(object sender, EventArgs e)
        {
            TryLoadTypes();   // nạp dropdown Loại
            TryLoadAll();     // nạp lưới
        }

        // ===== Nạp danh mục LOẠI (product_types / product_categories / categories) =====
        private void TryLoadTypes()
        {
            _typeList.Clear();
            if (_db == null) return;

            // Thử lần lượt các bảng thường gặp
            var tryTables = new[]
            {
                new { Table="product_types",      IdCols=new[]{"id","code"}, NameCols=new[]{"name","title"} },
                new { Table="product_categories", IdCols=new[]{"id","code"}, NameCols=new[]{"name","title"} },
                new { Table="categories",         IdCols=new[]{"id","code"}, NameCols=new[]{"name","title"} },
            };

            foreach (var t in tryTables)
            {
                try
                {
                    var list = LoadLookupStrings(t.Table, t.IdCols, t.NameCols);
                    if (list.Count > 0)
                    {
                        _typeList = list;
                        break;
                    }
                }
                catch
                {
                    // Bảng không tồn tại hoặc không hợp lệ -> thử bảng tiếp theo
                }
            }

            // Gán nguồn cho ComboBox (vẫn cho phép gõ tự do nếu danh mục rỗng)
            cmbType.Items.Clear();
            if (_typeList.Count > 0)
                cmbType.Items.AddRange(_typeList.Cast<object>().ToArray());
        }

        // Đọc bảng danh mục thành list string (ưu tiên "id - name" nếu có name)
        private List<string> LoadLookupStrings(string table, string[] idCandidates, string[] nameCandidates)
        {
            var rs = _db.Execute(new SimpleStatement($"SELECT * FROM {table};"));
            var cols = new HashSet<string>(rs.Columns.Select(c => c.Name), StringComparer.OrdinalIgnoreCase);

            string pickId = idCandidates.FirstOrDefault(c => cols.Contains(c));
            string pickName = nameCandidates.FirstOrDefault(c => cols.Contains(c));
            if (pickId == null) throw new Exception($"Bảng {table} không có cột khoá hợp lệ.");

            var list = new List<string>();
            foreach (var r in rs)
            {
                var id = r.GetValue<string>(pickId);
                if (!string.IsNullOrWhiteSpace(pickName) && !r.IsNull(pickName))
                {
                    var name = r.GetValue<string>(pickName);
                    list.Add(string.IsNullOrWhiteSpace(name) ? id : $"{id} - {name}");
                }
                else list.Add(id);
            }
            return list.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // ===== Nạp lưới =====
        private void TryLoadAll()
        {
            try
            {
                if (_db == null) { SetStatus("Chưa kết nối cơ sở dữ liệu."); return; }

                var rs = _db.Execute("SELECT id, name, type, specs, manufacture_date FROM products;");
                _cache = rs.Select(r => new ProductRow
                {
                    Id = r.GetValue<string>("id"),
                    Name = r.GetValue<string>("name"),
                    Type = r.IsNull("type") ? null : r.GetValue<string>("type"),
                    Specs = r.IsNull("specs") ? null : r.GetValue<string>("specs"),
                    ManufactureDate = r.IsNull("manufacture_date") ? (DateTime?)null : r.GetValue<DateTime>("manufacture_date")
                })
                .OrderBy(p => p.Id, StringComparer.OrdinalIgnoreCase)
                .ToList();

                BindGrid(_cache);
                SetStatus($"Đã nạp {_cache.Count} sản phẩm.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi tải dữ liệu.");
            }
        }

        private void BindGrid(IEnumerable<ProductRow> data)
        {
            var dt = new DataTable();
            // Tên cột nội bộ
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("Type");
            dt.Columns.Add("Specs");
            dt.Columns.Add("ManufactureDate");

            foreach (var p in data)
                dt.Rows.Add(p.Id, p.Name, p.Type, p.Specs, p.ManufactureDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");

            grid.DataSource = dt;

            // Việt hoá header
            if (grid.Columns.Contains("ID")) grid.Columns["ID"].HeaderText = "Mã SP";
            if (grid.Columns.Contains("Name")) grid.Columns["Name"].HeaderText = "Tên sản phẩm";
            if (grid.Columns.Contains("Type")) grid.Columns["Type"].HeaderText = "Loại";
            if (grid.Columns.Contains("Specs")) grid.Columns["Specs"].HeaderText = "Thông số kỹ thuật";
            if (grid.Columns.Contains("ManufactureDate")) grid.Columns["ManufactureDate"].HeaderText = "Ngày sản xuất";
        }

        // ===== Sự kiện =====
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            TryLoadTypes();
            TryLoadAll();
            ClearInputs();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch.Text ?? "").Trim();
            if (string.IsNullOrEmpty(kw)) { BindGrid(_cache); SetStatus($"Đang hiển thị {_cache.Count} sản phẩm."); return; }

            var q = kw.ToLowerInvariant();
            var filtered = _cache.Where(p =>
                (p.Id ?? "").ToLowerInvariant().Contains(q) ||
                (p.Name ?? "").ToLowerInvariant().Contains(q) ||
                (p.Type ?? "").ToLowerInvariant().Contains(q)
            ).ToList();

            BindGrid(filtered);
            SetStatus($"Tìm thấy {filtered.Count}/{_cache.Count} sản phẩm khớp \"{kw}\".");
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

            // Gán vào dropdown Loại (vẫn cho gõ tự do nếu giá trị không có trong danh mục)
            var t = row.Cells["Type"].Value?.ToString() ?? "";
            cmbType.Text = t;

            txtSpecs.Text = row.Cells["Specs"].Value?.ToString();

            var sDate = row.Cells["ManufactureDate"].Value?.ToString();
            if (DateTime.TryParse(sDate, out var d))
            {
                dtpMfgDate.Checked = true;
                dtpMfgDate.Value = d;
            }
            else
            {
                dtpMfgDate.Checked = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            cmbType.Text = "";
            txtSpecs.Clear();
            dtpMfgDate.Checked = false;
            txtId.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            var name = (txtName.Text ?? "").Trim();
            var type = (cmbType.Text ?? "").Trim(); // từ dropdown (hoặc gõ tự do)
            var specs = (txtSpecs.Text ?? "").Trim();
            DateTime? mfg = dtpMfgDate.Checked ? dtpMfgDate.Value : (DateTime?)null;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập tối thiểu Mã SP và Tên sản phẩm.", "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // kiểm tra trùng ID
                var check = _db.Execute(_db.Prepare("SELECT id FROM products WHERE id = ?;").Bind(id)).FirstOrDefault();
                if (check != null)
                {
                    MessageBox.Show($"Mã '{id}' đã tồn tại.", "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var ps = _db.Prepare("INSERT INTO products (id, name, type, specs, manufacture_date) VALUES (?, ?, ?, ?, ?);");
                _db.Execute(ps.Bind(id,
                                    name,
                                    string.IsNullOrWhiteSpace(type) ? null : type,
                                    string.IsNullOrWhiteSpace(specs) ? null : specs,
                                    mfg.HasValue ? (object)mfg.Value : null));

                // Nếu nhập loại mới mà chưa có trong danh mục -> thêm vào dropdown tạm
                if (!string.IsNullOrWhiteSpace(type) && !_typeList.Contains(type))
                {
                    _typeList.Add(type);
                    cmbType.Items.Add(type);
                }

                SetStatus($"Đã thêm sản phẩm {id}.");
                TryLoadAll();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm: " + ex.Message, "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi thêm.");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Chọn 1 dòng hoặc nhập Mã SP cần sửa.", "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var name = (txtName.Text ?? "").Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Tên sản phẩm không được rỗng.", "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var type = (cmbType.Text ?? "").Trim();
            var specs = (txtSpecs.Text ?? "").Trim();
            DateTime? mfg = dtpMfgDate.Checked ? dtpMfgDate.Value : (DateTime?)null;

            try
            {
                var ps = _db.Prepare("UPDATE products SET name = ?, type = ?, specs = ?, manufacture_date = ? WHERE id = ?;");
                _db.Execute(ps.Bind(name,
                                    string.IsNullOrWhiteSpace(type) ? null : type,
                                    string.IsNullOrWhiteSpace(specs) ? null : specs,
                                    mfg.HasValue ? (object)mfg.Value : null,
                                    id));

                if (!string.IsNullOrWhiteSpace(type) && !_typeList.Contains(type))
                {
                    _typeList.Add(type);
                    cmbType.Items.Add(type);
                }

                SetStatus($"Đã cập nhật sản phẩm {id}.");
                TryLoadAll();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi cập nhật.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var id = (txtId.Text ?? "").Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Nhập Mã SP cần xoá hoặc chọn 1 dòng.", "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Xác nhận xoá sản phẩm '{id}'?",
                                "Xoá sản phẩm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                var ps = _db.Prepare("DELETE FROM products WHERE id = ?;");
                _db.Execute(ps.Bind(id));

                SetStatus($"Đã xoá sản phẩm {id}.");
                TryLoadAll();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xoá: " + ex.Message, "Sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi xoá.");
            }
        }

        private void SelectRowById(string id)
        {
            if (grid.DataSource is DataTable dt)
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

        private void SetStatus(string text) => lblStatus.Text = text;

        private class ProductRow
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Specs { get; set; }
            public DateTime? ManufactureDate { get; set; }
        }
    }
}
