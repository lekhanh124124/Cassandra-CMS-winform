using Cassandra;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    public partial class DashboardForm : Form
    {
        private ISession _db => Program.DbSession;

        // Danh sách trạng thái chuẩn hóa cho biểu đồ hôm nay
        private static readonly string[] STATUSES = new[]
        {
            "PENDING", "IN_PROGRESS", "WAITING_PARTS", "COMPLETED", "REJECTED"
        };

        public DashboardForm()
        {
            InitializeComponent();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            TryLoadSummaryFromDb();
            TryLoadTodayChart();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            TryLoadSummaryFromDb();
            TryLoadTodayChart();
        }

        /// <summary>
        /// Đếm trực tiếp trên bảng phân vùng theo trạng thái:
        ///   warranty_tickets_by_status(status, created_at, id, ...)
        /// - Tổng IN_PROGRESS, WAITING_PARTS
        /// - Tổng COMPLETED trong HÔM NAY (lọc theo created_at nằm trong hôm nay)
        /// Lưu ý: "COMPLETED hôm nay" ở đây đang hiểu là các phiếu có status hiện tại là COMPLETED
        /// và created_at thuộc "hôm nay". Nếu bạn muốn "ngày hoàn thành", nên tạo thêm bảng tổng hợp theo ngày hoàn thành.
        /// </summary>
        private void TryLoadSummaryFromDb()
        {
            try
            {
                if (_db == null) return;

                // --- 1) Tổng IN_PROGRESS
                long inProgress = CountByStatus("IN_PROGRESS");

                // --- 2) Tổng WAITING_PARTS
                long waitingParts = CountByStatus("WAITING_PARTS");

                // --- 3) Tổng COMPLETED TRONG HÔM NAY
                GetTodayUtcRange(out DateTime startUtc, out DateTime endUtc);
                long completedToday = CountByStatusWithinDay("COMPLETED", startUtc, endUtc);

                // Gán ra UI (chỉ hiển thị số cho sạch)
                lblInProgress.Text = inProgress.ToString("N0", CultureInfo.InvariantCulture);
                lblWaitingParts.Text = waitingParts.ToString("N0", CultureInfo.InvariantCulture);
                lblCompleted.Text = completedToday.ToString("N0", CultureInfo.InvariantCulture);
            }
            catch
            {
                // mềm mại, không chặn UI
            }
        }

        // Ensure adding points uses DataPoint or AddXY (prevents 'int'...AxisLabel/ToolTip errors)
        private void TryLoadTodayChart()
        {
            if (this.chartToday == null || this.chartToday.Series.Count == 0) return;

            this.chartToday.Series[0].Points.Clear();

            DateTime startUtc, endUtc;
            GetTodayUtcRange(out startUtc, out endUtc);

            foreach (var status in STATUSES)
            {
                long count = CountByStatusWithinDay(status, startUtc, endUtc);
                int idx = this.chartToday.Series[0].Points.AddXY(status, count);
                var pt = this.chartToday.Series[0].Points[idx];
                pt.ToolTip = status + ": " + count;
                // pt.Label is auto when IsValueShownAsLabel = true; set explicitly if you want:
                // pt.Label = count.ToString();
            }
        }

        /// <summary>
        /// Đếm số bản ghi của 1 trạng thái trên bảng warranty_tickets_by_status (toàn bộ partition).
        /// </summary>
        private long CountByStatus(string status)
        {
            // SELECT COUNT(*) FROM warranty_tickets_by_status WHERE status = ?;
            var stmt = _db.Prepare("SELECT COUNT(*) FROM warranty_tickets_by_status WHERE status = ?;");
            var row = _db.Execute(stmt.Bind(status)).FirstOrDefault();
            if (row == null) return 0L;

            try
            {
                // C* driver trả về cột "count" kiểu long
                return row.GetValue<long>("count");
            }
            catch
            {
                // fallback theo index cột 0
                try { return row.GetValue<long>(0); } catch { return 0L; }
            }
        }

        /// <summary>
        /// Đếm số bản ghi của 1 trạng thái TRONG KHOẢNG "hôm nay" (UTC) theo cột created_at (timeuuid).
        /// created_at > minTimeuuid(startUtc) AND created_at <= maxTimeuuid(endUtc)
        /// </summary>
        private long CountByStatusWithinDay(string status, DateTime startUtc, DateTime endUtc)
        {
            // Lưu ý: dùng minTimeuuid/maxTimeuuid (KHÁC với toTimeUuid). Trước đó bạn gặp lỗi totimeuuid.
            var cql = @"SELECT COUNT(*) FROM warranty_tickets_by_status
                        WHERE status = ?
                          AND created_at > minTimeuuid(?)
                          AND created_at <= maxTimeuuid(?);";
            var ps = _db.Prepare(cql);
            var row = _db.Execute(ps.Bind(status, startUtc, endUtc)).FirstOrDefault();
            if (row == null) return 0L;

            try { return row.GetValue<long>("count"); }
            catch
            {
                try { return row.GetValue<long>(0); } catch { return 0L; }
            }
        }

        /// <summary>
        /// Tính khoảng "hôm nay" theo LOCAL TIME rồi đổi sang UTC để dùng min/maxTimeuuid.
        /// Nếu bạn muốn tính thuần UTC day thì thay DateTime.Today bằng DateTime.UtcNow.Date.
        /// </summary>
        private static void GetTodayUtcRange(out DateTime startUtc, out DateTime endUtc)
        {
            // Hôm nay (local)
            var localStart = DateTime.Today;                       // 00:00:00 local
            var localEnd = localStart.AddDays(1).AddTicks(-1);     // 23:59:59.9999999 local

            // Đổi sang UTC
            startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart);
            endUtc = TimeZoneInfo.ConvertTimeToUtc(localEnd);
        }
    }
}
