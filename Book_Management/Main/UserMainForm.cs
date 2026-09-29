using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;



namespace Book_Management
{
    public partial class UserMainForm : Form
    {

        private enum UserPage
        {
            Search,
            Loans,
        }

        private static readonly string[] Categories =
        {
            "전체", "총류", "철학", "종교", "사회과학",
            "자연과학", "기술과학", "예술", "언어", "문학", "역사"
        };

        private readonly UserBookRepository _repository = new UserBookRepository();

        private LoginMember _member;
        private UserPage _currentPage;
        private bool _isBusy;
        private Func<Task<DataTable>> _lastSearch;
        public bool LogoutRequested { get; private set; }


        public UserMainForm()
        {
            InitializeComponent();

            dgvBooks.AutoGenerateColumns = false;

            SetComboItems(cmbSearchType, new[] { "통합검색", "제목", "저자", "출판사" }, 0);

            SetComboItems(cmbCategory, Categories, 0);

            SetComboItems(cmbAvailability, new[] { "가능", "불가능" }, 0);


            btnSearch.Click += async (_, _) => await Search();
            btnLoans.Click += async (_, _) => await ShowLoans();

            btnRequest.Click += BtnRequest_Click;

            btnRefreshLoans.Click += async (_, _) => await ShowLoans();
            btnReturn.Click += async (_, _) => await ReturnBook();

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
            // 최초 화면을 검색 화면으로 설정합니다.
            ShowPage(UserPage.Search);

            // 로그인 후 사용자 폼이 처음 표시되면 자동으로 검색합니다.
            Shown += async (_, _) =>
            {
                await Search();
            };
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

            pnlList.Visible = true;
            pnlList.BringToFront();

            pnlLoanActions.Visible = isLoans;
            colDueDate.Visible = isLoans;

            lblPage.Text = isSearch ? "검색 결과" : "내 대출 목록";

            dgvBooks.DataSource = null;

            SetMenuColor(btnSearch, isSearch);
            SetMenuColor(btnLoans, isLoans);
            SetMenuColor(btnRequest, false);

            AcceptButton = isSearch ? btnSearch : null;

            SetBusy(_isBusy);
        }

        private static void SetMenuColor(Button button, bool selected)
        {
            button.BackColor = selected ? Color.SteelBlue : Color.LightSteelBlue;

            button.ForeColor = selected ? Color.White : Color.Black;
        }

        private void DisplayBooks(DataTable table)
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
                if (!short.TryParse(txtYear.Text.Trim(), out short parsed) || parsed < 1 || parsed > 9999)
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

            Func<Task<DataTable>> search = () => _repository.Search(mode, keyword, year, category, available);

            await Run(async () =>
            {
                DataTable table = await search();

                // 조회에 성공한 조건을 팝업 종료 후 재사용합니다.
                _lastSearch = search;

                DisplayBooks(table);
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

                    DisplayBooks(await _lastSearch());
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

            DisplayBooks(table);
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

        private void BtnRequest_Click(object sender, EventArgs e)
        {
            if (_isBusy || _member == null)
            {
                return;
            }

            SetBusy(true);

            try
            {
                using var form = new BookModifyForm(isRequestMode: true);

                form.ShowDialog(this);
            }
            finally
            {
                SetBusy(false);
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
    }
}