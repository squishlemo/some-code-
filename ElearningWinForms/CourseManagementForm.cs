using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class CourseManagementForm : Form
    {
        private int? selectedCourseId = null;

        public CourseManagementForm()
        {
            InitializeComponent();
        }

        private void CourseManagementForm_Load(object sender, EventArgs e)
        {
            LoadStatusCombo();
            LoadMajors();
            LoadCourses();
        }

        private void LoadStatusCombo()
        {
            // Nếu bạn đã set Items trong Designer thì hàm này chỉ để chọn mặc định
            if (cboStatus.Items.Count == 0)
            {
                cboStatus.Items.Add("Active");
                cboStatus.Items.Add("Inactive");
            }
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cboStatus.Items.Count > 0)
                cboStatus.SelectedIndex = 0;
        }

        private void LoadMajors()
        {
            try
            {
                string sql = "SELECT MajorId, MajorName FROM Majors ORDER BY MajorName";
                DataTable dt = DbHelper.GetDataTable(sql);
                cboMajor.DataSource = dt;
                cboMajor.DisplayMember = "MajorName";
                cboMajor.ValueMember = "MajorId";
                cboMajor.SelectedIndex = -1; // cho phép bỏ trống nếu muốn
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách ngành: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCourses()
        {
            try
            {
                string sql = @"
SELECT c.CourseId,
       c.CourseCode,
       c.CourseName,
       c.Description,
       c.Credit,
       c.Status,
       c.MajorId,
       m.MajorName
FROM   Courses c
LEFT JOIN Majors m ON c.MajorId = m.MajorId
ORDER BY c.CourseCode";

                DataTable dt = DbHelper.GetDataTable(sql);
                dgvCourses.DataSource = dt;

                if (dgvCourses.Columns["CourseId"] != null)
                    dgvCourses.Columns["CourseId"].Visible = false;
                if (dgvCourses.Columns["MajorId"] != null)
                    dgvCourses.Columns["MajorId"].Visible = false;

                dgvCourses.ClearSelection();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách khóa học: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCourses.CurrentRow == null || dgvCourses.CurrentRow.Index < 0)
                return;

            var row = dgvCourses.CurrentRow;

            if (row.Cells["CourseId"].Value == null ||
                row.Cells["CourseId"].Value == DBNull.Value)
            {
                selectedCourseId = null;
                return;
            }

            selectedCourseId = Convert.ToInt32(row.Cells["CourseId"].Value);

            txtCourseCode.Text = row.Cells["CourseCode"].Value?.ToString();
            txtCourseName.Text = row.Cells["CourseName"].Value?.ToString();
            txtDescription.Text = row.Cells["Description"].Value?.ToString();
            txtCredit.Text = row.Cells["Credit"].Value?.ToString();
            string status = row.Cells["Status"].Value?.ToString();

            if (!string.IsNullOrEmpty(status))
                cboStatus.SelectedItem = status;

            // Major
            if (row.Cells["MajorId"].Value == null ||
                row.Cells["MajorId"].Value == DBNull.Value)
            {
                cboMajor.SelectedIndex = -1;
            }
            else
            {
                int majorId = Convert.ToInt32(row.Cells["MajorId"].Value);
                cboMajor.SelectedValue = majorId;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCourseCode.Text))
            {
                MessageBox.Show("Mã môn học không được để trống.",
                    "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCourseCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Tên môn học không được để trống.",
                    "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCourseName.Focus();
                return false;
            }

            if (!int.TryParse(txtCredit.Text, out int credit) || credit <= 0)
            {
                MessageBox.Show("Số tín chỉ phải là số nguyên dương.",
                    "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCredit.Focus();
                return false;
            }

            if (cboStatus.SelectedItem == null)
            {
                MessageBox.Show("Hãy chọn trạng thái khóa học.",
                    "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboStatus.DroppedDown = true;
                return false;
            }

            if (cboMajor.SelectedValue == null || cboMajor.SelectedIndex < 0)
            {
                MessageBox.Show("Hãy chọn ngành cho khóa học.",
                    "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMajor.DroppedDown = true;
                return false;
            }

            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            string code = txtCourseCode.Text.Trim();
            string name = txtCourseName.Text.Trim();
            string desc = string.IsNullOrWhiteSpace(txtDescription.Text)
                ? null
                : txtDescription.Text.Trim();
            int credit = int.Parse(txtCredit.Text.Trim());
            string status = cboStatus.SelectedItem.ToString();
            int majorId = Convert.ToInt32(cboMajor.SelectedValue);

            try
            {
                string sql = @"
INSERT INTO Courses (CourseCode, CourseName, Description, Credit, Status, MajorId)
VALUES (@Code, @Name, @Desc, @Credit, @Status, @MajorId);";

                int rows = DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@Code", code),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Desc", (object)desc ?? DBNull.Value),
                    new SqlParameter("@Credit", credit),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@MajorId", majorId)
                );

                if (rows > 0)
                {
                    MessageBox.Show("Thêm khóa học thành công.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCourses();
                }
            }
            catch (SqlException ex)
            {
                // Thường gặp: trùng CourseCode (UNIQUE)
                MessageBox.Show("Không thể thêm khóa học.\nChi tiết: " + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khóa học: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCourseId == null)
            {
                MessageBox.Show("Hãy chọn một khóa học trong bảng để sửa.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInput())
                return;

            string code = txtCourseCode.Text.Trim();
            string name = txtCourseName.Text.Trim();
            string desc = string.IsNullOrWhiteSpace(txtDescription.Text)
                ? null
                : txtDescription.Text.Trim();
            int credit = int.Parse(txtCredit.Text.Trim());
            string status = cboStatus.SelectedItem.ToString();
            int majorId = Convert.ToInt32(cboMajor.SelectedValue);

            try
            {
                string sql = @"
UPDATE Courses
SET CourseCode  = @Code,
    CourseName  = @Name,
    Description = @Desc,
    Credit      = @Credit,
    Status      = @Status,
    MajorId     = @MajorId
WHERE CourseId  = @Id;";

                int rows = DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@Code", code),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Desc", (object)desc ?? DBNull.Value),
                    new SqlParameter("@Credit", credit),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@MajorId", majorId),
                    new SqlParameter("@Id", selectedCourseId.Value)
                );

                if (rows > 0)
                {
                    MessageBox.Show("Cập nhật khóa học thành công.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCourses();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể cập nhật khóa học.\nChi tiết: " + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật khóa học: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCourseId == null)
            {
                MessageBox.Show("Hãy chọn một khóa học trong bảng để xóa.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Bạn chắc chắn muốn xóa khóa học này?\n" +
                                "Nếu đã có lớp học phần hoặc sinh viên đăng ký thì có thể không xóa được.",
                                "Xác nhận xóa",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string sql = "DELETE FROM Courses WHERE CourseId = @Id;";
                int rows = DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@Id", selectedCourseId.Value));

                if (rows > 0)
                {
                    MessageBox.Show("Xóa khóa học thành công.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCourses();
                }
                else
                {
                    MessageBox.Show("Không có bản ghi nào bị xóa.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể xóa khóa học (có thể đang được dùng ở lớp học phần hoặc bảng khác).\nChi tiết: " + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khóa học: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            dgvCourses.ClearSelection();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCourses();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearForm()
        {
            selectedCourseId = null;
            txtCourseCode.Clear();
            txtCourseName.Clear();
            txtDescription.Clear();
            txtCredit.Clear();

            if (cboStatus.Items.Count > 0)
                cboStatus.SelectedIndex = 0;

            if (cboMajor.DataSource != null)
                cboMajor.SelectedIndex = -1;
        }
    }
}
