using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;

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
            // Có thể để trống
        }

        // ================== ĐĂNG NHẬP ==================
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

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
                string role = row["Role"].ToString();   // "SV" / "GV" / "Admin"

                string code = GetUserCode(userId, role);

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

        // ================== LẤY MÃ SV / GV / ADMIN ==================
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
                return role + userId;   // role lạ thì trả tạm

            object result = DbHelper.ExecuteScalar(
                sql,
                new SqlParameter("@uid", SqlDbType.Int) { Value = userId }
            );

            if (result == null || result == DBNull.Value)
                return role + userId;

            int id = Convert.ToInt32(result);

            string prefix;
            if (role == "SV")
                prefix = "SV";
            else if (role == "GV")
                prefix = "GV";
            else if (role == "Admin")
                prefix = "AD";
            else
                prefix = role;

            return string.Format("{0}{1:0000}", prefix, id);
        }

        // ================== LINK ĐĂNG KÝ ==================
        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var f = new RegisterForm())
            {
                f.ShowDialog();
            }
        }
    }
}
