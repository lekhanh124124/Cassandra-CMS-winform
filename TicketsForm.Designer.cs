using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBaoHanhSanPham
{
    partial class TicketsForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelTop;
        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnRefresh;

        private DataGridView grid;
        private Panel panelRight;

        // Bộ lọc/chế độ xem
        private Label lblMode;
        private ComboBox cmbMode;          // Theo Trạng thái / Theo Khách hàng / Theo Sản phẩm
        private Label lblStatus;
        private ComboBox cmbStatus;

        private Label lblCustomer;
        private ComboBox cmbCustomer;      // dropdown khách
        private Label lblProduct;
        private ComboBox cmbProduct;       // dropdown sản phẩm

        // Hành động
        private Button btnNewTicket;
        private Button btnViewDetail;
        private Button btnAssign;
        private Button btnChangeStatus;
        private Button btnReplacePart;

        private Label lblFooter;

        protected override void Dispose(bool disposing)
        { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();

            this.grid = new System.Windows.Forms.DataGridView();
            this.panelRight = new System.Windows.Forms.Panel();

            this.lblMode = new System.Windows.Forms.Label();
            this.cmbMode = new System.Windows.Forms.ComboBox();

            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();

            this.lblCustomer = new System.Windows.Forms.Label();
            this.cmbCustomer = new System.Windows.Forms.ComboBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cmbProduct = new System.Windows.Forms.ComboBox();

            this.btnNewTicket = new System.Windows.Forms.Button();
            this.btnViewDetail = new System.Windows.Forms.Button();
            this.btnAssign = new System.Windows.Forms.Button();
            this.btnChangeStatus = new System.Windows.Forms.Button();
            this.btnReplacePart = new System.Windows.Forms.Button();

            this.lblFooter = new System.Windows.Forms.Label();

            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();

            // panelTop
            this.panelTop.BackColor = Color.FromArgb(248, 249, 250);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.btnSearch);
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new Padding(20);
            this.panelTop.Size = new System.Drawing.Size(1100, 70);

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(33, 37, 41);
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Text = "Phiếu bảo hành";

            // txtSearch
            this.txtSearch.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearch.Font = new Font("Segoe UI", 11F);
            this.txtSearch.ForeColor = Color.FromArgb(73, 80, 87);
            this.txtSearch.Location = new System.Drawing.Point(600, 22);
            this.txtSearch.Size = new System.Drawing.Size(300, 27);
            this.txtSearch.Text = "Tìm (id / sản phẩm / khách)...";

            // btnSearch
            this.btnSearch.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnSearch.BackColor = Color.FromArgb(0, 123, 255);
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = FlatStyle.Flat;
            this.btnSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnSearch.ForeColor = Color.White;
            this.btnSearch.Location = new System.Drawing.Point(910, 20);
            this.btnSearch.Size = new System.Drawing.Size(80, 32);
            this.btnSearch.Text = "Lọc";
            this.btnSearch.Click += new EventHandler(this.btnSearch_Click);

            // btnRefresh
            this.btnRefresh.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnRefresh.BackColor = Color.FromArgb(108, 117, 125);
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(1000, 20);
            this.btnRefresh.Size = new System.Drawing.Size(80, 32);
            this.btnRefresh.Text = "Tải lại";
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // grid
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.BackgroundColor = Color.White;
            this.grid.BorderStyle = BorderStyle.None;
            this.grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            this.grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(73, 80, 87);
            this.grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 8, 12, 8);
            this.grid.ColumnHeadersHeight = 44;
            this.grid.Dock = DockStyle.Fill;
            this.grid.EnableHeadersVisualStyles = false;
            this.grid.GridColor = Color.FromArgb(233, 236, 239);
            this.grid.Location = new System.Drawing.Point(0, 70);
            this.grid.MultiSelect = false;
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 36;
            this.grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.grid.CellDoubleClick += new DataGridViewCellEventHandler(this.grid_CellDoubleClick);

            // panelRight
            this.panelRight.BackColor = Color.FromArgb(248, 249, 250);
            this.panelRight.Dock = DockStyle.Right;
            this.panelRight.Padding = new Padding(20);
            this.panelRight.Width = 360;

            // lblMode + cmbMode
            this.lblMode.AutoSize = true;
            this.lblMode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblMode.ForeColor = Color.FromArgb(73, 80, 87);
            this.lblMode.Location = new System.Drawing.Point(20, 20);
            this.lblMode.Text = "Chế độ xem";

            this.cmbMode.DropDownStyle = ComboBoxStyle.DropDownList;
            // IMPORTANT: Set AutoCompleteSource before AutoCompleteMode
            this.cmbMode.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cmbMode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cmbMode.Font = new Font("Segoe UI", 10F);
            this.cmbMode.Items.AddRange(new object[] { "Theo trạng thái", "Theo khách hàng", "Theo sản phẩm" });
            this.cmbMode.Location = new System.Drawing.Point(24, 44);
            this.cmbMode.Width = 300;
            this.cmbMode.SelectedIndexChanged += new EventHandler(this.cmbMode_SelectedIndexChanged);

            // lblStatus + cmbStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblStatus.ForeColor = Color.FromArgb(73, 80, 87);
            this.lblStatus.Location = new System.Drawing.Point(20, 84);
            this.lblStatus.Text = "Trạng thái";

            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cmbStatus.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cmbStatus.Font = new Font("Segoe UI", 10F);
            this.cmbStatus.Items.AddRange(new object[] { "PENDING", "IN_PROGRESS", "WAITING_PARTS", "COMPLETED", "REJECTED" });
            this.cmbStatus.Location = new System.Drawing.Point(24, 108);
            this.cmbStatus.Width = 300;

            // Khách hàng
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblCustomer.ForeColor = Color.FromArgb(73, 80, 87);
            this.lblCustomer.Location = new System.Drawing.Point(20, 148);
            this.lblCustomer.Text = "Khách hàng";
            this.cmbCustomer.DropDownStyle = ComboBoxStyle.DropDown;
            this.cmbCustomer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cmbCustomer.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cmbCustomer.Font = new Font("Segoe UI", 10F);
            this.cmbCustomer.Location = new System.Drawing.Point(24, 172);
            this.cmbCustomer.Width = 300;

            // Sản phẩm
            this.lblProduct.AutoSize = true;
            this.lblProduct.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblProduct.ForeColor = Color.FromArgb(73, 80, 87);
            this.lblProduct.Location = new System.Drawing.Point(20, 212);
            this.lblProduct.Text = "Sản phẩm (model)";
            this.cmbProduct.DropDownStyle = ComboBoxStyle.DropDown;
            this.cmbProduct.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.cmbProduct.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cmbProduct.Font = new Font("Segoe UI", 10F);
            this.cmbProduct.Location = new System.Drawing.Point(24, 236);
            this.cmbProduct.Width = 300;

            // Buttons
            int bw = 140; int bh = 34; int left = 24; int gap = 10; int top = 290;

            this.btnNewTicket = new Button();
            this.btnNewTicket.BackColor = Color.FromArgb(40, 167, 69);
            this.btnNewTicket.FlatAppearance.BorderSize = 0;
            this.btnNewTicket.FlatStyle = FlatStyle.Flat;
            this.btnNewTicket.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnNewTicket.ForeColor = Color.White;
            this.btnNewTicket.Location = new System.Drawing.Point(left, top);
            this.btnNewTicket.Size = new System.Drawing.Size(bw, bh);
            this.btnNewTicket.Text = "Tạo phiếu";
            this.btnNewTicket.Click += new EventHandler(this.btnNewTicket_Click);

            this.btnViewDetail = new Button();
            this.btnViewDetail.BackColor = Color.FromArgb(0, 123, 255);
            this.btnViewDetail.FlatAppearance.BorderSize = 0;
            this.btnViewDetail.FlatStyle = FlatStyle.Flat;
            this.btnViewDetail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnViewDetail.ForeColor = Color.White;
            this.btnViewDetail.Location = new System.Drawing.Point(left + bw + gap, top);
            this.btnViewDetail.Size = new System.Drawing.Size(bw, bh);
            this.btnViewDetail.Text = "Xem chi tiết";
            this.btnViewDetail.Click += new EventHandler(this.btnViewDetail_Click);

            top += bh + gap;

            this.btnAssign = new Button();
            this.btnAssign.BackColor = Color.FromArgb(255, 193, 7);
            this.btnAssign.FlatAppearance.BorderSize = 0;
            this.btnAssign.FlatStyle = FlatStyle.Flat;
            this.btnAssign.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnAssign.ForeColor = Color.FromArgb(33, 37, 41);
            this.btnAssign.Location = new System.Drawing.Point(left, top);
            this.btnAssign.Size = new System.Drawing.Size(bw, bh);
            this.btnAssign.Text = "Phân công KTV";
            this.btnAssign.Click += new EventHandler(this.btnAssign_Click);

            this.btnChangeStatus = new Button();
            this.btnChangeStatus.BackColor = Color.FromArgb(108, 117, 125);
            this.btnChangeStatus.FlatAppearance.BorderSize = 0;
            this.btnChangeStatus.FlatStyle = FlatStyle.Flat;
            this.btnChangeStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnChangeStatus.ForeColor = Color.White;
            this.btnChangeStatus.Location = new System.Drawing.Point(left + bw + gap, top);
            this.btnChangeStatus.Size = new System.Drawing.Size(bw, bh);
            this.btnChangeStatus.Text = "Đổi trạng thái";
            this.btnChangeStatus.Click += new EventHandler(this.btnChangeStatus_Click);

            top += bh + gap;

            this.btnReplacePart = new Button();
            this.btnReplacePart.BackColor = Color.FromArgb(220, 53, 69);
            this.btnReplacePart.FlatAppearance.BorderSize = 0;
            this.btnReplacePart.FlatStyle = FlatStyle.Flat;
            this.btnReplacePart.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnReplacePart.ForeColor = Color.White;
            this.btnReplacePart.Location = new System.Drawing.Point(left, top);
            this.btnReplacePart.Size = new System.Drawing.Size(bw, bh);
            this.btnReplacePart.Text = "Ghi linh kiện";
            this.btnReplacePart.Click += new EventHandler(this.btnReplacePart_Click);

            // Footer
            this.lblFooter = new Label();
            this.lblFooter.Dock = DockStyle.Bottom;
            this.lblFooter.BackColor = Color.FromArgb(248, 249, 250);
            this.lblFooter.ForeColor = Color.FromArgb(108, 117, 125);
            this.lblFooter.TextAlign = ContentAlignment.MiddleLeft;
            this.lblFooter.Padding = new Padding(20, 0, 0, 0);
            this.lblFooter.Height = 36;
            this.lblFooter.Text = "Sẵn sàng";

            // panelRight add
            this.panelRight.Controls.Add(this.lblMode);
            this.panelRight.Controls.Add(this.cmbMode);
            this.panelRight.Controls.Add(this.lblStatus);
            this.panelRight.Controls.Add(this.cmbStatus);
            this.panelRight.Controls.Add(this.lblCustomer);
            this.panelRight.Controls.Add(this.cmbCustomer);
            this.panelRight.Controls.Add(this.lblProduct);
            this.panelRight.Controls.Add(this.cmbProduct);
            this.panelRight.Controls.Add(this.btnNewTicket);
            this.panelRight.Controls.Add(this.btnViewDetail);
            this.panelRight.Controls.Add(this.btnAssign);
            this.panelRight.Controls.Add(this.btnChangeStatus);
            this.panelRight.Controls.Add(this.btnReplacePart);

            // TicketsForm
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new System.Drawing.Size(1100, 680);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.lblFooter);
            this.MinimumSize = new System.Drawing.Size(1000, 640);
            this.Name = "TicketsForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Quản lý phiếu bảo hành";
            this.Load += new EventHandler(this.TicketsForm_Load);

            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
