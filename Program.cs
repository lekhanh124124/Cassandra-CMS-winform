// File: Program.cs
using System;
using System.Windows.Forms;
using Cassandra;

namespace QuanLyBaoHanhSanPham
{
    internal static class Program
    {
        public static Cluster DbCluster;
        public static ISession DbSession;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                DbCluster = Cluster.Builder()
                                   .AddContactPoint("127.0.0.1")
                                   .WithPort(9042)
                                   .Build();
                DbSession = DbCluster.Connect("warrantymanagement");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối Cassandra hoặc seed dữ liệu:\n" + ex.Message,
                                "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var main = new MainForm
                    {
                        CurrentUsername = login.LoggedUsername,
                        CurrentRole = login.LoggedRole
                    };
                    Application.Run(main);
                }
            }

            try { DbSession?.Dispose(); } catch { }
            try { DbCluster?.Dispose(); } catch { }
        }
    }
}
