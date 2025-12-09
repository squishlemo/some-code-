using System;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class AdminForm : Form
    {
        public int AdminUserId { get; set; }

        public AdminForm(int adminUserId)
        {
            InitializeComponent();
            AdminUserId = adminUserId;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            // TODO: load danh sách Users, Courses,... để admin quản lý.
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            // TODO: tạo user mới (Users + Students/Teachers/Admins).
        }

        private void btnLockUser_Click(object sender, EventArgs e)
        {
            // TODO: khóa user (update IsActive = 0).
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            // TODO: reset mật khẩu user.
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AdminForm
            // 
            this.ClientSize = new System.Drawing.Size(472, 261);
            this.Name = "AdminForm";
            this.ResumeLayout(false);

        }
    }
}
