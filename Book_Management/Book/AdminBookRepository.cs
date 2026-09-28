using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace Book_Management.Book
{
    public enum AdminPage
    {
        Books,
        Members,
        Requests
    }

    internal class AdminBookRepository
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
                            [BookNumber] AS [관리번호],
                            [Title] AS [제목],
                            [Author] AS [저자],
                            [Publisher] AS [출판사],
                            [PublicationYear] AS [발행연도],
                            [Category] AS [카테고리],
                            [Isbn] AS [ISBN],
                            [IsAvailable] AS [대출가능여부],
                            [BorrowerLoginId] AS [대출자]
                        FROM dbo.[Books]";

                    orderColumn = "BookNumber";
                    break;

                case AdminPage.Members:
                    sql = @"
                        SELECT
                            m.[MemberNumber] AS [회원번호],
                            m.[Name] AS [이름],
                            m.[Phone] AS [연락처],
                            m.[LoginId] AS [아이디],
                            m.[MemberCode] AS [회원코드],
                            (
                                SELECT COUNT(*)
                                FROM dbo.[Books] AS b
                                WHERE b.[BorrowerLoginId] = m.[LoginId]
                            ) AS [대출현황]
                        FROM dbo.[Members] AS m";

                    orderColumn = "MemberNumber";
                    break;

                case AdminPage.Requests:
                    sql = @"
                        SELECT
                            [Title] AS [제목],
                            [Author] AS [저자],
                            [Publisher] AS [출판사],
                            [PublicationYear] AS [발행연도],
                            [Category] AS [카테고리],
                            [Isbn] AS [ISBN]
                        FROM dbo.[BookRequests]";

                    orderColumn = "Isbn";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(page));
            }

            // 허용된 컬럼만 SQL에 사용할 수 있습니다.
            if (!GetSearchColumns(page).Contains(searchColumn))
            {
                throw new ArgumentException("잘못된 검색 항목입니다.");
            }

            string dbColumn = searchColumn switch
            {
                "관리번호" => "BookNumber",
                "제목" => "Title",
                "저자" => "Author",
                "출판사" => "Publisher",
                "발행연도" => "PublicationYear",
                "카테고리" => "Category",
                "ISBN" => "Isbn",
                "회원번호" => "MemberNumber",
                "이름" => "Name",
                "연락처" => "Phone",
                "아이디" => "LoginId",
                "회원코드" => "MemberCode",
                _ => throw new ArgumentException("잘못된 검색 항목입니다.")
            };

            bool useSearch = !string.IsNullOrWhiteSpace(keyword);

            if (useSearch)
            {
                sql += $@"
                    WHERE CONVERT(nvarchar(4000), [{dbColumn}])
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
                INSERT INTO dbo.[Books]
                (
                    [Title], [Author], [Publisher],
                    [PublicationYear], [Category], [Isbn],
                    [IsAvailable], [BorrowerLoginId]
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
                UPDATE dbo.[Books]
                SET
                    [Title] = @Title,
                    [Author] = @Author,
                    [Publisher] = @Publisher,
                    [PublicationYear] = @Year,
                    [Category] = @Category,
                    [Isbn] = @Isbn
                WHERE [BookNumber] = @BookNumber
                  AND [IsAvailable] = 1
                  AND [BorrowerLoginId] IS NULL
                  AND [DueDate] IS NULL;";

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
                DELETE FROM dbo.[Books]
                WHERE [BookNumber] = @BookNumber
                  AND [IsAvailable] = 1
                  AND [BorrowerLoginId] IS NULL
                  AND [DueDate] IS NULL;";

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@BookNumber", SqlDbType.Int).Value = bookNumber;

            return await command.ExecuteNonQueryAsync();
        }
        public async Task<bool> RegisterRequestedBookAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                throw new ArgumentException("요청 도서의 ISBN이 없습니다.", nameof(isbn));
            }

            const string sql = @"
                SET NOCOUNT ON;
                SET XACT_ABORT ON;

                BEGIN TRY
                    BEGIN TRANSACTION;

                    -- 같은 요청을 여러 관리자가 동시에 등록하지 못하게 잠급니다.
                    INSERT INTO dbo.[Books]
                    (
                        [Title],
                        [Author],
                        [Publisher],
                        [PublicationYear],
                        [Category],
                        [Isbn],
                        [IsAvailable],
                        [BorrowerLoginId],
                        [DueDate],
                        [ViewCount]
                    )
                    SELECT
                        [Title],
                        [Author],
                        [Publisher],
                        [PublicationYear],
                        [Category],
                        [Isbn],
                        1,
                        NULL,
                        NULL,
                        0
                    FROM dbo.[BookRequests] WITH (UPDLOCK, HOLDLOCK)
                    WHERE [Isbn] = @Isbn;

                    DECLARE @InsertedCount INT = @@ROWCOUNT;

                    IF @InsertedCount = 1
                    BEGIN
                        DELETE FROM dbo.[BookRequests]
                        WHERE [Isbn] = @Isbn;

                        IF @@ROWCOUNT <> 1
                        BEGIN
                            ;THROW 50001, N'요청 삭제에 실패했습니다.', 1;
                        END;
                    END;

                    COMMIT TRANSACTION;

                    SELECT @InsertedCount;
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                    BEGIN
                        ROLLBACK TRANSACTION;
                    END;

                    THROW;
                END CATCH;";

            using var connection =
                new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Isbn", SqlDbType.VarChar, 13).Value = isbn;

            int insertedCount =
                Convert.ToInt32(await command.ExecuteScalarAsync());

            return insertedCount == 1;
        }

        public async Task<bool> IsIsbnExists(string isbn, int? excludeBookNumber = null)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.[Books]
                    WHERE [Isbn] = @Isbn
                      AND
                      (
                          @ExcludeBookNumber IS NULL
                          OR [BookNumber] <> @ExcludeBookNumber
                      )
                )
                THEN 1 ELSE 0 END;";

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Isbn", SqlDbType.VarChar, 13).Value = isbn.Trim().ToUpperInvariant();

            command.Parameters.Add("@ExcludeBookNumber", SqlDbType.Int).Value = (object)excludeBookNumber ?? DBNull.Value;

            return Convert.ToInt32(await command.ExecuteScalarAsync()) == 1;
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