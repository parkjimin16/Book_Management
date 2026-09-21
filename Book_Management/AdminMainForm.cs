using Book_Management.Book;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Book_Management.MemberData;



namespace Book_Management
{
    public partial class AdminMainForm : Form
    {
        private readonly BookRepository _repository = new BookRepository();
        private readonly MemberRepository _memberRepository = new MemberRepository();
        private LoginMember _loginMember;
        private AdminPage _currentPage = AdminPage.Books;
        private bool _isBusy;

        public AdminMainForm()
        {
            InitializeComponent();

            //조회 결과의 컬럼을 표에 자동으로 연결
            dgvBookList.AutoGenerateColumns = true;

            Shown += async (_, _) =>
            {
                await ChangePage(AdminPage.Books);
            };
            btnBooks.Click += async (_, _) =>
            {
                await ChangePage(AdminPage.Books);
            };

            btnMembers.Click += async (_, _) =>
            {
                await ChangePage(AdminPage.Members);
            };

            btnRequestBook.Click += async (_, _) =>
            {
                await ChangePage(AdminPage.Requests);
            };

            btnSearch.Click += async (_, _) =>
            {
                await LoadListAsync();
            };

            btnRefresh.Click += async (_, _) =>
            {
                await LoadListAsync();
            };

            txtSearch.KeyDown += TxtSearch_KeyDown;

            btnAddBook.Click += BtnAddBook_Click;
            btnDeleteBook.Click += BtnDeleteBook_Click;

            btnAddMember.Click += BtnAddMember_Click;
            btnDeleteMember.Click += BtnDeleteMember_Click;


            dgvBookList.CellMouseDoubleClick += DgvBookList_CellMouseDoubleClick;

            FormClosing += (_, e) =>
            {
                if (_isBusy)
                {
                    e.Cancel = true;
                }
            };
        }

        // 도서 / 회원 / 요청 현황 전환
        private async Task ChangePage(AdminPage page)
        {
            if (_isBusy)
            {
                return;
            }

            _currentPage = page;

            txtSearch.Clear();

            cmbSearchColumn.Items.Clear();
            cmbSearchColumn.Items.AddRange(BookRepository.GetSearchColumns(page));

            cmbSearchColumn.SelectedIndex = 0;

            switch (page)
            {
                case AdminPage.Books:
                    lPage.Text = "도서 관리";
                    break;

                case AdminPage.Members:
                    lPage.Text = "회원 관리";
                    break;

                case AdminPage.Requests:
                    lPage.Text = "신규 도서 요청 현황";
                    break;
            }

            await LoadListAsync();
        }

        // 현재 화면 목록 조회
        private async Task LoadListAsync()
        {
            if (_isBusy)
            {
                return;
            }

            if (cmbSearchColumn.SelectedItem == null)
            {
                return;
            }

            string column = cmbSearchColumn.SelectedItem.ToString();

            string keyword = txtSearch.Text.Trim();

            SetBusy(true);

            // 조회 실패 시 이전 화면 데이터가 남지 않도록 비웁니다.
            dgvBookList.DataSource = null;

            try
            {
                DataTable table = await _repository.GetList(_currentPage, column, keyword);
                dgvBookList.DataSource = table;
                dgvBookList.ClearSelection();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(this, $"목록 조회에 실패했습니다.\n오류 번호: {ex.Number}\n{ex.Message}");
            }
            finally
            {
                SetBusy(false);
            }
        }

        // 검색창에서 Enter로 검색
        private async void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;

            await LoadListAsync();
        }

        // 새 도서 등록 팝업
        private async void BtnAddBook_Click(object sender, EventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            using var form = new BookModifyForm();

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // 등록 후 전체 도서 목록으로 이동
                await ChangePage(AdminPage.Books);
            }
        }

        // 선택한 도서 삭제
        private async void BtnDeleteBook_Click(
            object sender,
            EventArgs e)
        {
            if (_isBusy || _currentPage != AdminPage.Books)
            {
                return;
            }

            if (dgvBookList.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "삭제할 도서를 선택해주세요.");
                return;
            }

            var selected = dgvBookList.SelectedRows[0];

            if (!(selected.DataBoundItem is DataRowView rowView))
            {
                return;
            }

            int bookNumber = Convert.ToInt32(rowView.Row["관리번호"]);

            string title = Convert.ToString(rowView.Row["제목"]);

            DialogResult answer = MessageBox.Show(
                this,
                $"선택한 도서를 삭제하겠습니까?\n\n" +
                $"관리번호: {bookNumber}\n제목: {title}",
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
                affected = await _repository.Delete(bookNumber);
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

            MessageBox.Show(
                this,
                affected > 0
                    ? "삭제되었습니다."
                    : "이미 삭제된 도서입니다.");

            await LoadListAsync();
        }

        // 도서 더블클릭 → 수정 팝업
        private async void DgvBookList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0 ||
                e.Button != MouseButtons.Left)
            {
                return;
            }

            var clickedRow = dgvBookList.Rows[e.RowIndex];

            if (!(clickedRow.DataBoundItem is DataRowView rowView))
            {
                return;
            }

            DataRow row = rowView.Row;

            // 회원 수정
            if (_currentPage == AdminPage.Members)
            {
                var member = new MemberData
                {
                    MemberNumber = Convert.ToInt32(row["회원번호"]),
                    Name = Convert.ToString(row["이름"]),
                    Phone = Convert.ToString(row["연락처"]),
                    LoginId = Convert.ToString(row["아이디"])
                };

                using var memberForm = new MemberEditForm(member);

                if (memberForm.ShowDialog(this) == DialogResult.OK)
                {
                    // 본인 이름을 수정했으면 상단 표시도 변경
                    if (_loginMember != null &&
                        _loginMember.MemberNumber == member.MemberNumber)
                    {
                        _loginMember.Name = memberForm.SavedName;

                        adminmain.Text =
                            $"회원명: {_loginMember.Name}" +
                            $"({_loginMember.LoginId})";
                    }

                    await LoadListAsync();
                }

                return;
            }

            // 도서 관리 화면에서만 도서 수정
            if (_currentPage != AdminPage.Books)
            {
                return;
            }

            var book = new BookData
            {
                BookNumber = Convert.ToInt32(row["관리번호"]),
                Title = Convert.ToString(row["제목"]),
                Author = Convert.ToString(row["저자"]),
                Publisher = Convert.ToString(row["출판사"]),

                PublicationYear = row.IsNull("발행연도")
                    ? (short)0
                    : Convert.ToInt16(row["발행연도"]),

                Category = Convert.ToString(row["카테고리"]),
                Isbn = Convert.ToString(row["ISBN"])
            };

            using var bookForm = new BookModifyForm(book);

            if (bookForm.ShowDialog(this) == DialogResult.OK)
            {
                await LoadListAsync();
            }
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            bool isMemberPage =
                _currentPage == AdminPage.Members;

            btnBooks.Enabled = !busy;
            btnMembers.Enabled = !busy;

            cmbSearchColumn.Enabled = !busy;
            txtSearch.Enabled = !busy;
            btnSearch.Enabled = !busy;
            btnRefresh.Enabled = !busy;

            dgvBookList.Enabled = !busy;

            // 화면에 따라 버튼 표시 전환
            btnAddBook.Visible = !isMemberPage;
            btnDeleteBook.Visible = !isMemberPage;
            btnRequestBook.Visible = !isMemberPage;

            btnAddMember.Visible = isMemberPage;
            btnDeleteMember.Visible = isMemberPage;

            // 도서 관련 버튼
            btnAddBook.Enabled = !busy && !isMemberPage;
            btnRequestBook.Enabled = !busy && !isMemberPage;

            btnDeleteBook.Enabled =
                !busy && _currentPage == AdminPage.Books;

            // 회원 관련 버튼
            btnAddMember.Enabled = !busy && isMemberPage;
            btnDeleteMember.Enabled = !busy && isMemberPage;

            UseWaitCursor = busy;
        }

        // 신규 회원 등록
        private async void BtnAddMember_Click(
            object sender,
            EventArgs e)
        {
            if (_isBusy || _currentPage != AdminPage.Members)
            {
                return;
            }

            using var form = new RegisterForm(true);

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // 검색어도 비우고 전체 회원 목록 표시
                await ChangePage(AdminPage.Members);
            }
        }

        // 선택한 회원 삭제
        private async void BtnDeleteMember_Click(
            object sender,
            EventArgs e)
        {
            if (_isBusy || _currentPage != AdminPage.Members)
            {
                return;
            }

            if (dgvBookList.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "삭제할 회원을 선택해주세요.");
                return;
            }

            var selected = dgvBookList.SelectedRows[0];

            if (!(selected.DataBoundItem is DataRowView rowView))
            {
                return;
            }

            int memberNumber =
                Convert.ToInt32(rowView.Row["회원번호"]);

            string name =
                Convert.ToString(rowView.Row["이름"]);

            bool isCurrentMember =
                _loginMember != null &&
                _loginMember.MemberNumber == memberNumber;

            string message =
                $"선택한 회원을 삭제하겠습니까?\n\n" +
                $"회원번호: {memberNumber}\n이름: {name}";

            if (isCurrentMember)
            {
                message +=
                    "\n\n현재 로그인한 계정입니다.\n" +
                    "삭제하면 프로그램이 종료됩니다.";
            }

            DialogResult answer = MessageBox.Show(
                this,
                message,
                "회원 삭제",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            MemberDeleteResult result;

            SetBusy(true);

            try
            {
                result = await _memberRepository.DeleteMember(
                    memberNumber);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    this,
                    "연결된 대출 정보 등이 있어 삭제할 수 없습니다.\n" +
                    "관련 정보를 확인하고 반납을 먼저 처리해주세요.");

                return;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    this,
                    $"회원 삭제에 실패했습니다.\n" +
                    $"오류 번호: {ex.Number}\n{ex.Message}");

                return;
            }
            finally
            {
                SetBusy(false);
            }

            if (result == MemberDeleteResult.HasLoans)
            {
                MessageBox.Show(
                    this,
                    "대출 중인 도서가 있습니다.\n" +
                    "반납 처리 후 회원을 삭제해주세요.");

                await LoadListAsync();
                return;
            }

            MessageBox.Show(
                this,
                result == MemberDeleteResult.Success
                    ? "회원이 삭제되었습니다."
                    : "이미 삭제된 회원입니다.");

            if (isCurrentMember)
            {
                Close();
                return;
            }

            await LoadListAsync();
        }

        public AdminMainForm(LoginMember member) : this()
        {
            if (member.MemberCode != "01")
            {
                throw new InvalidOperationException("관리자 계정이 아닙니다.");
            }

            _loginMember = member;

            adminmain.Text = $"회원명: {member.Name}({member.LoginId})";
        }
    }
}
