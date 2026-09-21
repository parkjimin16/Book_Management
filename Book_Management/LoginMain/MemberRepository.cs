using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Book_Management.MemberData;



namespace Book_Management
{
    public enum RegisterResult
    {
        Success,
        DuplicateId,
        DuplicatePhone
    }

    public class MemberRepository
    {
        private const string ConnectionDB = DatabaseConfig.ConnectionString;

        // 아이디 중복 검사
        public async Task<bool> IsIdExists(string id)
        {
            using var connection = new SqlConnection(ConnectionDB);

            await connection.OpenAsync();

            const string sql = """
                SELECT COUNT(*)
                FROM dbo.[회원]
                WHERE [아이디] = @Id;
                """;

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = id;

            int count = Convert.ToInt32(await command.ExecuteScalarAsync());

            return count > 0;
        }
        // 연락처 중복 검사
        public async Task<bool> IsPhoneExists(string phone)
        {
            using var connection = new SqlConnection(ConnectionDB);

            await connection.OpenAsync();

            const string sql = """
                SELECT COUNT(*)
                FROM dbo.[회원]
                WHERE [연락처] = @Phone;
                """;

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = phone;

            int count = Convert.ToInt32(await command.ExecuteScalarAsync());

            return count > 0;
        }

        // 로그인
        public async Task<LoginMember?> Login(string id, string password)
        {
            LoginMember member;
            string storedPassword;

            using var connection = new SqlConnection(ConnectionDB);

            await connection.OpenAsync();

            const string sql = """
                SELECT
                    [회원번호],
                    [이름],
                    [아이디],
                    [비밀번호],
                    [회원코드]
                FROM dbo.[회원]
                WHERE [아이디] = @Id;
                """;

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = id;

            using var reader = await command.ExecuteReaderAsync();

            // 해당 아이디가 없으면 로그인 실패
            if (!await reader.ReadAsync())
            {
                return null;
            }

            storedPassword = reader["비밀번호"].ToString()!;

            member = new LoginMember
            {
                MemberNumber = Convert.ToInt32(reader["회원번호"]),

                Name = reader["이름"].ToString()!,

                LoginId = reader["아이디"].ToString()!,

                MemberCode = reader["회원코드"].ToString()!
            };

            if (member.MemberCode != "01" && member.MemberCode != "02")
            {
                return null;
            }

            bool isCorrect = string.Equals(password, storedPassword, StringComparison.Ordinal);

            return isCorrect ? member : null;
        }

        // 회원가입
        public async Task<RegisterResult> Register(
            string name,
            string phone,
            string id,
            string password)
        {
            if (await IsPhoneExists(phone))
            {
                return RegisterResult.DuplicatePhone;
            }

            if (await IsIdExists(id))
            {
                return RegisterResult.DuplicateId;
            }

            try
            {
                using var connection = new SqlConnection(ConnectionDB);

                await connection.OpenAsync();

                const string sql = """
                    SET ARITHABORT ON;

                    INSERT INTO dbo.[회원]
                    (
                        [이름],
                        [연락처],
                        [아이디],
                        [비밀번호],
                        [회원코드]
                    )
                    VALUES
                    (
                        @Name,
                        @Phone,
                        @Id,
                        @Password,
                        '02'
                    );
                    """;

                using var command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = name;

                command.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = phone;

                command.Parameters.Add("@Id", SqlDbType.NVarChar, 50).Value = id;

                command.Parameters.Add("@Password", SqlDbType.VarChar, 500).Value = password;

                await command.ExecuteNonQueryAsync();

                return RegisterResult.Success;
            }
            catch (SqlException ex)
                when (ex.Number == 2601 || ex.Number == 2627)
            {
                // 사전 확인 후 다른 프로그램이 먼저 가입한 경우
                if (await IsPhoneExists(phone))
                {
                    return RegisterResult.DuplicatePhone;
                }

                if (await IsIdExists(id))
                {
                    return RegisterResult.DuplicateId;
                }

                throw;
            }
        }

        // 회원 정보 수정
        public async Task<MemberUpdateResult> UpdateMember(
            int memberNumber,
            string name,
            string phone,
            string newPassword)
        {
            const string sql = @"
                SET ARITHABORT ON;

                IF EXISTS
                (
                    SELECT 1
                    FROM dbo.[회원]
                    WHERE [연락처] = @Phone
                      AND [회원번호] <> @MemberNumber
                )
                BEGIN
                    SELECT -1;
                END
                ELSE
                BEGIN
                    UPDATE dbo.[회원]
                    SET
                        [이름] = @Name,
                        [연락처] = @Phone,
                        [비밀번호] =
                            CASE
                                WHEN @Password IS NULL
                                    THEN [비밀번호]
                                ELSE @Password
                            END
                    WHERE [회원번호] = @MemberNumber;

                    SELECT @@ROWCOUNT;
                END;";

            using var connection = new SqlConnection(ConnectionDB);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@MemberNumber", SqlDbType.Int).Value = memberNumber;

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = name;

            command.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = phone;

            command.Parameters.Add("@Password", SqlDbType.VarChar, 500).Value = string.IsNullOrEmpty(newPassword) ? (object)DBNull.Value : newPassword;

            try
            {
                int result = Convert.ToInt32(await command.ExecuteScalarAsync());

                if (result == -1)
                {
                    return MemberUpdateResult.DuplicatePhone;
                }

                return result == 1 ? MemberUpdateResult.Success : MemberUpdateResult.NotFound;
            }
            catch (SqlException ex)
                when (ex.Number == 2601 || ex.Number == 2627)
            {
                // 동시에 같은 연락처로 변경했을 때도 UNIQUE로 차단
                return MemberUpdateResult.DuplicatePhone;
            }
        }

        // 회원 삭제
        public async Task<MemberDeleteResult> DeleteMember(int memberNumber)
        {
            const string sql = @"
                DELETE FROM dbo.[회원]
                WHERE [회원번호] = @MemberNumber
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM dbo.[도서]
                      WHERE [대출자] = @MemberNumber
                  );

                SELECT
                    CASE
                        WHEN @@ROWCOUNT = 1 THEN 1
                        WHEN EXISTS
                        (
                            SELECT 1
                            FROM dbo.[회원]
                            WHERE [회원번호] = @MemberNumber
                        ) THEN -1
                        ELSE 0
                    END;";

            using var connection = new SqlConnection(ConnectionDB);
            await connection.OpenAsync();

            using var command = new SqlCommand(sql, connection);

            command.Parameters.Add("@MemberNumber", SqlDbType.Int).Value = memberNumber;

            int result = Convert.ToInt32(await command.ExecuteScalarAsync());

            if (result == 1)
            {
                return MemberDeleteResult.Success;
            }

            if (result == -1)
            {
                return MemberDeleteResult.HasLoans;
            }

            return MemberDeleteResult.NotFound;
        }


    }
}
