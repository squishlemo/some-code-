using System;
using System.Windows.Forms;

namespace ElearningWinForms
{
    /// <summary>
    /// Main shell: 1 khung duy nhất, chứa menu + khu vực hiển thị các use case.
    /// </summary>
    public partial class HomeForm : Form
    {
        // Thông tin user hiện tại
        public int CurrentUserId { get; private set; }
        public string CurrentRole { get; private set; }     // "SV", "GV", "Admin"
        public string CurrentUserName { get; private set; } // Họ tên
        public string CurrentCode { get; private set; }     // Mã SV / GV / Admin

        // Form con đang hiển thị
        private Form _currentChildForm;

        // Constructor dùng khi gọi sau khi đăng nhập
        public HomeForm(int userId, string role, string userName, string code)
        {
            InitializeComponent();

            CurrentUserId = userId;
            CurrentRole = role;
            CurrentUserName = userName;
            CurrentCode = code;
        }

        public HomeForm(int userId, string role)
        {
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            // Gán thông tin lên header
            lblUserName.Text = CurrentUserName;
            lblUserCode.Text = CurrentCode;
            lblRole.Text = CurrentRole;

            // Demo phân quyền đơn giản
            if (CurrentRole != "Admin")
            {
                btnAdmin.Enabled = false;
            }

            // Mặc định mở trang Khóa học chẳng hạn
            // OpenChildForm(new CourseForm(CurrentUserId, CurrentRole));
        }

        /// <summary>
        /// Mở 1 form con bên trong panelContent (Dock = Fill, không border).
        /// </summary>
        private void OpenChildForm(Form childForm)
        {
            if (_currentChildForm != null)
            {
                _currentChildForm.Close();
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

        // ================== MENU BUTTONS ==================

        private void btnCourses_Click(object sender, EventArgs e)
        {
            // UC-02: Quản lý khóa học
            OpenChildForm(new CourseForm());
        }

        private void btnAssignments_Click(object sender, EventArgs e)
        {
            // UC-03: Quản lý bài tập
            OpenChildForm(new AssignmentForm());
        }

        private void btnSubmissions_Click(object sender, EventArgs e)
        {
            // UC-04: Nộp bài / chấm điểm
            OpenChildForm(new SubmissionForm(CurrentUserId, CurrentRole));
        }

        private void btnGrades_Click(object sender, EventArgs e)
        {
            // UC-05: Điểm & phản hồi
            OpenChildForm(new GradeManagementForm());
        }

        private void btnNotifications_Click(object sender, EventArgs e)
        {
            // UC-06: Thông báo
            OpenChildForm(new NotificationForm(CurrentUserId, CurrentRole));
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            // UC-07: Quản trị – chỉ cho Admin
            if (CurrentRole != "Admin")
            {
                MessageBox.Show("Chỉ Admin mới được dùng chức năng này.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenChildForm(new AdminForm(CurrentUserId));
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Đăng xuất: quay lại LoginForm
            this.Hide();
            using (var login = new LoginForm())
            {
                login.ShowDialog();
            }
            this.Close();
        }
    }
}
