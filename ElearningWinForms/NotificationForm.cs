using System;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class NotificationForm : Form
    {
        public int CurrentUserId { get; set; }
        public string CurrentRole { get; set; }

        public NotificationForm(int userId, string role)
        {
            InitializeComponent();
            CurrentUserId = userId;
            CurrentRole = role;
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            // TODO: load thông báo cho user hiện tại từ Notifications + NotificationRecipients.
        }

        private void btnSendNotification_Click(object sender, EventArgs e)
        {
            // TODO: INSERT vào Notifications + NotificationRecipients (tùy targetType).
        }
    }
}
