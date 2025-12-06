using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace QuanLyBaoHanhSanPham
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelRoot;
        private Panel panelStats;

        private Panel cardInProgress;
        private Panel cardWaitingParts;
        private Panel cardCompleted;

        private Label lblInProgressTitle;
        private Label lblWaitingPartsTitle;
        private Label lblCompletedTitle;

        private Label lblInProgress;
        private Label lblWaitingParts;
        private Label lblCompleted;

        private GroupBox grpToday;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartToday; // Use fully qualified name
        private Button btnRefresh;

        protected override void Dispose(bool disposing)
        { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.panelRoot = new System.Windows.Forms.Panel();
            this.grpToday = new System.Windows.Forms.GroupBox();
            this.chartToday = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelStats = new System.Windows.Forms.Panel();
            this.cardCompleted = new System.Windows.Forms.Panel();
            this.lblCompleted = new System.Windows.Forms.Label();
            this.lblCompletedTitle = new System.Windows.Forms.Label();
            this.cardWaitingParts = new System.Windows.Forms.Panel();
            this.lblWaitingParts = new System.Windows.Forms.Label();
            this.lblWaitingPartsTitle = new System.Windows.Forms.Label();
            this.cardInProgress = new System.Windows.Forms.Panel();
            this.lblInProgress = new System.Windows.Forms.Label();
            this.lblInProgressTitle = new System.Windows.Forms.Label();
            this.panelRoot.SuspendLayout();
            this.grpToday.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartToday)).BeginInit();
            this.panelStats.SuspendLayout();
            this.cardCompleted.SuspendLayout();
            this.cardWaitingParts.SuspendLayout();
            this.cardInProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRoot
            // 
            this.panelRoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelRoot.Controls.Add(this.grpToday);
            this.panelRoot.Controls.Add(this.panelStats);
            this.panelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRoot.Location = new System.Drawing.Point(0, 0);
            this.panelRoot.Name = "panelRoot";
            this.panelRoot.Padding = new System.Windows.Forms.Padding(20);
            this.panelRoot.Size = new System.Drawing.Size(1000, 600);
            this.panelRoot.TabIndex = 0;
            // 
            // grpToday
            // 
            this.grpToday.BackColor = System.Drawing.Color.White;
            this.grpToday.Controls.Add(this.btnRefresh);
            this.grpToday.Controls.Add(this.chartToday);
            this.grpToday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpToday.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.grpToday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.grpToday.Location = new System.Drawing.Point(20, 160);
            this.grpToday.Name = "grpToday";
            this.grpToday.Padding = new System.Windows.Forms.Padding(20);
            this.grpToday.Size = new System.Drawing.Size(960, 420);
            this.grpToday.TabIndex = 1;
            this.grpToday.TabStop = false;
            this.grpToday.Text = "📊 Công việc hôm nay";
            // 
            // chartToday
            // 
            chartArea3.AxisX.Interval = 1D;
            chartArea3.AxisX.MajorGrid.Enabled = false;
            chartArea3.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea3.Name = "MainArea";
            this.chartToday.ChartAreas.Add(chartArea3);
            this.chartToday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartToday.Location = new System.Drawing.Point(20, 52);
            this.chartToday.Name = "chartToday";
            series3.ChartArea = "MainArea";
            series3.IsValueShownAsLabel = true;
            series3.Name = "Hôm nay";
            this.chartToday.Series.Add(series3);
            this.chartToday.Size = new System.Drawing.Size(920, 348);
            this.chartToday.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(800, 30);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(140, 36);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄 Nạp dữ liệu";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // panelStats
            // 
            this.panelStats.BackColor = System.Drawing.Color.Transparent;
            this.panelStats.Controls.Add(this.cardCompleted);
            this.panelStats.Controls.Add(this.cardWaitingParts);
            this.panelStats.Controls.Add(this.cardInProgress);
            this.panelStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStats.Location = new System.Drawing.Point(20, 20);
            this.panelStats.Name = "panelStats";
            this.panelStats.Size = new System.Drawing.Size(960, 140);
            this.panelStats.TabIndex = 0;
            // 
            // cardCompleted
            // 
            this.cardCompleted.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.cardCompleted.Controls.Add(this.lblCompleted);
            this.cardCompleted.Controls.Add(this.lblCompletedTitle);
            this.cardCompleted.Location = new System.Drawing.Point(520, 0);
            this.cardCompleted.Name = "cardCompleted";
            this.cardCompleted.Padding = new System.Windows.Forms.Padding(20);
            this.cardCompleted.Size = new System.Drawing.Size(240, 100);
            this.cardCompleted.TabIndex = 2;
            // 
            // lblCompleted
            // 
            this.lblCompleted.AutoSize = true;
            this.lblCompleted.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCompleted.ForeColor = System.Drawing.Color.White;
            this.lblCompleted.Location = new System.Drawing.Point(20, 40);
            this.lblCompleted.Name = "lblCompleted";
            this.lblCompleted.Size = new System.Drawing.Size(46, 54);
            this.lblCompleted.TabIndex = 1;
            this.lblCompleted.Text = "0";
            // 
            // lblCompletedTitle
            // 
            this.lblCompletedTitle.AutoSize = true;
            this.lblCompletedTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCompletedTitle.ForeColor = System.Drawing.Color.White;
            this.lblCompletedTitle.Location = new System.Drawing.Point(20, 15);
            this.lblCompletedTitle.Name = "lblCompletedTitle";
            this.lblCompletedTitle.Size = new System.Drawing.Size(199, 20);
            this.lblCompletedTitle.TabIndex = 0;
            this.lblCompletedTitle.Text = "HOÀN THÀNH (HÔM NAY)";
            // 
            // cardWaitingParts
            // 
            this.cardWaitingParts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.cardWaitingParts.Controls.Add(this.lblWaitingParts);
            this.cardWaitingParts.Controls.Add(this.lblWaitingPartsTitle);
            this.cardWaitingParts.Location = new System.Drawing.Point(260, 0);
            this.cardWaitingParts.Name = "cardWaitingParts";
            this.cardWaitingParts.Padding = new System.Windows.Forms.Padding(20);
            this.cardWaitingParts.Size = new System.Drawing.Size(240, 100);
            this.cardWaitingParts.TabIndex = 1;
            // 
            // lblWaitingParts
            // 
            this.lblWaitingParts.AutoSize = true;
            this.lblWaitingParts.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblWaitingParts.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblWaitingParts.Location = new System.Drawing.Point(20, 40);
            this.lblWaitingParts.Name = "lblWaitingParts";
            this.lblWaitingParts.Size = new System.Drawing.Size(46, 54);
            this.lblWaitingParts.TabIndex = 1;
            this.lblWaitingParts.Text = "0";
            // 
            // lblWaitingPartsTitle
            // 
            this.lblWaitingPartsTitle.AutoSize = true;
            this.lblWaitingPartsTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblWaitingPartsTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblWaitingPartsTitle.Location = new System.Drawing.Point(20, 15);
            this.lblWaitingPartsTitle.Name = "lblWaitingPartsTitle";
            this.lblWaitingPartsTitle.Size = new System.Drawing.Size(120, 20);
            this.lblWaitingPartsTitle.TabIndex = 0;
            this.lblWaitingPartsTitle.Text = "CHỜ LINH KIỆN";
            // 
            // cardInProgress
            // 
            this.cardInProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.cardInProgress.Controls.Add(this.lblInProgress);
            this.cardInProgress.Controls.Add(this.lblInProgressTitle);
            this.cardInProgress.Location = new System.Drawing.Point(0, 0);
            this.cardInProgress.Name = "cardInProgress";
            this.cardInProgress.Padding = new System.Windows.Forms.Padding(20);
            this.cardInProgress.Size = new System.Drawing.Size(240, 100);
            this.cardInProgress.TabIndex = 0;
            // 
            // lblInProgress
            // 
            this.lblInProgress.AutoSize = true;
            this.lblInProgress.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblInProgress.ForeColor = System.Drawing.Color.White;
            this.lblInProgress.Location = new System.Drawing.Point(20, 40);
            this.lblInProgress.Name = "lblInProgress";
            this.lblInProgress.Size = new System.Drawing.Size(46, 54);
            this.lblInProgress.TabIndex = 1;
            this.lblInProgress.Text = "0";
            // 
            // lblInProgressTitle
            // 
            this.lblInProgressTitle.AutoSize = true;
            this.lblInProgressTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInProgressTitle.ForeColor = System.Drawing.Color.White;
            this.lblInProgressTitle.Location = new System.Drawing.Point(20, 15);
            this.lblInProgressTitle.Name = "lblInProgressTitle";
            this.lblInProgressTitle.Size = new System.Drawing.Size(100, 20);
            this.lblInProgressTitle.TabIndex = 0;
            this.lblInProgressTitle.Text = "ĐANG XỬ LÝ";
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.panelRoot);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "DashboardForm";
            this.Text = "Bảng điều khiển - Quản lý bảo hành";
            this.Load += new System.EventHandler(this.DashboardForm_Load);
            this.panelRoot.ResumeLayout(false);
            this.grpToday.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartToday)).EndInit();
            this.panelStats.ResumeLayout(false);
            this.cardCompleted.ResumeLayout(false);
            this.cardCompleted.PerformLayout();
            this.cardWaitingParts.ResumeLayout(false);
            this.cardWaitingParts.PerformLayout();
            this.cardInProgress.ResumeLayout(false);
            this.cardInProgress.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
