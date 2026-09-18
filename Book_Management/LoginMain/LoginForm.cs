using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;


namespace Book_Management
{
    public partial class LoginForm : Form
    {
        private readonly MemberRepository _repository = new();
        private bool _isBusy;
        public LoginMember? CurrentMember { get; private set; }

        public LoginForm()
        {
            InitializeComponent();

            btn_login.Click += Btn_login_click;
            btn_register.Click += Btn_register_click;

            FormClosing += (_, e) =>
            {
                if (_isBusy)
                {
                    e.Cancel = true;
                }
            };
        }


        private async void Btn_login_click(object? sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            string id = ID_input.Text.Trim();
            string password = PW_input.Text;

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("아이디와 비밀번호가 다릅니다");
                return;
            }
            LoginMember? member;

            SetBusy(true);

            try
            {
                member = await _repository.Login(id, password);
            }
            catch (SqlException)
            {
                MessageBox.Show(
                    "DB 연결 또는 조회에 실패했습니다.\n" +
                    "서버 이름과 회원 테이블을 확인해주세요.");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            if (member == null)
            {
                MessageBox.Show("아이디와 비밀번호가 다릅니다");

                PW_input.SelectAll();
                PW_input.Focus();

                return;
            }

            CurrentMember = member;

            DialogResult = DialogResult.OK; // Program.cs에 로그인 성공 전달

        }
        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            ID_input.Enabled = !busy;
            PW_input.Enabled = !busy;
            btn_login.Enabled = !busy;
            btn_register.Enabled = !busy;

            UseWaitCursor = busy;
        }

        private void Btn_register_click(object? sender, EventArgs e)
        {
            using var registerForm = new RegisterForm();
            // 로그인 창을 부모로 하는 팝업
            if (registerForm.ShowDialog(this) == DialogResult.OK)
            {
                 ID_input.Text = registerForm.RegisteredId;

                PW_input.Clear();
                PW_input.Focus();
            }
        }

    }
}
