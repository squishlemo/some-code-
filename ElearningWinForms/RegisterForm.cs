using System;
using System.Windows.Forms;
using ElearningWinForms.Data;

namespace ElearningWinForms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            // TODO: chuẩn bị dữ liệu dropdown role (SV, GV) nếu cần.
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // TODO:
            // - Đọc thông tin từ các textbox
            // - Dùng DbHelper.ExecuteNonQuery để INSERT vào bảng Users / Students / Teachers.
        }
    }
}
