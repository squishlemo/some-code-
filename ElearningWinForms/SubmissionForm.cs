using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using ElearningWinForms.Data;
using Microsoft.VisualBasic;   // để dùng Interaction.InputBox (chấm điểm, comment)

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

        // ======================== FORM LOAD ==========================
        private void SubmissionForm_Load(object sender, EventArgs e)
        {
            if (CurrentRole == "SV")
            {
                btnSubmit.Visible = true;
                btnSubmit.Enabled = true;

                btnGrade.Visible = false;

                LoadAssignmentsForStudent();
            }
            else if (CurrentRole == "GV")
            {
                btnSubmit.Visible = false;

                btnGrade.Visible = true;
                btnGrade.Enabled = true;

                LoadSubmissionsForTeacher();
            }
            else
            {
                // Vai trò khác (Admin, ...): tạm khóa 2 nút
                btnSubmit.Enabled = false;
                btnGrade.Enabled = false;

                MessageBox.Show("Chức năng này chỉ dành cho Sinh viên và Giảng viên.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // =============================================================
        // ========== DÙNG CHUNG GIỮA NHIỀU UC (NÊN TÁCH RA) ===========
        // Sau này bạn có thể tạo class:
        //   ElearningWinForms.Data.UserLookup
        // rồi chuyển 2 hàm này vào đó.
        // =============================================================

        private int? GetStudentIdByUserId(int userId)
        {
            string sql = "SELECT TOP 1 StudentId FROM Students WHERE UserId = @uid";

            object result = DbHelper.ExecuteScalar(
                sql,
                new SqlParameter("@uid", SqlDbType.Int) { Value = userId });

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        private int? GetTeacherIdByUserId(int userId)
        {
            string sql = "SELECT TOP 1 TeacherId FROM Teachers WHERE UserId = @uid";

            object result = DbHelper.ExecuteScalar(
                sql,
                new SqlParameter("@uid", SqlDbType.Int) { Value = userId });

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        // =================== HẾT PHẦN DÙNG CHUNG =====================


        // =============================================================
        // 1) SINH VIÊN: LOAD DANH SÁCH BÀI TẬP + TRẠNG THÁI NỘP
        // =============================================================

        private void LoadAssignmentsForStudent()
        {
            int? studentId = GetStudentIdByUserId(CurrentUserId);
            // NOTE:
            // - Nếu bạn có bảng Enrollments/CourseSections, có thể JOIN thêm để
            //   chỉ hiển thị bài tập mà SV đã đăng ký.
            // - Ở đây mình để đơn giản: lấy tất cả Assignments.

            string sql = @"
                SELECT 
                    a.AssignmentId,
                    a.Title,
                    a.Deadline,
                    s.SubmissionId,
                    s.SubmittedTime,
                    s.IsLate,
                    CASE 
                        WHEN s.SubmissionId IS NULL THEN N'Chưa nộp'
                        WHEN s.IsLate = 1 THEN N'Đã nộp (Trễ)'
                        ELSE N'Đã nộp'
                    END AS SubmissionStatus
                FROM Assignments a
                LEFT JOIN Submissions s
                    ON s.AssignmentId = a.AssignmentId
                   AND s.StudentId    = @sid
                ORDER BY a.Deadline DESC, a.AssignmentId;";

            DataTable table = DbHelper.GetDataTable(
                sql,
                new SqlParameter("@sid", SqlDbType.Int) { Value = studentId.Value });

            dgvSubmissions.DataSource = table;
        }

        // =============================================================
        // 2) GIẢNG VIÊN: LOAD DANH SÁCH BÀI NỘP ĐỂ CHẤM
        // =============================================================

        private void LoadSubmissionsForTeacher()
        {
            int? studentId = GetStudentIdByUserId(CurrentUserId);
            string sql = @"
                SELECT 
                    s.SubmissionId,
                    s.AssignmentId,
                    a.Title      AS AssignmentTitle,
                    u.FullName   AS StudentName,
                    s.SubmittedTime,
                    s.IsLate,
                    g.Score,
                    g.Comment
                FROM Submissions s
                JOIN Assignments a ON s.AssignmentId = a.AssignmentId
                JOIN Students st   ON s.StudentId   = st.StudentId
                JOIN Users u       ON st.UserId    = u.UserId
                LEFT JOIN Grades g ON g.SubmissionId = s.SubmissionId
                ORDER BY a.Deadline DESC, u.FullName;";

            DataTable table = DbHelper.GetDataTable(sql);
            dgvSubmissions.DataSource = table;
        }

        // =============================================================
        // 3) NÚT NỘP BÀI – DÀNH CHO SINH VIÊN
        // =============================================================

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (CurrentRole != "SV")
            {
                MessageBox.Show("Nút này chỉ dùng cho Sinh viên nộp bài.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvSubmissions.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một bài tập trong danh sách.",
                    "Thiếu chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rowView = dgvSubmissions.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("Không đọc được dữ liệu dòng đang chọn.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow row = rowView.Row;

            int assignmentId = Convert.ToInt32(row["AssignmentId"]);
            DateTime deadline = row["Deadline"] == DBNull.Value
                ? DateTime.MaxValue
                : Convert.ToDateTime(row["Deadline"]);

            // Nếu đã có SubmissionId → hỏi có nộp lại không
            if (row["SubmissionId"] != DBNull.Value)
            {
                DialogResult confirm = MessageBox.Show(
                    "Bạn đã nộp bài cho bài tập này.\nBạn có muốn nộp lại (file cũ sẽ bị ghi đè)?",
                    "Xác nhận nộp lại",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Chọn file bài làm để nộp";
                ofd.Filter = "Tất cả các file (*.*)|*.*";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string sourcePath = ofd.FileName;

                // ======== DÙNG CHUNG: kiểm tra dung lượng file Upload ========
                // Sau này bạn có thể đưa MaxFileSize lên cấu hình hoặc bảng Assignments.
                const double MaxFileSizeMb = 20;

                FileInfo fi = new FileInfo(sourcePath);
                double fileSizeMb = fi.Length / (1024.0 * 1024.0);

                if (fileSizeMb > MaxFileSizeMb)
                {
                    MessageBox.Show(
                        $"File vượt quá dung lượng cho phép ({MaxFileSizeMb} MB).",
                        "File quá lớn",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // ======== DÙNG CHUNG: copy file vào thư mục Uploads =========
                // Có thể tách thành FileStorageService.SaveSubmissionFile(...)
                string uploadFolder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Uploads");
                Directory.CreateDirectory(uploadFolder);

                string destFileName =
                    $"{assignmentId}_U{CurrentUserId}_{DateTime.Now:yyyyMMddHHmmss}{fi.Extension}";
                string destPath = Path.Combine(uploadFolder, destFileName);

                File.Copy(sourcePath, destPath, overwrite: true);
                // ===================== HẾT PHẦN DÙNG CHUNG ===================

                int? studentId = GetStudentIdByUserId(CurrentUserId);
                if (studentId == null)
                {
                    MessageBox.Show("Không tìm thấy mã sinh viên cho tài khoản hiện tại.",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool isLate = DateTime.Now > deadline;

                // Upsert vào Submissions (INSERT nếu chưa có, UPDATE nếu đã có)
                string sqlCheck = @"
                    SELECT SubmissionId 
                    FROM Submissions 
                    WHERE AssignmentId = @aid AND StudentId = @sid;";

                object existingId = DbHelper.ExecuteScalar(
                    sqlCheck,
                    new SqlParameter("@aid", SqlDbType.Int) { Value = assignmentId },
                    new SqlParameter("@sid", SqlDbType.Int) { Value = studentId.Value });

                if (existingId == null || existingId == DBNull.Value)
                {
                    string sqlInsert = @"
                        INSERT INTO Submissions
                            (AssignmentId, StudentId, FilePath, SubmittedTime, IsLate)
                        VALUES
                            (@aid, @sid, @path, @time, @late);";

                    DbHelper.ExecuteNonQuery(
                        sqlInsert,
                        new SqlParameter("@aid", SqlDbType.Int) { Value = assignmentId },
                        new SqlParameter("@sid", SqlDbType.Int) { Value = studentId.Value },
                        new SqlParameter("@path", SqlDbType.NVarChar, 500) { Value = destPath },
                        new SqlParameter("@time", SqlDbType.DateTime) { Value = DateTime.Now },
                        new SqlParameter("@late", SqlDbType.Bit) { Value = isLate });
                }
                else
                {
                    int submissionId = Convert.ToInt32(existingId);

                    string sqlUpdate = @"
                        UPDATE Submissions
                        SET FilePath      = @path,
                            SubmittedTime = @time,
                            IsLate        = @late
                        WHERE SubmissionId = @id;";

                    DbHelper.ExecuteNonQuery(
                        sqlUpdate,
                        new SqlParameter("@path", SqlDbType.NVarChar, 500) { Value = destPath },
                        new SqlParameter("@time", SqlDbType.DateTime) { Value = DateTime.Now },
                        new SqlParameter("@late", SqlDbType.Bit) { Value = isLate },
                        new SqlParameter("@id", SqlDbType.Int) { Value = submissionId });
                }

                MessageBox.Show("Nộp bài thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload lại grid để cập nhật trạng thái
                LoadAssignmentsForStudent();
            }
        }

        // =============================================================
        // 4) NÚT CHẤM ĐIỂM – DÀNH CHO GIẢNG VIÊN
        // =============================================================

        private void btnGrade_Click(object sender, EventArgs e)
        {
            if (CurrentRole != "GV")
            {
                MessageBox.Show("Nút này chỉ dùng cho Giảng viên chấm điểm.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvSubmissions.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một bài nộp trong danh sách.",
                    "Thiếu chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rowView = dgvSubmissions.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("Không đọc được dữ liệu dòng đang chọn.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow row = rowView.Row;

            int submissionId = Convert.ToInt32(row["SubmissionId"]);

            string studentName = row.Table.Columns.Contains("StudentName")
                ? row["StudentName"].ToString()
                : "";
            string assignmentTitle = row.Table.Columns.Contains("AssignmentTitle")
                ? row["AssignmentTitle"].ToString()
                : "";

            string currentScoreStr =
                row.Table.Columns.Contains("Score") && row["Score"] != DBNull.Value
                    ? row["Score"].ToString()
                    : "";

            // ======== DÙNG CHUNG: nhập điểm nhanh bằng InputBox =========
            string inputScore = Interaction.InputBox(
                $"Nhập điểm cho {studentName}\nBài: {assignmentTitle}",
                "Chấm điểm",
                currentScoreStr);

            if (string.IsNullOrWhiteSpace(inputScore))
                return;

            if (!decimal.TryParse(inputScore, out decimal score))
            {
                MessageBox.Show("Điểm phải là số.",
                    "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (score < 0 || score > 10)
            {
                MessageBox.Show("Điểm phải nằm trong khoảng 0 - 10.",
                    "Giá trị không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentComment =
                row.Table.Columns.Contains("Comment") && row["Comment"] != DBNull.Value
                    ? row["Comment"].ToString()
                    : "";

            string inputComment = Interaction.InputBox(
                "Nhập nhận xét (có thể để trống):",
                "Nhận xét",
                currentComment);
            // ===================== HẾT PHẦN DÙNG CHUNG ===================

            // Upsert vào Grades (INSERT hoặc UPDATE)
            string sqlCheck = "SELECT GradeId FROM Grades WHERE SubmissionId = @sid;";

            object gradeIdObj = DbHelper.ExecuteScalar(
                sqlCheck,
                new SqlParameter("@sid", SqlDbType.Int) { Value = submissionId });

            DateTime now = DateTime.Now;

            if (gradeIdObj == null || gradeIdObj == DBNull.Value)
            {
                string sqlInsert = @"
                    INSERT INTO Grades
                        (SubmissionId, Score, Comment, GradedBy, GradedTime)
                    VALUES
                        (@sid, @score, @comment, @grader, @time);";

                DbHelper.ExecuteNonQuery(
                    sqlInsert,
                    new SqlParameter("@sid", SqlDbType.Int) { Value = submissionId },
                    new SqlParameter("@score", SqlDbType.Decimal) { Value = score },
                    new SqlParameter("@comment", SqlDbType.NVarChar, 1000)
                    {
                        Value = string.IsNullOrWhiteSpace(inputComment)
                            ? (object)DBNull.Value
                            : inputComment
                    },
                    new SqlParameter("@grader", SqlDbType.Int) { Value = CurrentUserId },
                    new SqlParameter("@time", SqlDbType.DateTime) { Value = now });
            }
            else
            {
                int gradeId = Convert.ToInt32(gradeIdObj);

                string sqlUpdate = @"
                    UPDATE Grades
                    SET Score      = @score,
                        Comment    = @comment,
                        GradedBy   = @grader,
                        GradedTime = @time
                    WHERE GradeId  = @id;";

                DbHelper.ExecuteNonQuery(
                    sqlUpdate,
                    new SqlParameter("@score", SqlDbType.Decimal) { Value = score },
                    new SqlParameter("@comment", SqlDbType.NVarChar, 1000)
                    {
                        Value = string.IsNullOrWhiteSpace(inputComment)
                            ? (object)DBNull.Value
                            : inputComment
                    },
                    new SqlParameter("@grader", SqlDbType.Int) { Value = CurrentUserId },
                    new SqlParameter("@time", SqlDbType.DateTime) { Value = now },
                    new SqlParameter("@id", SqlDbType.Int) { Value = gradeId });
            }

            MessageBox.Show("Lưu điểm thành công.",
                "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Load lại để thấy điểm update
            LoadSubmissionsForTeacher();
        }
    }
}
