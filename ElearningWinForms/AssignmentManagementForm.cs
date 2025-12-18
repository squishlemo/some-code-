using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    /// <summary>
    /// UC-03: Quản lý bài tập (dành cho Giảng viên).
    /// - Tạo bài tập mới
    /// - Chỉnh sửa / xóa bài tập
    /// - Xem danh sách sinh viên nộp bài
    /// - Kiểm tra deadline hợp lệ, kiểm tra file đính kèm
    /// </summary>
    public partial class AssignmentForm : Form
    {
        private readonly int _currentUserId;
        private readonly string _currentRole; // "GV" / "Admin"
        private int? _teacherId;
        private int? _selectedAssignmentId;

        public AssignmentForm(int userId, string role)
        {
            InitializeComponent();

            _currentUserId = userId;
            _currentRole = role;
        }

        private void AssignmentForm_Load(object sender, EventArgs e)
        {
            if (_currentRole != "GV" && _currentRole != "Admin")
            {
                MessageBox.Show("Chức năng quản lý bài tập chỉ dành cho Giảng viên (hoặc Admin).",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            _teacherId = GetTeacherIdByUserId(_currentUserId);
            if (_teacherId == null && _currentRole == "GV")
            {
                MessageBox.Show("Không tìm thấy thông tin giảng viên cho tài khoản hiện tại.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            LoadStatusCombo();
            LoadTeacherSections();
        }

        // ================== TRA CỨU GV / LỚP HỌC PHẦN ==================

        private int? GetTeacherIdByUserId(int userId)
        {
            const string sql = "SELECT TOP 1 TeacherId FROM Teachers WHERE UserId = @uid";

            object result = DbHelper.ExecuteScalar(
                sql,
                new SqlParameter("@uid", SqlDbType.Int) { Value = userId });

            if (result == null || result == DBNull.Value)
                return null;

            return Convert.ToInt32(result);
        }

        private void LoadStatusCombo()
        {
            cboStatus.Items.Clear();
            cboStatus.Items.Add("Open");
            cboStatus.Items.Add("Closed");
            if (cboStatus.Items.Count > 0)
                cboStatus.SelectedIndex = 0;
        }

        private void LoadTeacherSections()
        {
            string sql;
            SqlParameter param;

            if (_currentRole == "GV" && _teacherId != null)
            {
                sql = @"
SELECT 
    cs.SectionId,
    c.CourseCode + N' - ' + c.CourseName + N' (HK ' + cs.Semester + N'/' + CONVERT(NVARCHAR(10), cs.[Year]) + N')' 
        AS SectionName
FROM CourseSections cs
JOIN Courses c ON cs.CourseId = c.CourseId
WHERE cs.TeacherId = @tid
ORDER BY c.CourseCode, cs.Semester, cs.[Year];";

                param = new SqlParameter("@tid", SqlDbType.Int) { Value = _teacherId.Value };
            }
            else
            {
                // Admin: xem được tất cả
                sql = @"
SELECT 
    cs.SectionId,
    c.CourseCode + N' - ' + c.CourseName + N' (HK ' + cs.Semester + N'/' + CONVERT(NVARCHAR(10), cs.[Year]) + N')' 
        AS SectionName
FROM CourseSections cs
JOIN Courses c ON cs.CourseId = c.CourseId
ORDER BY c.CourseCode, cs.Semester, cs.[Year];";

                param = null;
            }

            DataTable table = param == null
                ? DbHelper.GetDataTable(sql)
                : DbHelper.GetDataTable(sql, param);

            cboSection.DataSource = table;
            cboSection.DisplayMember = "SectionName";
            cboSection.ValueMember = "SectionId";

            if (table.Rows.Count > 0)
                cboSection.SelectedIndex = 0;

            LoadAssignmentsForCurrentSection();
        }

        // ================== LOAD & BIND ASSIGNMENTS ==================

        private void LoadAssignmentsForCurrentSection()
        {
            if (cboSection.SelectedValue == null || cboSection.SelectedValue == DBNull.Value)
            {
                dgvAssignments.DataSource = null;
                _selectedAssignmentId = null;
                return;
            }

            int sectionId;
            // Khi binding DataTable, đôi lúc SelectedValue trả về DataRowView.
            // Xử lý trường hợp đó cho chắc chắn.
            if (cboSection.SelectedValue is System.Data.DataRowView drv &&
                drv.Row.Table.Columns.Contains("SectionId"))
            {
                sectionId = Convert.ToInt32(drv["SectionId"]);
            }
            else
            {
                sectionId = Convert.ToInt32(cboSection.SelectedValue);
            }


            string sql = @"
SELECT 
    AssignmentId,
    Title          AS [Tiêu đề],
    Description    AS [Mô tả],
    AttachmentPath AS [File đính kèm],
    CreatedTime    AS [Ngày tạo],
    Deadline       AS [Hạn nộp],
    Status         AS [Trạng thái]
FROM Assignments
WHERE SectionId = @sec
ORDER BY CreatedTime DESC;";

            DataTable table = DbHelper.GetDataTable(
                sql,
                new SqlParameter("@sec", SqlDbType.Int) { Value = sectionId });

            dgvAssignments.DataSource = table;
            dgvAssignments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAssignments.ReadOnly = true;
            dgvAssignments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAssignments.MultiSelect = false;

            _selectedAssignmentId = null;
            ClearAssignmentInputs();
            dgvSubmissions.DataSource = null;
        }

        private void ClearAssignmentInputs()
        {
            txtTitle.Clear();
            txtDescription.Clear();
            txtAttachmentPath.Clear();
            dtpDeadline.Value = DateTime.Now.AddDays(7);
            if (cboStatus.Items.Count > 0)
                cboStatus.SelectedIndex = 0;
        }

        private void cboSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAssignmentsForCurrentSection();
        }

        private void dgvAssignments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAssignments.CurrentRow == null ||
                dgvAssignments.CurrentRow.DataBoundItem == null)
            {
                _selectedAssignmentId = null;
                return;
            }

            var rowView = dgvAssignments.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                _selectedAssignmentId = null;
                return;
            }

            DataRow row = rowView.Row;

            if (!row.Table.Columns.Contains("AssignmentId"))
            {
                _selectedAssignmentId = null;
                return;
            }

            _selectedAssignmentId = Convert.ToInt32(row["AssignmentId"]);

            txtTitle.Text = row["Tiêu đề"]?.ToString() ?? string.Empty;
            txtDescription.Text = row["Mô tả"]?.ToString() ?? string.Empty;
            txtAttachmentPath.Text = row["File đính kèm"]?.ToString() ?? string.Empty;

            if (row["Hạn nộp"] != DBNull.Value)
            {
                dtpDeadline.Value = Convert.ToDateTime(row["Hạn nộp"]);
            }

            string status = row["Trạng thái"]?.ToString() ?? "Open";
            int idx = cboStatus.Items.IndexOf(status);
            if (idx >= 0)
                cboStatus.SelectedIndex = idx;
        }

        // ================== VALIDATE INPUT (INCLUDE: KIỂM TRA DEADLINE + FILE) ==================

        private bool ValidateInput(out int sectionId, out DateTime deadline, out string attachmentPath)
        {
            sectionId = 0;
            deadline = dtpDeadline.Value;
            attachmentPath = txtAttachmentPath.Text.Trim();

            if (cboSection.SelectedValue == null || cboSection.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            sectionId = Convert.ToInt32(cboSection.SelectedValue);

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề bài tập.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return false;
            }

            // include: kiểm tra deadline hợp lệ
            if (deadline <= DateTime.Now)
            {
                MessageBox.Show("Deadline phải lớn hơn thời điểm hiện tại.", "Deadline không hợp lệ",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDeadline.Focus();
                return false;
            }

            // include: kiểm tra file đính kèm (nếu có)
            if (!string.IsNullOrEmpty(attachmentPath))
            {
                try
                {
                    if (!File.Exists(attachmentPath))
                    {
                        var result = MessageBox.Show(
                            "Đường dẫn file đính kèm không tồn tại. Bạn vẫn muốn lưu bài tập không?",
                            "File đính kèm không tồn tại",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                            return false;
                    }
                }
                catch
                {
                    MessageBox.Show("Đường dẫn file đính kèm không hợp lệ.",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        // ================== TẠO BÀI TẬP MỚI ==================

        private void btnCreateAssignment_Click(object sender, EventArgs e)
        {
            if (_currentRole != "GV" && _currentRole != "Admin")
            {
                MessageBox.Show("Chỉ giảng viên (hoặc Admin) mới được tạo bài tập.");
                return;
            }

            if (!ValidateInput(out int sectionId, out DateTime deadline, out string attachmentPath))
                return;

            string sql = @"
INSERT INTO Assignments (SectionId, Title, Description, AttachmentPath, Deadline, Status)
VALUES (@sec, @title, @desc, @attach, @deadline, @status);";

            try
            {
                DbHelper.ExecuteNonQuery(
                    sql,
                    new SqlParameter("@sec", SqlDbType.Int) { Value = sectionId },
                    new SqlParameter("@title", SqlDbType.NVarChar, 200) { Value = txtTitle.Text.Trim() },
                    new SqlParameter("@desc", SqlDbType.NVarChar) { Value = (object)txtDescription.Text.Trim() ?? DBNull.Value },
                    new SqlParameter("@attach", SqlDbType.NVarChar, 255) { Value = (object)attachmentPath ?? DBNull.Value },
                    new SqlParameter("@deadline", SqlDbType.DateTime) { Value = deadline },
                    new SqlParameter("@status", SqlDbType.NVarChar, 20) { Value = cboStatus.SelectedItem?.ToString() ?? "Open" });

                MessageBox.Show("Đã tạo bài tập mới.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAssignmentsForCurrentSection();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL khi tạo bài tập:\n" + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo bài tập:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== CẬP NHẬT BÀI TẬP ==================

        private void btnUpdateAssignment_Click(object sender, EventArgs e)
        {
            if (_selectedAssignmentId == null)
            {
                MessageBox.Show("Vui lòng chọn bài tập cần chỉnh sửa.");
                return;
            }

            if (!ValidateInput(out int sectionId, out DateTime deadline, out string attachmentPath))
                return;

            string sql = @"
UPDATE Assignments
SET SectionId      = @sec,
    Title          = @title,
    Description    = @desc,
    AttachmentPath = @attach,
    Deadline       = @deadline,
    Status         = @status
WHERE AssignmentId = @id;";

            try
            {
                DbHelper.ExecuteNonQuery(
                    sql,
                    new SqlParameter("@sec", SqlDbType.Int) { Value = sectionId },
                    new SqlParameter("@title", SqlDbType.NVarChar, 200) { Value = txtTitle.Text.Trim() },
                    new SqlParameter("@desc", SqlDbType.NVarChar) { Value = (object)txtDescription.Text.Trim() ?? DBNull.Value },
                    new SqlParameter("@attach", SqlDbType.NVarChar, 255) { Value = (object)attachmentPath ?? DBNull.Value },
                    new SqlParameter("@deadline", SqlDbType.DateTime) { Value = deadline },
                    new SqlParameter("@status", SqlDbType.NVarChar, 20) { Value = cboStatus.SelectedItem?.ToString() ?? "Open" },
                    new SqlParameter("@id", SqlDbType.Int) { Value = _selectedAssignmentId.Value });

                MessageBox.Show("Đã cập nhật bài tập.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAssignmentsForCurrentSection();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL khi cập nhật bài tập:\n" + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật bài tập:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== XÓA BÀI TẬP ==================

        private void btnDeleteAssignment_Click(object sender, EventArgs e)
        {
            if (_selectedAssignmentId == null)
            {
                MessageBox.Show("Vui lòng chọn bài tập cần xóa.");
                return;
            }

            if (MessageBox.Show(
                    "Bạn chắc chắn muốn xóa bài tập này?\n(Các bài nộp liên quan có thể bị ảnh hưởng.)",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            string sql = "DELETE FROM Assignments WHERE AssignmentId = @id;";

            try
            {
                DbHelper.ExecuteNonQuery(
                    sql,
                    new SqlParameter("@id", SqlDbType.Int) { Value = _selectedAssignmentId.Value });

                MessageBox.Show("Đã xóa bài tập.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAssignmentsForCurrentSection();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) // FK constraint
                {
                    MessageBox.Show("Không thể xóa vì đã có sinh viên nộp bài cho bài tập này.",
                        "Ràng buộc dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi SQL khi xóa bài tập:\n" + ex.Message,
                        "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa bài tập:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== BUTTON PHỤ ==================

        private void btnBrowseAttachment_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Chọn file đính kèm";
                dlg.Filter = "Tất cả file (*.*)|*.*";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    txtAttachmentPath.Text = dlg.FileName;
                }
            }
        }

        private void btnRefreshAssignments_Click(object sender, EventArgs e)
        {
            LoadAssignmentsForCurrentSection();
        }

        // ================== XEM DANH SÁCH SINH VIÊN NỘP BÀI ==================

        private void btnLoadSubmissions_Click(object sender, EventArgs e)
        {
            if (_selectedAssignmentId == null)
            {
                MessageBox.Show("Vui lòng chọn một bài tập để xem danh sách sinh viên nộp bài.");
                return;
            }

            string sql = @"
SELECT 
    sub.SubmissionId,
    stu.StudentId      AS [Mã SV],
    u.FullName         AS [Họ tên],
    sub.FilePath       AS [File nộp],
    sub.SubmittedTime  AS [Thời gian nộp],
    sub.IsLate         AS [Nộp muộn?]
FROM Submissions sub
JOIN Students stu ON sub.StudentId = stu.StudentId
JOIN Users u     ON stu.UserId    = u.UserId
WHERE sub.AssignmentId = @aid
ORDER BY sub.SubmittedTime;";

            try
            {
                DataTable table = DbHelper.GetDataTable(
                    sql,
                    new SqlParameter("@aid", SqlDbType.Int) { Value = _selectedAssignmentId.Value });

                dgvSubmissions.DataSource = table;
                dgvSubmissions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvSubmissions.ReadOnly = true;
                dgvSubmissions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvSubmissions.MultiSelect = false;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL khi tải danh sách bài nộp:\n" + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách bài nộp:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
