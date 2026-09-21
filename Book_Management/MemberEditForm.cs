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
using static Book_Management.MemberData;


namespace Book_Management
{
    public partial class MemberEditForm : Form
    {
        private readonly MemberRepository _repository = new MemberRepository();
        private readonly TextBox[] _editableBoxes;
        private int _memberNumber;
        private bool _isBusy;
        public string SavedName { get; private set; } = "";


        public MemberEditForm()
        {
            InitializeComponent();
            _editableBoxes = new[]
            {
                txtName,
                txtPhone,
                txtPw
            };

            foreach (TextBox box in _editableBoxes)
            {
                box.TextChanged += (_, _) =>
                {
                    box.BackColor = Color.White;
                };
            }

            btnSave.Click += BtnSave_Click;
            btnResetPassword.Click += BtnResetPassword_Click;

            FormClosing += (_, e) =>
            {
                if (_isBusy)
                {
                    e.Cancel = true;
                }
            };
        }

        public MemberEditForm(MemberData member) : this()
        {
            _memberNumber = member.MemberNumber;

            txtMemberNumber.Text = member.MemberNumber.ToString();
            txtName.Text = member.Name;
            txtPhone.Text = member.Phone;
            txtId.Text = member.LoginId;

            txtPw.Clear();
            txtPwConfirm.Clear();
        }

        private bool ValidateInputs()
        {
            foreach (TextBox box in _editableBoxes)
            {
                box.BackColor = Color.White;
            }

            var errors = new List<string>();

            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (name.Length < 2)
            {
                txtName.BackColor = Color.LightCoral;
                errors.Add("이름은 최소 2글자 이상입니다.");
            }

            if (!Regex.IsMatch(phone, @"\A010-[0-9]{4}-[0-9]{4}\z"))
            {
                txtPhone.BackColor = Color.LightCoral;
                errors.Add("연락처 형식이 잘못되었습니다.");
            }

            string password = txtPw.Text;
            string confirmation = txtPwConfirm.Text;

            // 둘 다 비어 있으면 비밀번호를 변경하지 않습니다.
            bool changePassword =
                password.Length > 0 || confirmation.Length > 0;

            if (changePassword)
            {
                if (string.IsNullOrWhiteSpace(password) ||
                    password.Length < 4)
                {
                    txtPw.BackColor = Color.LightCoral;
                    errors.Add("비밀번호는 최소 4글자 이상입니다.");
                }

                if (!string.Equals(
                    password,
                    confirmation,
                    StringComparison.Ordinal))
                {
                    txtPw.BackColor = Color.LightCoral;
                    txtPwConfirm.BackColor = Color.LightCoral;

                    errors.Add("비밀번호가 틀립니다.");
                }
            }

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    this,
                    string.Join(Environment.NewLine, errors));

                return false;
            }

            return true;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (_isBusy || !ValidateInputs())
            {
                return;
            }

            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPw.Text;

            MemberUpdateResult result;

            SetBusy(true);

            try
            {
                result = await _repository.UpdateMember(
                    _memberNumber,
                    name,
                    phone,
                    password);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"회원 수정에 실패했습니다.\n" +
                    $"오류 번호: {ex.Number}\n{ex.Message}");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            if (result == MemberUpdateResult.DuplicatePhone)
            {
                txtPhone.BackColor = Color.LightCoral;

                MessageBox.Show(
                    this,
                    "다른 회원에게 등록된 연락처입니다.");

                return;
            }

            if (result == MemberUpdateResult.NotFound)
            {
                MessageBox.Show(
                    this,
                    "해당 회원이 없습니다. 목록을 새로고침해주세요.");

                return;
            }

            SavedName = name;

            MessageBox.Show(this, "회원 정보가 수정되었습니다.");

            DialogResult = DialogResult.OK;
        }

        private void BtnResetPassword_Click(
            object sender,
            EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            const string resetPassword = "1111";

            txtPw.Text = resetPassword;
            txtPwConfirm.Text = resetPassword;

            MessageBox.Show(
                this,
                "아직 DB에는 반영되지 않았습니다.\n" +
                "‘수정’ 버튼을 누르면 비밀번호가 초기화됩니다.");
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            foreach (TextBox box in _editableBoxes)
            {
                box.Enabled = !busy;
            }

            btnSave.Enabled = !busy;
            btnClose.Enabled = !busy;
            btnResetPassword.Enabled = !busy;

            UseWaitCursor = busy;
        }

        private void lTitle_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
