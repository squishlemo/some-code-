using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using System.Drawing;
using ElearningWinForms.Data;
using Microsoft.VisualBasic;

namespace ElearningWinForms
{
    public partial class RegisterForm : Form
    {
        // ===== CẤU HÌNH EMAIL HỆ THỐNG (GMAIL + APP PASSWORD) =====
        private const string SystemEmail = "elearning.demoing@gmail.com";
        private const string SystemEmailAppPassword = "dkve qxvo pcyc jajy";
        // ==========================================================

        // ===== BẢNG DANH MỤC LẤY TỪ DATABASE =====
        private DataTable majorsTable;      // Ngành (kèm khoa)
        private DataTable classesTable;     // Lớp theo ngành
        private DataTable titlesTable;      // Học vị GV

        // ===== CONTROL FIELD =====
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblFullName;
        private Label lblEmail;
        private Label lblRole;
        private Label lblMajor;
        private Label lblClassName;
        private Label lblFaculty;
        private Label lblDepartment;
        private Label lblAcademicTitle;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtFaculty;

        private ComboBox cboRole;
        private ComboBox cboMajor;          // chọn ngành SV
        private ComboBox txtClassName;      // combo lớp SV
        private ComboBox txtDepartment;     // combo ngành dạy GV
        private ComboBox txtAcademicTitle;  // combo học vị GV

        private Button btnRegister;

        // ==========================================================

        public RegisterForm()
        {
            // Ở file Designer chỉ cần một InitializeComponent() rỗng
            InitializeComponent();

            // Dựng toàn bộ giao diện bằng code
            BuildUI();

            // Gắn event
            this.Load += RegisterForm_Load;
            cboRole.SelectedIndexChanged += cboRole_SelectedIndexChanged;
            cboMajor.SelectedIndexChanged += cboMajor_SelectedIndexChanged;
            btnRegister.Click += btnRegister_Click;
        }

        // ==========================================================
        //              KHỞI TẠO GIAO DIỆN BẰNG CODE
        // ==========================================================
        private void BuildUI()
        {
            this.SuspendLayout();

            // Form
            this.Text = "Đăng ký tài khoản";
            this.ClientSize = new Size(420, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int marginLeftLabel = 20;
            int marginLeftInput = 140;
            int inputWidth = 240;
            int currentTop = 20;
            int verticalSpacing = 8;
            int rowHeight = 26;

            // ===== Title =====
            lblTitle = new Label
            {
                Text = "Đăng ký",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                AutoSize = true
            };
            lblTitle.Location =
                new Point((this.ClientSize.Width - lblTitle.PreferredWidth) / 2, currentTop);
            this.Controls.Add(lblTitle);

            currentTop += 40;

            // ===== Username =====
            lblUsername = new Label
            {
                Text = "Tài khoản:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtUsername = new TextBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth
            };
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);

            currentTop += rowHeight + verticalSpacing;

            // ===== Password =====
            lblPassword = new Label
            {
                Text = "Mật khẩu:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtPassword = new TextBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                UseSystemPasswordChar = true
            };
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);

            currentTop += rowHeight + verticalSpacing;

            // ===== FullName =====
            lblFullName = new Label
            {
                Text = "Họ tên:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtFullName = new TextBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth
            };
            this.Controls.Add(lblFullName);
            this.Controls.Add(txtFullName);

            currentTop += rowHeight + verticalSpacing;

            // ===== Email =====
            lblEmail = new Label
            {
                Text = "Email:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtEmail = new TextBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth
            };
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);

            currentTop += rowHeight + verticalSpacing;

            // ===== Role =====
            lblRole = new Label
            {
                Text = "Vai trò:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            cboRole = new ComboBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboRole.Items.AddRange(new object[] { "SV", "GV", "Admin" });
            this.Controls.Add(lblRole);
            this.Controls.Add(cboRole);

            currentTop += rowHeight + verticalSpacing;

            // ===== Major (SV) =====
            lblMajor = new Label
            {
                Text = "Ngành:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            cboMajor = new ComboBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(lblMajor);
            this.Controls.Add(cboMajor);

            currentTop += rowHeight + verticalSpacing;

            // ===== ClassName (SV) =====
            lblClassName = new Label
            {
                Text = "Tên lớp:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtClassName = new ComboBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(lblClassName);
            this.Controls.Add(txtClassName);

            currentTop += rowHeight + verticalSpacing;

            // ===== Faculty (SV) =====
            lblFaculty = new Label
            {
                Text = "Khoa:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtFaculty = new TextBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                ReadOnly = true
            };
            this.Controls.Add(lblFaculty);
            this.Controls.Add(txtFaculty);

            currentTop += rowHeight + verticalSpacing;

            // ===== Department (GV) =====
            lblDepartment = new Label
            {
                Text = "Ngành dạy:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtDepartment = new ComboBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(lblDepartment);
            this.Controls.Add(txtDepartment);

            currentTop += rowHeight + verticalSpacing;

            // ===== Academic Title (GV) =====
            lblAcademicTitle = new Label
            {
                Text = "Học vị:",
                AutoSize = true,
                Location = new Point(marginLeftLabel, currentTop)
            };
            txtAcademicTitle = new ComboBox
            {
                Location = new Point(marginLeftInput, currentTop - 3),
                Width = inputWidth,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(lblAcademicTitle);
            this.Controls.Add(txtAcademicTitle);

            currentTop += rowHeight + 2 * verticalSpacing;

            // ===== Button Register =====
            btnRegister = new Button
            {
                Text = "Tạo tài khoản",
                Width = 160,
                Height = 32,
                Location = new Point((this.ClientSize.Width - 160) / 2, currentTop)
            };
            this.Controls.Add(btnRegister);

            this.AcceptButton = btnRegister;

            this.ResumeLayout(false);
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

                // Lớp sẽ load theo ngành, nên chưa gán DataSource ở đây
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

            // Lấy dòng đang chọn
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

            // load lớp theo ngành (phần dưới giữ nguyên)
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

                // 3) Sinh OTP 6 chữ số
                var rnd = new Random();
                string otp = rnd.Next(0, 1000000).ToString("D6");

                // 4) Lưu yêu cầu vào RegistrationRequests
                string sqlInsert = @"
                    INSERT INTO RegistrationRequests
                        (Username, PasswordHash, FullName, Email, Role, Status, OTP,
                         ClassName, Faculty, Department, AcademicTitle)
                    VALUES
                        (@u, @p, @name, @em, @role, N'PendingEmail', @otp,
                         @class, @faculty, @dept, @title);
                    SELECT SCOPE_IDENTITY();";

                object insertedId = DbHelper.ExecuteScalar(
                    sqlInsert,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username },
                    new SqlParameter("@p", SqlDbType.NVarChar, 255) { Value = password }, // demo: chưa hash
                    new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = fullName },
                    new SqlParameter("@em", SqlDbType.NVarChar, 100) { Value = email },
                    new SqlParameter("@role", SqlDbType.NVarChar, 20) { Value = role },
                    new SqlParameter("@otp", SqlDbType.NVarChar, 10) { Value = otp },
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

                // 5) Gửi email OTP
                if (!SendOtpEmail(email, otp))
                {
                    MessageBox.Show("Không gửi được OTP. Vui lòng thử lại sau.",
                                    "Lỗi gửi OTP",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // 6) Nhập OTP
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

                // 7) Kiểm tra OTP trong DB
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

                // 8) Cập nhật trạng thái sang PendingAdmin
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

        /// <summary>
        /// Gửi email OTP (dùng Gmail + app password)
        /// </summary>
        private bool SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                string fromEmail = SystemEmail;
                string fromPass = SystemEmailAppPassword;

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
                client.UseDefaultCredentials = false;
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
