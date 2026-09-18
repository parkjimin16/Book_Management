using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Book_Management.Book
{
    public enum AdminPage
    {
        Books,
        Members,
        Requests
    }

    internal class BookRepository
    {

        // 화면별 검색 가능한 컬럼
        public static string[] GetSearchColumns(AdminPage page)
        {
            if (page == AdminPage.Members)
            {
                return new[]
                {
                    "이름", "아이디", "연락처",
                    "회원번호", "회원코드"
                };
            }

            if (page == AdminPage.Requests)
            {
                return new[]
                {
                    "제목", "저자", "출판사",
                    "발행연도", "카테고리", "ISBN"
                };
            }

            return new[]
            {
                "제목", "저자", "출판사",
                "발행연도", "카테고리", "ISBN", "관리번호"
            };
        }

        // 도서 / 회원 / 신규 도서 요청 목록 조회
        public async Task<DataTable> GetList(AdminPage page, string searchColumn, string keyword)
        {
            string sql;
            string orderColumn;

            switch (page)
            {
                case AdminPage.Books:
                    sql = @"
                        SELECT
                            [관리번호], [제목], [저자], [출판사],
                            [발행연도], [카테고리], [ISBN],
                            [대출가능여부], [대출자]
                        FROM dbo.[도서]";

                    orderColumn = "관리번호";
                    break;

                case AdminPage.Members:
                    sql = @"
                        SELECT
                            [회원번호], [이름], [연락처],
                            [아이디], [회원코드]
                        FROM dbo.[회원]";

                    orderColumn = "회원번호";
                    break;

                case AdminPage.Requests:
                    sql = @"
                        SELECT
                            [제목], [저자], [출판사],
                            [발행연도], [카테고리], [ISBN]
                        FROM dbo.[신규도서]";

                    orderColumn = "ISBN";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(page));
            }

            // 허용된 컬럼만 SQL에 사용할 수 있습니다.
            if (!GetSearchColumns(page).Contains(searchColumn))
            {
                throw new ArgumentException("잘못된 검색 항목입니다.");
            }

            bool useSearch = !string.IsNullOrWhiteSpace(keyword);

            if (useSearch)
            {
                sql += $@"
                    WHERE CONVERT(nvarchar(4000), [{searchColumn}])
                    LIKE @Keyword ESCAPE N'~'";
            }

            sql += $" ORDER BY [{orderColumn}];";

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            if (useSearch)
            {
                // %, _, [ 등이 검색 명령이 아닌 글자 그대로 검색되게 처리
                string escaped = keyword
                    .Replace("~", "~~")
                    .Replace("%", "~%")
                    .Replace("_", "~_")
                    .Replace("[", "~[");

                command.Parameters.Add("@Keyword", SqlDbType.NVarChar, 500).Value = "%" + escaped + "%";
            }

            using var reader = await command.ExecuteReaderAsync();

            var table = new DataTable();
            table.Load(reader);

            return table;
        }

        // 도서 등록
        public async Task Add(BookData book)
        {
            const string sql = @"
                INSERT INTO dbo.[도서]
                (
                    [제목], [저자], [출판사],
                    [발행연도], [카테고리], [ISBN],
                    [대출가능여부], [대출자]
                )
                VALUES
                (
                    @Title, @Author, @Publisher,
                    @Year, @Category, @Isbn,
                    1, NULL
                );";

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            AddBookParameters(command, book);

            await command.ExecuteNonQueryAsync();
        }

        // 선택한 도서의 기본 정보 수정
        public async Task<int> Update(BookData book)
        {
            const string sql = @"
                UPDATE dbo.[도서]
                SET
                    [제목] = @Title,
                    [저자] = @Author,
                    [출판사] = @Publisher,
                    [발행연도] = @Year,
                    [카테고리] = @Category,
                    [ISBN] = @Isbn
                WHERE [관리번호] = @BookNumber;";

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            AddBookParameters(command, book);

            command.Parameters.Add("@BookNumber", SqlDbType.Int).Value = book.BookNumber;

            return await command.ExecuteNonQueryAsync();
        }

        // 선택한 한 권 삭제
        public async Task<int> Delete(int bookNumber)
        {
            const string sql = @"
                DELETE FROM dbo.[도서]
                WHERE [관리번호] = @BookNumber;";

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@BookNumber", SqlDbType.Int).Value = bookNumber;

            return await command.ExecuteNonQueryAsync();
        }

        
        private static void AddBookParameters(SqlCommand command, BookData book)
        {
            command.Parameters.Add("@Title", SqlDbType.NVarChar, 200).Value = book.Title;

            command.Parameters.Add("@Author", SqlDbType.NVarChar, 100).Value = book.Author;

            command.Parameters.Add("@Publisher", SqlDbType.NVarChar, 100).Value = book.Publisher;

            command.Parameters.Add("@Year", SqlDbType.SmallInt).Value = book.PublicationYear;

            command.Parameters.Add("@Category", SqlDbType.NVarChar, 50).Value = book.Category;

            command.Parameters.Add("@Isbn", SqlDbType.VarChar, 13).Value = book.Isbn;
        }
    }
}