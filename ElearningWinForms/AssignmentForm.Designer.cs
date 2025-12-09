namespace ElearningWinForms
{
    partial class AssignmentForm
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
            this.dgvAssignments = new System.Windows.Forms.DataGridView();
            this.btnCreateAssignment = new System.Windows.Forms.Button();
            this.btnUpdateAssignment = new System.Windows.Forms.Button();
            this.btnDeleteAssignment = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignments)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAssignments
            // 
            this.dgvAssignments.AllowUserToAddRows = false;
            this.dgvAssignments.AllowUserToDeleteRows = false;
            this.dgvAssignments.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAssignments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAssignments.Location = new System.Drawing.Point(12, 12);
            this.dgvAssignments.MultiSelect = false;
            this.dgvAssignments.Name = "dgvAssignments";
            this.dgvAssignments.ReadOnly = true;
            this.dgvAssignments.RowTemplate.Height = 25;
            this.dgvAssignments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAssignments.Size = new System.Drawing.Size(560, 260);
            this.dgvAssignments.TabIndex = 0;
            // 
            // btnCreateAssignment
            // 
            this.btnCreateAssignment.Location = new System.Drawing.Point(12, 285);
            this.btnCreateAssignment.Name = "btnCreateAssignment";
            this.btnCreateAssignment.Size = new System.Drawing.Size(160, 30);
            this.btnCreateAssignment.TabIndex = 1;
            this.btnCreateAssignment.Text = "Tạo bài tập mới";
            this.btnCreateAssignment.UseVisualStyleBackColor = true;
            this.btnCreateAssignment.Click += new System.EventHandler(this.btnCreateAssignment_Click);
            // 
            // btnUpdateAssignment
            // 
            this.btnUpdateAssignment.Location = new System.Drawing.Point(210, 285);
            this.btnUpdateAssignment.Name = "btnUpdateAssignment";
            this.btnUpdateAssignment.Size = new System.Drawing.Size(160, 30);
            this.btnUpdateAssignment.TabIndex = 2;
            this.btnUpdateAssignment.Text = "Sửa bài tập";
            this.btnUpdateAssignment.UseVisualStyleBackColor = true;
            this.btnUpdateAssignment.Click += new System.EventHandler(this.btnUpdateAssignment_Click);
            // 
            // btnDeleteAssignment
            // 
            this.btnDeleteAssignment.Location = new System.Drawing.Point(412, 285);
            this.btnDeleteAssignment.Name = "btnDeleteAssignment";
            this.btnDeleteAssignment.Size = new System.Drawing.Size(160, 30);
            this.btnDeleteAssignment.TabIndex = 3;
            this.btnDeleteAssignment.Text = "Xóa bài tập";
            this.btnDeleteAssignment.UseVisualStyleBackColor = true;
            this.btnDeleteAssignment.Click += new System.EventHandler(this.btnDeleteAssignment_Click);
            // 
            // AssignmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 331);
            this.Controls.Add(this.btnDeleteAssignment);
            this.Controls.Add(this.btnUpdateAssignment);
            this.Controls.Add(this.btnCreateAssignment);
            this.Controls.Add(this.dgvAssignments);
            this.Name = "AssignmentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý bài tập";
            this.Load += new System.EventHandler(this.AssignmentForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssignments)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvAssignments;
        private System.Windows.Forms.Button btnCreateAssignment;
        private System.Windows.Forms.Button btnUpdateAssignment;
        private System.Windows.Forms.Button btnDeleteAssignment;
    }
}
