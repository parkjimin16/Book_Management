using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Book_Management
{
    internal sealed class UserBookRepository
    {
        private const string Columns = @"
            [BookNumber] AS [관리번호],
            [Title] AS [제목],
            [Author] AS [저자],
            [Publisher] AS [출판사],
            [PublicationYear] AS [발행연도],
            [Category] AS [카테고리],
            [Isbn] AS [ISBN],
            [IsAvailable] AS [대출가능여부],
            CASE
                WHEN [IsAvailable] = 1 THEN N'대출 가능'
                ELSE N'대출중'
            END AS [대출여부],
            [DueDate] AS [반납일],
            [ViewCount] AS [조회수]";

        public Task<DataTable> Search(int searchMode, string keyword, short? year, string category, bool available)
        {
            if (searchMode < 0 || searchMode > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(searchMode));
            }

            string escaped = keyword.Trim()
                .Replace("~", "~~")
                .Replace("%", "~%")
                .Replace("_", "~_")
                .Replace("[", "~[");

            string sql = $@"
                SELECT {Columns}
                FROM dbo.[Books]
                WHERE [IsAvailable] = @Available
                  AND (@Year IS NULL OR [PublicationYear] = @Year)
                  AND (@Category = N'전체' OR [Category] = @Category)
                  AND
                  (
                      @Keyword = N'%%'
                      OR
                      (
                          @Mode = 0
                          AND
                          (
                              [Title] LIKE @Keyword ESCAPE N'~'
                              OR [Author] LIKE @Keyword ESCAPE N'~'
                              OR [Publisher] LIKE @Keyword ESCAPE N'~'
                          )
                      )
                      OR
                      (
                          @Mode = 1
                          AND [Title] LIKE @Keyword ESCAPE N'~'
                      )
                      OR
                      (
                          @Mode = 2
                          AND [Author] LIKE @Keyword ESCAPE N'~'
                      )
                      OR
                      (
                          @Mode = 3
                          AND [Publisher] LIKE @Keyword ESCAPE N'~'
                      )
                  )
                ORDER BY [Title], [BookNumber];";

            return Query(
                sql,
                P("@Available", SqlDbType.Bit, available),
                P("@Year", SqlDbType.SmallInt, year),
                P("@Category", SqlDbType.NVarChar, category, 50),
                P("@Keyword", SqlDbType.NVarChar, "%" + escaped + "%", 500),
                P("@Mode", SqlDbType.Int, searchMode));
        }

        public async Task<DataRow> GetDetail(int bookNumber, bool increaseViews)
        {
            // 상세 팝업 최초 로드에서만 증가시킵니다.
            string sql = "SET NOCOUNT ON;";

            if (increaseViews)
            {
                sql += @"
                    UPDATE dbo.[Books]
                    SET [ViewCount] = [ViewCount] + 1
                    WHERE [BookNumber] = @BookNumber;";
            }

            sql += $@"
                SELECT {Columns}
                FROM dbo.[Books]
                WHERE [BookNumber] = @BookNumber;";

            DataTable table = await Query(
                sql,
                P("@BookNumber", SqlDbType.Int, bookNumber));

            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        public async Task<DateTime?> Borrow(int bookNumber, string loginId)
        {
            const string sql = @"
                SET NOCOUNT ON;

                DECLARE @Borrowed TABLE ([DueDate] DATE);

                UPDATE dbo.[Books]
                SET
                    [IsAvailable] = 0,
                    [BorrowerLoginId] = @LoginId,
                    [DueDate] = DATEADD(DAY, 7, CONVERT(DATE, GETDATE()))
                OUTPUT INSERTED.[DueDate]
                    INTO @Borrowed ([DueDate])
                WHERE [BookNumber] = @BookNumber
                  AND [IsAvailable] = 1
                  AND [BorrowerLoginId] IS NULL
                  AND [DueDate] IS NULL
                  AND EXISTS
                  (
                      SELECT 1
                      FROM dbo.[Members]
                      WHERE [LoginId] = @LoginId
                        AND [MemberCode] = '02'
                  );

                SELECT [DueDate] FROM @Borrowed;";

            object result = await Scalar(
                sql,
                P("@BookNumber", SqlDbType.Int, bookNumber),
                P("@LoginId", SqlDbType.NVarChar, loginId, 50));

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(result);
        }

        public Task<DataTable> GetMyLoans(string loginId)
        {
            string sql = $@"
                SELECT {Columns}
                FROM dbo.[Books]
                WHERE [BorrowerLoginId] = @LoginId
                  AND [IsAvailable] = 0
                ORDER BY [DueDate], [BookNumber];";

            return Query(
                sql,
                P("@LoginId", SqlDbType.NVarChar, loginId, 50));
        }

        public async Task<bool> Return(int bookNumber, string loginId)
        {
            const string sql = @"
                SET NOCOUNT ON;

                UPDATE dbo.[Books]
                SET
                    [IsAvailable] = 1,
                    [BorrowerLoginId] = NULL,
                    [DueDate] = NULL
                WHERE [BookNumber] = @BookNumber
                  AND [BorrowerLoginId] = @LoginId
                  AND [IsAvailable] = 0;

                SELECT @@ROWCOUNT;";

            object result = await Scalar(
                sql,
                P("@BookNumber", SqlDbType.Int, bookNumber),
                P("@LoginId", SqlDbType.NVarChar, loginId, 50));

            return Convert.ToInt32(result) == 1;
        }

        public async Task Request(BookData book)
        {
            const string sql = @"
                INSERT INTO dbo.[BookRequests]
                (
                    [Title], [Author], [Publisher],
                    [PublicationYear], [Category], [Isbn]
                )
                VALUES
                (
                    @Title, @Author, @Publisher,
                    @Year, @Category, @Isbn
                );

                SELECT 1;";

            await Scalar(
                sql,
                P("@Title", SqlDbType.NVarChar, book.Title, 200),
                P("@Author", SqlDbType.NVarChar, book.Author, 100),
                P("@Publisher", SqlDbType.NVarChar, book.Publisher, 100),
                P("@Year", SqlDbType.SmallInt, book.PublicationYear),
                P("@Category", SqlDbType.NVarChar, book.Category, 50),
                P("@Isbn", SqlDbType.VarChar, book.Isbn, 13));
        }

        private static SqlParameter P(string name, SqlDbType type, object value, int size = 0)
        {
            var parameter = new SqlParameter(name, type)
            {
                Value = value ?? DBNull.Value
            };

            if (size > 0)
            {
                parameter.Size = size;
            }

            return parameter;
        }

        private static async Task<DataTable> Query(string sql, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            var table = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            while (await reader.ReadAsync())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                table.Rows.Add(values);
            }

            return table;
        }

        private static async Task<object> Scalar(string sql, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            return await command.ExecuteScalarAsync();
        }
    }
}
