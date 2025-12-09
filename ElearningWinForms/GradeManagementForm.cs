using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class GradeManagementForm : Form
    {
        public int CurrentUserId { get; }
        public string CurrentRole { get; }

        public GradeManagementForm(int userId, string role)
        {
            InitializeComponent();
            CurrentUserId = userId;
            CurrentRole = role;
        }

        private void GradeManagementForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (CurrentRole == "SV")
                {
                    btnRequestReview.Text = "Yêu cầu phúc khảo";
                    btnRequestReview.Enabled = true;
                    LoadGradesForStudent();
                }
                else if (CurrentRole == "GV")
                {
                    btnRequestReview.Text = "Yêu cầu phúc khảo (SV)";
                    btnRequestReview.Enabled = false;
                    LoadGradesForTeacher();
                }
                else
                {
                    btnRequestReview.Enabled = false;
                    LoadGradesForStudent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu điểm:\n" + ex.Message);
            }
        }

        private void LoadGradesForStudent()
        {
            object objStudentId = DbHelper.ExecuteScalar(
                "SELECT StudentId FROM Students WHERE UserId = @UserId",
                new SqlParameter("@UserId", CurrentUserId));

            if (objStudentId == null || objStudentId == DBNull.Value)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên.");
                dgvGrades.DataSource = null;
                return;
            }

            int studentId = Convert.ToInt32(objStudentId);

            string sql = @"
SELECT 
    g.GradeId     AS [Mã điểm],
    c.CourseCode  AS [Mã môn],
    c.CourseName  AS [Tên môn],
    a.Title       AS [Bài tập],
    g.Score       AS [Điểm],
    g.Comment     AS [Nhận xét],
    ug.FullName   AS [Giảng viên chấm],
    g.GradedTime  AS [Thời gian chấm]
FROM Grades g
JOIN Submissions s      ON g.SubmissionId = s.SubmissionId
JOIN Assignments a      ON s.AssignmentId = a.AssignmentId
JOIN CourseSections cs  ON a.SectionId = cs.SectionId
JOIN Courses c          ON cs.CourseId = c.CourseId
LEFT JOIN Users ug      ON g.GradedBy = ug.UserId
WHERE s.StudentId = @StudentId
ORDER BY g.GradedTime DESC;";

            DataTable dt = DbHelper.GetDataTable(
                sql,
                new SqlParameter("@StudentId", studentId));

            dgvGrades.DataSource = dt;
        }

        private void LoadGradesForTeacher()
        {
            // Với GV, GradedBy đang FK tới Users.UserId → dùng luôn CurrentUserId
            string sql = @"
SELECT 
    g.GradeId       AS [Mã điểm],
    sv.StudentId    AS [Mã SV],
    usv.FullName    AS [Sinh viên],
    c.CourseCode    AS [Mã môn],
    c.CourseName    AS [Tên môn],
    a.Title         AS [Bài tập],
    g.Score         AS [Điểm],
    g.Comment       AS [Nhận xét],
    g.GradedTime    AS [Thời gian chấm]
FROM Grades g
JOIN Submissions s      ON g.SubmissionId = s.SubmissionId
JOIN Students sv        ON s.StudentId = sv.StudentId
JOIN Users usv          ON sv.UserId = usv.UserId
JOIN Assignments a      ON s.AssignmentId = a.AssignmentId
JOIN CourseSections cs  ON a.SectionId = cs.SectionId
JOIN Courses c          ON cs.CourseId = c.CourseId
WHERE g.GradedBy = @GraderUserId
ORDER BY g.GradedTime DESC;";

            DataTable dt = DbHelper.GetDataTable(
                sql,
                new SqlParameter("@GraderUserId", CurrentUserId));

            dgvGrades.DataSource = dt;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (CurrentRole == "GV")
                LoadGradesForTeacher();
            else
                LoadGradesForStudent();
        }

        private void btnRequestReview_Click(object sender, EventArgs e)
        {
            if (CurrentRole != "SV") return;
            if (dgvGrades.CurrentRow == null)
            {
                MessageBox.Show("Chọn một dòng điểm trước đã.");
                return;
            }

            object gradeIdObj = dgvGrades.CurrentRow.Cells[0].Value;
            if (gradeIdObj == null || gradeIdObj == DBNull.Value) return;

            int gradeId = Convert.ToInt32(gradeIdObj);
            string reason = Microsoft.VisualBasic.Interaction.InputBox(
                "Nhập lý do phúc khảo:",
                "Yêu cầu phúc khảo",
                "");

            if (string.IsNullOrWhiteSpace(reason)) return;

            MessageBox.Show(
                $"[DEMO] Đã ghi nhận yêu cầu phúc khảo cho mã điểm {gradeId}.\nLý do: {reason}");
        }
    }
}
