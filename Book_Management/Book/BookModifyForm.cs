using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using Book_Management.Book;
using System.Threading.Tasks;

namespace Book_Management
{
    public partial class BookModifyForm : Form
    {

        private readonly AdminBookRepository _repository = new AdminBookRepository();
        private readonly UserBookRepository _requestRepository = new UserBookRepository();

        private readonly TextBox[] _inputs;
        // null이면 등록, 값이 있으면 해당 관리번호의 도서 수정
        private int? _bookNumber;
        private bool _isBusy;
        private bool _isRequestMode;

        // 값이 있으면 관리자가 요청 도서를 정식 등록하는 화면입니다.
        private string _requestIsbn;
        private bool IsRequestApprovalMode => _requestIsbn != null;

        public BookModifyForm()
        {
            InitializeComponent();

            cmbCategory.Items.Remove("전체");

            txtTitle.MaxLength = 200;
            txtAuthor.MaxLength = 100;
            txtPublisher.MaxLength = 100;
            txtYear.MaxLength = 4;
            txtIsbn.MaxLength = 13;

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

        public BookModifyForm(bool isRequestMode) : this()
        {
            _isRequestMode = isRequestMode;

            if (_isRequestMode)
            {
                Text = "신규 도서 요청";
                lTitle.Text = "신규 도서 요청";
                btnSave.Text = "요청 등록";
                btnDelete.Visible = false;
            }
        }

        // 관리자: 신규 도서 요청을 확인하고 정식 도서로 등록
        public BookModifyForm(DataRow requestRow) : this()
        {
            if (requestRow == null)
            {
                throw new ArgumentNullException(nameof(requestRow));
            }

            _requestIsbn = Convert.ToString(requestRow["ISBN"]);

            if (string.IsNullOrWhiteSpace(_requestIsbn))
            {
                throw new ArgumentException("요청 도서의 ISBN이 없습니다.");
            }

            Text = "신규 도서 등록";
            lTitle.Text = "신규 도서 등록";
            btnSave.Text = "등록";
            btnDelete.Visible = false;

            StartPosition = FormStartPosition.CenterParent;

            txtTitle.Text = Convert.ToString(requestRow["제목"]);
            txtAuthor.Text = Convert.ToString(requestRow["저자"]);
            txtPublisher.Text = Convert.ToString(requestRow["출판사"]);
            txtYear.Text = Convert.ToString(requestRow["발행연도"]);
            txtIsbn.Text = _requestIsbn;

            // 저장된 카테고리를 그대로 표시하고 선택 변경은 막습니다.
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCategory.SelectedIndex = -1;
            cmbCategory.Text = Convert.ToString(requestRow["카테고리"]);

            SetBusy(false);
        }

        private bool ValidateInputs(out short year)
        {
            year = 0;

            foreach (TextBox box in _inputs)
            {
                box.BackColor = Color.White;
            }

            cmbCategory.BackColor = Color.White;

            TextBox[] emptyBoxes = _inputs
                .Where(box => string.IsNullOrWhiteSpace(box.Text))
                .ToArray();

            bool categoryNotSelected = cmbCategory.SelectedIndex == -1;

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

                MessageBox.Show(this, "입력하지 않은 칸이 있습니다.");

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

            // 입력한 발행연도를 숫자로 변환하여 year에 저장합니다.
            if (!short.TryParse(txtYear.Text.Trim(), out year) ||
                year < 1 || year > 9999)
            {
                txtYear.BackColor = Color.LightCoral;

                MessageBox.Show(
                    this,
                    "발행연도는 1~9999 사이의 숫자로 입력해주세요.");

                txtYear.Focus();
                return false;
            }

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

            // 요청 승인 화면은 입력 검증 대신 저장된 요청을 그대로 등록합니다.
            if (IsRequestApprovalMode)
            {
                await RegisterRequestedBook();
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
                if (!_isRequestMode && await _repository.IsIsbnExists(book.Isbn, _bookNumber))
                {
                    txtIsbn.BackColor = Color.LightCoral;

                    MessageBox.Show(
                        this,
                        "같은 ISBN의 도서가 이미 등록되어 있습니다.");

                    return;
                }

                if (_isRequestMode)
                {
                    await _requestRepository.Request(book);
                }
                else if (isEdit)
                {
                    int affected = await _repository.Update(book);

                    if (affected == 0)
                    {
                        MessageBox.Show(
                            this,
                            "대출 중이거나 이미 삭제된 도서여서 수정할 수 없습니다.\n" +
                            "목록을 새로고침해주세요.");

                        return;
                    }
                }
                else
                {
                    await _repository.Add(book);
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                txtIsbn.BackColor = Color.LightCoral;

                MessageBox.Show(
                    this,
                    _isRequestMode
                        ? "같은 ISBN의 신규 도서 요청이 이미 등록되어 있습니다."
                        : "같은 ISBN의 도서가 이미 등록되어 있습니다.");

                return;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"저장에 실패했습니다.\n오류 번호: {ex.Number}");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            string message = _isRequestMode
                ? "신규 도서 요청이 등록되었습니다."
                : isEdit
                    ? "수정되었습니다."
                    : "등록되었습니다.";

            MessageBox.Show(this, message);

            DialogResult = DialogResult.OK;
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_isBusy || _isRequestMode || !_bookNumber.HasValue)
            {
                return;
            }

            DialogResult answer = MessageBox.Show(
                this,
                $"관리번호 {_bookNumber.Value} 도서를 삭제하겠습니까?",
                "도서 삭제",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            int affected;

            SetBusy(true);

            try
            {
                affected = await _repository.Delete(_bookNumber.Value);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"삭제에 실패했습니다.\n오류 번호: {ex.Number}\n{ex.Message}");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            if (affected == 0)
            {
                MessageBox.Show(
                    this,
                    "대출 중이거나 이미 삭제된 도서여서 삭제할 수 없습니다.\n" +
                    "목록을 새로고침해주세요.");

                return;
            }

            MessageBox.Show(this, "삭제되었습니다.");

            DialogResult = DialogResult.OK;
        }

        private async Task RegisterRequestedBook()
        {
            bool registered;

            SetBusy(true);

            try
            {
                if (await _repository.IsIsbnExists(_requestIsbn))
                {
                    MessageBox.Show(
                        this,
                        "같은 ISBN의 도서가 이미 등록되어 있습니다.\n" +
                        "신규 도서 요청은 삭제하지 않았습니다.");

                    return;
                }

                registered =
                    await _repository.RegisterRequestedBookAsync(_requestIsbn);
            }
            catch (SqlException ex)
                when (ex.Number == 2601 || ex.Number == 2627)
            {
                MessageBox.Show(
                    this,
                    "같은 ISBN의 도서가 이미 등록되어 있습니다.\n" +
                    "신규 도서 요청은 삭제하지 않았습니다.");

                return;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"요청 도서 등록에 실패했습니다.\n오류 번호: {ex.Number}");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            if (!registered)
            {
                MessageBox.Show(
                    this,
                    "이미 처리되었거나 삭제된 요청입니다.\n목록을 다시 불러옵니다.");

                DialogResult = DialogResult.OK;
                return;
            }

            MessageBox.Show(
                this,
                "도서가 등록되었으며 신규 도서 요청 목록에서 삭제되었습니다.");

            DialogResult = DialogResult.OK;
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            foreach (TextBox box in _inputs)
            {
                box.Enabled = !busy;
                box.ReadOnly = IsRequestApprovalMode;
            }

            cmbCategory.Enabled = !busy && !IsRequestApprovalMode;

            btnSave.Enabled = !busy;
            btnClose.Enabled = !busy;

            btnDelete.Enabled =
                !busy &&
                !_isRequestMode &&
                !IsRequestApprovalMode &&
                _bookNumber.HasValue;

            UseWaitCursor = busy;
        }
    }
}
