using System.Windows.Forms;

namespace ElearningWinForms
{
    partial class GradeForm
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
            this.lblGrades = new System.Windows.Forms.Label();
            this.dgvGrades = new System.Windows.Forms.DataGridView();
            this.lblReviews = new System.Windows.Forms.Label();
            this.dgvReviewRequests = new System.Windows.Forms.DataGridView();
            this.btnRequestReview = new System.Windows.Forms.Button();
            this.btnViewSubmission = new System.Windows.Forms.Button();
            this.btnApproveReview = new System.Windows.Forms.Button();
            this.btnRejectReview = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrades)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReviewRequests)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGrades
            // 
            this.lblGrades.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblGrades.AutoSize = true;
            this.lblGrades.Location = new System.Drawing.Point(21, 42);
            this.lblGrades.Name = "lblGrades";
            this.lblGrades.Size = new System.Drawing.Size(81, 13);
            this.lblGrades.TabIndex = 0;
            this.lblGrades.Text = "Bảng điểm (SV)";
            this.lblGrades.Click += new System.EventHandler(this.lblGrades_Click);
            // 
            // dgvGrades
            // 
            this.dgvGrades.AllowUserToAddRows = false;
            this.dgvGrades.AllowUserToDeleteRows = false;
            this.dgvGrades.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dgvGrades.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGrades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGrades.Location = new System.Drawing.Point(15, 72);
            this.dgvGrades.MultiSelect = false;
            this.dgvGrades.Name = "dgvGrades";
            this.dgvGrades.ReadOnly = true;
            this.dgvGrades.RowTemplate.Height = 25;
            this.dgvGrades.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGrades.Size = new System.Drawing.Size(651, 100);
            this.dgvGrades.TabIndex = 1;
            // 
            // lblReviews
            // 
            this.lblReviews.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblReviews.AutoSize = true;
            this.lblReviews.Location = new System.Drawing.Point(21, 42);
            this.lblReviews.Name = "lblReviews";
            this.lblReviews.Size = new System.Drawing.Size(181, 13);
            this.lblReviews.TabIndex = 2;
            this.lblReviews.Text = "Yêu cầu phúc khảo (xem ở phía GV)";
            // 
            // dgvReviewRequests
            // 
            this.dgvReviewRequests.AllowUserToAddRows = false;
            this.dgvReviewRequests.AllowUserToDeleteRows = false;
            this.dgvReviewRequests.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReviewRequests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReviewRequests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReviewRequests.Location = new System.Drawing.Point(15, 72);
            this.dgvReviewRequests.MultiSelect = false;
            this.dgvReviewRequests.Name = "dgvReviewRequests";
            this.dgvReviewRequests.ReadOnly = true;
            this.dgvReviewRequests.RowTemplate.Height = 25;
            this.dgvReviewRequests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReviewRequests.Size = new System.Drawing.Size(651, 241);
            this.dgvReviewRequests.TabIndex = 3;
            // 
            // btnRequestReview
            // 
            this.btnRequestReview.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRequestReview.Location = new System.Drawing.Point(162, 350);
            this.btnRequestReview.Name = "btnRequestReview";
            this.btnRequestReview.Size = new System.Drawing.Size(146, 23);
            this.btnRequestReview.TabIndex = 4;
            this.btnRequestReview.Text = "SV yêu cầu phúc khảo";
            this.btnRequestReview.UseVisualStyleBackColor = true;
            this.btnRequestReview.Click += new System.EventHandler(this.btnRequestReview_Click);
            // 
            // btnViewSubmission
            // 
            this.btnViewSubmission.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnViewSubmission.Location = new System.Drawing.Point(379, 350);
            this.btnViewSubmission.Name = "btnViewSubmission";
            this.btnViewSubmission.Size = new System.Drawing.Size(146, 23);
            this.btnViewSubmission.TabIndex = 5;
            this.btnViewSubmission.Text = "Mở file bài nộp";
            this.btnViewSubmission.UseVisualStyleBackColor = true;
            this.btnViewSubmission.Click += new System.EventHandler(this.btnViewSubmission_Click);
            // 
            // btnApproveReview
            // 
            this.btnApproveReview.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnApproveReview.Location = new System.Drawing.Point(10, 350);
            this.btnApproveReview.Name = "btnApproveReview";
            this.btnApproveReview.Size = new System.Drawing.Size(146, 23);
            this.btnApproveReview.TabIndex = 6;
            this.btnApproveReview.Text = "GV duyệt phúc khảo";
            this.btnApproveReview.UseVisualStyleBackColor = true;
            this.btnApproveReview.Click += new System.EventHandler(this.btnApproveReview_Click);
            // 
            // btnRejectReview
            // 
            this.btnRejectReview.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRejectReview.Location = new System.Drawing.Point(531, 350);
            this.btnRejectReview.Name = "btnRejectReview";
            this.btnRejectReview.Size = new System.Drawing.Size(146, 23);
            this.btnRejectReview.TabIndex = 7;
            this.btnRejectReview.Text = "GV từ chối phúc khảo";
            this.btnRejectReview.UseVisualStyleBackColor = true;
            this.btnRejectReview.Click += new System.EventHandler(this.btnRejectReview_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRefresh.Location = new System.Drawing.Point(297, 392);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(86, 23);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Tải lại";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // GradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 427);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnRejectReview);
            this.Controls.Add(this.btnApproveReview);
            this.Controls.Add(this.btnViewSubmission);
            this.Controls.Add(this.btnRequestReview);
            this.Controls.Add(this.dgvReviewRequests);
            this.Controls.Add(this.lblReviews);
            this.Controls.Add(this.dgvGrades);
            this.Controls.Add(this.lblGrades);
            this.Name = "GradeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý điểm & phúc khảo";
            this.Load += new System.EventHandler(this.GradeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrades)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReviewRequests)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGrades;
        private System.Windows.Forms.DataGridView dgvGrades;
        private System.Windows.Forms.Label lblReviews;
        private System.Windows.Forms.DataGridView dgvReviewRequests;
        private System.Windows.Forms.Button btnRequestReview;
        private System.Windows.Forms.Button btnViewSubmission;
        private System.Windows.Forms.Button btnApproveReview;
        private System.Windows.Forms.Button btnRejectReview;
        private System.Windows.Forms.Button btnRefresh;
    }
}
