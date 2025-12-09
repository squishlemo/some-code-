namespace ElearningWinForms
{
    partial class NotificationForm
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
            this.dgvNotifications = new System.Windows.Forms.DataGridView();
            this.grpCompose = new System.Windows.Forms.GroupBox();
            this.lblScope = new System.Windows.Forms.Label();
            this.cboScope = new System.Windows.Forms.ComboBox();
            this.lblFaculty = new System.Windows.Forms.Label();
            this.cboFaculty = new System.Windows.Forms.ComboBox();
            this.lblSection = new System.Windows.Forms.Label();
            this.cboSection = new System.Windows.Forms.ComboBox();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.btnSendNotification = new System.Windows.Forms.Button();
            this.btnDeleteNotification = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotifications)).BeginInit();
            this.grpCompose.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvNotifications
            // 
            this.dgvNotifications.AllowUserToAddRows = false;
            this.dgvNotifications.AllowUserToDeleteRows = false;
            this.dgvNotifications.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNotifications.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNotifications.Location = new System.Drawing.Point(12, 12);
            this.dgvNotifications.MultiSelect = false;
            this.dgvNotifications.Name = "dgvNotifications";
            this.dgvNotifications.ReadOnly = true;
            this.dgvNotifications.RowTemplate.Height = 25;
            this.dgvNotifications.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNotifications.Size = new System.Drawing.Size(760, 220);
            this.dgvNotifications.TabIndex = 0;
            this.dgvNotifications.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNotifications_CellDoubleClick);
            // 
            // grpCompose
            // 
            this.grpCompose.Controls.Add(this.lblScope);
            this.grpCompose.Controls.Add(this.cboScope);
            this.grpCompose.Controls.Add(this.lblFaculty);
            this.grpCompose.Controls.Add(this.cboFaculty);
            this.grpCompose.Controls.Add(this.lblSection);
            this.grpCompose.Controls.Add(this.cboSection);
            this.grpCompose.Controls.Add(this.txtContent);
            this.grpCompose.Controls.Add(this.lblContent);
            this.grpCompose.Controls.Add(this.btnSendNotification);
            this.grpCompose.Location = new System.Drawing.Point(12, 238);
            this.grpCompose.Name = "grpCompose";
            this.grpCompose.Size = new System.Drawing.Size(760, 150);
            this.grpCompose.TabIndex = 1;
            this.grpCompose.TabStop = false;
            this.grpCompose.Text = "Thông báo";
            // 
            // lblScope
            // 
            this.lblScope.AutoSize = true;
            this.lblScope.Location = new System.Drawing.Point(520, 22);
            this.lblScope.Name = "lblScope";
            this.lblScope.Size = new System.Drawing.Size(57, 15);
            this.lblScope.TabIndex = 7;
            this.lblScope.Text = "Phạm vi:";
            // 
            // cboScope
            // 
            this.cboScope.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboScope.FormattingEnabled = true;
            this.cboScope.Location = new System.Drawing.Point(520, 40);
            this.cboScope.Name = "cboScope";
            this.cboScope.Size = new System.Drawing.Size(220, 23);
            this.cboScope.TabIndex = 3;
            this.cboScope.SelectedIndexChanged += new System.EventHandler(this.cboScope_SelectedIndexChanged);
            // 
            // lblFaculty
            // 
            this.lblFaculty.AutoSize = true;
            this.lblFaculty.Location = new System.Drawing.Point(520, 70);
            this.lblFaculty.Name = "lblFaculty";
            this.lblFaculty.Size = new System.Drawing.Size(39, 15);
            this.lblFaculty.TabIndex = 8;
            this.lblFaculty.Text = "Khoa:";
            // 
            // cboFaculty
            // 
            this.cboFaculty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFaculty.FormattingEnabled = true;
            this.cboFaculty.Location = new System.Drawing.Point(520, 88);
            this.cboFaculty.Name = "cboFaculty";
            this.cboFaculty.Size = new System.Drawing.Size(220, 23);
            this.cboFaculty.TabIndex = 4;
            // 
            // lblSection
            // 
            this.lblSection.AutoSize = true;
            this.lblSection.Location = new System.Drawing.Point(520, 70);
            this.lblSection.Name = "lblSection";
            this.lblSection.Size = new System.Drawing.Size(77, 15);
            this.lblSection.TabIndex = 9;
            this.lblSection.Text = "Lớp học phần:";
            // 
            // cboSection
            // 
            this.cboSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSection.FormattingEnabled = true;
            this.cboSection.Location = new System.Drawing.Point(520, 88);
            this.cboSection.Name = "cboSection";
            this.cboSection.Size = new System.Drawing.Size(220, 23);
            this.cboSection.TabIndex = 5;
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(15, 40);
            this.txtContent.Multiline = true;
            this.txtContent.Name = "txtContent";
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtContent.Size = new System.Drawing.Size(480, 71);
            this.txtContent.TabIndex = 1;
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Location = new System.Drawing.Point(12, 22);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(60, 15);
            this.lblContent.TabIndex = 0;
            this.lblContent.Text = "Nội dung:";
            // 
            // btnSendNotification
            // 
            this.btnSendNotification.Location = new System.Drawing.Point(520, 117);
            this.btnSendNotification.Name = "btnSendNotification";
            this.btnSendNotification.Size = new System.Drawing.Size(220, 23);
            this.btnSendNotification.TabIndex = 6;
            this.btnSendNotification.Text = "Gửi thông báo";
            this.btnSendNotification.UseVisualStyleBackColor = true;
            this.btnSendNotification.Click += new System.EventHandler(this.btnSendNotification_Click);
            // 
            // btnDeleteNotification
            // 
            this.btnDeleteNotification.Location = new System.Drawing.Point(12, 394);
            this.btnDeleteNotification.Name = "btnDeleteNotification";
            this.btnDeleteNotification.Size = new System.Drawing.Size(120, 27);
            this.btnDeleteNotification.TabIndex = 2;
            this.btnDeleteNotification.Text = "Xóa thông báo";
            this.btnDeleteNotification.UseVisualStyleBackColor = true;
            this.btnDeleteNotification.Click += new System.EventHandler(this.btnDeleteNotification_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(150, 394);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 27);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Tải lại";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // NotificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 431);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDeleteNotification);
            this.Controls.Add(this.grpCompose);
            this.Controls.Add(this.dgvNotifications);
            this.Name = "NotificationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thông báo";
            this.Load += new System.EventHandler(this.NotificationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNotifications)).EndInit();
            this.grpCompose.ResumeLayout(false);
            this.grpCompose.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvNotifications;
        private System.Windows.Forms.GroupBox grpCompose;
        private System.Windows.Forms.Label lblScope;
        private System.Windows.Forms.ComboBox cboScope;
        private System.Windows.Forms.Label lblFaculty;
        private System.Windows.Forms.ComboBox cboFaculty;
        private System.Windows.Forms.Label lblSection;
        private System.Windows.Forms.ComboBox cboSection;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.Button btnSendNotification;
        private System.Windows.Forms.Button btnDeleteNotification;
        private System.Windows.Forms.Button btnRefresh;
    }
}
