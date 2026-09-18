using Book_Management.Book;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Book_Management
{
    public partial class AdminMainForm : Form
    {
        private readonly BookRepository _repository = new BookRepository();

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
            cmbSearchColumn.Items.AddRange(
                BookRepository.GetSearchColumns(page));

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

            string column =
                cmbSearchColumn.SelectedItem.ToString();

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
                MessageBox.Show(
                    this,
                    $"목록 조회에 실패했습니다.\n오류 번호: {ex.Number}\n{ex.Message}");
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
            if (_isBusy || _currentPage != AdminPage.Books)
            {
                return;
            }

            // 헤더를 누르거나 왼쪽 버튼이 아니면 처리하지 않습니다.
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Button != MouseButtons.Left)
            {
                return;
            }

            var clickedRow = dgvBookList.Rows[e.RowIndex];

            if (!(clickedRow.DataBoundItem is DataRowView rowView))
            {
                return;
            }

            DataRow row = rowView.Row;

            var book = new BookData
            {
                BookNumber = Convert.ToInt32(row["관리번호"]),
                Title = Convert.ToString(row["제목"]),
                Author = Convert.ToString(row["저자"]),
                Publisher = Convert.ToString(row["출판사"]),

                PublicationYear = row.IsNull("발행연도") ? (short)0 : Convert.ToInt16(row["발행연도"]),

                Category = Convert.ToString(row["카테고리"]),
                Isbn = Convert.ToString(row["ISBN"])
            };

            using var form = new BookModifyForm(book);

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                await LoadListAsync();
            }
        }

        private void SetBusy(bool busy)
        {
            _isBusy = busy;

            btnBooks.Enabled = !busy;
            btnMembers.Enabled = !busy;
            btnRequestBook.Enabled = !busy;

            cmbSearchColumn.Enabled = !busy;
            txtSearch.Enabled = !busy;
            btnSearch.Enabled = !busy;
            btnRefresh.Enabled = !busy;

            dgvBookList.Enabled = !busy;

            // 도서 목록과 요청 현황에서 등록 가능
            btnAddBook.Enabled =
                !busy && _currentPage != AdminPage.Members;

            // 실제 보유 도서 목록에서만 삭제 가능
            btnDeleteBook.Enabled =
                !busy && _currentPage == AdminPage.Books;

            UseWaitCursor = busy;
        }

        public AdminMainForm(LoginMember member) : this()
        {
            if (member.MemberCode != "01")
            {
                throw new InvalidOperationException("관리자 계정이 아닙니다.");
            }

            adminmain.Text = $"회원명: {member.Name}({member.LoginId})";
        }
    }
}
