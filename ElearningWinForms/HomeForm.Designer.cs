namespace ElearningWinForms
{
    partial class HomeForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblUserCode = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.btnAdmin = new System.Windows.Forms.Button();
            this.btnNotifications = new System.Windows.Forms.Button();
            this.btnGrades = new System.Windows.Forms.Button();
            this.btnSubmissions = new System.Windows.Forms.Button();
            this.btnAssignments = new System.Windows.Forms.Button();
            this.btnCourses = new System.Windows.Forms.Button();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelHeader.Controls.Add(this.btnLogout);
            this.panelHeader.Controls.Add(this.lblRole);
            this.panelHeader.Controls.Add(this.lblUserCode);
            this.panelHeader.Controls.Add(this.lblUserName);
            this.panelHeader.Controls.Add(this.lblAppTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(884, 60);
            this.panelHeader.TabIndex = 0;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.Location = new System.Drawing.Point(797, 17);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 25);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblRole
            // 
            this.lblRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(600, 35);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(41, 15);
            this.lblRole.TabIndex = 3;
            this.lblRole.Text = "Role: -";
            // 
            // lblUserCode
            // 
            this.lblUserCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserCode.AutoSize = true;
            this.lblUserCode.Location = new System.Drawing.Point(600, 20);
            this.lblUserCode.Name = "lblUserCode";
            this.lblUserCode.Size = new System.Drawing.Size(60, 15);
            this.lblUserCode.TabIndex = 2;
            this.lblUserCode.Text = "Mã: 0000";
            // 
            // lblUserName
            // 
            this.lblUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(600, 5);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(98, 15);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "Xin chào: (trống)";
            // 
            // lblAppTitle
            // 
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAppTitle.Location = new System.Drawing.Point(12, 18);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(276, 21);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "Hệ thống quản lý học tập trực tuyến";
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.Gainsboro;
            this.panelMenu.Controls.Add(this.btnAdmin);
            this.panelMenu.Controls.Add(this.btnNotifications);
            this.panelMenu.Controls.Add(this.btnGrades);
            this.panelMenu.Controls.Add(this.btnSubmissions);
            this.panelMenu.Controls.Add(this.btnAssignments);
            this.panelMenu.Controls.Add(this.btnCourses);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 60);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(180, 401);
            this.panelMenu.TabIndex = 1;
            // 
            // btnAdmin
            // 
            this.btnAdmin.Location = new System.Drawing.Point(12, 260);
            this.btnAdmin.Name = "btnAdmin";
            this.btnAdmin.Size = new System.Drawing.Size(156, 35);
            this.btnAdmin.TabIndex = 5;
            this.btnAdmin.Text = "Quản trị hệ thống";
            this.btnAdmin.UseVisualStyleBackColor = true;
            this.btnAdmin.Click += new System.EventHandler(this.btnAdmin_Click);
            // 
            // btnNotifications
            // 
            this.btnNotifications.Location = new System.Drawing.Point(12, 210);
            this.btnNotifications.Name = "btnNotifications";
            this.btnNotifications.Size = new System.Drawing.Size(156, 35);
            this.btnNotifications.TabIndex = 4;
            this.btnNotifications.Text = "Thông báo";
            this.btnNotifications.UseVisualStyleBackColor = true;
            this.btnNotifications.Click += new System.EventHandler(this.btnNotifications_Click);
            // 
            // btnGrades
            // 
            this.btnGrades.Location = new System.Drawing.Point(12, 160);
            this.btnGrades.Name = "btnGrades";
            this.btnGrades.Size = new System.Drawing.Size(156, 35);
            this.btnGrades.TabIndex = 3;
            this.btnGrades.Text = "Điểm & phản hồi";
            this.btnGrades.UseVisualStyleBackColor = true;
            this.btnGrades.Click += new System.EventHandler(this.btnGrades_Click);
            // 
            // btnSubmissions
            // 
            this.btnSubmissions.Location = new System.Drawing.Point(12, 110);
            this.btnSubmissions.Name = "btnSubmissions";
            this.btnSubmissions.Size = new System.Drawing.Size(156, 35);
            this.btnSubmissions.TabIndex = 2;
            this.btnSubmissions.Text = "Nộp bài / Chấm điểm";
            this.btnSubmissions.UseVisualStyleBackColor = true;
            this.btnSubmissions.Click += new System.EventHandler(this.btnSubmissions_Click);
            // 
            // btnAssignments
            // 
            this.btnAssignments.Location = new System.Drawing.Point(12, 60);
            this.btnAssignments.Name = "btnAssignments";
            this.btnAssignments.Size = new System.Drawing.Size(156, 35);
            this.btnAssignments.TabIndex = 1;
            this.btnAssignments.Text = "Bài tập";
            this.btnAssignments.UseVisualStyleBackColor = true;
            this.btnAssignments.Click += new System.EventHandler(this.btnAssignments_Click);
            // 
            // btnCourses
            // 
            this.btnCourses.Location = new System.Drawing.Point(12, 10);
            this.btnCourses.Name = "btnCourses";
            this.btnCourses.Size = new System.Drawing.Size(156, 35);
            this.btnCourses.TabIndex = 0;
            this.btnCourses.Text = "Khóa học";
            this.btnCourses.UseVisualStyleBackColor = true;
            this.btnCourses.Click += new System.EventHandler(this.btnCourses_Click);
            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(180, 60);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(704, 401);
            this.panelContent.TabIndex = 2;
            // 
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelHeader);
            this.Name = "HomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Elearning - Main Shell";
            this.Load += new System.EventHandler(this.HomeForm_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserCode;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button btnCourses;
        private System.Windows.Forms.Button btnAssignments;
        private System.Windows.Forms.Button btnSubmissions;
        private System.Windows.Forms.Button btnGrades;
        private System.Windows.Forms.Button btnNotifications;
        private System.Windows.Forms.Button btnAdmin;
        private System.Windows.Forms.Panel panelContent;
    }
}
