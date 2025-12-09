using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using ElearningWinForms.Data;
using Microsoft.VisualBasic;   // để dùng InputBox cho nhập OTP


namespace ElearningWinForms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // Chọn sẵn SV nếu chưa chọn gì
            if (cboRole.Items.Count > 0 && cboRole.SelectedIndex < 0)
                cboRole.SelectedIndex = 0;
        }

        // Nút "Tạo tài khoản" / "Gửi yêu cầu đăng ký"
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string role = cboRole.SelectedItem == null
                                ? ""
                                : cboRole.SelectedItem.ToString();

            // 1) Kiểm tra dữ liệu nhập
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và chọn vai trò.",
                                "Thiếu thông tin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2) Check trùng username trong Users
                string sqlCheckUsers = "SELECT COUNT(*) FROM Users WHERE Username = @u";
                object resultUsers = DbHelper.ExecuteScalar(
                    sqlCheckUsers,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username }
                );
                int countUsers = Convert.ToInt32(resultUsers);

                // 3) Check trùng username trong RegistrationRequests còn đang chờ
                string sqlCheckReq = @"
                    SELECT COUNT(*) 
                    FROM RegistrationRequests 
                    WHERE Username = @u 
                      AND Status IN (N'PendingEmail', N'PendingAdmin')";
                object resultReq = DbHelper.ExecuteScalar(
                    sqlCheckReq,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username }
                );
                int countReq = Convert.ToInt32(resultReq);

                if (countUsers > 0 || countReq > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại hoặc đang chờ duyệt.",
                                    "Trùng tài khoản",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // 4) Sinh OTP ngẫu nhiên 6 chữ số
                var rnd = new Random();
                string otp = rnd.Next(0, 1000000).ToString("D6"); // luôn 6 số, ví dụ: 004239


                // 5) Lưu yêu cầu đăng ký vào RegistrationRequests
                string sqlInsert = @"
                    INSERT INTO RegistrationRequests
                        (Username, PasswordHash, FullName, Email, Role, Status, OTP)
                    VALUES
                        (@u, @p, @name, @em, @role, N'PendingEmail', @otp);
                    SELECT SCOPE_IDENTITY();";

                object insertedId = DbHelper.ExecuteScalar(
                    sqlInsert,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username },
                    new SqlParameter("@p", SqlDbType.NVarChar, 255) { Value = password }, // demo: chưa hash
                    new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = fullName },
                    new SqlParameter("@em", SqlDbType.NVarChar, 100) { Value = email },
                    new SqlParameter("@role", SqlDbType.NVarChar, 20) { Value = role },
                    new SqlParameter("@otp", SqlDbType.NVarChar, 10) { Value = otp }
                );

                int requestId = Convert.ToInt32(insertedId);

                // 6) Gửi email OTP
                if (!SendOtpEmail(email, otp))
                {
                    // tuỳ bạn: có thể xóa bản ghi request nếu muốn
                    MessageBox.Show("Không gửi được email OTP. Vui lòng thử lại sau.",
                                    "Lỗi gửi OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // 7) Cho user nhập OTP (InputBox đơn giản)
                string inputOtp = Interaction.InputBox(
                    "Mã OTP đã được gửi tới email của bạn.\nVui lòng nhập mã OTP để xác thực.",
                    "Xác thực OTP",
                    ""
                );

                if (string.IsNullOrWhiteSpace(inputOtp))
                {
                    MessageBox.Show("Bạn chưa nhập OTP. Yêu cầu đăng ký tạm dừng.",
                                    "OTP thiếu",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                inputOtp = inputOtp.Trim();

                // 8) Lấy OTP trong DB kiểm tra lại
                string sqlGetOtp = @"
                    SELECT OTP 
                    FROM RegistrationRequests 
                    WHERE RequestId = @id AND Status = N'PendingEmail'";
                object otpDbObj = DbHelper.ExecuteScalar(
                    sqlGetOtp,
                    new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
                );

                if (otpDbObj == null || otpDbObj == DBNull.Value)
                {
                    MessageBox.Show("Không tìm thấy yêu cầu đăng ký, vui lòng thử lại.",
                                    "Lỗi OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                string otpDb = otpDbObj.ToString().Trim();

                if (!string.Equals(inputOtp, otpDb, StringComparison.Ordinal))
                {
                    MessageBox.Show("Mã OTP không chính xác.",
                                    "Sai OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // 9) OTP đúng → cập nhật trạng thái sang PendingAdmin
                string sqlUpdateStatus = @"
                    UPDATE RegistrationRequests
                    SET Status = N'PendingAdmin'
                    WHERE RequestId = @id";
                DbHelper.ExecuteNonQuery(
                    sqlUpdateStatus,
                    new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
                );

                MessageBox.Show(
                    "Xác thực email thành công!\nYêu cầu đăng ký của bạn đang chờ Admin duyệt.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý đăng ký: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        // Hàm gửi email OTP (skeleton)
        private bool SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                // ==== 1. THAY ĐÚNG 2 DÒNG NÀY ====
                string fromEmail = "elearning.demoing@gmail.com";   // ví dụ: elearning.demo@gmail.com
                string fromPass = "tlhp onyo mxdf bnyo";      // app password 16 ký tự, không có dấu cách

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail, "Elearning System");
                message.To.Add(toEmail);
                message.Subject = "Mã OTP xác thực đăng ký Elearning";

                string html = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8' />
  <style>
    .box {
        max-width: 480px;
        margin: 0 auto;
        padding: 20px;
        font-family: Arial, sans-serif;
        border: 1px solid #e0e0e0;
        border-radius: 8px;
    }
    .title {
        font-size: 18px;
        font-weight: bold;
        margin-bottom: 10px;
    }
    .code {
        font-size: 32px;
        letter-spacing: 8px;
        font-weight: bold;
        padding: 10px 0;
        text-align: center;
        background-color: #f5f5f5;
        border-radius: 6px;
        margin: 15px 0;
    }
    .note {
        font-size: 12px;
        color: #777;
    }
  </style>
</head>
<body>
  <div class='box'>
    <div class='title'>Mã xác thực đăng ký tài khoản Elearning</div>
    <p>Chào bạn,</p>
    <p>Mã OTP để xác thực đăng ký tài khoản Elearning của bạn là:</p>
    <div class='code'>" + otp + @"</div>
    <p>Vui lòng nhập mã này vào màn hình đăng ký để hoàn tất bước xác thực email.</p>
    <p class='note'>
        Mã OTP có hiệu lực trong một thời gian ngắn. 
        Nếu bạn không yêu cầu đăng ký, hãy bỏ qua email này.
    </p>
  </div>
</body>
</html>";

                message.Body = html;
                message.IsBodyHtml = true;

                var client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false; // quan trọng: dùng credential mình set bên dưới
                client.Credentials = new NetworkCredential(fromEmail, fromPass);

                client.Send(message);
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

    }
}

