using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class AdminForm : Form
    {
        private int CurrentUserId;

        // ==== CONTROL FIELD ====
        private DataGridView dgvRequests;
        private Button btnRefreshRequests;
        private Button btnApprove;
        private Button btnReject;
        private Button btnLogout;
        // =======================

        public AdminForm(int currentUserId)
        {
            InitializeComponent();
            CurrentUserId = currentUserId;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            LoadPendingRequests();
        }

        // =============== LOAD DANH SÁCH YÊU CẦU ĐĂNG KÝ ===============

        private void LoadPendingRequests()
        {
            try
            {
                string sql = @"
                    SELECT RequestId, Username, FullName, Email, Role, Status, CreatedAt
                    FROM RegistrationRequests
                    WHERE Status = N'PendingAdmin'
                    ORDER BY CreatedAt DESC";

                DataTable table = DbHelper.GetDataTable(sql);
                dgvRequests.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load yêu cầu đăng ký: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnRefreshRequests_Click(object sender, EventArgs e)
        {
            LoadPendingRequests();
        }

        // =============== LẤY REQUEST ĐANG CHỌN ===============

        private DataRow GetSelectedRequest()
        {
            if (dgvRequests.CurrentRow == null || dgvRequests.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn một yêu cầu trong danh sách.",
                                "Chưa chọn dòng",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return null;
            }

            DataRowView view = dgvRequests.CurrentRow.DataBoundItem as DataRowView;
            if (view == null) return null;

            return view.Row;
        }

        // =============== DUYỆT YÊU CẦU (APPROVE) ===============

        private void btnApprove_Click(object sender, EventArgs e)
        {
            DataRow row = GetSelectedRequest();
            if (row == null) return;

            int requestId = Convert.ToInt32(row["RequestId"]);
            string username = row["Username"].ToString();
            string fullName = row["FullName"].ToString();
            string email = row["Email"].ToString();
            string role = row["Role"].ToString();

            string sqlGetDetail = @"
                SELECT PasswordHash
                FROM RegistrationRequests
                WHERE RequestId = @id AND Status = N'PendingAdmin'";

            object passObj = DbHelper.ExecuteScalar(
                sqlGetDetail,
                new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
            );

            if (passObj == null || passObj == DBNull.Value)
            {
                MessageBox.Show("Không tìm thấy chi tiết yêu cầu (có thể đã được xử lý).",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                LoadPendingRequests();
                return;
            }

            string password = passObj.ToString();

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn DUYỆT yêu cầu này và tạo tài khoản cho người dùng?\n\n" +
                $"Username: {username}\nRole: {role}",
                "Xác nhận duyệt",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                // 1) Tạo user mới trong bảng Users
                string sqlInsertUser = @"
                    INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
                    VALUES (@u, @p, @name, @em, @role, 1);
                    SELECT SCOPE_IDENTITY();";

                object userIdObj = DbHelper.ExecuteScalar(
                    sqlInsertUser,
                    new SqlParameter("@u", SqlDbType.NVarChar, 50) { Value = username },
                    new SqlParameter("@p", SqlDbType.NVarChar, 255) { Value = password },
                    new SqlParameter("@name", SqlDbType.NVarChar, 100) { Value = fullName },
                    new SqlParameter("@em", SqlDbType.NVarChar, 100) { Value = email },
                    new SqlParameter("@role", SqlDbType.NVarChar, 20) { Value = role }
                );

                int newUserId = Convert.ToInt32(userIdObj);

                // 2) Tạo bản ghi SV / GV / Admin tương ứng
                if (role == "SV")
                {
                    string sql = @"
                        INSERT INTO Students (UserId, ClassName, Faculty)
                        VALUES (@uid, @class, @faculty);";

                    DbHelper.ExecuteNonQuery(
                        sql,
                        new SqlParameter("@uid", SqlDbType.Int) { Value = newUserId },
                        new SqlParameter("@class", SqlDbType.NVarChar, 50) { Value = "Chưa cập nhật" },
                        new SqlParameter("@faculty", SqlDbType.NVarChar, 100) { Value = "Chưa cập nhật" }
                    );
                }
                else if (role == "GV")
                {
                    string sql = @"
                        INSERT INTO Teachers (UserId, Department, AcademicTitle)
                        VALUES (@uid, @dept, @title);";

                    DbHelper.ExecuteNonQuery(
                        sql,
                        new SqlParameter("@uid", SqlDbType.Int) { Value = newUserId },
                        new SqlParameter("@dept", SqlDbType.NVarChar, 100) { Value = "Chưa cập nhật" },
                        new SqlParameter("@title", SqlDbType.NVarChar, 50) { Value = "GV" }
                    );
                }
                else if (role == "Admin")
                {
                    string sql = @"
                        INSERT INTO Admins (UserId, PermissionLevel)
                        VALUES (@uid, @perm);";

                    DbHelper.ExecuteNonQuery(
                        sql,
                        new SqlParameter("@uid", SqlDbType.Int) { Value = newUserId },
                        new SqlParameter("@perm", SqlDbType.Int) { Value = 1 }
                    );
                }

                // 3) Cập nhật trạng thái request thành Approved
                string sqlUpdateReq = @"
                    UPDATE RegistrationRequests
                    SET Status = N'Approved'
                    WHERE RequestId = @id";

                DbHelper.ExecuteNonQuery(
                    sqlUpdateReq,
                    new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
                );

                MessageBox.Show("Đã duyệt yêu cầu và tạo tài khoản thành công.",
                                "Thành công",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                LoadPendingRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi duyệt yêu cầu: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        // =============== TỪ CHỐI YÊU CẦU (REJECT) ===============

        private void btnReject_Click(object sender, EventArgs e)
        {
            DataRow row = GetSelectedRequest();
            if (row == null) return;

            int requestId = Convert.ToInt32(row["RequestId"]);
            string username = row["Username"].ToString();

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn TỪ CHỐI yêu cầu đăng ký của: " + username + "?",
                "Xác nhận từ chối",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string sql = @"
                    UPDATE RegistrationRequests
                    SET Status = N'Rejected'
                    WHERE RequestId = @id";

                DbHelper.ExecuteNonQuery(
                    sql,
                    new SqlParameter("@id", SqlDbType.Int) { Value = requestId }
                );

                MessageBox.Show("Đã từ chối yêu cầu đăng ký.",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                LoadPendingRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi từ chối yêu cầu: " + ex.Message,
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        // Ví dụ: sự kiện cho nút Đăng Xuất
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                                                    "Xác nhận đăng xuất",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // =============== KHỞI TẠO CONTROL (TỰ DESIGN BẰNG CODE) ===============

        private void InitializeComponent()
        {
            this.dgvRequests = new System.Windows.Forms.DataGridView();
            this.btnRefreshRequests = new System.Windows.Forms.Button();
            this.btnApprove = new System.Windows.Forms.Button();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRequests
            // 
            this.dgvRequests.AllowUserToAddRows = false;
            this.dgvRequests.AllowUserToDeleteRows = false;
            this.dgvRequests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRequests.Location = new System.Drawing.Point(12, 12);
            this.dgvRequests.MultiSelect = false;
            this.dgvRequests.Name = "dgvRequests";
            this.dgvRequests.ReadOnly = true;
            this.dgvRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequests.Size = new System.Drawing.Size(760, 350);
            this.dgvRequests.TabIndex = 0;
            // 
            // btnRefreshRequests
            // 
            this.btnRefreshRequests.Location = new System.Drawing.Point(12, 380);
            this.btnRefreshRequests.Name = "btnRefreshRequests";
            this.btnRefreshRequests.Size = new System.Drawing.Size(120, 35);
            this.btnRefreshRequests.TabIndex = 1;
            this.btnRefreshRequests.Text = "Tải lại";
            this.btnRefreshRequests.UseVisualStyleBackColor = true;
            this.btnRefreshRequests.Click += new System.EventHandler(this.btnRefreshRequests_Click);
            // 
            // btnApprove
            // 
            this.btnApprove.Location = new System.Drawing.Point(150, 380);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(120, 35);
            this.btnApprove.TabIndex = 2;
            this.btnApprove.Text = "Duyệt";
            this.btnApprove.UseVisualStyleBackColor = true;
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(288, 380);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(120, 35);
            this.btnReject.TabIndex = 3;
            this.btnReject.Text = "Từ chối";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(426, 380);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(120, 35);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Đóng";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnReject);
            this.Controls.Add(this.btnApprove);
            this.Controls.Add(this.btnRefreshRequests);
            this.Controls.Add(this.dgvRequests);
            this.Name = "AdminForm";
            this.Text = "Quản trị hệ thống - Duyệt yêu cầu đăng ký";
            this.Load += new System.EventHandler(this.AdminForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
