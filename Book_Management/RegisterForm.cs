using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Book_Management
{
    public partial class RegisterForm : Form
    {
        private readonly MemberRepository _repository = new MemberRepository();
        private readonly TextBox[] _inputBoxes;
        private readonly Dictionary<TextBox, Color> _originalColors = new Dictionary<TextBox, Color>();
        private string? _checkedId; //중복 아이디 체크
        private bool _isBusy;
        public string RegisteredId { get; private set; } = "";


        public RegisterForm()
        {
            InitializeComponent();

            _inputBoxes = new[]
            {
                Name_input,
                Phone_input,
                Id_input,
                Pw_input,
                CheckPw_input
            };
            foreach(TextBox box in _inputBoxes)
            {
                _originalColors[box] = box.BackColor;
                box.TextChanged += InputBox_TextChanged; // 다시 입력시 원래 색상으로
            }
            // 아이디 변경 시 중복확인 상태 해제
            Id_input.TextChanged += (_, _) => ResetIdCheck();

            btn_idcheck.Click += Btn_idcheck_Click;
            btn_register.Click += Btn_register_Click;
        }

        private void InputBox_TextChanged(object? sender, EventArgs e)
        {
            if (sender is TextBox box)
            {
                box.BackColor = _originalColors[box];
            }
        }

        private void ResetIdCheck()
        {
            _checkedId = null;

            Id_check_mark.Visible = false;
            btn_idcheck.Enabled = !_isBusy;
        }

        private async void Btn_idcheck_Click(object? sender, EventArgs e)
        {
            if (_isBusy) return;
            string id = Id_input.Text.Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                MarkError(Id_input);
                MessageBox.Show("빈 칸을 입력해주세요");
                return;
            }
            if (id.Length < 3)
            {
                MarkError(Id_input);
                MessageBox.Show("아이디는 최소 3글자 이상이어야 합니다");
                return;
            }
            SetBusy(true);

            try
            {
                bool exists = await _repository.IsIdExists(id);
                if (exists)
                {
                    ResetIdCheck();
                    MarkError(Id_input);
                    MessageBox.Show("이미 존재하는 아이디입니다");
                }
                else
                {
                    _checkedId = id;
                    Id_check_mark.Visible = true;
                    Id_input.BackColor = _originalColors[Id_input];
                }
            }
            catch (SqlException)
            {
                ResetIdCheck();
                MarkError(Id_input);
                MessageBox.Show("DB 연결 또는 아이디 조회에 실패했습니다.");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void Btn_register_Click(object? sender, EventArgs e)
        {
            if (_isBusy) return;

            if (!ValidateInputs())
            {
                return;
            }

            string name = Name_input.Text.Trim();
            string phone = Phone_input.Text.Trim();
            string id = Id_input.Text.Trim();
            string password = Pw_input.Text;

            RegisterResult result;

            SetBusy(true);

            try
            {
                result = await _repository.Register(name, phone, id, password);
            }
            catch (SqlException)
            {
                MessageBox.Show("회원가입 처리 중 오류가 발생했습니다");
                return;
            }
            finally { SetBusy(false); }

            switch (result)
            {
                case RegisterResult.DuplicatePhone:
                    MarkError(Phone_input);
                    MessageBox.Show("이미 등록된 회원입니다.");
                    return;

                case RegisterResult.DuplicateId:
                    MarkError(Id_input);
                    MessageBox.Show("중복된 아이디입니다.");
                    return;

                case RegisterResult.Success:
                    RegisteredId = id;
                    MessageBox.Show("회원가입이 완료되었습니다.");

                    DialogResult = DialogResult.OK;
                    return;
            }
        }

        private bool ValidateInputs()
        {
            RestoreColors();

            bool hasEmpty = false;

            foreach (TextBox box in _inputBoxes)
            {
                if (string.IsNullOrWhiteSpace(box.Text))
                {
                    MarkError(box);
                    hasEmpty = true;
                }
            }

            if (hasEmpty)
            {
                MessageBox.Show("빈 칸을 입력해주세요");
                return false;
            }

            var errors = new List<string>();

            if (Name_input.Text.Trim().Length < 2)
            {
                MarkError(Name_input);
                errors.Add("이름은 최소 2글자 이상입니다.");
            }

            if (Id_input.Text.Trim().Length < 3)
            {
                MarkError(Id_input);
                errors.Add("아이디는 최소 3글자 이상입니다.");
            }

            if (Pw_input.Text.Length < 4)
            {
                MarkError(Pw_input);
                errors.Add("비밀번호는 최소 4글자 이상입니다.");
            }

            string phone = Phone_input.Text.Trim();

            if (!Regex.IsMatch(
                    phone,
                    @"\A010-[0-9]{4}-[0-9]{4}\z"))
            {
                MarkError(Phone_input);
                errors.Add("연락처 형식이 잘못되었습니다");
            }

            if (Pw_input.Text != CheckPw_input.Text)
            {
                MarkError(Pw_input);
                MarkError(CheckPw_input);

                errors.Add("비밀번호가 틀립니다");
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errors));

                return false;
            }

            if (_checkedId != Id_input.Text.Trim())
            {
                MarkError(Id_input);

                MessageBox.Show("아이디 중복확인을 해주세요");

                return false;
            }

            return true;
        }

        private void RestoreColors()
        {
            foreach (TextBox box in _inputBoxes)
            {
                box.BackColor = _originalColors[box];
            }
        }
        private void MarkError(TextBox box)
        {
            box.BackColor = Color.LightCoral;
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            foreach (TextBox box in _inputBoxes)
            {
                box.Enabled = !busy;
            }

            btn_register.Enabled = !busy;
            btn_cancel.Enabled = !busy;

            // 확인된 아이디가 있으면 버튼을 계속 비활성화
            btn_idcheck.Enabled = !busy && _checkedId == null;

            UseWaitCursor = busy;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void btn_register_Click(object sender, EventArgs e)
        {

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {

        }

        private void Name_input_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
