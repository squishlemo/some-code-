using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Windows.Forms;
using ElearningWinForms.Data;
using ElearningWinForms.Security;
using Microsoft.VisualBasic;

namespace ElearningWinForms
{
    public partial class LoginForm : Form
    {
        // ===== LOGIN LOCKOUT CONFIG =====
        private const int MAX_FAILED_PASSWORD_ATTEMPTS = 5;
        private const int LOCK_MINUTES = 15;
        // ================================

        // Email hệ thống được đọc từ App.config (SystemEmail/SystemEmailAppPassword)
        // hoặc env var (ELEARNING_SYSTEM_EMAIL / ELEARNING_SYSTEM_EMAIL_APP_PASSWORD).

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Có thể để trống
        }

        // ================== ĐĂNG NHẬP + OTP ==================
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
                // (Bỏ đoạn “pending”/RegistrationRequests: Login chỉ dựa trên bảng Users)
                string sql = @"
                    SELECT TOP 1
                        UserId, FullName, Email, Role, IsActive, PasswordHash,
                        FailedLoginCount, LockedUntil
                    FROM Users
                    WHERE Username = @u";

                DataTable table = DbHelper.GetDataTable(
                    sql,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username }
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

                int userId = Convert.ToInt32(row["UserId"]);
                bool isActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]);

                int failedCount = 0;
                if (row.Table.Columns.Contains("FailedLoginCount") && row["FailedLoginCount"] != DBNull.Value)
                    failedCount = Convert.ToInt32(row["FailedLoginCount"]);

                DateTime? lockedUntil = null;
                if (row.Table.Columns.Contains("LockedUntil") && row["LockedUntil"] != DBNull.Value)
                    lockedUntil = Convert.ToDateTime(row["LockedUntil"]);

                if (!isActive)
                {
                    MessageBox.Show("Tài khoản đã bị khóa/vô hiệu hóa. Liên hệ Admin.",
                                    "Đăng nhập thất bại",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                if (lockedUntil.HasValue && lockedUntil.Value > DateTime.Now)
                {
                    var remain = lockedUntil.Value - DateTime.Now;
                    var mins = (int)Math.Ceiling(remain.TotalMinutes);
                    if (mins < 1) mins = 1;

                    MessageBox.Show(
                        $"Tài khoản đang bị khóa tạm thời đến {lockedUntil.Value:dd/MM/yyyy HH:mm} (còn khoảng {mins} phút).",
                        "Tài khoản bị khóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                string storedHash = row["PasswordHash"]?.ToString() ?? "";
                if (!PasswordHasher.VerifyPassword(password, storedHash))
                {
                    HandleFailedPassword(userId, failedCount);
                    return;
                }

                // Đúng mật khẩu -> reset bộ đếm sai mật khẩu/lock trước khi qua OTP
                ResetFailedPassword(userId);

                string fullName = row["FullName"]?.ToString() ?? "";
                string role = row["Role"]?.ToString() ?? "";        // "SV" / "GV" / "Admin"
                string email = row["Email"]?.ToString() ?? "";

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Tài khoản này chưa có email để nhận OTP. Vui lòng cập nhật email hoặc liên hệ Admin.",
                                    "Thiếu email",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // ===== BƯỚC 2: OTP =====
                if (!VerifyOtpLogin(email, fullName))
                    return;

                // Cập nhật lần đăng nhập gần nhất (chỉ khi đã qua OTP)
                UpdateLastLoginDate(userId);

                // ===== VÀO HỆ THỐNG =====
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

        /// <summary>
        /// Xác thực OTP khi đăng nhập (gửi OTP qua email + người dùng nhập OTP).
        /// Không dùng “pending” hay bảng trung gian — chỉ xác thực ngay trên UI.
        /// </summary>
        private bool VerifyOtpLogin(string toEmail, string fullName)
        {
            // Tạo OTP 6 số
            string otp = OtpHasher.GenerateNumericOtp(6);
            DateTime createdAt = DateTime.Now;

            if (!SendOtpEmailForLogin(toEmail, otp, fullName))
                return false;

            const int maxTries = 3;

            for (int attempt = 1; attempt <= maxTries; attempt++)
            {
                string input = Interaction.InputBox(
                    $"Hệ thống đã gửi OTP tới email: {toEmail}\n\nNhập mã OTP (6 số) để hoàn tất đăng nhập:",
                    "Xác thực OTP",
                    ""
                )?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("Bạn đã hủy/không nhập OTP. Đăng nhập bị hủy.",
                                    "Hủy OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return false;
                }

                // Hết hạn OTP (3 phút)
                if ((DateTime.Now - createdAt).TotalMinutes > 3)
                {
                    MessageBox.Show("OTP đã hết hạn. Vui lòng đăng nhập lại để nhận OTP mới.",
                                    "OTP hết hạn",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return false;
                }

                if (input == otp)
                    return true;

                MessageBox.Show($"OTP không đúng. Còn {maxTries - attempt} lần thử.",
                                "Sai OTP",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            return false;
        }

        /// <summary>
        /// Gửi email OTP cho bước đăng nhập (Gmail + app password).
        /// </summary>
        private bool SendOtpEmailForLogin(string toEmail, string otp, string fullName)
        {
            try
            {
                string subject = "Mã OTP xác thực đăng nhập Elearning";

                string safeName = string.IsNullOrWhiteSpace(fullName) ? "bạn" : fullName;

                string html = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8' />
  <style>
    .box { max-width: 520px; margin: 0 auto; padding: 20px; font-family: Arial, sans-serif; border: 1px solid #e0e0e0; border-radius: 8px; }
    .title { font-size: 18px; font-weight: bold; margin-bottom: 10px; }
    .code { font-size: 32px; font-weight: bold; letter-spacing: 6px; padding: 10px 0; }
    .note { color: #666; font-size: 12px; margin-top: 10px; }
  </style>
</head>
<body>
  <div class='box'>
    <div class='title'>Xác thực đăng nhập Elearning</div>
    <p>Chào " + WebUtility.HtmlEncode(safeName) + @",</p>
    <p>Mã OTP để đăng nhập vào hệ thống là:</p>
    <div class='code'>" + otp + @"</div>
    <p class='note'>Mã OTP có hiệu lực trong 3 phút. Nếu bạn không yêu cầu đăng nhập, hãy bỏ qua email này.</p>
  </div>
</body>
</html>";

                EmailSender.SendHtmlEmail(toEmail, subject, html);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi email OTP: " + ex.Message,
                                "Lỗi gửi email",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
        }


        // ================== FAILED LOGIN / LOCKOUT ==================
        private void HandleFailedPassword(int userId, int currentFailedCount)
        {
            try
            {
                int newCount = currentFailedCount + 1;

                if (newCount >= MAX_FAILED_PASSWORD_ATTEMPTS)
                {
                    DateTime lockUntil = DateTime.Now.AddMinutes(LOCK_MINUTES);

                    string sql = @"
                        UPDATE Users
                        SET FailedLoginCount = @c,
                            LockedUntil = @lu
                        WHERE UserId = @id";

                    DbHelper.ExecuteNonQuery(
                        sql,
                        new SqlParameter("@c", SqlDbType.Int) { Value = newCount },
                        new SqlParameter("@lu", SqlDbType.DateTime) { Value = lockUntil },
                        new SqlParameter("@id", SqlDbType.Int) { Value = userId }
                    );

                    MessageBox.Show(
                        $"Bạn đã nhập sai mật khẩu quá {MAX_FAILED_PASSWORD_ATTEMPTS} lần. " +
                        $"Tài khoản bị khóa tạm thời đến {lockUntil:dd/MM/yyyy HH:mm}.",
                        "Tài khoản bị khóa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    string sql = "UPDATE Users SET FailedLoginCount = @c WHERE UserId = @id";

                    DbHelper.ExecuteNonQuery(
                        sql,
                        new SqlParameter("@c", SqlDbType.Int) { Value = newCount },
                        new SqlParameter("@id", SqlDbType.Int) { Value = userId }
                    );

                    int remain = MAX_FAILED_PASSWORD_ATTEMPTS - newCount;

                    MessageBox.Show(
                        $"Sai tài khoản hoặc mật khẩu. Còn {remain} lần thử.",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch
            {
                // Nếu DB lỗi, vẫn hiển thị lỗi chung để tránh "bypass"
                MessageBox.Show("Sai tài khoản hoặc mật khẩu.",
                                "Đăng nhập thất bại",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void ResetFailedPassword(int userId)
        {
            try
            {
                string sql = "UPDATE Users SET FailedLoginCount = 0, LockedUntil = NULL WHERE UserId = @id";
                DbHelper.ExecuteNonQuery(sql, new SqlParameter("@id", SqlDbType.Int) { Value = userId });
            }
            catch
            {
                // ignore
            }
        }

        private void UpdateLastLoginDate(int userId)
        {
            try
            {
                string sql = "UPDATE Users SET LastLoginDate = GETDATE() WHERE UserId = @id";
                DbHelper.ExecuteNonQuery(sql, new SqlParameter("@id", SqlDbType.Int) { Value = userId });
            }
            catch
            {
                // ignore
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
