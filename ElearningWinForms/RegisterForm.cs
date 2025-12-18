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
    public partial class RegisterForm : Form
    {
        // Email hệ thống được đọc từ App.config (SystemEmail/SystemEmailAppPassword)
        // hoặc env var (ELEARNING_SYSTEM_EMAIL / ELEARNING_SYSTEM_EMAIL_APP_PASSWORD).

        // ===== BẢNG DANH MỤC LẤY TỪ DATABASE =====
        private DataTable majorsTable;      // Ngành (kèm khoa)
        private DataTable classesTable;     // Lớp theo ngành
        private DataTable titlesTable;      // Học vị GV

        public RegisterForm()
        {
            InitializeComponent();

            // Gắn event
            this.Load += RegisterForm_Load;
            cboRole.SelectedIndexChanged += cboRole_SelectedIndexChanged;
            cboMajor.SelectedIndexChanged += cboMajor_SelectedIndexChanged;
            btnRegister.Click += btnRegister_Click;
        }

        // ==========================================================
        //                    LOGIC FORM
        // ==========================================================

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // Load danh mục từ database
            LoadLookupFromDatabase();

            // role mặc định
            if (cboRole.Items.Count > 0 && cboRole.SelectedIndex < 0)
                cboRole.SelectedIndex = 0;

            UpdateRoleFields();
        }

        /// <summary>
        /// Lấy danh mục Ngành / Lớp / Học vị từ DB
        /// </summary>
        private void LoadLookupFromDatabase()
        {
            try
            {
                // 1) Lấy ngành + khoa (cho SV và GV)
                string sqlMajors = @"
                    SELECT m.MajorId, m.MajorName, m.FacultyId, f.FacultyName
                    FROM Majors m
                    JOIN Faculties f ON m.FacultyId = f.FacultyId
                    ORDER BY m.MajorName";

                majorsTable = DbHelper.GetDataTable(sqlMajors);

                // SV: chọn ngành
                cboMajor.DataSource = majorsTable;
                cboMajor.DisplayMember = "MajorName";
                cboMajor.ValueMember = "MajorId";
                cboMajor.SelectedIndex = -1;

                // GV: ngành dạy (dùng cùng danh sách ngành, copy để tránh bị “dính” selection)
                DataTable majorsForTeacher = majorsTable.Copy();
                txtDepartment.DataSource = majorsForTeacher;
                txtDepartment.DisplayMember = "MajorName";
                txtDepartment.ValueMember = "MajorId";
                txtDepartment.SelectedIndex = -1;

                // 2) Học vị GV
                string sqlTitles = @"
                    SELECT TitleId, TitleName
                    FROM AcademicTitles
                    ORDER BY TitleName";

                titlesTable = DbHelper.GetDataTable(sqlTitles);
                txtAcademicTitle.DataSource = titlesTable;
                txtAcademicTitle.DisplayMember = "TitleName";
                txtAcademicTitle.ValueMember = "TitleId";
                txtAcademicTitle.SelectedIndex = -1;

                // Lớp sẽ load theo ngành
                txtClassName.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu danh mục: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void cboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRoleFields();
        }

        /// <summary>
        /// Khi chọn ngành SV -> tự điền khoa + danh sách lớp từ DB
        /// </summary>
        private void cboMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMajor.SelectedIndex < 0 || majorsTable == null)
            {
                txtFaculty.Text = string.Empty;
                txtClassName.DataSource = null;
                return;
            }

            var rowView = cboMajor.SelectedItem as DataRowView;
            if (rowView == null)
            {
                txtFaculty.Text = string.Empty;
                txtClassName.DataSource = null;
                return;
            }

            int majorId = Convert.ToInt32(rowView["MajorId"]);

            // set khoa
            DataRow[] rows = majorsTable.Select("MajorId = " + majorId);
            if (rows.Length > 0)
                txtFaculty.Text = rows[0]["FacultyName"].ToString();
            else
                txtFaculty.Text = string.Empty;

            // load lớp theo ngành
            try
            {
                string sqlClasses = @"
            SELECT ClassId, ClassName
            FROM Classes
            WHERE MajorId = @mid
            ORDER BY ClassName";

                classesTable = DbHelper.GetDataTable(
                    sqlClasses,
                    new SqlParameter("@mid", SqlDbType.Int) { Value = majorId });

                txtClassName.DataSource = classesTable;
                txtClassName.DisplayMember = "ClassName";
                txtClassName.ValueMember = "ClassId";
                txtClassName.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hiện/ẩn các ô nhập theo Role: SV/GV/Admin
        /// </summary>
        private void UpdateRoleFields()
        {
            string role = cboRole.SelectedItem == null
                ? ""
                : cboRole.SelectedItem.ToString();

            bool isStudent = role == "SV";
            bool isTeacher = role == "GV";

            // ----- Sinh viên: Ngành + Lớp + Khoa -----
            lblMajor.Visible = isStudent;
            cboMajor.Visible = isStudent;
            lblClassName.Visible = isStudent;
            txtClassName.Visible = isStudent;
            lblFaculty.Visible = isStudent;
            txtFaculty.Visible = isStudent;

            // ----- Giảng viên: Ngành dạy + Học vị -----
            lblDepartment.Visible = isTeacher;
            txtDepartment.Visible = isTeacher;
            lblAcademicTitle.Visible = isTeacher;
            txtAcademicTitle.Visible = isTeacher;
        }

        /// <summary>
        /// Nút Đăng ký: lưu request + gửi OTP + xác thực OTP
        /// </summary>
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string role = cboRole.SelectedItem == null
                            ? ""
                            : cboRole.SelectedItem.ToString();

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

            // Thông tin thêm theo Role
            string className = null;
            string faculty = null;
            string department = null;
            string academicTitle = null;

            if (role == "SV")
            {
                if (cboMajor.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn Ngành cho sinh viên.",
                                    "Thiếu thông tin",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                if (txtClassName.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn Lớp cho sinh viên.",
                                    "Thiếu thông tin",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                DataRowView classRowView = txtClassName.SelectedItem as DataRowView;
                className = classRowView != null
                    ? classRowView["ClassName"].ToString()
                    : txtClassName.Text;

                faculty = txtFaculty.Text.Trim();
            }
            else if (role == "GV")
            {
                if (txtDepartment.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn Ngành dạy cho giảng viên.",
                                    "Thiếu thông tin",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                if (txtAcademicTitle.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn Học vị cho giảng viên.",
                                    "Thiếu thông tin",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                DataRowView depRowView = txtDepartment.SelectedItem as DataRowView;
                department = depRowView != null
                    ? depRowView["MajorName"].ToString()
                    : txtDepartment.Text;

                DataRowView titleRowView = txtAcademicTitle.SelectedItem as DataRowView;
                academicTitle = titleRowView != null
                    ? titleRowView["TitleName"].ToString()
                    : txtAcademicTitle.Text;
            }

            try
            {
                // 1) Check trùng username trong Users
                string sqlCheckUsers = "SELECT COUNT(*) FROM Users WHERE Username = @u";
                int countUsers = Convert.ToInt32(DbHelper.ExecuteScalar(
                    sqlCheckUsers,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username }
                ));

                // 2) Check trùng username trong RegistrationRequests đang chờ
                string sqlCheckReq = @"
                    SELECT COUNT(*)
                    FROM RegistrationRequests
                    WHERE Username = @u
                      AND Status IN (N'PendingEmail', N'PendingAdmin')";
                int countReq = Convert.ToInt32(DbHelper.ExecuteScalar(
                    sqlCheckReq,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username }
                ));

                if (countUsers > 0 || countReq > 0)
                {
                    MessageBox.Show("Tên đăng nhập đã tồn tại hoặc đang chờ duyệt.",
                                    "Trùng tài khoản",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // 3) Hash mật khẩu (PBKDF2)
                string passwordHash = PasswordHasher.HashPassword(password);

                // 4) Sinh OTP (crypto RNG) + hash OTP (HMAC-SHA256 + pepper)
                const int otpExpiryMinutes = 1;   // đổi tùy ý (1, 3, 5...)
                const int otpMaxAttempts = 5;

                string otp = OtpHasher.GenerateNumericOtp(6);
                byte[] otpHash = OtpHasher.ComputeOtpHash(username, otp);
                DateTime otpExpiresAt = DateTime.Now.AddMinutes(otpExpiryMinutes);

                // 5) Lưu yêu cầu vào RegistrationRequests (KHÔNG lưu OTP dạng rõ)
                string sqlInsert = @"
                    INSERT INTO RegistrationRequests
                        (Username, PasswordHash, FullName, Email, Role, Status,
                         OTP, OTPHash, OTPExpiresAt, OTPAttemptCount, OTPUsedAt,
                         ClassName, Faculty, Department, AcademicTitle)
                    VALUES
                        (@u, @p, @name, @em, @role, N'PendingEmail',
                         NULL, @otpHash, @otpExp, 0, NULL,
                         @class, @faculty, @dept, @title);
                    SELECT SCOPE_IDENTITY();";

                object insertedId = DbHelper.ExecuteScalar(
                    sqlInsert,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username },
                    new SqlParameter("@p", SqlDbType.NVarChar, 400) { Value = passwordHash },
                    new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = fullName },
                    new SqlParameter("@em", SqlDbType.NVarChar, 100) { Value = email },
                    new SqlParameter("@role", SqlDbType.NVarChar, 20) { Value = role },
                    new SqlParameter("@otpHash", SqlDbType.VarBinary, 32) { Value = otpHash },
                    new SqlParameter("@otpExp", SqlDbType.DateTime) { Value = otpExpiresAt },
                    new SqlParameter("@class", SqlDbType.NVarChar, 100)
                    { Value = (object)className ?? DBNull.Value },
                    new SqlParameter("@faculty", SqlDbType.NVarChar, 100)
                    { Value = (object)faculty ?? DBNull.Value },
                    new SqlParameter("@dept", SqlDbType.NVarChar, 100)
                    { Value = (object)department ?? DBNull.Value },
                    new SqlParameter("@title", SqlDbType.NVarChar, 50)
                    { Value = (object)academicTitle ?? DBNull.Value }
                );

                int requestId = Convert.ToInt32(insertedId);

                // 6) Gửi email OTP
                if (!SendOtpEmail(email, otp, otpExpiryMinutes))
                {
                    RejectRegistrationRequest(requestId, "Không gửi được OTP (email lỗi)");
                    MessageBox.Show("Không gửi được OTP. Vui lòng thử lại sau.",
                                    "Lỗi gửi OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // 7) Nhập OTP (tối đa otpMaxAttempts lần)
                bool verified = false;

                for (int attempt = 1; attempt <= otpMaxAttempts; attempt++)
                {
                    // Tải trạng thái OTP hiện tại từ DB (để check hết hạn / số lần thử)
                    string sqlState = @"
                        SELECT OTPHash, OTPExpiresAt, OTPAttemptCount, OTPUsedAt
                        FROM RegistrationRequests
                        WHERE RequestId = @id AND Status = N'PendingEmail'";

                    DataTable stateTable = DbHelper.GetDataTable(
                        sqlState,
                        new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
                    );

                    if (stateTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy yêu cầu đăng ký (có thể đã được xử lý).",
                                        "Lỗi OTP",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    DataRow state = stateTable.Rows[0];

                    if (state["OTPUsedAt"] != DBNull.Value)
                    {
                        MessageBox.Show("OTP đã được sử dụng. Vui lòng đăng ký lại để nhận OTP mới.",
                                        "OTP đã dùng",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (state["OTPExpiresAt"] == DBNull.Value || DateTime.Now > Convert.ToDateTime(state["OTPExpiresAt"]))
                    {
                        RejectRegistrationRequest(requestId, "OTP hết hạn");
                        MessageBox.Show("OTP đã hết hạn. Vui lòng đăng ký lại để nhận OTP mới.",
                                        "OTP hết hạn",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    int attemptCount = state["OTPAttemptCount"] == DBNull.Value ? 0 : Convert.ToInt32(state["OTPAttemptCount"]);
                    int remain = Math.Max(0, otpMaxAttempts - attemptCount);

                    string inputOtp = Interaction.InputBox(
                        $"Mã OTP đã được gửi tới email của bạn.\n\nNhập OTP (6 số) để xác thực.\nOTP hết hạn sau {otpExpiryMinutes} phút.\nCòn {remain} lần thử.",
                        "Xác thực OTP",
                        ""
                    )?.Trim();

                    if (string.IsNullOrWhiteSpace(inputOtp))
                    {
                        RejectRegistrationRequest(requestId, "Người dùng hủy OTP");
                        MessageBox.Show("Bạn đã hủy/không nhập OTP. Vui lòng đăng ký lại khi sẵn sàng.",
                                        "Hủy OTP",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }

                    if (state["OTPHash"] == DBNull.Value)
                    {
                        RejectRegistrationRequest(requestId, "OTPHash null");
                        MessageBox.Show("Dữ liệu OTP không hợp lệ. Vui lòng đăng ký lại.",
                                        "Lỗi OTP",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    byte[] otpHashDb = (byte[])state["OTPHash"];
                    byte[] inputHash = OtpHasher.ComputeOtpHash(username, inputOtp);

                    if (OtpHasher.FixedTimeEquals(inputHash, otpHashDb))
                    {
                        verified = true;
                        break;
                    }

                    // Sai OTP -> tăng số lần thử
                    int newCount = attemptCount + 1;

                    if (newCount >= otpMaxAttempts)
                    {
                        RejectRegistrationRequest(requestId, "Sai OTP quá số lần");
                        MessageBox.Show("Bạn đã nhập sai OTP quá số lần cho phép. Vui lòng đăng ký lại để nhận OTP mới.",
                                        "Khóa OTP",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }

                    string sqlInc = @"
                        UPDATE RegistrationRequests
                        SET OTPAttemptCount = @c
                        WHERE RequestId = @id AND Status = N'PendingEmail'";

                    DbHelper.ExecuteNonQuery(
                        sqlInc,
                        new SqlParameter("@c", SqlDbType.Int) { Value = newCount },
                        new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
                    );

                    MessageBox.Show($"Mã OTP không chính xác. Còn {otpMaxAttempts - newCount} lần thử.",
                                    "Sai OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }

                if (!verified)
                {
                    RejectRegistrationRequest(requestId, "Không xác thực được OTP");
                    return;
                }

                // 8) Cập nhật trạng thái sang PendingAdmin + đánh dấu OTP đã dùng
                string sqlUpdateStatus = @"
                    UPDATE RegistrationRequests
                    SET Status = N'PendingAdmin',
                        OTPUsedAt = GETDATE(),
                        OTPHash = NULL,
                        OTPExpiresAt = NULL
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

        private void RejectRegistrationRequest(int requestId, string reason)
        {
            try
            {
                string sql = @"
                    UPDATE RegistrationRequests
                    SET Status = N'Rejected',
                        OTPHash = NULL,
                        OTPExpiresAt = NULL
                    WHERE RequestId = @id";

                DbHelper.ExecuteNonQuery(
                    sql,
                    new System.Data.SqlClient.SqlParameter("@id", System.Data.SqlDbType.Int) { Value = requestId }
                );
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// Gửi email OTP (dùng Gmail + app password)
        /// </summary>
        private bool SendOtpEmail(string toEmail, string otp, int expiryMinutes)
        {
            try
            {
                string subject = "Mã OTP xác thực đăng ký Elearning";

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
        Mã OTP có hiệu lực trong " + expiryMinutes + @" phút.
        Nếu bạn không yêu cầu đăng ký, hãy bỏ qua email này.
    </p>
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
    }
}
