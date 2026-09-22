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



namespace Book_Management
{
    public partial class UserMainForm : Form
    {

        private enum UserPage
        {
            Search,
            Loans,
            Request
        }

        private static readonly string[] Categories =
        {
            "전체", "총류", "철학", "종교", "사회과학",
            "자연과학", "기술과학", "예술", "언어", "문학", "역사"
        };

        private readonly UserBookRepository _repository = new UserBookRepository();
        private readonly TextBox[] _requestInputs;
        private LoginMember _member;
        private UserPage _currentPage;
        private bool _isBusy;
        private Func<Task<DataTable>> _lastSearch;
        public bool LogoutRequested { get; private set; }


        public UserMainForm()
        {
            InitializeComponent();

            _requestInputs = new[]
            {
                txtRequestTitle,
                txtRequestAuthor,
                txtRequestPublisher,
                txtRequestYear,
                txtRequestIsbn
            };

            dgvBooks.AutoGenerateColumns = false;

            SetComboItems(cmbSearchType, new[] { "통합검색", "제목", "저자", "출판사" }, 0);

            SetComboItems(cmbCategory, Categories, 0);

            SetComboItems(cmbAvailability, new[] { "가능", "불가능" }, 0);

            SetComboItems(cmbRequestCategory, Categories.Skip(1).ToArray(), -1);

            btnSearch.Click += async (_, _) => await Search();
            btnLoans.Click += async (_, _) => await ShowLoans();

            btnRequest.Click += (_, _) =>
            {
                if (!_isBusy)
                {
                    ShowPage(UserPage.Request);
                }
            };

            btnRefreshLoans.Click += async (_, _) => await ShowLoans();
            btnReturn.Click += async (_, _) => await ReturnBook();
            btnSaveRequest.Click += async (_, _) => await SaveRequest();

            btnLogout.Click += (_, _) =>
            {
                if (_isBusy)
                {
                    return;
                }

                LogoutRequested = true;
                Close();
            };

            dgvBooks.CellMouseDoubleClick += DgvBooks_CellMouseDoubleClick;

            FormClosing += (_, e) =>
            {
                if (_isBusy)
                {
                    e.Cancel = true;
                }
            };
            // 최초 화면은 검색 화면이며, DB 조회는 하지 않습니다.
            ShowPage(UserPage.Search);
        }
        public UserMainForm(LoginMember member) : this()
        {
            if (member == null || member.MemberCode != "02")
            {
                throw new InvalidOperationException("일반사용자 계정이 아닙니다.");
            }

            _member = member;

            usermain.Text = $"회원명: {member.Name}({member.LoginId})";
        }

        private static void SetComboItems(ComboBox combo, string[] items, int selectedIndex)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Clear();
            combo.Items.AddRange(items);
            combo.SelectedIndex = selectedIndex;
        }

        private void ShowPage(UserPage page)
        {
            _currentPage = page;

            bool isSearch = page == UserPage.Search;
            bool isLoans = page == UserPage.Loans;
            bool isRequest = page == UserPage.Request;

            pnlList.Visible = !isRequest;
            pnlRequest.Visible = isRequest;

            pnlLoanActions.Visible = isLoans;
            colDueDate.Visible = isLoans;

            if (isRequest)
            {
                pnlRequest.BringToFront();
                lblPage.Text = "신규 도서 요청";
            }
            else
            {
                pnlList.BringToFront();
                lblPage.Text = isSearch ? "검색 결과" : "내 대출 목록";
            }

            // 이전 화면의 데이터가 다른 제목 아래 남지 않게 합니다.
            dgvBooks.DataSource = null;

            SetMenuColor(btnSearch, isSearch);
            SetMenuColor(btnLoans, isLoans);
            SetMenuColor(btnRequest, isRequest);

            AcceptButton = isSearch ? btnSearch : null;

            SetBusy(_isBusy);
        }

        private static void SetMenuColor(Button button, bool selected)
        {
            button.BackColor = selected ? Color.SteelBlue : Color.LightSteelBlue;

            button.ForeColor = selected ? Color.White : Color.Black;
        }

        private void BindBooks(DataTable table)
        {
            dgvBooks.DataSource = table;
            dgvBooks.ClearSelection();

            string title = _currentPage == UserPage.Loans ? "내 대출 목록" : "검색 결과";

            lblPage.Text = $"{title} ({table.Rows.Count}권)";
        }

        private async Task Search()
        {
            if (_isBusy || _member == null)
            {
                return;
            }

            ShowPage(UserPage.Search);
            _lastSearch = null;

            short? year = null;

            if (!string.IsNullOrWhiteSpace(txtYear.Text))
            {
                if (!short.TryParse(txtYear.Text.Trim(), out short parsed) ||
                    parsed < 1 || parsed > 9999)
                {
                    MessageBox.Show(
                        this,
                        "발행연도는 1~9999로 입력해주세요.\n" +
                        "비워두면 전체 연도로 검색합니다.");

                    txtYear.Focus();
                    return;
                }

                year = parsed;
            }

            int mode = cmbSearchType.SelectedIndex;
            string keyword = txtKeyword.Text.Trim();
            string category = cmbCategory.SelectedItem.ToString();
            bool available = cmbAvailability.SelectedIndex == 0;

            await Run(async () =>
            {
                DataTable table = await _repository.Search(
                    mode,
                    keyword,
                    year,
                    category,
                    available);

                // 상세 팝업을 닫은 후 동일한 조건으로 갱신합니다.
                _lastSearch = () => _repository.Search(
                    mode,
                    keyword,
                    year,
                    category,
                    available);

                BindBooks(table);
            });
        }

        private async void DgvBooks_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_isBusy ||
                _member == null ||
                _currentPage != UserPage.Search ||
                e.RowIndex < 0 ||
                e.ColumnIndex < 0 ||
                e.Button != MouseButtons.Left)
            {
                return;
            }

            var row = dgvBooks.Rows[e.RowIndex].DataBoundItem as DataRowView;

            if (row == null)
            {
                return;
            }

            int bookNumber = Convert.ToInt32(row["관리번호"]);

            if (bookNumber <= 0)
            {
                return;
            }

            await Run(async () =>
            {
                using var detail = new BookDetailForm(_member, bookNumber);

                detail.ShowDialog(this);

                if (_lastSearch != null)
                {
                    dgvBooks.DataSource = null;
                    lblPage.Text = "검색 결과";

                    BindBooks(await _lastSearch());
                }
            });
        }

        private async Task ShowLoans()
        {
            if (_isBusy || _member == null)
            {
                return;
            }

            ShowPage(UserPage.Loans);

            await Run(LoadLoans);
        }

        private async Task LoadLoans()
        {
            dgvBooks.DataSource = null;
            lblPage.Text = "내 대출 목록";

            DataTable table = await _repository.GetMyLoans(_member.LoginId);

            BindBooks(table);
        }

        private async Task ReturnBook()
        {
            if (_isBusy ||
                _member == null ||
                _currentPage != UserPage.Loans)
            {
                return;
            }

            if (dgvBooks.SelectedRows.Count != 1 || !(dgvBooks.SelectedRows[0].DataBoundItem is DataRowView row))
            {
                MessageBox.Show(this, "반납할 도서를 선택해주세요.");
                return;
            }

            int bookNumber = Convert.ToInt32(row["관리번호"]);
            string title = Convert.ToString(row["제목"]);

            if (bookNumber <= 0)
            {
                MessageBox.Show(this, "올바른 도서를 선택해주세요.");
                return;
            }

            DialogResult answer = MessageBox.Show(
                this,
                $"선택한 도서를 반납하시겠습니까?\n\n" +
                $"관리번호: {bookNumber}\n제목: {title}",
                "반납 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            await Run(async () =>
            {
                bool returned = await _repository.Return(bookNumber, _member.LoginId);

                MessageBox.Show(
                    this,
                    returned
                        ? "반납되었습니다."
                        : "이미 반납되었거나 본인의 대출 도서가 아닙니다.");

                await LoadLoans();
            });
        }

        private bool ValidateRequest(out BookData book)
        {
            book = null;

            if (_requestInputs.Any(box => string.IsNullOrWhiteSpace(box.Text)) || cmbRequestCategory.SelectedIndex < 0)
            {
                MessageBox.Show(
                    this,
                    "모든 입력값을 채우고 카테고리를 선택해주세요.");

                return false;
            }

            if (!short.TryParse(txtRequestYear.Text.Trim(), out short year) || year < 1 || year > 9999)
            {
                MessageBox.Show(this, "발행연도는 1~9999로 입력해주세요.");
                txtRequestYear.Focus();
                return false;
            }

            string isbn = txtRequestIsbn.Text.Trim().ToUpperInvariant();

            if (!Regex.IsMatch(
                isbn,
                @"\A(?:[0-9]{13}|[0-9]{9}[0-9X])\z"))
            {
                MessageBox.Show(
                    this,
                    "ISBN은 하이픈 없이 10자리 또는 13자리로 입력해주세요.");

                txtRequestIsbn.Focus();
                return false;
            }

            book = new BookData
            {
                Title = txtRequestTitle.Text.Trim(),
                Author = txtRequestAuthor.Text.Trim(),
                Publisher = txtRequestPublisher.Text.Trim(),
                PublicationYear = year,
                Category = cmbRequestCategory.SelectedItem.ToString(),
                Isbn = isbn
            };

            return true;
        }

        private async Task SaveRequest()
        {
            if (_isBusy ||
                _member == null ||
                _currentPage != UserPage.Request)
            {
                return;
            }

            if (!ValidateRequest(out BookData book))
            {
                return;
            }

            await Run(async () =>
            {
                try
                {
                    await _repository.Request(book);
                }
                catch (SqlException ex)
                    when (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show(
                        this,
                        "같은 ISBN의 신규 도서 요청이 이미 등록되어 있습니다.");

                    return;
                }

                MessageBox.Show(this, "신규 도서 요청이 등록되었습니다.");

                foreach (TextBox input in _requestInputs)
                {
                    input.Clear();
                }

                cmbRequestCategory.SelectedIndex = -1;
            });
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
                    $"DB 작업에 실패했습니다.\n" +
                    $"오류 번호: {ex.Number}\n다시 시도해주세요.");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            btnSearch.Enabled = !busy;
            btnLoans.Enabled = !busy;
            btnRequest.Enabled = !busy;
            btnLogout.Enabled = !busy;

            pnlFilters.Enabled = !busy && _currentPage == UserPage.Search;

            pnlContent.Enabled = !busy;

            UseWaitCursor = busy;
        }


        private void pnlFilters_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlList_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
