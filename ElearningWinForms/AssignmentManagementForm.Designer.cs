namespace ElearningWinForms
{
    partial class AssignmentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblSection;
        private System.Windows.Forms.ComboBox cboSection;
        private System.Windows.Forms.DataGridView dgvAssignments;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDeadline;
        private System.Windows.Forms.DateTimePicker dtpDeadline;
        private System.Windows.Forms.Label lblAttachment;
        private System.Windows.Forms.TextBox txtAttachmentPath;
        private System.Windows.Forms.Button btnBrowseAttachment;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Button btnCreateAssignment;
        private System.Windows.Forms.Button btnUpdateAssignment;
        private System.Windows.Forms.Button btnDeleteAssignment;
        private System.Windows.Forms.Button btnRefreshAssignments;
        private System.Windows.Forms.Label lblSubmissions;
        private System.Windows.Forms.DataGridView dgvSubmissions;
        private System.Windows.Forms.Button btnLoadSubmissions;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblSection = new System.Windows.Forms.Label();
            this.cboSection = new System.Windows.Forms.ComboBox();
            this.dgvAssignments = new System.Windows.Forms.DataGridView();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDeadline = new System.Windows.Forms.Label();
            this.dtpDeadline = new System.Windows.Forms.DateTimePicker();
            this.lblAttachment = new System.Windows.Forms.Label();
            this.txtAttachmentPath = new System.Windows.Forms.TextBox();
            this.btnBrowseAttachment = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.btnCreateAssignment = new System.Windows.Forms.Button();
            this.btnUpdateAssignment = new System.Windows.Forms.Button();
            this.btnDeleteAssignment = new System.Windows.Forms.Button();
            this.btnRefreshAssignments = new System.Windows.Forms.Button();
            this.lblSubmissions = new System.Windows.Forms.Label();
            this.dgvSubmissions = new System.Windows.Forms.DataGridView();
            this.btnLoadSubmissions = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubmissions)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(10, 8);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(164, 21);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Quản lý bài tập (GV)";
            // 
            // lblSection
            // 
            this.lblSection.AutoSize = true;
            this.lblSection.Location = new System.Drawing.Point(10, 36);
            this.lblSection.Name = "lblSection";
            this.lblSection.Size = new System.Drawing.Size(76, 13);
            this.lblSection.TabIndex = 1;
            this.lblSection.Text = "Lớp học phần:";
            // 
            // cboSection
            // 
            this.cboSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSection.FormattingEnabled = true;
            this.cboSection.Location = new System.Drawing.Point(87, 34);
            this.cboSection.Name = "cboSection";
            this.cboSection.Size = new System.Drawing.Size(301, 21);
            this.cboSection.TabIndex = 2;
            this.cboSection.SelectedIndexChanged += new System.EventHandler(this.cboSection_SelectedIndexChanged);
            // 
            // dgvAssignments
            // 
            this.dgvAssignments.AllowUserToAddRows = false;
            this.dgvAssignments.AllowUserToDeleteRows = false;
            this.dgvAssignments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAssignments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAssignments.Location = new System.Drawing.Point(10, 61);
            this.dgvAssignments.MultiSelect = false;
            this.dgvAssignments.Name = "dgvAssignments";
            this.dgvAssignments.ReadOnly = true;
            this.dgvAssignments.RowTemplate.Height = 25;
            this.dgvAssignments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAssignments.Size = new System.Drawing.Size(737, 156);
            this.dgvAssignments.TabIndex = 3;
            this.dgvAssignments.SelectionChanged += new System.EventHandler(this.dgvAssignments_SelectionChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(10, 230);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(47, 13);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Tiêu đề:";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(60, 227);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(258, 20);
            this.txtTitle.TabIndex = 5;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(10, 256);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(37, 13);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "Mô tả:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(60, 253);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(258, 70);
            this.txtDescription.TabIndex = 7;
            // 
            // lblDeadline
            // 
            this.lblDeadline.AutoSize = true;
            this.lblDeadline.Location = new System.Drawing.Point(334, 230);
            this.lblDeadline.Name = "lblDeadline";
            this.lblDeadline.Size = new System.Drawing.Size(52, 13);
            this.lblDeadline.TabIndex = 8;
            this.lblDeadline.Text = "Deadline:";
            // 
            // dtpDeadline
            // 
            this.dtpDeadline.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpDeadline.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDeadline.Location = new System.Drawing.Point(390, 227);
            this.dtpDeadline.Name = "dtpDeadline";
            this.dtpDeadline.Size = new System.Drawing.Size(172, 20);
            this.dtpDeadline.TabIndex = 9;
            // 
            // lblAttachment
            // 
            this.lblAttachment.AutoSize = true;
            this.lblAttachment.Location = new System.Drawing.Point(334, 256);
            this.lblAttachment.Name = "lblAttachment";
            this.lblAttachment.Size = new System.Drawing.Size(75, 13);
            this.lblAttachment.TabIndex = 10;
            this.lblAttachment.Text = "File đính kèm:";
            // 
            // txtAttachmentPath
            // 
            this.txtAttachmentPath.Location = new System.Drawing.Point(402, 253);
            this.txtAttachmentPath.Name = "txtAttachmentPath";
            this.txtAttachmentPath.Size = new System.Drawing.Size(241, 20);
            this.txtAttachmentPath.TabIndex = 11;
            // 
            // btnBrowseAttachment
            // 
            this.btnBrowseAttachment.Location = new System.Drawing.Point(647, 252);
            this.btnBrowseAttachment.Name = "btnBrowseAttachment";
            this.btnBrowseAttachment.Size = new System.Drawing.Size(64, 22);
            this.btnBrowseAttachment.TabIndex = 12;
            this.btnBrowseAttachment.Text = "Chọn...";
            this.btnBrowseAttachment.UseVisualStyleBackColor = true;
            this.btnBrowseAttachment.Click += new System.EventHandler(this.btnBrowseAttachment_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(334, 282);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(58, 13);
            this.lblStatus.TabIndex = 13;
            this.lblStatus.Text = "Trạng thái:";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.FormattingEnabled = true;
            this.cboStatus.Location = new System.Drawing.Point(402, 279);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(129, 21);
            this.cboStatus.TabIndex = 14;
            // 
            // btnCreateAssignment
            // 
            this.btnCreateAssignment.Location = new System.Drawing.Point(10, 334);
            this.btnCreateAssignment.Name = "btnCreateAssignment";
            this.btnCreateAssignment.Size = new System.Drawing.Size(137, 26);
            this.btnCreateAssignment.TabIndex = 15;
            this.btnCreateAssignment.Text = "Tạo bài tập mới";
            this.btnCreateAssignment.UseVisualStyleBackColor = true;
            this.btnCreateAssignment.Click += new System.EventHandler(this.btnCreateAssignment_Click);
            // 
            // btnUpdateAssignment
            // 
            this.btnUpdateAssignment.Location = new System.Drawing.Point(153, 334);
            this.btnUpdateAssignment.Name = "btnUpdateAssignment";
            this.btnUpdateAssignment.Size = new System.Drawing.Size(137, 26);
            this.btnUpdateAssignment.TabIndex = 16;
            this.btnUpdateAssignment.Text = "Chỉnh sửa bài tập";
            this.btnUpdateAssignment.UseVisualStyleBackColor = true;
            this.btnUpdateAssignment.Click += new System.EventHandler(this.btnUpdateAssignment_Click);
            // 
            // btnDeleteAssignment
            // 
            this.btnDeleteAssignment.Location = new System.Drawing.Point(295, 334);
            this.btnDeleteAssignment.Name = "btnDeleteAssignment";
            this.btnDeleteAssignment.Size = new System.Drawing.Size(137, 26);
            this.btnDeleteAssignment.TabIndex = 17;
            this.btnDeleteAssignment.Text = "Xóa bài tập";
            this.btnDeleteAssignment.UseVisualStyleBackColor = true;
            this.btnDeleteAssignment.Click += new System.EventHandler(this.btnDeleteAssignment_Click);
            // 
            // btnRefreshAssignments
            // 
            this.btnRefreshAssignments.Location = new System.Drawing.Point(437, 334);
            this.btnRefreshAssignments.Name = "btnRefreshAssignments";
            this.btnRefreshAssignments.Size = new System.Drawing.Size(137, 26);
            this.btnRefreshAssignments.TabIndex = 18;
            this.btnRefreshAssignments.Text = "Làm mới danh sách";
            this.btnRefreshAssignments.UseVisualStyleBackColor = true;
            this.btnRefreshAssignments.Click += new System.EventHandler(this.btnRefreshAssignments_Click);
            // 
            // lblSubmissions
            // 
            this.lblSubmissions.AutoSize = true;
            this.lblSubmissions.Location = new System.Drawing.Point(10, 371);
            this.lblSubmissions.Name = "lblSubmissions";
            this.lblSubmissions.Size = new System.Drawing.Size(128, 13);
            this.lblSubmissions.TabIndex = 19;
            this.lblSubmissions.Text = "Danh sách sinh viên nộp:";
            // 
            // dgvSubmissions
            // 
            this.dgvSubmissions.AllowUserToAddRows = false;
            this.dgvSubmissions.AllowUserToDeleteRows = false;
            this.dgvSubmissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSubmissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubmissions.Location = new System.Drawing.Point(10, 387);
            this.dgvSubmissions.MultiSelect = false;
            this.dgvSubmissions.Name = "dgvSubmissions";
            this.dgvSubmissions.ReadOnly = true;
            this.dgvSubmissions.RowTemplate.Height = 25;
            this.dgvSubmissions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubmissions.Size = new System.Drawing.Size(737, 130);
            this.dgvSubmissions.TabIndex = 20;
            // 
            // btnLoadSubmissions
            // 
            this.btnLoadSubmissions.Location = new System.Drawing.Point(579, 334);
            this.btnLoadSubmissions.Name = "btnLoadSubmissions";
            this.btnLoadSubmissions.Size = new System.Drawing.Size(168, 26);
            this.btnLoadSubmissions.TabIndex = 21;
            this.btnLoadSubmissions.Text = "Xem DS sinh viên nộp bài";
            this.btnLoadSubmissions.UseVisualStyleBackColor = true;
            this.btnLoadSubmissions.Click += new System.EventHandler(this.btnLoadSubmissions_Click);
            // 
            // AssignmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 530);
            this.Controls.Add(this.btnLoadSubmissions);
            this.Controls.Add(this.dgvSubmissions);
            this.Controls.Add(this.lblSubmissions);
            this.Controls.Add(this.btnRefreshAssignments);
            this.Controls.Add(this.btnDeleteAssignment);
            this.Controls.Add(this.btnUpdateAssignment);
            this.Controls.Add(this.btnCreateAssignment);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnBrowseAttachment);
            this.Controls.Add(this.txtAttachmentPath);
            this.Controls.Add(this.lblAttachment);
            this.Controls.Add(this.dtpDeadline);
            this.Controls.Add(this.lblDeadline);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dgvAssignments);
            this.Controls.Add(this.cboSection);
            this.Controls.Add(this.lblSection);
            this.Controls.Add(this.lblHeader);
            this.Name = "AssignmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.AssignmentForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubmissions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
