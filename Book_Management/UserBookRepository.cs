using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Book_Management
{
    internal sealed class UserBookRepository
    {
        private const string Columns = @"
            [관리번호],
            [제목],
            [저자],
            [출판사],
            [발행연도],
            [카테고리],
            [ISBN],
            [대출가능여부],
            CASE
                WHEN [대출가능여부] = 1 THEN N'대출 가능'
                ELSE N'대출중'
            END AS [대출여부],
            [반납일],
            [조회수]";

        public Task<DataTable> GetPopular()
        {
            string sql = $@"
                SELECT TOP (30) {Columns}
                FROM dbo.[도서]
                ORDER BY [조회수] DESC, [관리번호] ASC;";

            return Query(sql);
        }

        public Task<DataTable> Search(
            int searchMode,
            string keyword,
            short? year,
            string category,
            bool available)
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
                FROM dbo.[도서]
                WHERE [대출가능여부] = @Available
                  AND (@Year IS NULL OR [발행연도] = @Year)
                  AND (@Category = N'전체' OR [카테고리] = @Category)
                  AND
                  (
                      @Keyword = N'%%'
                      OR
                      (
                          @Mode = 0
                          AND
                          (
                              [제목] LIKE @Keyword ESCAPE N'~'
                              OR [저자] LIKE @Keyword ESCAPE N'~'
                              OR [출판사] LIKE @Keyword ESCAPE N'~'
                          )
                      )
                      OR
                      (
                          @Mode = 1
                          AND [제목] LIKE @Keyword ESCAPE N'~'
                      )
                      OR
                      (
                          @Mode = 2
                          AND [저자] LIKE @Keyword ESCAPE N'~'
                      )
                      OR
                      (
                          @Mode = 3
                          AND [출판사] LIKE @Keyword ESCAPE N'~'
                      )
                  )
                ORDER BY [제목], [관리번호];";

            return Query(
                sql,
                P("@Available", SqlDbType.Bit, available),
                P("@Year", SqlDbType.SmallInt, year),
                P("@Category", SqlDbType.NVarChar, category, 50),
                P("@Keyword", SqlDbType.NVarChar, "%" + escaped + "%", 500),
                P("@Mode", SqlDbType.Int, searchMode));
        }

        public async Task<DataRow> GetDetail(
            int bookNumber,
            bool increaseViews)
        {
            // 상세 팝업 최초 로드에서만 증가시킵니다.
            string sql = "SET NOCOUNT ON;";

            if (increaseViews)
            {
                sql += @"
                    UPDATE dbo.[도서]
                    SET [조회수] = [조회수] + 1
                    WHERE [관리번호] = @BookNumber;";
            }

            sql += $@"
                SELECT {Columns}
                FROM dbo.[도서]
                WHERE [관리번호] = @BookNumber;";

            DataTable table = await Query(
                sql,
                P("@BookNumber", SqlDbType.Int, bookNumber));

            return table.Rows.Count == 0 ? null : table.Rows[0];
        }

        public async Task<DateTime?> Borrow(
            int bookNumber,
            string loginId)
        {
            const string sql = @"
                SET NOCOUNT ON;

                DECLARE @Borrowed TABLE ([반납일] DATE);

                UPDATE dbo.[도서]
                SET
                    [대출가능여부] = 0,
                    [대출자] = @LoginId,
                    [반납일] = DATEADD(DAY, 7, CONVERT(DATE, GETDATE()))
                OUTPUT INSERTED.[반납일]
                    INTO @Borrowed ([반납일])
                WHERE [관리번호] = @BookNumber
                  AND [대출가능여부] = 1
                  AND [대출자] IS NULL
                  AND [반납일] IS NULL
                  AND EXISTS
                  (
                      SELECT 1
                      FROM dbo.[회원]
                      WHERE [아이디] = @LoginId
                        AND [회원코드] = '02'
                  );

                SELECT [반납일] FROM @Borrowed;";

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
                FROM dbo.[도서]
                WHERE [대출자] = @LoginId
                  AND [대출가능여부] = 0
                ORDER BY [반납일], [관리번호];";

            return Query(
                sql,
                P("@LoginId", SqlDbType.NVarChar, loginId, 50));
        }

        public async Task<bool> Return(
            int bookNumber,
            string loginId)
        {
            const string sql = @"
                SET NOCOUNT ON;

                UPDATE dbo.[도서]
                SET
                    [대출가능여부] = 1,
                    [대출자] = NULL,
                    [반납일] = NULL
                WHERE [관리번호] = @BookNumber
                  AND [대출자] = @LoginId
                  AND [대출가능여부] = 0;

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
                INSERT INTO dbo.[신규도서]
                (
                    [제목], [저자], [출판사],
                    [발행연도], [카테고리], [ISBN]
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

        private static SqlParameter P(
            string name,
            SqlDbType type,
            object value,
            int size = 0)
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

        private static async Task<DataTable> Query(
            string sql,
            params SqlParameter[] parameters)
        {
            using var connection =
                new SqlConnection(DatabaseConfig.ConnectionString);

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

        private static async Task<object> Scalar(
            string sql,
            params SqlParameter[] parameters)
        {
            using var connection =
                new SqlConnection(DatabaseConfig.ConnectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters);

            return await command.ExecuteScalarAsync();
        }
    }
}
