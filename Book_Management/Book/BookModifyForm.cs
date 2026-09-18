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
using System.Text.RegularExpressions;
using Book_Management.Book;


namespace Book_Management
{
    public partial class BookModifyForm : Form
    {

        private readonly BookRepository _repository = new BookRepository();
        private readonly TextBox[] _inputs;
        // null이면 등록, 값이 있으면 해당 관리번호의 도서 수정
        private int? _bookNumber;
        private bool _isBusy;

        public BookModifyForm()
        {
            InitializeComponent();

            cmbCategory.SelectedIndex = -1;
            // 카테고리를 선택하면 오류 색상 복원
            cmbCategory.SelectedIndexChanged += (_, _) =>
            {
                cmbCategory.BackColor = Color.White;
            };

            _inputs = new[]
            {
                txtTitle,
                txtAuthor,
                txtPublisher,
                txtYear,
                txtIsbn
            };

            Text = "도서 등록";
            lTitle.Text = "도서 등록";
            btnSave.Text = "등록";
            btnDelete.Visible = false;

            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;

            FormClosing += (_, e) =>
            {
                if (_isBusy)
                {
                    e.Cancel = true;
                }
            };
        }

        // 수정용 생성자
        public BookModifyForm(BookData book) : this()
        {
            _bookNumber = book.BookNumber;

            Text = "도서 수정";
            lTitle.Text = "도서 수정";
            btnSave.Text = "수정";
            btnDelete.Visible = true;

            txtTitle.Text = book.Title;
            txtAuthor.Text = book.Author;
            txtPublisher.Text = book.Publisher;

            txtYear.Text = book.PublicationYear == 0 ? "" : book.PublicationYear.ToString();

            cmbCategory.SelectedItem = book.Category;
            txtIsbn.Text = book.Isbn;
        }

        private bool ValidateInputs(out short year)
        {
            year = 0;

            foreach (TextBox box in _inputs)
            {
                box.BackColor = Color.White;
            }

            TextBox[] emptyBoxes = _inputs.Where(box => string.IsNullOrWhiteSpace(box.Text)).ToArray();

            bool categoryNotSelected = cmbCategory.SelectedIndex == -1; //-1은 값이 없음

            if (emptyBoxes.Length > 0 || categoryNotSelected)
            {
                foreach (TextBox box in emptyBoxes)
                {
                    box.BackColor = Color.LightCoral;
                }

                if (categoryNotSelected)
                {
                    cmbCategory.BackColor = Color.LightCoral;
                }

                MessageBox.Show(this, "입력하지 않은 칸이 있습니다");

                if (emptyBoxes.Length > 0)
                {
                    emptyBoxes[0].Focus();
                }
                else
                {
                    cmbCategory.Focus();
                }

                return false;
            }

            if (emptyBoxes.Length > 0)
            {
                foreach (TextBox box in emptyBoxes)
                {
                    box.BackColor = Color.LightCoral;
                }

                MessageBox.Show(this, "입력하지 않은 칸이 있습니다");
                emptyBoxes[0].Focus();

                return false;
            }

            if (!short.TryParse(txtYear.Text.Trim(), out year) ||
                year < 1 || year > 9999)
            {
                txtYear.BackColor = Color.LightCoral;

                MessageBox.Show(
                    this,
                    "발행 연도는 1~9999 사이의 숫자로 입력해주세요.");

                txtYear.Focus();
                return false;
            }

            // 하이픈 없는 ISBN-10 또는 ISBN-13 형태 확인
            string isbn = txtIsbn.Text.Trim();

            if (!Regex.IsMatch(
                isbn,
                @"\A(?:[0-9]{13}|[0-9]{9}[0-9Xx])\z"))
            {
                txtIsbn.BackColor = Color.LightCoral;

                MessageBox.Show(
                    this,
                    "ISBN은 하이픈 없이 10자리 또는 13자리로 입력해주세요.");

                txtIsbn.Focus();
                return false;
            }

            return true;
        }


        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            if (!ValidateInputs(out short year))
            {
                return;
            }

            var book = new BookData
            {
                BookNumber = _bookNumber ?? 0,
                Title = txtTitle.Text.Trim(),
                Author = txtAuthor.Text.Trim(),
                Publisher = txtPublisher.Text.Trim(),
                PublicationYear = year,
                Category = cmbCategory.SelectedItem.ToString(),
                Isbn = txtIsbn.Text.Trim().ToUpperInvariant()
            };

            bool isEdit = _bookNumber.HasValue;

            SetBusy(true);

            try
            {
                if (isEdit)
                {
                    int affected = await _repository.Update(book);

                    if (affected == 0)
                    {
                        MessageBox.Show(
                            this,
                            "수정할 도서가 없습니다. 목록을 새로고침해주세요.");

                        return;
                    }
                }
                else
                {
                    await _repository.Add(book);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"저장에 실패했습니다.\n오류 번호: {ex.Number}\n{ex.Message}");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            MessageBox.Show(
                this,
                isEdit ? "수정되었습니다." : "등록되었습니다.");

            // 부모 화면에서 목록을 다시 조회하도록 알립니다.
            DialogResult = DialogResult.OK;


        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            foreach (TextBox box in _inputs)
            {
                box.Enabled = !busy;
            }
            cmbCategory.Enabled = !busy;

            btnSave.Enabled = !busy;
            btnClose.Enabled = !busy;
            btnDelete.Enabled = !busy && _bookNumber.HasValue;

            UseWaitCursor = busy;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
