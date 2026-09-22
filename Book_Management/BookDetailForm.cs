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
    public partial class BookDetailForm : Form
    {
        private readonly UserBookRepository _repository = new UserBookRepository();
        private LoginMember _member;
        private int _bookNumber;
        private bool _isBusy;
        private bool _canBorrow;


        public BookDetailForm()
        {
            InitializeComponent();

            btnBorrow.Enabled = false;
            btnBorrow.Click += BtnBorrow_Click;
            btnClose.Click += (_, _) => Close();

            Shown += async (_, _) =>
            {
                if (_member != null && _bookNumber > 0)
                {
                    await Run(() => LoadDetail(true));
                }
            };

            FormClosing += (_, e) =>
            {
                if (_isBusy)
                {
                    e.Cancel = true;
                }
            };

        }

        public BookDetailForm(LoginMember member, int bookNumber) : this()
        {
            _member = member ?? throw new ArgumentNullException(nameof(member));

            if (bookNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bookNumber));
            }

            _bookNumber = bookNumber;
        }

        private async Task LoadDetail(bool increaseViews)
        {
            _canBorrow = false;

            DataRow row = await _repository.GetDetail(
                _bookNumber,
                increaseViews);

            if (row == null)
            {
                txtTitle.Clear();
                txtAuthor.Clear();
                txtPublisher.Clear();
                txtYear.Clear();
                txtCategory.Clear();
                txtLoanStatus.Clear();
                txtDueDate.Clear();

                MessageBox.Show(this, "해당 도서가 존재하지 않습니다.");
                return;
            }

            txtTitle.Text = Convert.ToString(row["제목"]);
            txtAuthor.Text = Convert.ToString(row["저자"]);
            txtPublisher.Text = Convert.ToString(row["출판사"]);
            txtYear.Text = Convert.ToString(row["발행연도"]);
            txtCategory.Text = Convert.ToString(row["카테고리"]);
            txtLoanStatus.Text = Convert.ToString(row["대출여부"]);

            txtDueDate.Text = row.IsNull("반납일") ? "" : Convert.ToDateTime(row["반납일"]).ToString("yyyy-MM-dd");

            _canBorrow = Convert.ToBoolean(row["대출가능여부"]);
        }

        private async void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (_isBusy || !_canBorrow || _member == null)
            {
                return;
            }

            bool succeeded = false;

            await Run(async () =>
            {
                DateTime? dueDate = await _repository.Borrow(
                    _bookNumber,
                    _member.LoginId);

                if (!dueDate.HasValue)
                {
                    MessageBox.Show(
                        this,
                        "대출할 수 없습니다.\n" +
                        "다른 사용자가 대출했거나 도서·회원 상태가 변경되었습니다.");

                    await LoadDetail(false);
                    return;
                }

                succeeded = true;
                _canBorrow = false;

                MessageBox.Show(
                    this,
                    $"대출되었습니다.\n반납 예정일: {dueDate.Value:yyyy-MM-dd}");
            });

            if (succeeded)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        private async Task Run(Func<Task> action)
        {
            if (_isBusy)
            {
                return;
            }

            SetBusy(true);

            try
            {
                await action();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"도서 처리에 실패했습니다.\n오류 번호: {ex.Number}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            btnBorrow.Enabled = !busy && _canBorrow;
            btnClose.Enabled = !busy;

            UseWaitCursor = busy;
        }
        private void BookDetailForm_Load(object sender, EventArgs e)
        {

        }
    }
}