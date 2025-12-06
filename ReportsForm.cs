using Cassandra;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class ReportsForm : Form
    {
        private ISession _db => Program.DbSession;



        // Giữ cấu hình đã dò
        private string _sourceTable = "replaced_parts_by_part_and_month";
        private string _colMonth = "month";
        private string _colPart = "part_code";

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void ReportsForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (_db == null) return;

                // 1) Chọn bảng nguồn đang có dữ liệu
                _sourceTable = PickSourceTable();

                // 2) Dò tên cột
                DetectColumns(_sourceTable, out _colPart, out _colMonth);

                // 3) Nạp tháng từ DB (giữ nguyên giá trị cột)
                LoadMonthsFromDb(_sourceTable, _colMonth);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo báo cáo: " + ex.Message, "Báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            var part = (txtPart.Text ?? "").Trim();
            var sel = cmbMonth.SelectedItem as MonthItem;

            if (sel == null)
            {
                MessageBox.Show("Vui lòng chọn giá trị tháng từ danh sách.", "Báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var monthRaw = sel.Raw;   
            var monthText = sel.Text; 

            try
            {
                if (_db == null)
                {
                    MessageBox.Show("Chưa kết nối cơ sở dữ liệu.", "Báo cáo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                RowSet rs;

                // Nếu là bảng tổng hợp
                if (string.Equals(_sourceTable, "replaced_parts_by_part_and_month", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(part) && !string.IsNullOrEmpty(_colPart))
                    {
                        rs = _db.Execute(new SimpleStatement(
                            $"SELECT * FROM {_sourceTable} WHERE {_colPart} = ? AND {_colMonth} = ? ALLOW FILTERING;",
                            part, monthRaw));
                    }
                    else
                    {
                        rs = _db.Execute(new SimpleStatement(
                            $"SELECT * FROM {_sourceTable} WHERE {_colMonth} = ? ALLOW FILTERING;",
                            monthRaw));
                    }
                }
                else // replaced_parts (bảng chi tiết)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        rs = _db.Execute(new SimpleStatement(
                            $"SELECT * FROM {_sourceTable} WHERE part_code = ? AND {_colMonth} = ? ALLOW FILTERING;",
                            part, monthRaw));
                    }
                    else
                    {
                        rs = _db.Execute(new SimpleStatement(
                            $"SELECT * FROM {_sourceTable} WHERE {_colMonth} = ? ALLOW FILTERING;",
                            monthRaw));
                    }
                }

                var dt = RowSetToDataTable(rs);
                grid.DataSource = dt;
                grid.ClearSelection();
                grid.CurrentCell = null;
                lblStatus.Text = $"Kết quả: {dt.Rows.Count} dòng cho tháng {monthText}" +
                                 (string.IsNullOrEmpty(part) ? "" : $", linh kiện '{part}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chạy báo cáo: " + ex.Message, "Báo cáo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---- Helpers ----

        private string PickSourceTable()
        {
            // Thử bảng tổng hợp
            try
            {
                var rs = _db.Execute(new SimpleStatement(
                    "SELECT * FROM replaced_parts_by_part_and_month LIMIT 1;"));
                if (rs != null && rs.GetEnumerator().MoveNext())
                    return "replaced_parts_by_part_and_month";
            }
            catch { /* bảng không tồn tại hoặc lỗi quyền… */ }

            try
            {
                var rs = _db.Execute(new SimpleStatement(
                    "SELECT * FROM replaced_parts LIMIT 1;"));
                if (rs != null && rs.GetEnumerator().MoveNext())
                    return "replaced_parts";
            }
            catch { }

            return "replaced_parts_by_part_and_month";
        }

        private void DetectColumns(string table, out string partCol, out string monthCol)
        {
            partCol = "part_code";
            monthCol = "month";
            try
            {
                var rs = _db.Execute(new SimpleStatement($"SELECT * FROM {table} LIMIT 1;"));
                var cols = rs.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                // part
                if (cols.Contains("part_code")) partCol = "part_code";
                else if (cols.Contains("part")) partCol = "part";
                else partCol = null; // có thể null với 1 số schema tổng hợp theo tháng không phân tách part

                // month
                if (cols.Contains("month")) monthCol = "month";
                else if (cols.Contains("yyyymm")) monthCol = "yyyymm";
                else monthCol = "month";
            }
            catch
            {
                // giữ mặc định
            }
        }

        private void LoadMonthsFromDb(string table, string monthCol)
        {
            cmbMonth.Items.Clear();

            var rs = _db.Execute(new SimpleStatement(
                $"SELECT {monthCol} FROM {table} LIMIT 5000;"));

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var items = new List<MonthItem>();

            foreach (var row in rs)
            {
                object raw = null;

                // cố gắng lấy object để giữ đúng kiểu
                try { raw = row.GetValue<object>(monthCol); }
                catch
                {
                    try { raw = row.GetValue<string>(monthCol); } catch { }
                }

                if (raw == null) continue;

                var text = raw.ToString(); // hiển thị nguyên văn
                if (seen.Add(text))
                    items.Add(new MonthItem { Raw = raw, Text = text });
            }

            // Sắp xếp cho dễ nhìn (theo text)
            items = items.OrderByDescending(i => i.Text).ToList();

            foreach (var it in items) cmbMonth.Items.Add(it);
            if (cmbMonth.Items.Count > 0) cmbMonth.SelectedIndex = 0;
        }

        private DataTable RowSetToDataTable(RowSet rs)
        {
            var dt = new DataTable();
            if (rs == null) return dt;

            var colDefs = rs.Columns.ToList();
            if (colDefs.Count == 0) return dt;

            foreach (var c in colDefs)
                dt.Columns.Add(c.Name);

            foreach (var row in rs)
            {
                var values = new object[colDefs.Count];
                for (int i = 0; i < colDefs.Count; i++)
                {
                    var colName = colDefs[i].Name;
                    try
                    {
                        if (row.IsNull(colName))
                        {
                            values[i] = null;
                        }
                        else
                        {
                            var raw = row.GetValue<object>(colName);

                            if (string.Equals(colName, "replaced_at", StringComparison.OrdinalIgnoreCase))
                                values[i] = FormatHumanTime(raw);
                            else
                                values[i] = raw;
                        }
                    }
                    catch
                    {
                        values[i] = null;
                    }
                }
                dt.Rows.Add(values);
            }
            return dt;
        }


        private static string FormatHumanTime(object val)
        {
            if (val == null) return null;

            if (val is Guid g)
            {
                try
                {
                    var timeUuid = TimeUuid.Parse(g.ToString());
                    var dto = timeUuid.GetDate();
                    return dto.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch { /* fall through */ }
            }

            if (val is DateTimeOffset dto2)
                return dto2.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");

            if (val is DateTime dt)
            {
                return dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            }

            return val.ToString();
        }


        private class MonthItem
        {
            public object Raw { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;
        }
    }
}
