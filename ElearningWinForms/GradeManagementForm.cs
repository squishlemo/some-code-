using System;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class GradeForm : Form
    {
        public int CurrentUserId { get; set; }
        public string CurrentRole { get; set; } // "SV" hoặc "GV"

        public GradeForm(int userId, string role)
        {
            InitializeComponent();
            CurrentUserId = userId;
            CurrentRole = role;
        }

        private void GradeForm_Load(object sender, EventArgs e)
        {
            // TODO:
            // SV: load điểm từ bảng Grades thông qua Submissions của sinh viên.
            // GV: thống kê điểm theo Assignment/Section.
        }

        private void btnRequestReview_Click(object sender, EventArgs e)
        {
            // TODO: tạo yêu cầu phúc khảo (nếu bạn thêm bảng ReviewRequests).
        }
    }
    /// <summary>
    /// Placeholder for GradeManagementForm to resolve CS0246.
    /// Replace with actual implementation as needed.
    /// </summary>
    public class GradeManagementForm : Form
    {
        public GradeManagementForm()
        {
            // Initialize form components here if needed
        }
    }
}
