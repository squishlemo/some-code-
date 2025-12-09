using System;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class SubmissionForm : Form
    {
        public int CurrentUserId { get; set; }
        public string CurrentRole { get; set; }   // "SV" hoặc "GV"

        public SubmissionForm(int userId, string role)
        {
            InitializeComponent();
            CurrentUserId = userId;
            CurrentRole = role;
        }

        private void SubmissionForm_Load(object sender, EventArgs e)
        {
            // TODO:
            // Nếu là SV: hiển thị các bài tập để nộp.
            // Nếu là GV: hiển thị danh sách bài nộp để chấm.
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // TODO: insert vào Submissions (FilePath, IsLate, ...).
        }

        private void btnGrade_Click(object sender, EventArgs e)
        {
            // TODO: GV chấm điểm → insert vào Grades.
        }
    }
}
