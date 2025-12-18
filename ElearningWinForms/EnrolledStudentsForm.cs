using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    /// <summary>
    /// Danh sách sinh viên đã đăng ký 1 học phần (Section).
    /// Quyền:
    /// - GV: chỉ xem được section do mình phụ trách
    /// - Admin: xem được mọi section
    /// </summary>
    public class EnrolledStudentsForm : Form
    {
        private readonly int _currentUserId;
        private readonly string _currentRole;
        private readonly int _sectionId;

        private Label lblHeader;
        private DataGridView dgv;
        private Button btnClose;
        private Button btnRefresh;

        public EnrolledStudentsForm(int currentUserId, string currentRole, int sectionId)
        {
            _currentUserId = currentUserId;
            _currentRole = currentRole ?? "";
            _sectionId = sectionId;

            BuildUi();
            this.Load += EnrolledStudentsForm_Load;
        }

        private void BuildUi()
        {
            this.Text = "Danh sách SV đăng ký";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(920, 520);

            lblHeader = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0)
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 48 };

            btnRefresh = new Button
            {
                Text = "Làm mới",
                Width = 120,
                Height = 30,
                Left = 12,
                Top = 9
            };
            btnRefresh.Click += (s, e) => LoadStudents();

            btnClose = new Button
            {
                Text = "Đóng",
                Width = 120,
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Left = this.ClientSize.Width - 132,
                Top = 9
            };
            btnClose.Click += (s, e) => this.Close();
            bottomPanel.Resize += (s, e) => btnClose.Left = bottomPanel.ClientSize.Width - btnClose.Width - 12;

            bottomPanel.Controls.Add(btnRefresh);
            bottomPanel.Controls.Add(btnClose);

            this.Controls.Add(dgv);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(lblHeader);
        }

        private void EnrolledStudentsForm_Load(object sender, EventArgs e)
        {
            if (!CanViewThisSection())
            {
                MessageBox.Show("Bạn không có quyền xem danh sách SV của học phần này.",
                    "Không có quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            lblHeader.Text = "Học phần: " + GetSectionInfoLine(_sectionId);
            LoadStudents();
        }

        private bool CanViewThisSection()
        {
            if (string.Equals(_currentRole, "Admin", StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.Equals(_currentRole, "GV", StringComparison.OrdinalIgnoreCase))
                return false;

            object teacherIdObj = DbHelper.ExecuteScalar(
                "SELECT TeacherId FROM Teachers WHERE UserId = @uid",
                new SqlParameter("@uid", _currentUserId));
            if (teacherIdObj == null || teacherIdObj == DBNull.Value)
                return false;

            int teacherId = Convert.ToInt32(teacherIdObj);

            object ok = DbHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM CourseSections WHERE SectionId = @sid AND TeacherId = @tid",
                new SqlParameter("@sid", _sectionId),
                new SqlParameter("@tid", teacherId));

            return Convert.ToInt32(ok) > 0;
        }

        private void LoadStudents()
        {
            string sql = @"
SELECT 
    s.StudentId,
    u.FullName,
    u.Email,
    s.ClassName,
    s.Faculty,
    e.Status,
    e.EnrollmentDate
FROM Enrollments e
JOIN Students s ON e.StudentId = s.StudentId
JOIN Users u ON s.UserId = u.UserId
WHERE e.SectionId = @SectionId
ORDER BY s.StudentId, u.FullName;";

            DataTable dt = DbHelper.GetDataTable(sql, new SqlParameter("@SectionId", _sectionId));
            dgv.DataSource = dt;
            if (dgv.Columns["StudentId"] != null)
                dgv.Columns["StudentId"].HeaderText = "Mã SV";
            if (dgv.Columns["FullName"] != null)
                dgv.Columns["FullName"].HeaderText = "Họ tên";
            if (dgv.Columns["Email"] != null)
                dgv.Columns["Email"].HeaderText = "Email";
            if (dgv.Columns["ClassName"] != null)
                dgv.Columns["ClassName"].HeaderText = "Lớp";
            if (dgv.Columns["Faculty"] != null)
                dgv.Columns["Faculty"].HeaderText = "Khoa";
            if (dgv.Columns["Status"] != null)
                dgv.Columns["Status"].HeaderText = "Trạng thái";
            if (dgv.Columns["EnrolledAt"] != null)
                dgv.Columns["EnrolledAt"].HeaderText = "Thời gian đăng ký";
        }

        private string GetSectionInfoLine(int sectionId)
        {
            string sql = @"
SELECT TOP 1
    c.CourseCode + N' - ' + c.CourseName
    + N' (' + s.Semester + N'/' + CAST(s.[Year] AS NVARCHAR(4)) + N')'
FROM CourseSections s
JOIN Courses c ON s.CourseId = c.CourseId
WHERE s.SectionId = @SectionId;";

            object r = DbHelper.ExecuteScalar(sql, new SqlParameter("@SectionId", sectionId));
            return (r == null || r == DBNull.Value) ? ("Section " + sectionId) : r.ToString();
        }
    }
}
