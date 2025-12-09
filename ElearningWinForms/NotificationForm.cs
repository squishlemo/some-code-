using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class NotificationForm : Form
    {
        public int CurrentUserId { get; set; }
        public string CurrentRole { get; set; }   // "SV", "GV", "Admin"

        public NotificationForm(int userId, string role)
        {
            InitializeComponent();
            CurrentUserId = userId;
            CurrentRole = role;
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetupUiByRole();
                LoadFilterData();
                LoadNotificationsForCurrentUser();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thông báo: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region UI setup

        private void SetupUiByRole()
        {
            txtContent.Clear();

            if (string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                grpCompose.Text = "Gửi thông báo (Admin)";
                grpCompose.Enabled = true;

                cboScope.Items.Clear();
                cboScope.Items.Add("Toàn hệ thống");
                cboScope.Items.Add("Theo khoa");
                cboScope.SelectedIndex = 0;

                lblFaculty.Visible = true;
                cboFaculty.Visible = true;

                lblSection.Visible = false;
                cboSection.Visible = false;

                btnSendNotification.Enabled = true;
            }
            else if (string.Equals(CurrentRole, "GV", StringComparison.OrdinalIgnoreCase))
            {
                grpCompose.Text = "Gửi thông báo cho lớp đang dạy";
                grpCompose.Enabled = true;

                cboScope.Items.Clear();
                cboScope.Items.Add("Lớp học phần đang dạy");
                cboScope.SelectedIndex = 0;

                lblFaculty.Visible = false;
                cboFaculty.Visible = false;

                lblSection.Visible = true;
                cboSection.Visible = true;

                btnSendNotification.Enabled = true;
            }
            else
            {
                // SV: chỉ đọc, không gửi
                grpCompose.Text = "Thông báo (SV không gửi được)";
                btnSendNotification.Enabled = false;

                cboScope.Items.Clear();
                lblFaculty.Visible = false;
                cboFaculty.Visible = false;
                lblSection.Visible = false;
                cboSection.Visible = false;
            }
        }

        private void LoadFilterData()
        {
            if (string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                // Admin chọn khoa: lấy danh sách khoa từ Students/Teachers (đảm bảo trùng với dữ liệu thực tế)
                string sql = @"
SELECT DISTINCT Faculty AS Name
FROM Students
WHERE Faculty IS NOT NULL AND LTRIM(RTRIM(Faculty)) <> ''
UNION
SELECT DISTINCT Department AS Name
FROM Teachers
WHERE Department IS NOT NULL AND LTRIM(RTRIM(Department)) <> ''
ORDER BY Name";

                DataTable dt = DbHelper.GetDataTable(sql);
                cboFaculty.DataSource = dt;
                cboFaculty.DisplayMember = "Name";
                cboFaculty.ValueMember = "Name";
                cboFaculty.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
            }
            else if (string.Equals(CurrentRole, "GV", StringComparison.OrdinalIgnoreCase))
            {
                // GV: lấy TeacherId
                string sqlTeacher = "SELECT TeacherId FROM Teachers WHERE UserId = @UserId";
                DataTable dtTeacher = DbHelper.GetDataTable(sqlTeacher,
                    new SqlParameter("@UserId", CurrentUserId));

                if (dtTeacher.Rows.Count == 0)
                {
                    cboSection.DataSource = null;
                    cboSection.Enabled = false;
                    return;
                }

                int teacherId = Convert.ToInt32(dtTeacher.Rows[0]["TeacherId"]);

                string sqlSection = @"
SELECT s.SectionId,
       c.CourseCode + N' - ' + c.CourseName + N' (' +
       s.Semester + N'/' + CAST(s.[Year] AS NVARCHAR(4)) + N')' AS SectionName
FROM CourseSections s
JOIN Courses c ON s.CourseId = c.CourseId
WHERE s.TeacherId = @TeacherId
ORDER BY c.CourseCode, s.Semester, s.[Year];";

                DataTable dt = DbHelper.GetDataTable(sqlSection,
                    new SqlParameter("@TeacherId", teacherId));

                cboSection.DataSource = dt;
                cboSection.DisplayMember = "SectionName";
                cboSection.ValueMember = "SectionId";
                cboSection.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
            }
        }

        #endregion

        #region Load & hiển thị thông báo

        private void LoadNotificationsForCurrentUser()
        {
            string sql = @"
SELECT 
    n.NotificationId,
    u.FullName      AS SenderName,
    n.TargetType,
    n.CreatedTime,
    n.Status,
    nr.IsRead,
    nr.ReadTime,
    n.Content
FROM NotificationRecipients nr
JOIN Notifications n ON nr.NotificationId = n.NotificationId
JOIN Users u ON n.SenderId = u.UserId
WHERE nr.UserId = @UserId
ORDER BY n.CreatedTime DESC";

            DataTable dt = DbHelper.GetDataTable(sql,
                new SqlParameter("@UserId", CurrentUserId));

            dgvNotifications.DataSource = dt;

            if (dgvNotifications.Columns["NotificationId"] != null)
                dgvNotifications.Columns["NotificationId"].Visible = false;

            if (dgvNotifications.Columns["Content"] != null)
                dgvNotifications.Columns["Content"].Visible = false;
        }

        private void dgvNotifications_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvNotifications.CurrentRow == null)
                return;

            DataGridViewRow row = dgvNotifications.CurrentRow;

            string content = row.Cells["Content"].Value?.ToString();
            txtContent.Text = content ?? string.Empty;

            int notificationId = Convert.ToInt32(row.Cells["NotificationId"].Value);
            MarkNotificationAsRead(notificationId);
        }

        private void MarkNotificationAsRead(int notificationId)
        {
            string sql = @"
UPDATE NotificationRecipients
SET IsRead = 1,
    ReadTime = ISNULL(ReadTime, GETDATE())
WHERE NotificationId = @NotificationId
  AND UserId = @UserId
  AND IsRead = 0;";

            DbHelper.ExecuteNonQuery(sql,
                new SqlParameter("@NotificationId", notificationId),
                new SqlParameter("@UserId", CurrentUserId));

            LoadNotificationsForCurrentUser();
        }

        #endregion

        #region Gửi thông báo

        private void cboScope_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                bool byFaculty = (cboScope.SelectedIndex == 1); // 0: Global, 1: Theo khoa
                lblFaculty.Enabled = byFaculty;
                cboFaculty.Enabled = byFaculty;
            }
        }

        private void btnSendNotification_Click(object sender, EventArgs e)
        {
            if (string.Equals(CurrentRole, "SV", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Sinh viên không có quyền gửi thông báo.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string content = txtContent.Text.Trim();
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Vui lòng nhập nội dung thông báo.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContent.Focus();
                return;
            }

            try
            {
                if (string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    SendNotificationAsAdmin(content);
                }
                else if (string.Equals(CurrentRole, "GV", StringComparison.OrdinalIgnoreCase))
                {
                    SendNotificationAsTeacher(content);
                }

                txtContent.Clear();
                LoadNotificationsForCurrentUser();

                MessageBox.Show("Đã gửi thông báo.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi thông báo: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendNotificationAsAdmin(string content)
        {
            if (cboScope.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn phạm vi gửi.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string scope = cboScope.SelectedItem.ToString();

            if (scope.StartsWith("Toàn hệ thống"))
            {
                int notificationId = InsertNotification(content, "GLOBAL", null);
                InsertRecipientsGlobal(notificationId);
            }
            else // Theo khoa
            {
                if (cboFaculty.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn khoa.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string facultyName = cboFaculty.SelectedValue.ToString();
                int notificationId = InsertNotification(content, "FACULTY", null);
                InsertRecipientsForFaculty(notificationId, facultyName);
            }
        }

        private void SendNotificationAsTeacher(string content)
        {
            if (cboSection.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int sectionId = Convert.ToInt32(cboSection.SelectedValue);

            int notificationId = InsertNotification(content, "SECTION", sectionId);
            InsertRecipientsForSection(notificationId, sectionId);
        }

        private int InsertNotification(string content, string targetType, int? targetRefId)
        {
            string sql = @"
INSERT INTO Notifications (SenderId, Content, TargetType, TargetRefId)
VALUES (@SenderId, @Content, @TargetType, @TargetRefId);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            object result = DbHelper.ExecuteScalar(sql,
                new SqlParameter("@SenderId", CurrentUserId),
                new SqlParameter("@Content", content),
                new SqlParameter("@TargetType", targetType),
                new SqlParameter("@TargetRefId",
                    targetRefId.HasValue ? (object)targetRefId.Value : DBNull.Value));

            return Convert.ToInt32(result);
        }

        private void InsertRecipientsGlobal(int notificationId)
        {
            string sql = @"
INSERT INTO NotificationRecipients (NotificationId, UserId)
SELECT @NotificationId, UserId
FROM Users
WHERE IsActive = 1;";

            DbHelper.ExecuteNonQuery(sql,
                new SqlParameter("@NotificationId", notificationId));
        }

        private void InsertRecipientsForFaculty(int notificationId, string facultyName)
        {
            string sql = @"
INSERT INTO NotificationRecipients (NotificationId, UserId)
SELECT DISTINCT @NotificationId, u.UserId
FROM Users u
LEFT JOIN Students s ON u.UserId = s.UserId
LEFT JOIN Teachers t ON u.UserId = t.UserId
WHERE u.IsActive = 1
  AND (
       s.Faculty = @FacultyName
       OR t.Department = @FacultyName
  );";

            DbHelper.ExecuteNonQuery(sql,
                new SqlParameter("@NotificationId", notificationId),
                new SqlParameter("@FacultyName", facultyName));
        }

        private void InsertRecipientsForSection(int notificationId, int sectionId)
        {
            string sql = @"
INSERT INTO NotificationRecipients (NotificationId, UserId)
SELECT DISTINCT @NotificationId, u.UserId
FROM Enrollments e
JOIN Students s ON e.StudentId = s.StudentId
JOIN Users u ON s.UserId = u.UserId
WHERE e.SectionId = @SectionId
  AND e.Status = N'Active'
  AND u.IsActive = 1;";

            DbHelper.ExecuteNonQuery(sql,
                new SqlParameter("@NotificationId", notificationId),
                new SqlParameter("@SectionId", sectionId));
        }

        #endregion

        #region Xóa & refresh

        private void btnDeleteNotification_Click(object sender, EventArgs e)
        {
            if (dgvNotifications.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn thông báo cần xóa.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int notificationId = Convert.ToInt32(
                dgvNotifications.CurrentRow.Cells["NotificationId"].Value);

            DialogResult dr = MessageBox.Show(
                "Xóa thông báo này khỏi hộp thư của bạn? (Thông báo sẽ bị xóa hẳn nếu không còn người nhận)",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr != DialogResult.Yes)
                return;

            string sql = @"
DELETE FROM NotificationRecipients
WHERE NotificationId = @NotificationId
  AND UserId = @UserId;

IF NOT EXISTS (SELECT 1 FROM NotificationRecipients WHERE NotificationId = @NotificationId)
BEGIN
    DELETE FROM Notifications WHERE NotificationId = @NotificationId;
END;";

            DbHelper.ExecuteNonQuery(sql,
                new SqlParameter("@NotificationId", notificationId),
                new SqlParameter("@UserId", CurrentUserId));

            LoadNotificationsForCurrentUser();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadNotificationsForCurrentUser();
        }

        #endregion
    }
}
