using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ElearningWinForms.Data;
using Microsoft.VisualBasic;

namespace ElearningWinForms
{
    public partial class GradeForm : Form
    {
        private readonly int _currentUserId;
        private readonly string _currentRole;  // "SV" hoặc "GV" hoặc "Admin"

        public GradeForm(int currentUserId, string currentRole)
        {
            InitializeComponent();
            _currentUserId = currentUserId;
            _currentRole = currentRole;
            AdjustUiByRole();
        }

        private void GradeForm_Load(object sender, EventArgs e)
        {
            AdjustUiByRole();
            try
            {
                if (_currentRole == "SV")
                {
                    LoadGradesForStudent();
                }
                else LoadReviewRequestsForTeacher();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu điểm/phúc khảo: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdjustUiByRole()
        {
            if (_currentRole == "SV")
            {
                // SV: chỉ dùng bảng điểm, ẩn phần phúc khảo GV
                lblReviews.Visible = false;
                dgvReviewRequests.Visible = false;

                lblGrades.Visible = true;
                dgvGrades.Visible = true;
                dgvGrades.Dock = DockStyle.Fill;
            }
            else if (_currentRole == "GV")
            {
                // GV: chỉ dùng bảng phúc khảo, ẩn bảng điểm SV
                lblGrades.Visible = false;
                dgvGrades.Visible = false;

                // Đẩy label + grid phúc khảo lên chỗ của bảng điểm
                lblReviews.Top = lblGrades.Top;
                dgvReviewRequests.Top = dgvGrades.Top;

                // Cho bảng phúc khảo chiếm gần hết form
                dgvReviewRequests.Height = this.ClientSize.Height - dgvReviewRequests.Top - 70;
                dgvReviewRequests.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }
            else
            {
                // Admin: tùy, tạm coi giống GV
                lblGrades.Visible = false;
                dgvGrades.Visible = false;

                lblReviews.Top = lblGrades.Top;
                dgvReviewRequests.Top = dgvGrades.Top;
                dgvReviewRequests.Height = this.ClientSize.Height - dgvReviewRequests.Top - 70;
                dgvReviewRequests.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }
        }

        // ====================== PHẦN SV – XEM ĐIỂM & GỬI PHÚC KHẢO ======================
        private void LoadGradesForStudent()
        {
            // Lấy StudentId từ UserId
            object studentIdObj = DbHelper.ExecuteScalar(
                "SELECT StudentId FROM Students WHERE UserId = @uid",
                new SqlParameter("@uid", _currentUserId));

            if (studentIdObj == null || studentIdObj == DBNull.Value)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvGrades.DataSource = null;
                return;
            }

            int studentId = Convert.ToInt32(studentIdObj);

            string sql = @"
SELECT 
    g.GradeId                         AS GradeIdInternal,
    c.CourseCode                      AS [Mã môn],
    c.CourseName                      AS [Tên môn],
    a.Title                           AS [Bài tập],
    g.Score                           AS [Điểm],
    g.Comment                         AS [Nhận xét],
    uGV.FullName                      AS [Giảng viên],
    g.GradedTime                      AS [Thời gian chấm],
    sub.FilePath                      AS FilePathInternal,
    ISNULL(rr.Status, N'Chưa yêu cầu')    AS [Trạng thái phúc khảo],
    rr.Response                       AS [Phản hồi phúc khảo],
    rr.NewScore                       AS [Điểm sau phúc khảo]
FROM Grades g
JOIN Submissions sub  ON g.SubmissionId = sub.SubmissionId
JOIN Assignments a    ON sub.AssignmentId = a.AssignmentId
JOIN CourseSections cs ON a.SectionId = cs.SectionId
JOIN Courses c        ON cs.CourseId = c.CourseId
LEFT JOIN Users uGV   ON g.GradedBy = uGV.UserId
OUTER APPLY (
    SELECT TOP 1 r.Status, r.Response, r.NewScore
    FROM GradeReviewRequests r
    WHERE r.GradeId = g.GradeId
      AND r.StudentId = @StudentId
    ORDER BY r.RequestedTime DESC
) rr
WHERE sub.StudentId = @StudentId
ORDER BY g.GradedTime DESC;";

            DataTable dt = DbHelper.GetDataTable(sql,
                new SqlParameter("@StudentId", studentId));

            dgvGrades.DataSource = dt;

            // Ẩn cột kỹ thuật
            if (dgvGrades.Columns.Contains("GradeIdInternal"))
                dgvGrades.Columns["GradeIdInternal"].Visible = false;
            if (dgvGrades.Columns.Contains("FilePathInternal"))
                dgvGrades.Columns["FilePathInternal"].Visible = false;
        }

        private void btnRequestReview_Click(object sender, EventArgs e)
        {
            if (_currentRole != "SV")
                return;

            if (dgvGrades.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng điểm để gửi phúc khảo.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            object gradeIdObj = dgvGrades.CurrentRow.Cells["GradeIdInternal"].Value;
            if (gradeIdObj == null || gradeIdObj == DBNull.Value)
            {
                MessageBox.Show("Không lấy được mã điểm của dòng đang chọn.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int gradeId = Convert.ToInt32(gradeIdObj);

            // Lấy StudentId
            object studentIdObj = DbHelper.ExecuteScalar(
                "SELECT StudentId FROM Students WHERE UserId = @uid",
                new SqlParameter("@uid", _currentUserId));

            if (studentIdObj == null || studentIdObj == DBNull.Value)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int studentId = Convert.ToInt32(studentIdObj);

            // Kiểm tra đã có yêu cầu Pending chưa
            string checkSql = @"
SELECT COUNT(*) FROM GradeReviewRequests
WHERE GradeId = @GradeId
  AND StudentId = @StudentId
  AND Status = N'Pending';";

            int pendingCount = Convert.ToInt32(DbHelper.ExecuteScalar(
                checkSql,
                new SqlParameter("@GradeId", gradeId),
                new SqlParameter("@StudentId", studentId)));

            if (pendingCount > 0)
            {
                MessageBox.Show("Bạn đã gửi yêu cầu phúc khảo cho điểm này, đang chờ giảng viên xử lý.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Hỏi lý do
            string reason = Interaction.InputBox(
                "Nhập nội dung / lý do phúc khảo:",
                "Yêu cầu phúc khảo",
                "");

            if (string.IsNullOrWhiteSpace(reason))
            {
                // Bấm Cancel hoặc bỏ trống
                return;
            }

            string insertSql = @"
INSERT INTO GradeReviewRequests
    (GradeId, StudentId, RequesterUserId, Reason)
VALUES
    (@GradeId, @StudentId, @RequesterUserId, @Reason);";

            DbHelper.ExecuteNonQuery(insertSql,
                new SqlParameter("@GradeId", gradeId),
                new SqlParameter("@StudentId", studentId),
                new SqlParameter("@RequesterUserId", _currentUserId),
                new SqlParameter("@Reason", reason.Trim()));

            MessageBox.Show("Đã gửi yêu cầu phúc khảo, vui lòng chờ giảng viên xem xét.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Tải lại bảng điểm để cập nhật trạng thái
            LoadGradesForStudent();
        }

        // ====================== MỞ FILE BÀI NỘP (SV & GV) ======================
        private void btnViewSubmission_Click(object sender, EventArgs e)
        {
            // Ưu tiên: nếu GV đang chọn một yêu cầu phúc khảo -> mở file của dòng đó.
            if (_currentRole != "SV" && dgvReviewRequests.Visible && dgvReviewRequests.CurrentRow != null)
            {
                OpenSubmissionFileFromGrid(dgvReviewRequests);
            }
            else
            {
                OpenSubmissionFileFromGrid(dgvGrades);
            }
        }

        private void OpenSubmissionFileFromGrid(DataGridView grid)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng có bài nộp.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!grid.Columns.Contains("FilePathInternal"))
            {
                MessageBox.Show("Không tìm thấy đường dẫn file trong dữ liệu.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            object pathObj = grid.CurrentRow.Cells["FilePathInternal"].Value;
            if (pathObj == null || pathObj == DBNull.Value)
            {
                MessageBox.Show("Dòng được chọn chưa có file bài nộp.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = pathObj.ToString();
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Đường dẫn file trống.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("File không tồn tại ở đường dẫn:\n" + path,
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở file: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ====================== PHẦN GV – XEM & XỬ LÝ PHÚC KHẢO ======================
        private void LoadReviewRequestsForTeacher()
{
    // GradedBy đang lưu Users.UserId của GV, nên dùng luôn _currentUserId
    int graderUserId = _currentUserId;

    string sql = @"
SELECT 
    r.ReviewId          AS ReviewIdInternal,
    r.GradeId           AS GradeIdInternal,
    s.StudentId         AS StudentIdInternal,
    usv.FullName        AS [Sinh viên],
    c.CourseCode        AS [Mã môn],
    c.CourseName        AS [Tên môn],
    a.Title             AS [Bài tập],
    g.Score             AS [Điểm hiện tại],
    r.Status            AS [Trạng thái],
    r.Reason            AS [Lý do phúc khảo],
    r.RequestedTime     AS [Thời gian yêu cầu],
    r.NewScore          AS [Điểm sau phúc khảo],
    r.Response          AS [Phản hồi với SV],
    sub.FilePath        AS FilePathInternal
FROM GradeReviewRequests r
JOIN Grades g        ON r.GradeId = g.GradeId
JOIN Submissions sub ON g.SubmissionId = sub.SubmissionId
JOIN Students s      ON r.StudentId = s.StudentId
JOIN Users usv       ON s.UserId = usv.UserId
JOIN Assignments a   ON sub.AssignmentId = a.AssignmentId
JOIN CourseSections cs ON a.SectionId = cs.SectionId
JOIN Courses c       ON cs.CourseId = c.CourseId
WHERE g.GradedBy = @GraderUserId   -- CHỈ lấy những bài do GV này chấm
ORDER BY 
    CASE WHEN r.Status = N'Pending' THEN 0 ELSE 1 END,
    r.RequestedTime;";

    DataTable dt = DbHelper.GetDataTable(sql,
        new SqlParameter("@GraderUserId", graderUserId));

    dgvReviewRequests.DataSource = dt;

    // Ẩn các cột kỹ thuật
    if (dgvReviewRequests.Columns.Contains("ReviewIdInternal"))
        dgvReviewRequests.Columns["ReviewIdInternal"].Visible = false;
    if (dgvReviewRequests.Columns.Contains("GradeIdInternal"))
        dgvReviewRequests.Columns["GradeIdInternal"].Visible = false;
    if (dgvReviewRequests.Columns.Contains("StudentIdInternal"))
        dgvReviewRequests.Columns["StudentIdInternal"].Visible = false;
    if (dgvReviewRequests.Columns.Contains("FilePathInternal"))
        dgvReviewRequests.Columns["FilePathInternal"].Visible = false;
}

        private void btnApproveReview_Click(object sender, EventArgs e)
        {
            if (_currentRole != "GV" && _currentRole != "Admin")
                return;

            if (dgvReviewRequests.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một yêu cầu phúc khảo.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            object reviewIdObj = dgvReviewRequests.CurrentRow.Cells["ReviewIdInternal"].Value;
            object gradeIdObj = dgvReviewRequests.CurrentRow.Cells["GradeIdInternal"].Value;
            if (reviewIdObj == null || gradeIdObj == null)
            {
                MessageBox.Show("Không lấy được mã yêu cầu/điểm.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int reviewId = Convert.ToInt32(reviewIdObj);
            int gradeId = Convert.ToInt32(gradeIdObj);

            decimal currentScore = 0;
            try
            {
                object curScoreObj = dgvReviewRequests.CurrentRow.Cells["Điểm hiện tại"].Value;
                if (curScoreObj != null && curScoreObj != DBNull.Value)
                    currentScore = Convert.ToDecimal(curScoreObj);
            }
            catch { }

            // Nhập điểm mới
            string scoreText = Interaction.InputBox(
                "Nhập điểm mới (0-10):",
                "Duyệt phúc khảo",
                currentScore.ToString("0.##"));

            if (string.IsNullOrWhiteSpace(scoreText))
                return;

            if (!decimal.TryParse(scoreText, out decimal newScore))
            {
                MessageBox.Show("Điểm mới không hợp lệ.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Nhập phản hồi cho SV
            string response = Interaction.InputBox(
                "Nhập phản hồi gửi cho sinh viên:",
                "Phản hồi phúc khảo",
                "Đã điều chỉnh điểm theo bài làm.");

            if (string.IsNullOrWhiteSpace(response))
                response = "Đã duyệt phúc khảo và điều chỉnh điểm.";

            // Cập nhật Grades + GradeReviewRequests
            string updateGradeSql = @"
UPDATE Grades
SET Score = @NewScore,
    Comment = ISNULL(Comment, N'') + CHAR(13)+CHAR(10) + N'[Phúc khảo] ' + @Response,
    GradedTime = GETDATE()
WHERE GradeId = @GradeId;";

            string updateReviewSql = @"
UPDATE GradeReviewRequests
SET Status       = N'Approved',
    Response     = @Response,
    NewScore     = @NewScore,
    ReviewedBy   = @Reviewer,
    ReviewedTime = GETDATE()
WHERE ReviewId = @ReviewId;";

            DbHelper.ExecuteNonQuery(updateGradeSql,
                new SqlParameter("@NewScore", newScore),
                new SqlParameter("@Response", response),
                new SqlParameter("@GradeId", gradeId));

            DbHelper.ExecuteNonQuery(updateReviewSql,
                new SqlParameter("@NewScore", newScore),
                new SqlParameter("@Response", response),
                new SqlParameter("@Reviewer", _currentUserId),
                new SqlParameter("@ReviewId", reviewId));

            MessageBox.Show("Đã duyệt phúc khảo và cập nhật điểm.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadReviewRequestsForTeacher();
        }

        private void btnRejectReview_Click(object sender, EventArgs e)
        {
            if (_currentRole != "GV" && _currentRole != "Admin")
                return;

            if (dgvReviewRequests.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một yêu cầu phúc khảo.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            object reviewIdObj = dgvReviewRequests.CurrentRow.Cells["ReviewIdInternal"].Value;
            if (reviewIdObj == null)
            {
                MessageBox.Show("Không lấy được mã yêu cầu.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int reviewId = Convert.ToInt32(reviewIdObj);

            string response = Interaction.InputBox(
                "Nhập lý do từ chối phúc khảo để gửi cho sinh viên:",
                "Từ chối phúc khảo",
                "Bài làm đúng với thang điểm đã chấm, không điều chỉnh điểm.");

            if (string.IsNullOrWhiteSpace(response))
                response = "Không điều chỉnh điểm sau khi xem lại bài.";

            string sql = @"
UPDATE GradeReviewRequests
SET Status       = N'Rejected',
    Response     = @Response,
    ReviewedBy   = @Reviewer,
    ReviewedTime = GETDATE()
WHERE ReviewId = @ReviewId;";

            DbHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Response", response),
                new SqlParameter("@Reviewer", _currentUserId),
                new SqlParameter("@ReviewId", reviewId));

            MessageBox.Show("Đã từ chối yêu cầu phúc khảo.",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadReviewRequestsForTeacher();
        }

        // ====================== NÚT TẢI LẠI ======================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentRole == "SV")
                    LoadGradesForStudent();
                else
                    LoadReviewRequestsForTeacher();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lại dữ liệu: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblGrades_Click(object sender, EventArgs e)
        {

        }
    }
}
