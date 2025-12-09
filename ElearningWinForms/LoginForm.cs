using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;   // <- dùng DbHelper trong namespace này


namespace ElearningWinForms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // có thể để trống
        }

        // ======= ĐĂNG NHẬP DÙNG DBHELPER =======

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;     // script .sql đang dùng '123456' dạng plain-text

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu.",
                                "Thiếu thông tin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1) Lấy user từ bảng Users bằng DbHelper
                string sql = @"
                    SELECT TOP 1 UserId, FullName, Role, IsActive
                    FROM Users
                    WHERE Username = @u AND PasswordHash = @p";

                DataTable table = DbHelper.GetDataTable(
                    sql,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username },
                    new SqlParameter("@p", SqlDbType.NVarChar, 255) { Value = password }
                );

                if (table.Rows.Count == 0)
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu.",
                                    "Đăng nhập thất bại",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                DataRow row = table.Rows[0];

                // 2) Kiểm tra tài khoản còn hoạt động không
                if (!(bool)row["IsActive"])
                {
                    MessageBox.Show("Tài khoản đã bị khóa. Liên hệ Admin.",
                                    "Đăng nhập thất bại",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                int userId = Convert.ToInt32(row["UserId"]);
                string fullName = row["FullName"].ToString();
                string role = row["Role"].ToString();     // "SV", "GV", "Admin"

                // 3) Lấy mã SV / GV / Admin bằng DbHelper.ExecuteScalar
                string code = GetUserCode(userId, role);

                // 4) Mở main shell (HomeForm) – DÙNG DbHelper gián tiếp qua dữ liệu vừa lấy
                var home = new HomeForm(userId, role, fullName, code);
                home.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng nhập: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        // Lấy mã sinh viên/giảng viên/admin tương ứng với UserId & Role
        private string GetUserCode(int userId, string role)
        {
            string sql;

            if (role == "SV")
                sql = "SELECT TOP 1 StudentId FROM Students WHERE UserId = @uid";
            else if (role == "GV")
                sql = "SELECT TOP 1 TeacherId FROM Teachers WHERE UserId = @uid";
            else if (role == "Admin")
                sql = "SELECT TOP 1 AdminId FROM Admins WHERE UserId = @uid";
            else
                return role + userId;

            object result = DbHelper.ExecuteScalar(
                sql,
                new SqlParameter("@uid", SqlDbType.Int) { Value = userId }
            );

            if (result == null || result == DBNull.Value)
                return role + userId;

            int id = Convert.ToInt32(result);

            return role switch
            {
                "SV" => $"SV{id:0000}",
                "GV" => $"GV{id:0000}",
                "Admin" => $"AD{id:0000}",
                _ => role + id
            };
        }

        // Link "Đăng ký"
        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var f = new RegisterForm())
            {
                f.ShowDialog();
            }
        }
    }
}
