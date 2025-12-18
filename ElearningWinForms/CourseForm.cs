using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class CourseForm : Form
    {
        private readonly int _currentUserId;
        private readonly string _currentRole; // "SV" hoặc "GV"
        private int? _selectedSectionId = null;

        public CourseForm(int userId, string role)
        {
            InitializeComponent();
            _currentUserId = userId;
            _currentRole = role;
        }

        private void CourseForm_Load(object sender, EventArgs e)
        {
            if (_currentRole == "SV")
            {
                lblTitle.Text = "Đăng ký / hủy đăng ký học phần (Sinh viên)";
                btnEnroll.Enabled = true;
                btnCancelEnroll.Enabled = true;
                btnViewStudents.Visible = false;
                LoadSectionsForStudent();
            }
            else if (_currentRole == "GV")
            {
                lblTitle.Text = "Các học phần đang giảng dạy (Giảng viên)";
                btnEnroll.Enabled = false;
                btnCancelEnroll.Enabled = false;
                btnViewStudents.Visible = true;
                LoadSectionsForTeacher();
            }
            else if (_currentRole == "Admin")
            {
                lblTitle.Text = "Xem học phần & danh sách SV đăng ký (Admin)";
                btnEnroll.Enabled = false;
                btnCancelEnroll.Enabled = false;
                btnViewStudents.Visible = true;
                LoadSectionsForAdmin();
            }
            else
            {
                // Trường hợp khác: chỉ xem danh sách
                lblTitle.Text = "Danh sách học phần";
                btnEnroll.Enabled = false;
                btnCancelEnroll.Enabled = false;
                btnViewStudents.Visible = false;
                LoadAllOpenSections();
            }
        }

        // ================== HELPER LẤY ID SV / GV ==================

        private int? GetCurrentStudentId()
        {
            object result = DbHelper.ExecuteScalar(
                "SELECT StudentId FROM Students WHERE UserId = @uid",
                new SqlParameter("@uid", _currentUserId));

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        private int? GetCurrentTeacherId()
        {
            object result = DbHelper.ExecuteScalar(
                "SELECT TeacherId FROM Teachers WHERE UserId = @uid",
                new SqlParameter("@uid", _currentUserId));

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        // ================== LOAD DỮ LIỆU LÊN GRID ==================

        private void LoadSectionsForStudent()
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                MessageBox.Show("Tài khoản này chưa được gắn với sinh viên.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = @"
SELECT 
    s.SectionId,
    c.CourseCode,
    c.CourseName,
    s.Semester,
    s.[Year],
    s.Room,
    s.MaxEnrollment,
    s.CurrentEnrollment,
    s.Status,
    CASE WHEN e.EnrollmentId IS NULL THEN 0 ELSE 1 END AS IsEnrolled
FROM CourseSections s
JOIN Courses c ON s.CourseId = c.CourseId
LEFT JOIN Enrollments e
       ON e.SectionId = s.SectionId
      AND e.StudentId = @StudentId
WHERE s.Status = N'Open'
ORDER BY c.CourseCode, s.Semester, s.[Year];";

            DataTable table = DbHelper.GetDataTable(
                sql,
                new SqlParameter("@StudentId", studentId.Value));

            BindSectionGrid(table, forStudent: true);
        }

        private void LoadSectionsForTeacher()
        {
            var teacherId = GetCurrentTeacherId();
            if (teacherId == null)
            {
                MessageBox.Show("Tài khoản này chưa được gắn với giảng viên.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sql = @"
SELECT 
    s.SectionId,
    c.CourseCode,
    c.CourseName,
    s.Semester,
    s.[Year],
    s.Room,
    s.MaxEnrollment,
    s.CurrentEnrollment,
    s.Status
FROM CourseSections s
JOIN Courses c ON s.CourseId = c.CourseId
WHERE s.TeacherId = @TeacherId
ORDER BY c.CourseCode, s.Semester, s.[Year];";

            DataTable table = DbHelper.GetDataTable(
                sql,
                new SqlParameter("@TeacherId", teacherId.Value));

            BindSectionGrid(table, forStudent: false);
        }

        private void LoadAllOpenSections()
        {
            string sql = @"
SELECT 
    s.SectionId,
    c.CourseCode,
    c.CourseName,
    s.Semester,
    s.[Year],
    s.Room,
    s.MaxEnrollment,
    s.CurrentEnrollment,
    s.Status
FROM CourseSections s
JOIN Courses c ON s.CourseId = c.CourseId
WHERE s.Status = N'Open'
ORDER BY c.CourseCode, s.Semester, s.[Year];";

            DataTable table = DbHelper.GetDataTable(sql);
            BindSectionGrid(table, forStudent: false);
        }

        private void LoadSectionsForAdmin()
        {
            string sql = @"
SELECT 
    s.SectionId,
    c.CourseCode,
    c.CourseName,
    s.Semester,
    s.[Year],
    s.Room,
    s.MaxEnrollment,
    s.CurrentEnrollment,
    s.Status,
    u.FullName AS TeacherName
FROM CourseSections s
JOIN Courses c ON s.CourseId = c.CourseId
LEFT JOIN Teachers t ON s.TeacherId = t.TeacherId
LEFT JOIN Users u ON t.UserId = u.UserId
ORDER BY c.CourseCode, s.Semester, s.[Year];";

            DataTable table = DbHelper.GetDataTable(sql);
            BindSectionGrid(table, forStudent: false);
        }

        private void BindSectionGrid(DataTable table, bool forStudent)
        {
            dgvSections.DataSource = table;
            dgvSections.AutoGenerateColumns = true;

            if (dgvSections.Columns["SectionId"] != null)
                dgvSections.Columns["SectionId"].Visible = false;

            if (dgvSections.Columns["IsEnrolled"] != null)
            {
                dgvSections.Columns["IsEnrolled"].HeaderText = "Đã đăng ký?";
                dgvSections.Columns["IsEnrolled"].DisplayIndex = dgvSections.Columns.Count - 1;
            }

            dgvSections.ClearSelection();

            btnEnroll.Enabled = forStudent && _currentRole == "SV";
            btnCancelEnroll.Enabled = forStudent && _currentRole == "SV";
        }

        private void btnViewStudents_Click(object sender, EventArgs e)
        {
            if (_selectedSectionId == null)
            {
                MessageBox.Show("Vui lòng chọn một học phần trong danh sách.");
                return;
            }

            if (_currentRole != "GV" && _currentRole != "Admin")
            {
                MessageBox.Show("Chỉ Giảng viên/Admin mới xem được danh sách sinh viên đăng ký.");
                return;
            }

            using (var f = new EnrolledStudentsForm(_currentUserId, _currentRole, _selectedSectionId.Value))
            {
                f.ShowDialog(this);
            }
        }

        // ================== SỰ KIỆN GRID ==================

        private void dgvSections_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSections.CurrentRow != null &&
                dgvSections.CurrentRow.Cells["SectionId"].Value != null &&
                dgvSections.CurrentRow.Cells["SectionId"].Value != DBNull.Value)
            {
                _selectedSectionId =
                    Convert.ToInt32(dgvSections.CurrentRow.Cells["SectionId"].Value);
            }
        }

        // ================== ĐĂNG KÝ / HỦY ĐĂNG KÝ ==================

        private void btnEnroll_Click(object sender, EventArgs e)
        {
            if (_currentRole != "SV")
            {
                MessageBox.Show("Chỉ sinh viên mới được đăng ký học phần.");
                return;
            }

            if (_selectedSectionId == null)
            {
                MessageBox.Show("Vui lòng chọn một học phần trong danh sách.");
                return;
            }

            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên.");
                return;
            }

            // Kiểm tra xem đã đăng ký chưa
            string sqlCheck = @"
SELECT COUNT(*) 
FROM Enrollments 
WHERE StudentId = @StudentId AND SectionId = @SectionId;";

            int count = Convert.ToInt32(DbHelper.ExecuteScalar(
                sqlCheck,
                new SqlParameter("@StudentId", studentId.Value),
                new SqlParameter("@SectionId", _selectedSectionId.Value)));

            if (count > 0)
            {
                MessageBox.Show("Bạn đã đăng ký học phần này rồi.");
                return;
            }

            // Kiểm tra chỗ trống
            string sqlSlot = @"
SELECT MaxEnrollment, CurrentEnrollment, Status
FROM CourseSections
WHERE SectionId = @SectionId;";

            DataTable dt = DbHelper.GetDataTable(
                sqlSlot,
                new SqlParameter("@SectionId", _selectedSectionId.Value));

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Không tìm thấy thông tin học phần.");
                return;
            }

            int maxEnroll = Convert.ToInt32(dt.Rows[0]["MaxEnrollment"]);
            int currentEnroll = Convert.ToInt32(dt.Rows[0]["CurrentEnrollment"]);
            string status = dt.Rows[0]["Status"].ToString();

            if (!string.Equals(status, "Open", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Học phần này không còn trạng thái mở để đăng ký.");
                return;
            }

            if (currentEnroll >= maxEnroll)
            {
                MessageBox.Show("Học phần đã đủ số lượng sinh viên.");
                return;
            }

            string sql = @"
INSERT INTO Enrollments(StudentId, SectionId, Status)
VALUES (@StudentId, @SectionId, N'Active');

UPDATE CourseSections
SET CurrentEnrollment = CurrentEnrollment + 1
WHERE SectionId = @SectionId;";

            try
            {
                DbHelper.ExecuteNonQuery(
                    sql,
                    new SqlParameter("@StudentId", studentId.Value),
                    new SqlParameter("@SectionId", _selectedSectionId.Value));

                MessageBox.Show("Đăng ký học phần thành công.");
                LoadSectionsForStudent();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi khi đăng ký: " + ex.Message);
            }
        }

        private void btnCancelEnroll_Click(object sender, EventArgs e)
        {
            if (_currentRole != "SV")
            {
                MessageBox.Show("Chỉ sinh viên mới được hủy đăng ký.");
                return;
            }

            if (_selectedSectionId == null)
            {
                MessageBox.Show("Vui lòng chọn một học phần để hủy.");
                return;
            }

            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                MessageBox.Show("Không tìm thấy thông tin sinh viên.");
                return;
            }

            if (MessageBox.Show("Bạn chắc chắn muốn hủy đăng ký học phần này?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes)
            {
                return;
            }

            string sql = @"
IF EXISTS (SELECT 1 FROM Enrollments 
           WHERE StudentId = @StudentId AND SectionId = @SectionId AND Status = N'Active')
BEGIN
    DELETE FROM Enrollments
    WHERE StudentId = @StudentId AND SectionId = @SectionId AND Status = N'Active';

    UPDATE CourseSections
    SET CurrentEnrollment = CASE WHEN CurrentEnrollment > 0 
                                 THEN CurrentEnrollment - 1 
                                 ELSE 0 END
    WHERE SectionId = @SectionId;
END";

            try
            {
                DbHelper.ExecuteNonQuery(
                    sql,
                    new SqlParameter("@StudentId", studentId.Value),
                    new SqlParameter("@SectionId", _selectedSectionId.Value));

                MessageBox.Show("Đã hủy đăng ký học phần.");
                LoadSectionsForStudent();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi khi hủy đăng ký: " + ex.Message);
            }
        }

        // ================== NÚT LÀM MỚI ==================

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_currentRole == "SV")
                LoadSectionsForStudent();
            else if (_currentRole == "GV")
                LoadSectionsForTeacher();
            else if (_currentRole == "Admin")
                LoadSectionsForAdmin();
            else
                LoadAllOpenSections();
        }
    }
}
