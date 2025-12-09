namespace ElearningWinForms
{
    partial class GradeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRequestReview = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRequestReview
            // 
            this.btnRequestReview.Location = new System.Drawing.Point(12, 12);
            this.btnRequestReview.Name = "btnRequestReview";
            this.btnRequestReview.Size = new System.Drawing.Size(150, 23);
            this.btnRequestReview.TabIndex = 0;
            this.btnRequestReview.Text = "Request Review";
            this.btnRequestReview.UseVisualStyleBackColor = true;
            this.btnRequestReview.Click += new System.EventHandler(this.btnRequestReview_Click);
            // 
            // GradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.btnRequestReview);
            this.Name = "GradeForm";
            this.Text = "Grade Management";
            this.Load += new System.EventHandler(this.GradeForm_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnRequestReview;
    }
}
