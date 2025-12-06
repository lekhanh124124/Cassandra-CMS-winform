using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public class LookupItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => string.IsNullOrWhiteSpace(Name) ? Id : $"{Id} - {Name}";
    }

    public partial class TicketsForm : Form
    {
        private enum ViewMode { ByStatus, ByCustomer, ByProduct }
        private ISession _db => Program.DbSession;

        private List<TicketRow> _cache = new List<TicketRow>();

        private PreparedStatement _psByStatus;
        private PreparedStatement _psByCustomer;
        private PreparedStatement _psByProduct;

        private List<LookupItem> _customers = new List<LookupItem>();
        private List<LookupItem> _products = new List<LookupItem>();
        private List<LookupItem> _employees = new List<LookupItem>();

        public TicketsForm()
        {
            InitializeComponent();
        }
        private string GenerateTicketIdWithRule()
        {
            // Quy tắc: T-YYYYMMDD-SSSSS (UTC)
            for (int attempt = 0; attempt < 10; attempt++)
            {
                var id = $"T-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 5).ToUpperInvariant()}";
                var ps = _db.Prepare("SELECT id FROM warranty_tickets_by_id WHERE id = ? LIMIT 1;");
                var existed = _db.Execute(ps.Bind(id)).FirstOrDefault();
                if (existed == null) return id;
            }

            throw new Exception("Không thể sinh mã phiếu duy nhất sau nhiều lần thử. Vui lòng thử lại.");
        }

        private void TicketsForm_Load(object sender, EventArgs e)
        {
            try
            {
                _psByStatus = _db.Prepare(
                    "SELECT status, created_at, toTimestamp(created_at) AS created_ts, id, product_id, customer_id " +
                    "FROM warranty_tickets_by_status WHERE status = ?;"
                );
                _psByCustomer = _db.Prepare(
                    "SELECT customer_id, created_at, toTimestamp(created_at) AS created_ts, id, product_id, status " +
                    "FROM warranty_tickets_by_customer WHERE customer_id = ?;"
                );
                _psByProduct = _db.Prepare(
                    "SELECT product_id, created_at, toTimestamp(created_at) AS created_ts, id, customer_id, status " +
                    "FROM warranty_tickets_by_product WHERE product_id = ?;"
                );

                LoadForeignKeys();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chuẩn bị truy vấn: " + ex.Message, "Phiếu bảo hành", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmbMode.SelectedIndex = 0;            
            cmbStatus.SelectedItem = "IN_PROGRESS"; 
            ApplyModeVisibility();
            LoadList();
        }

        private List<LookupItem> LoadLookup(string table, string[] idCandidates, string[] nameCandidates)
        {
            var list = new List<LookupItem>();
            var rs = _db.Execute(new SimpleStatement($"SELECT * FROM {table};"));
            var cols = new HashSet<string>(rs.Columns.Select(c => c.Name), StringComparer.OrdinalIgnoreCase);

            string pickId = idCandidates.FirstOrDefault(c => cols.Contains(c));
            string pickName = nameCandidates.FirstOrDefault(c => cols.Contains(c));
            if (pickId == null) throw new Exception($"Bảng {table} không có cột khoá phù hợp.");

            foreach (var r in rs)
            {
                var id = r.GetValue<string>(pickId);
                string name = (pickName != null && !r.IsNull(pickName)) ? r.GetValue<string>(pickName) : null;
                list.Add(new LookupItem { Id = id, Name = name });
            }
            return list.OrderBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private List<LookupItem> LoadTechnicians()
        {
            var list = new List<LookupItem>();
            try
            {
                var rs = _db.Execute(new SimpleStatement("SELECT id, name, role FROM employees WHERE role = 'technician' ALLOW FILTERING;"));
                foreach (var r in rs)
                {
                    var id = r.GetValue<string>("id");
                    var name = r.IsNull("name") ? null : r.GetValue<string>("name");
                    list.Add(new LookupItem { Id = id, Name = name });
                }
            }
            catch
            {
                // fallback nếu bảng/role khác: nạp tất cả rồi lọc client
                var all = LoadLookup("employees", new[] { "id" }, new[] { "name" });
                list = all; // nếu không truy được role thì tạm thời giữ nguyên (tránh chặn UI)
            }

            return list.OrderBy(x => x.Id, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void LoadForeignKeys()
        {
            try
            {
                _customers = LoadLookup("customers", new[] { "id" }, new[] { "name" });
                cmbCustomer.DataSource = _customers.ToList();

                _products = LoadLookup("products", new[] { "id" }, new[] { "name" });
                cmbProduct.DataSource = _products.ToList();

                _employees = LoadTechnicians(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không nạp được danh mục: " + ex.Message, "Danh mục", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cmbMode_SelectedIndexChanged(object sender, EventArgs e) => ApplyModeVisibility();

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var kw = (txtSearch.Text ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(kw)) { BindGrid(_cache); SetStatus($"Hiển thị {_cache.Count} phiếu."); return; }

            var filtered = _cache.Where(t =>
                (t.Id ?? "").ToLowerInvariant().Contains(kw) ||
                (t.ProductId ?? "").ToLowerInvariant().Contains(kw) ||
                (t.CustomerId ?? "").ToLowerInvariant().Contains(kw) ||
                (t.Status ?? "").ToLowerInvariant().Contains(kw)
            ).ToList();


            BindGrid(filtered);
            SetStatus($"Tìm thấy {filtered.Count}/{_cache.Count} phiếu khớp \"{kw}\".");
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadList();

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        { if (e.RowIndex >= 0) btnViewDetail_Click(null, null); }

        private void LoadList()
        {
            var mode = CurrentMode();
            try
            {
                IEnumerable<Row> rs;
                switch (mode)
                {
                    case ViewMode.ByStatus:
                        var status = (cmbStatus.SelectedItem ?? "IN_PROGRESS").ToString();
                        rs = _db.Execute(_psByStatus.Bind(status));
                        _cache = rs.Select(r => new TicketRow
                        {
                            Status = r.GetValue<string>("status"),
                            Id = r.GetValue<string>("id"),
                            ProductId = r.GetValue<string>("product_id"),
                            CustomerId = r.GetValue<string>("customer_id"),
                            CreatedAt = r.GetValue<TimeUuid>("created_at"),
                            CreatedAtUtc = r.GetValue<DateTime>("created_ts")
                        }).ToList();


                        SetStatus($"Đã nạp {_cache.Count} phiếu cho trạng thái {status}.");
                        break;

                    case ViewMode.ByCustomer:
                        var cid = (cmbCustomer.SelectedItem as LookupItem)?.Id;
                        if (string.IsNullOrEmpty(cid)) { MessageBox.Show("Vui lòng chọn khách hàng."); return; }
                        rs = _db.Execute(_psByCustomer.Bind(cid));
                        _cache = rs.Select(r => new TicketRow
                        {
                            CustomerId = r.GetValue<string>("customer_id"),
                            Id = r.GetValue<string>("id"),
                            ProductId = r.GetValue<string>("product_id"),
                            Status = r.GetValue<string>("status"),
                            CreatedAt = r.GetValue<TimeUuid>("created_at"),
                            CreatedAtUtc = r.GetValue<DateTime>("created_ts")
                        }).ToList();

                        SetStatus($"Đã nạp {_cache.Count} phiếu của khách '{cid}'.");
                        break;

                    case ViewMode.ByProduct:
                        var pid = (cmbProduct.SelectedItem as LookupItem)?.Id;
                        if (string.IsNullOrEmpty(pid)) { MessageBox.Show("Vui lòng chọn sản phẩm."); return; }
                        rs = _db.Execute(_psByProduct.Bind(pid));
                        _cache = rs.Select(r => new TicketRow
                        {
                            ProductId = r.GetValue<string>("product_id"),
                            Id = r.GetValue<string>("id"),
                            CustomerId = r.GetValue<string>("customer_id"),
                            Status = r.GetValue<string>("status"),
                            CreatedAt = r.GetValue<TimeUuid>("created_at"),
                            CreatedAtUtc = r.GetValue<DateTime>("created_ts")
                        }).ToList();

                        SetStatus($"Đã nạp {_cache.Count} phiếu của model '{pid}'.");
                        break;

                    default:
                        _cache.Clear();
                        break;
                }

                BindGrid(_cache);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách: " + ex.Message, "Phiếu bảo hành", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("Lỗi tải dữ liệu.");
            }
        }

        private void BindGrid(IEnumerable<TicketRow> data)
        {
            grid.SuspendLayout();
            try
            {
                grid.DataSource = null;
                grid.Rows.Clear();
                grid.Columns.Clear();

                var dt = new DataTable();
                // cột hiển thị
                dt.Columns.Add("CreatedAtUtc");
                dt.Columns.Add("TicketId");
                dt.Columns.Add("Status");
                dt.Columns.Add("ProductId");
                dt.Columns.Add("CustomerId");
                dt.Columns.Add("CreatedAtRaw"); // ẩn

                foreach (var t in data)
                {
                    var utc = t.CreatedAtUtc; // đã là DateTime (UTC)
                    var utcText = t.CreatedAtUtc.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'");
                    dt.Rows.Add(utcText, t.Id, t.Status, t.ProductId, t.CustomerId, t.CreatedAt.ToString());
                }

                grid.AutoGenerateColumns = true;
                grid.DataSource = dt;

                // Việt hoá tiêu đề cột
                if (grid.Columns.Contains("CreatedAtUtc")) grid.Columns["CreatedAtUtc"].HeaderText = "Thời điểm tạo (UTC)";
                if (grid.Columns.Contains("TicketId")) grid.Columns["TicketId"].HeaderText = "Mã phiếu";
                if (grid.Columns.Contains("Status")) grid.Columns["Status"].HeaderText = "Trạng thái";
                if (grid.Columns.Contains("ProductId")) grid.Columns["ProductId"].HeaderText = "Model";
                if (grid.Columns.Contains("CustomerId")) grid.Columns["CustomerId"].HeaderText = "Khách hàng";
                if (grid.Columns.Contains("CreatedAtRaw")) grid.Columns["CreatedAtRaw"].Visible = false;


                if (grid.Rows.Count > 0) grid.ClearSelection();
            }
            finally
            {
                grid.ResumeLayout();
                grid.Refresh();
            }
        }

        private ViewMode CurrentMode()
        {
            switch ((cmbMode.SelectedItem ?? "").ToString())
            {
                case "Theo khách hàng": return ViewMode.ByCustomer;
                case "Theo sản phẩm": return ViewMode.ByProduct;
                default: return ViewMode.ByStatus;
            }
        }

        private void ApplyModeVisibility()
        {
            var m = CurrentMode();
            cmbStatus.Enabled = (m == ViewMode.ByStatus);
            cmbCustomer.Enabled = (m == ViewMode.ByCustomer);
            cmbProduct.Enabled = (m == ViewMode.ByProduct);
        }

        private void SetStatus(string s) => lblFooter.Text = s;

        private string GetSelectedTicketId(out TimeUuid createdAt, out string productId, out string customerId, out string status)
        {
            createdAt = default; productId = null; customerId = null; status = null;
            if (grid.CurrentRow == null) return null;

            var row = grid.CurrentRow;
            var id = row.Cells["TicketId"].Value?.ToString();
            status = row.Cells["Status"].Value?.ToString();
            productId = row.Cells["ProductId"].Value?.ToString();
            customerId = row.Cells["CustomerId"].Value?.ToString();
            try
            {
                var raw = row.Cells["CreatedAtRaw"].Value?.ToString();
                createdAt = TimeUuid.Parse(raw);
            }
            catch { createdAt = default; }
            return id;

        }


        private void SelectRowById(string id)
        {
            if (grid.DataSource is DataTable)
            {
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if ((r.Cells["TicketId"].Value?.ToString() ?? "") == id)
                    {
                        r.Selected = true;
                        grid.FirstDisplayedScrollingRowIndex = r.Index;
                        break;
                    }
                }
            }
        }

        // ===== Hành động =====

        private void btnNewTicket_Click(object sender, EventArgs e)
        {
            if (_db == null) { MessageBox.Show("Chưa kết nối CSDL."); return; }

            // Dialog chọn model + khách (mã phiếu tự sinh, không yêu cầu nhập)
            var dlg = new NewTicketDialog(_db, _products, _customers);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var productId = dlg.SelectedProductId;
            var customerId = dlg.SelectedCustomerId;

            if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(customerId))
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Model và Khách hàng.");
                return;
            }

            // Sinh mã theo quy tắc và đảm bảo không trùng
            string ticketId;
            try
            {
                ticketId = GenerateTicketIdWithRule();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi sinh mã: " + ex.Message);
                return;
            }

            var createdTid = TimeUuid.NewId();
            var now = DateTime.UtcNow;

            try
            {
                var psMain = _db.Prepare(@"
            INSERT INTO warranty_tickets_by_id
            (id, product_id, customer_id, created_at, start_date, end_date, status, assigned_employee_id)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?);");
                _db.Execute(psMain.Bind(ticketId, productId, customerId, createdTid, now, null, "PENDING", null));

                var psByCus = _db.Prepare(@"
            INSERT INTO warranty_tickets_by_customer
            (customer_id, created_at, id, product_id, status)
            VALUES (?, ?, ?, ?, ?);");
                _db.Execute(psByCus.Bind(customerId, createdTid, ticketId, productId, "PENDING"));

                var psByProd = _db.Prepare(@"
            INSERT INTO warranty_tickets_by_product
            (product_id, created_at, id, customer_id, status)
            VALUES (?, ?, ?, ?, ?);");
                _db.Execute(psByProd.Bind(productId, createdTid, ticketId, customerId, "PENDING"));

                var psBySt = _db.Prepare(@"
            INSERT INTO warranty_tickets_by_status
            (status, created_at, id, product_id, customer_id)
            VALUES (?, ?, ?, ?, ?);");
                _db.Execute(psBySt.Bind("PENDING", createdTid, ticketId, productId, customerId));

                var psHist = _db.Prepare(@"
            INSERT INTO warranty_history
            (ticket_id, at, employee_id, action, old_status, new_status, description)
            VALUES (?, ?, ?, ?, ?, ?, ?);");
                _db.Execute(psHist.Bind(ticketId, createdTid, null, "STATUS_CHANGE", null, "PENDING", "Mở phiếu"));

                SetStatus($"Đã tạo phiếu {ticketId}.");
                LoadList();

                if (CurrentMode() == ViewMode.ByStatus && (cmbStatus.SelectedItem?.ToString() ?? "") == "PENDING")
                    SelectRowById(ticketId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo phiếu: " + ex.Message, "Tạo phiếu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            if (_db == null) { MessageBox.Show("Chưa kết nối CSDL."); return; }
            var id = GetSelectedTicketId(out _, out _, out _, out _);
            if (string.IsNullOrEmpty(id)) { MessageBox.Show("Vui lòng chọn một phiếu."); return; }

            try
            {
                var ps = _db.Prepare(@"
                    SELECT id, product_id, customer_id, created_at, toTimestamp(created_at) AS created_ts,
                           start_date, end_date, status, assigned_employee_id
                    FROM warranty_tickets_by_id WHERE id = ?;");

                var row = _db.Execute(ps.Bind(id)).SingleOrDefault();
                if (row == null) { MessageBox.Show("Không tìm thấy phiếu."); return; }

                DateTime? start = row.IsNull("start_date") ? (DateTime?)null : row.GetValue<DateTime>("start_date");
                DateTime? end = row.IsNull("end_date") ? (DateTime?)null : row.GetValue<DateTime>("end_date");
                var createdUtc = row.GetValue<DateTime>("created_ts").ToUniversalTime();

                var sb = new StringBuilder();
                sb.AppendLine($"Mã phiếu : {row.GetValue<string>("id")}");
                sb.AppendLine($"Model    : {row.GetValue<string>("product_id")}");
                sb.AppendLine($"Khách    : {row.GetValue<string>("customer_id")}");
                sb.AppendLine($"Trạng thái: {row.GetValue<string>("status")}");
                sb.AppendLine($"Đang phụ trách: {row.GetValue<string>("assigned_employee_id")}");
                sb.AppendLine($"Thời điểm tạo (UTC): {createdUtc:yyyy-MM-dd HH:mm:ss 'UTC'}");
                if (start.HasValue) sb.AppendLine($"Bắt đầu  (UTC): {start.Value.ToUniversalTime():yyyy-MM-dd HH:mm:ss 'UTC'}");
                if (end.HasValue) sb.AppendLine($"Kết thúc (UTC): {end.Value.ToUniversalTime():yyyy-MM-dd HH:mm:ss 'UTC'}");

                // Lịch sử: toTimestamp(at) AS at_ts
                var psHist = _db.Prepare(@"
                SELECT toTimestamp(at) AS at_ts, employee_id, action, old_status, new_status, description
                FROM warranty_history WHERE ticket_id = ?;");
                var hist = _db.Execute(psHist.Bind(id))
                              .Select(h => new
                              {
                                  At = h.GetValue<DateTime>("at_ts").ToUniversalTime(),
                                  Emp = h.GetValue<string>("employee_id"),
                                  Act = h.GetValue<string>("action"),
                                  Old = h.GetValue<string>("old_status"),
                                  New = h.GetValue<string>("new_status"),
                                  Des = h.GetValue<string>("description")
                              }).ToList();

                sb.AppendLine();
                sb.AppendLine("Lịch sử:");
                foreach (var h in hist)
                    sb.AppendLine($" - {h.At:yyyy-MM-dd HH:mm:ss 'UTC'} | {h.Act} | {h.Old} -> {h.New} | NV: {h.Emp} | {h.Des}");

                MessageBox.Show(sb.ToString(), "Chi tiết phiếu", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xem chi tiết: " + ex.Message, "Chi tiết phiếu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (_db == null) { MessageBox.Show("Chưa kết nối CSDL."); return; }
            var id = GetSelectedTicketId(out var createdAt, out var productId, out var customerId, out var status);
            if (string.IsNullOrEmpty(id)) { MessageBox.Show("Vui lòng chọn một phiếu."); return; }

            // Chỉ hiện danh sách KTV
            var pick = new SingleLookupDialog("Chọn kỹ thuật viên", _employees);
            if (pick.ShowDialog(this) != DialogResult.OK) return;
            var employeeId = pick.SelectedId;
            if (string.IsNullOrWhiteSpace(employeeId)) return;

            try
            {
                var assignAt = TimeUuid.NewId();

                var psEmp = _db.Prepare(@"INSERT INTO warranty_tickets_by_employee_and_status
                    (employee_id, status, assigned_at, ticket_id)
                    VALUES (?, ?, ?, ?);");
                _db.Execute(psEmp.Bind(employeeId, status, assignAt, id));

                var psUpd = _db.Prepare(@"UPDATE warranty_tickets_by_id
                                          SET assigned_employee_id = ?
                                          WHERE id = ?;");
                _db.Execute(psUpd.Bind(employeeId, id));

                var psHist = _db.Prepare(@"
                    INSERT INTO warranty_history
                    (ticket_id, at, employee_id, action, old_status, new_status, description)
                    VALUES (?, ?, ?, ?, ?, ?, ?);");
                _db.Execute(psHist.Bind(id, assignAt, employeeId, "NOTE", null, null, "Phân công"));


                SetStatus($"Đã phân công KTV {employeeId} cho phiếu {id}.");
                LoadList();
                SelectRowById(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phân công: " + ex.Message, "Phân công", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangeStatus_Click(object sender, EventArgs e)
        {
            if (_db == null) { MessageBox.Show("Chưa kết nối CSDL."); return; }
            var id = GetSelectedTicketId(out var createdAt, out var productId, out var customerId, out var oldStatus);
            if (string.IsNullOrEmpty(id)) { MessageBox.Show("Vui lòng chọn một phiếu."); return; }

            var dlg = new StatusChangeDialog(oldStatus);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            var newStatus = dlg.SelectedStatus?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(newStatus)) return;
            if (oldStatus == newStatus) { MessageBox.Show("Trạng thái không thay đổi."); return; }

            var desc = string.IsNullOrWhiteSpace(dlg.Note) ? null : dlg.Note.Trim();

            try
            {
                var nowTid = TimeUuid.NewId();
                var nowTs = DateTime.UtcNow;

                var psUpd = _db.Prepare(@"UPDATE warranty_tickets_by_id
                                          SET status = ?, end_date = ?
                                          WHERE id = ?;");
                DateTime? endDate = (newStatus == "COMPLETED" || newStatus == "REJECTED") ? nowTs : (DateTime?)null;
                _db.Execute(psUpd.Bind(newStatus, endDate, id));

                var psCus = _db.Prepare(@"UPDATE warranty_tickets_by_customer
                                          SET status = ?
                                          WHERE customer_id = ? AND created_at = ? AND id = ?;");
                _db.Execute(psCus.Bind(newStatus, customerId, createdAt, id));

                var psProd = _db.Prepare(@"UPDATE warranty_tickets_by_product
                                          SET status = ?
                                          WHERE product_id = ? AND created_at = ? AND id = ?;");
                _db.Execute(psProd.Bind(newStatus, productId, createdAt, id));

                var psDel = _db.Prepare(@"DELETE FROM warranty_tickets_by_status
                                          WHERE status = ? AND created_at = ? AND id = ?;");
                _db.Execute(psDel.Bind(oldStatus, createdAt, id));

                var psIns = _db.Prepare(@"INSERT INTO warranty_tickets_by_status
                                            (status, created_at, id, product_id, customer_id)
                                            VALUES (?, ?, ?, ?, ?);");
                _db.Execute(psIns.Bind(newStatus, createdAt, id, productId, customerId));

                var psHist = _db.Prepare(@"INSERT INTO warranty_history
                    (ticket_id, at, employee_id, action, old_status, new_status, description)
                    VALUES (?, ?, ?, ?, ?, ?, ?);");
                _db.Execute(psHist.Bind(id, nowTid, null, "STATUS_CHANGE", oldStatus, newStatus, desc));

                SetStatus($"Đã đổi trạng thái {id}: {oldStatus} → {newStatus}.");
                LoadList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đổi trạng thái: " + ex.Message, "Đổi trạng thái", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReplacePart_Click(object sender, EventArgs e)
        {
            if (_db == null) { MessageBox.Show("Chưa kết nối CSDL."); return; }
            var id = GetSelectedTicketId(out _, out _, out _, out _);
            if (string.IsNullOrEmpty(id)) { MessageBox.Show("Vui lòng chọn một phiếu."); return; }

            var pick = new ReplacePartDialog();
            if (pick.ShowDialog(this) != DialogResult.OK) return;

            var part = pick.PartCode?.Trim();
            var qty = pick.Quantity;
            if (string.IsNullOrWhiteSpace(part) || qty <= 0) { MessageBox.Show("Mã linh kiện/Số lượng không hợp lệ."); return; }

            try
            {
                var nowTid = TimeUuid.NewId();
                var now = DateTime.UtcNow;
                var yyyymm = $"{now:yyyy-MM}";

                var ps1 = _db.Prepare(@"INSERT INTO replaced_parts_by_ticket
                                        (ticket_id, replaced_at, part_code, quantity)
                                        VALUES (?, ?, ?, ?);");
                _db.Execute(ps1.Bind(id, nowTid, part, qty));

                var ps2 = _db.Prepare(@"INSERT INTO replaced_parts_by_part_and_month
                                        (part_code, yyyymm, replaced_at, ticket_id, quantity)
                                        VALUES (?, ?, ?, ?, ?);");
                _db.Execute(ps2.Bind(part, yyyymm, nowTid, id, qty));

                var psHist = _db.Prepare(@"INSERT INTO warranty_history
                    (ticket_id, at, employee_id, action, old_status, new_status, description)
                    VALUES (?, ?, ?, ?, ?, ?, ?);");
                _db.Execute(psHist.Bind(id, nowTid, null, "REPLACE_PART", null, null, $"Thay {part} x{qty}"));

                SetStatus($"Đã ghi linh kiện {part} x{qty} cho phiếu {id}.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi ghi linh kiện: " + ex.Message, "Ghi linh kiện", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class TicketRow
        {
            public string Status { get; set; }
            public TimeUuid CreatedAt { get; set; }
            public DateTime CreatedAtUtc { get; set; }
            public string Id { get; set; }
            public string ProductId { get; set; }
            public string CustomerId { get; set; }
        }

    }

    // ===== Dialogs =====

    internal class NewTicketDialog : Form
    {
        private readonly ComboBox cmbProduct = new ComboBox();
        private readonly ComboBox cmbCustomer = new ComboBox();
        private readonly Button btnOk = new Button();
        private readonly Button btnCancel = new Button();

        public string SelectedProductId => (cmbProduct.SelectedItem as LookupItem)?.Id;
        public string SelectedCustomerId => (cmbCustomer.SelectedItem as LookupItem)?.Id;

        public NewTicketDialog(ISession db, IEnumerable<LookupItem> products, IEnumerable<LookupItem> customers)
        {
            Text = "Tạo phiếu mới";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Width = 560; Height = 240; MaximizeBox = MinimizeBox = false;

            var note = new Label()
            {
                Left = 20,
                Top = 16,
                Width = 500,
                ForeColor = Color.DimGray,
                Text = "Mã phiếu sẽ được sinh tự động theo quy tắc: T-YYYYMMDD-SSSSS"
            };

            var lblP = new Label() { Left = 20, Top = 56, Width = 160, Text = "Sản phẩm *" };
            cmbProduct.Left = 190; cmbProduct.Top = 52; cmbProduct.Width = 330;
            cmbProduct.DropDownStyle = ComboBoxStyle.DropDown;
            cmbProduct.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbProduct.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbProduct.DataSource = products.ToList();

            var lblC = new Label() { Left = 20, Top = 96, Width = 160, Text = "Khách hàng *" };
            cmbCustomer.Left = 190; cmbCustomer.Top = 92; cmbCustomer.Width = 330;
            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbCustomer.DataSource = customers.ToList();

            btnOk.Text = "Đồng ý"; btnOk.Left = 340; btnOk.Top = 140; btnOk.Width = 80; btnOk.DialogResult = DialogResult.OK;
            btnCancel.Text = "Hủy"; btnCancel.Left = 430; btnCancel.Top = 140; btnCancel.Width = 90; btnCancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { note, lblP, cmbProduct, lblC, cmbCustomer, btnOk, btnCancel });
            AcceptButton = btnOk; CancelButton = btnCancel;
        }
    }



    internal class SingleLookupDialog : Form
    {
        private readonly ComboBox cmb = new ComboBox();
        private readonly Button btnOk = new Button();
        private readonly Button btnCancel = new Button();
        public string SelectedId => (cmb.SelectedItem as LookupItem)?.Id;

        public SingleLookupDialog(string title, IEnumerable<LookupItem> items)
        {
            Text = title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Width = 520; Height = 160; MaximizeBox = MinimizeBox = false;

            var lbl = new Label() { Left = 20, Top = 20, Width = 120, Text = "Chọn" };
            cmb.Left = 150; cmb.Top = 16; cmb.Width = 320;
            cmb.DropDownStyle = ComboBoxStyle.DropDown;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmb.DataSource = items.ToList();

            btnOk.Text = "Đồng ý"; btnOk.Left = 300; btnOk.Top = 60; btnOk.Width = 80; btnOk.DialogResult = DialogResult.OK;
            btnCancel.Text = "Hủy"; btnCancel.Left = 390; btnCancel.Top = 60; btnCancel.Width = 90; btnCancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lbl, cmb, btnOk, btnCancel });
            AcceptButton = btnOk; CancelButton = btnCancel;
        }
    }

    // Dialog đổi trạng thái
    internal class StatusChangeDialog : Form
    {
        private readonly ComboBox cmb = new ComboBox();
        private readonly TextBox txtNote = new TextBox();
        private readonly Button btnOk = new Button();
        private readonly Button btnCancel = new Button();

        public string SelectedStatus => (cmb.SelectedItem ?? cmb.Text)?.ToString();
        public string Note => txtNote.Text;

        public StatusChangeDialog(string currentStatus)
        {
            Text = "Đổi trạng thái";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Width = 520; Height = 220; MaximizeBox = MinimizeBox = false;

            var lblS = new Label() { Left = 20, Top = 20, Width = 140, Text = "Trạng thái mới" };
            cmb.Left = 180; cmb.Top = 16; cmb.Width = 300;
            cmb.DropDownStyle = ComboBoxStyle.DropDown;
            cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmb.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmb.Items.AddRange(new object[] { "PENDING", "IN_PROGRESS", "WAITING_PARTS", "COMPLETED", "REJECTED" });
            if (!string.IsNullOrWhiteSpace(currentStatus)) cmb.Text = currentStatus;

            var lblN = new Label() { Left = 20, Top = 60, Width = 140, Text = "Ghi chú" };
            txtNote.Left = 180; txtNote.Top = 56; txtNote.Width = 300;

            btnOk.Text = "Đồng ý"; btnOk.Left = 300; btnOk.Top = 120; btnOk.Width = 80; btnOk.DialogResult = DialogResult.OK;
            btnCancel.Text = "Hủy"; btnCancel.Left = 390; btnCancel.Top = 120; btnCancel.Width = 90; btnCancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblS, cmb, lblN, txtNote, btnOk, btnCancel });
            AcceptButton = btnOk; CancelButton = btnCancel;
        }
    }

    // Dialog nhập linh kiện
    internal class ReplacePartDialog : Form
    {
        private readonly TextBox txtPart = new TextBox();
        private readonly NumericUpDown nudQty = new NumericUpDown();
        private readonly Button btnOk = new Button();
        private readonly Button btnCancel = new Button();

        public string PartCode => txtPart.Text?.Trim();
        public int Quantity => (int)nudQty.Value;

        public ReplacePartDialog()
        {
            Text = "Ghi linh kiện thay thế";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Width = 520; Height = 200; MaximizeBox = MinimizeBox = false;

            var lblP = new Label() { Left = 20, Top = 20, Width = 140, Text = "Mã linh kiện" };
            txtPart.Left = 180; txtPart.Top = 16; txtPart.Width = 300;

            var lblQ = new Label() { Left = 20, Top = 60, Width = 140, Text = "Số lượng" };
            nudQty.Left = 180; nudQty.Top = 56; nudQty.Width = 100;
            nudQty.Minimum = 1; nudQty.Maximum = 1000; nudQty.Value = 1;

            btnOk.Text = "Đồng ý"; btnOk.Left = 300; btnOk.Top = 110; btnOk.Width = 80; btnOk.DialogResult = DialogResult.OK;
            btnCancel.Text = "Hủy"; btnCancel.Left = 390; btnCancel.Top = 110; btnCancel.Width = 90; btnCancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblP, txtPart, lblQ, nudQty, btnOk, btnCancel });
            AcceptButton = btnOk; CancelButton = btnCancel;
        }
    }
}
