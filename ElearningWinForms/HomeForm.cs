using System;
using System.Windows.Forms;

namespace ElearningWinForms
{
    /// <summary>
    /// Form chính: bên trái là menu, giữa là vùng nội dung (panelContent) để mở các UC.
    /// </summary>
    public partial class HomeForm : Form
    {
        // Thông tin user hiện tại (nhận từ LoginForm)
        public int CurrentUserId { get; private set; }
        public string CurrentRole { get; private set; }      // "SV", "GV", "Admin"
        public string CurrentUserName { get; private set; }  // Họ tên
        public string CurrentCode { get; private set; }      // Mã SV/GV/Admin

        private Form _currentChildForm;

        public HomeForm(int userId, string role, string fullName, string code)
        {
            InitializeComponent();

            CurrentUserId = userId;
            CurrentRole = role;
            CurrentUserName = fullName;
            CurrentCode = code;
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            // Gán thông tin lên header
            lblUserName.Text = $"Xin chào: {CurrentUserName}";
            lblUserCode.Text = $"Mã: {CurrentCode}";
            lblRole.Text = $"Role: {CurrentRole}";

            // Phân quyền đơn giản: chỉ Admin dùng nút Admin
            btnAdmin.Enabled = (CurrentRole == "Admin");

            // Nếu muốn mở sẵn UC-02 khi vào Home:
            // OpenChildForm(new CourseForm(CurrentUserId, CurrentRole));
        }

        /// <summary>
        /// Mở 1 form con bên trong panelContent (Dock = Fill, không border).
        /// Đảm bảo mỗi lần chỉ có 1 child form đang hiển thị.
        /// </summary>
        private void OpenChildForm(Form childForm)
        {
            try
            {
                if (_currentChildForm != null)
                {
                    _currentChildForm.Close();
                    _currentChildForm.Dispose();
                    _currentChildForm = null;
                }

                _currentChildForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.Dock = DockStyle.Fill;

                panelContent.Controls.Clear();
                panelContent.Controls.Add(childForm);
                panelContent.Tag = childForm;

                childForm.BringToFront();
                childForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở màn hình con:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== MENU BUTTONS ==================

        // UC-02: Khóa học
        private void btnCourses_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentRole == "Admin")
                {
                    // Admin: chọn giữa quản lý môn học hoặc xem DS SV đăng ký học phần
                    var dr = MessageBox.Show(
                        "Admin chọn:\n\nYES = Quản lý môn học\nNO = Xem học phần & DS SV đăng ký\n\n(CANCEL để hủy)",
                        "Chức năng khóa học",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                        OpenChildForm(new CourseManagementForm());
                    else if (dr == DialogResult.No)
                        OpenChildForm(new CourseForm(CurrentUserId, "Admin"));
                    else
                        return;
                }
                else
                {
                    // SV, GV: xem / đăng ký / xem lớp đang dạy
                    OpenChildForm(new CourseForm(CurrentUserId, CurrentRole));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở chức năng khóa học:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UC-03: Bài tập
        private void btnAssignments_Click(object sender, EventArgs e)
        {
            try
            {
                // AssignmentForm requires userId and role as constructor parameters
                OpenChildForm(new AssignmentForm(CurrentUserId, CurrentRole));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở chức năng bài tập:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UC-04: Nộp bài
        private void btnSubmissions_Click(object sender, EventArgs e)
        {
            try
            {
                OpenChildForm(new SubmissionForm(CurrentUserId, CurrentRole));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở chức năng nộp bài:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UC-05: Điểm & phản hồi
        private void btnGrades_Click(object sender, EventArgs e)
        {
            try
            {
                // Class thật nằm trong GradeManagementForm.cs nhưng tên lớp là GradeForm
                OpenChildForm(new GradeForm(CurrentUserId, CurrentRole));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở chức năng điểm:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UC-06: Thông báo
        private void btnNotifications_Click(object sender, EventArgs e)
        {
            try
            {
                OpenChildForm(new NotificationForm(CurrentUserId, CurrentRole));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở chức năng thông báo:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UC-07: Admin
        private void btnAdmin_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentRole != "Admin")
                {
                    MessageBox.Show("Chỉ Admin mới được dùng chức năng này.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                OpenChildForm(new AdminForm(CurrentUserId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở chức năng Admin:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Đăng xuất
        private void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentChildForm != null)
                {
                    _currentChildForm.Close();
                    _currentChildForm.Dispose();
                    _currentChildForm = null;
                }

                this.Hide();
                using (var login = new LoginForm())
                {
                    login.ShowDialog();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng xuất:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HomeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Chỉ thoát hẳn app nếu không còn form nào mở
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

    }
}
