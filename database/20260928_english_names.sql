-- SQLCMD parameters: Undo=0/1, CommitChanges=0/1. See database/README.md.
USE [LibraryDB];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET LOCK_TIMEOUT 10000;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

DECLARE @Undo bit = CONVERT(bit, '$(Undo)');
DECLARE @CommitChanges bit = CONVERT(bit, '$(CommitChanges)');

IF @@TRANCOUNT <> 0
    THROW 50000, 'Run this migration in a new connection without an active transaction.', 1;

DECLARE @Tables TABLE
(
    TableNo int PRIMARY KEY,
    OldName sysname NOT NULL,
    NewName sysname NOT NULL,
    ObjectId int NULL
);
INSERT INTO @Tables (TableNo, OldName, NewName)
VALUES (1, N'도서', N'Books'), (2, N'신규도서', N'BookRequests'), (3, N'회원', N'Members');

DECLARE @Steps TABLE
(
    StepNo int IDENTITY(1,1) PRIMARY KEY,
    ParentPath nvarchar(776) NOT NULL,
    OldName sysname NOT NULL,
    NewName sysname NOT NULL,
    ObjectType varchar(6) NOT NULL
);
-- Reverse execution restores tables first, so column/index parent paths remain valid.
INSERT INTO @Steps (ParentPath, OldName, NewName, ObjectType)
VALUES
    (N'dbo.[도서]', N'관리번호', N'BookNumber', 'COLUMN'),
    (N'dbo.[도서]', N'제목', N'Title', 'COLUMN'),
    (N'dbo.[도서]', N'저자', N'Author', 'COLUMN'),
    (N'dbo.[도서]', N'출판사', N'Publisher', 'COLUMN'),
    (N'dbo.[도서]', N'발행연도', N'PublicationYear', 'COLUMN'),
    (N'dbo.[도서]', N'카테고리', N'Category', 'COLUMN'),
    (N'dbo.[도서]', N'ISBN', N'Isbn', 'COLUMN'),
    (N'dbo.[도서]', N'대출가능여부', N'IsAvailable', 'COLUMN'),
    (N'dbo.[도서]', N'대출자', N'BorrowerLoginId', 'COLUMN'),
    (N'dbo.[도서]', N'조회수', N'ViewCount', 'COLUMN'),
    (N'dbo.[도서]', N'반납일', N'DueDate', 'COLUMN'),
    (N'dbo.[신규도서]', N'제목', N'Title', 'COLUMN'),
    (N'dbo.[신규도서]', N'저자', N'Author', 'COLUMN'),
    (N'dbo.[신규도서]', N'출판사', N'Publisher', 'COLUMN'),
    (N'dbo.[신규도서]', N'발행연도', N'PublicationYear', 'COLUMN'),
    (N'dbo.[신규도서]', N'카테고리', N'Category', 'COLUMN'),
    (N'dbo.[신규도서]', N'ISBN', N'Isbn', 'COLUMN'),
    (N'dbo.[회원]', N'회원번호', N'MemberNumber', 'COLUMN'),
    (N'dbo.[회원]', N'이름', N'Name', 'COLUMN'),
    (N'dbo.[회원]', N'연락처', N'Phone', 'COLUMN'),
    (N'dbo.[회원]', N'아이디', N'LoginId', 'COLUMN'),
    (N'dbo.[회원]', N'비밀번호', N'Password', 'COLUMN'),
    (N'dbo.[회원]', N'회원코드', N'MemberCode', 'COLUMN'),
    (N'dbo', N'DF_도서_대출가능여부', N'DF_Books_IsAvailable', 'OBJECT'),
    (N'dbo', N'DF_도서_조회수', N'DF_Books_ViewCount', 'OBJECT'),
    (N'dbo', N'FK_도서_회원', N'FK_Books_Members_BorrowerLoginId', 'OBJECT'),
    (N'dbo', N'PK_도서', N'PK_Books', 'OBJECT'),
    (N'dbo', N'PK_신규도서', N'PK_BookRequests', 'OBJECT'),
    (N'dbo', N'DF_회원_회원코드', N'DF_Members_MemberCode', 'OBJECT'),
    (N'dbo', N'PK_회원', N'PK_Members', 'OBJECT'),
    (N'dbo', N'UQ_회원_아이디', N'UQ_Members_LoginId', 'OBJECT'),
    (N'dbo.[도서]', N'IX_도서_조회수', N'IX_Books_ViewCount', 'INDEX'),
    (N'dbo', N'도서', N'Books', 'OBJECT'),
    (N'dbo', N'신규도서', N'BookRequests', 'OBJECT'),
    (N'dbo', N'회원', N'Members', 'OBJECT');

-- SQL Server blocks renaming columns referenced by these schema-bound expressions.
-- Capture their definitions, then rebuild the same rules inside the transaction.
DECLARE @Checks TABLE
(
    CheckNo int IDENTITY(1,1) PRIMARY KEY,
    OldName sysname NOT NULL,
    NewName sysname NOT NULL,
    TableObjectId int NULL,
    Definition nvarchar(max) NULL
);
INSERT INTO @Checks (OldName, NewName) VALUES
    (N'CK_도서_대출상태', N'CK_Books_LoanState'),
    (N'CK_도서_발행연도', N'CK_Books_PublicationYear'),
    (N'CK_도서_조회수', N'CK_Books_ViewCount'),
    (N'CK_신규도서_발행연도', N'CK_BookRequests_PublicationYear'),
    (N'CK_회원_회원코드', N'CK_Members_MemberCode');

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE @Tables
    SET ObjectId = OBJECT_ID(N'dbo.' + QUOTENAME(
        CASE WHEN @Undo = 0 THEN OldName ELSE NewName END), N'U');

    IF EXISTS (SELECT 1 FROM @Tables WHERE ObjectId IS NULL)
       OR EXISTS
       (
           SELECT 1 FROM @Tables
           WHERE OBJECT_ID(N'dbo.' + QUOTENAME(
               CASE WHEN @Undo = 0 THEN NewName ELSE OldName END)) IS NOT NULL
       )
        THROW 50001, 'Source tables are missing or target names already exist. Check Undo; do not reapply a committed migration.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM sys.sql_modules AS m
        INNER JOIN sys.objects AS o ON o.object_id = m.object_id
        WHERE o.is_ms_shipped = 0
          AND
          (
              o.parent_object_id IN (SELECT ObjectId FROM @Tables)
              OR EXISTS
              (
                  SELECT 1 FROM sys.sql_expression_dependencies AS d
                  WHERE d.referencing_id = m.object_id
                    AND d.referenced_id IN (SELECT ObjectId FROM @Tables)
              )
              OR EXISTS
              (
                  SELECT 1 FROM @Tables AS t
                  WHERE CHARINDEX(CASE WHEN @Undo = 0 THEN t.OldName ELSE t.NewName END, m.definition) > 0
              )
          )
    )
        THROW 50002, 'A dependent SQL module exists. Review its definition before renaming.', 1;

    IF EXISTS (SELECT 1 FROM sys.synonyms)
        THROW 50003, 'Review database synonyms before applying this migration.', 1;

    UPDATE target
    SET TableObjectId = source.parent_object_id, Definition = source.definition
    FROM @Checks AS target
    INNER JOIN sys.check_constraints AS source
        ON source.name = CASE WHEN @Undo = 0 THEN target.OldName ELSE target.NewName END
       AND source.parent_object_id IN (SELECT ObjectId FROM @Tables)
       AND source.is_disabled = 0 AND source.is_not_trusted = 0
       AND source.is_not_for_replication = 0;
    IF EXISTS (SELECT 1 FROM @Checks WHERE Definition IS NULL)
       OR (SELECT COUNT(*) FROM sys.check_constraints WHERE parent_object_id IN (SELECT ObjectId FROM @Tables)) <> 5
        THROW 50012, 'Expected five enabled, trusted CHECK constraints without replication exceptions.', 1;

    DECLARE @MemberObjectId int = (SELECT ObjectId FROM @Tables WHERE TableNo = 3);
    DECLARE @PhoneIndexSource sysname = CASE WHEN @Undo = 0 THEN N'UX_회원_연락처' ELSE N'UX_Members_Phone' END;
    DECLARE @PhoneIndexTarget sysname = CASE WHEN @Undo = 0 THEN N'UX_Members_Phone' ELSE N'UX_회원_연락처' END;
    DECLARE @PhoneSourceColumn sysname = CASE WHEN @Undo = 0 THEN N'연락처' ELSE N'Phone' END;
    DECLARE @PhoneTargetColumn sysname = CASE WHEN @Undo = 0 THEN N'Phone' ELSE N'연락처' END;
    IF NOT EXISTS
    (
        SELECT 1 FROM sys.indexes AS i
        INNER JOIN sys.data_spaces AS ds ON ds.data_space_id = i.data_space_id
        INNER JOIN sys.stats AS st ON st.object_id = i.object_id AND st.stats_id = i.index_id
        WHERE i.object_id = @MemberObjectId AND i.name = @PhoneIndexSource
          AND i.type = 2 AND i.is_unique = 1 AND i.is_disabled = 0
          AND i.is_padded = 0 AND i.fill_factor = 0 AND i.ignore_dup_key = 0
          AND i.allow_row_locks = 1 AND i.allow_page_locks = 1
          AND i.optimize_for_sequential_key = 0 AND ds.name = N'PRIMARY'
          AND st.no_recompute = 0 AND st.is_incremental = 0
          AND i.filter_definition = N'(' + QUOTENAME(@PhoneSourceColumn) + N' IS NOT NULL)'
          AND (SELECT COUNT(*) FROM sys.index_columns WHERE object_id = i.object_id AND index_id = i.index_id AND key_ordinal > 0) = 1
          AND EXISTS (SELECT 1 FROM sys.index_columns AS ic WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
              AND ic.key_ordinal = 1 AND ic.is_descending_key = 0 AND COL_NAME(ic.object_id, ic.column_id) = @PhoneSourceColumn)
          AND NOT EXISTS (SELECT 1 FROM sys.index_columns AS ic WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 1)
          AND NOT EXISTS (SELECT 1 FROM sys.partitions AS p WHERE p.object_id = i.object_id AND p.index_id = i.index_id AND p.data_compression <> 0)
    )
        THROW 50013, 'The filtered phone index differs from the inspected schema; review its settings first.', 1;

    IF (SELECT CONVERT(int, value_in_use) FROM sys.configurations WHERE name = 'fill factor (%)') <> 0
        THROW 50016, 'The default fill factor changed; review filtered index recreation before proceeding.', 1;

    SELECT c.object_id, c.column_id, c.system_type_id, c.user_type_id,
           c.max_length, c.precision, c.scale, c.collation_name,
           c.is_nullable, c.is_identity, c.is_computed, c.default_object_id
    INTO #ColumnsBefore
    FROM sys.columns AS c WHERE c.object_id IN (SELECT ObjectId FROM @Tables);

    SELECT i.object_id, CASE WHEN i.has_filter = 1 THEN -1 ELSE i.index_id END AS index_id, i.type, i.is_unique, i.is_primary_key,
           i.is_unique_constraint, i.is_disabled, i.has_filter,
           i.is_padded, i.fill_factor, i.ignore_dup_key, i.allow_row_locks, i.allow_page_locks, i.data_space_id,
           ic.index_column_id, ic.column_id, ic.key_ordinal,
           ic.is_descending_key, ic.is_included_column
    INTO #IndexesBefore
    FROM sys.indexes AS i
    INNER JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
    WHERE i.object_id IN (SELECT ObjectId FROM @Tables);

    SELECT o.object_id, o.parent_object_id, o.type
    INTO #ConstraintsBefore
    FROM sys.objects AS o
    WHERE o.parent_object_id IN (SELECT ObjectId FROM @Tables)
      AND o.type IN ('PK', 'UQ', 'F', 'D');

    SELECT fk.object_id, fk.parent_object_id, fk.referenced_object_id,
           fk.is_disabled, fk.is_not_trusted, fk.delete_referential_action,
           fk.update_referential_action, fkc.constraint_column_id,
           fkc.parent_column_id, fkc.referenced_column_id
    INTO #ForeignKeysBefore
    FROM sys.foreign_keys AS fk
    INNER JOIN sys.foreign_key_columns AS fkc ON fkc.constraint_object_id = fk.object_id
    WHERE fk.parent_object_id IN (SELECT ObjectId FROM @Tables);

    CREATE TABLE #DataSnapshots
    (
        Phase int NOT NULL,
        ObjectId int NOT NULL,
        RecordCount bigint NOT NULL,
        DataHash varbinary(32) NOT NULL,
        PRIMARY KEY (Phase, ObjectId)
    );

    DECLARE @Phase int = 0, @TableNo int, @ObjectId int;
    DECLARE @Columns nvarchar(max), @OrderColumns nvarchar(max), @SnapshotSql nvarchar(max);
    DECLARE @RecordCount bigint, @DataHash varbinary(32);
    DECLARE @Step int, @StepCount int = (SELECT COUNT(*) FROM @Steps);
    DECLARE @SourceName nvarchar(776), @TargetName sysname, @ObjectType varchar(6), @RenameResult int;
    DECLARE @CheckNo int, @Definition nvarchar(max), @DependencySql nvarchar(max);
    DECLARE @FromColumn sysname, @ToColumn sysname;

    WHILE @Phase <= 1
    BEGIN
        SET @TableNo = 1;
        WHILE @TableNo <= 3
        BEGIN
            SELECT @ObjectId = ObjectId FROM @Tables WHERE TableNo = @TableNo;
            -- Stable ordinal aliases keep fingerprints independent of column names.
            SELECT @Columns = STRING_AGG(CONVERT(nvarchar(max), QUOTENAME(name) + N' AS ' +
                QUOTENAME(N'C' + CONVERT(nvarchar(10), column_id))), N',') WITHIN GROUP (ORDER BY column_id)
            FROM sys.columns WHERE object_id = @ObjectId;

            SELECT @OrderColumns = STRING_AGG(CONVERT(nvarchar(max), QUOTENAME(c.name)), N',')
                WITHIN GROUP (ORDER BY ic.key_ordinal)
            FROM sys.indexes AS i
            INNER JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            INNER JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
            WHERE i.object_id = @ObjectId AND i.is_primary_key = 1 AND ic.key_ordinal > 0;

            IF @OrderColumns IS NULL
                THROW 50004, 'A primary key is required for deterministic data verification.', 1;

            SET @SnapshotSql = N'SELECT @n = COUNT_BIG(*) FROM dbo.' + QUOTENAME(OBJECT_NAME(@ObjectId)) +
                N' WITH (TABLOCKX, HOLDLOCK); SELECT @h = HASHBYTES(''SHA2_256'', (SELECT ' + @Columns +
                N' FROM dbo.' + QUOTENAME(OBJECT_NAME(@ObjectId)) + N' ORDER BY ' + @OrderColumns +
                N' FOR JSON PATH, INCLUDE_NULL_VALUES));';
            EXEC sys.sp_executesql @SnapshotSql, N'@n bigint OUTPUT, @h varbinary(32) OUTPUT',
                @n = @RecordCount OUTPUT, @h = @DataHash OUTPUT;
            INSERT INTO #DataSnapshots VALUES (@Phase, @ObjectId, @RecordCount, @DataHash);
            SET @TableNo += 1;
        END;

        IF @Phase = 0
        BEGIN
            SET @CheckNo = 1;
            WHILE @CheckNo <= 5
            BEGIN
                SELECT @ObjectId = TableObjectId,
                    @SourceName = CASE WHEN @Undo = 0 THEN OldName ELSE NewName END
                FROM @Checks WHERE CheckNo = @CheckNo;
                SET @DependencySql = N'ALTER TABLE dbo.' + QUOTENAME(OBJECT_NAME(@ObjectId)) +
                    N' DROP CONSTRAINT ' + QUOTENAME(@SourceName) + N';';
                EXEC sys.sp_executesql @DependencySql;
                SET @CheckNo += 1;
            END;
            SET @DependencySql = N'DROP INDEX ' + QUOTENAME(@PhoneIndexSource) +
                N' ON dbo.' + QUOTENAME(OBJECT_NAME(@MemberObjectId)) + N';';
            EXEC sys.sp_executesql @DependencySql;

            -- Transform captured expressions, including the reverse direction.
            SET @Step = 1;
            WHILE @Step <= @StepCount
            BEGIN
                SELECT @FromColumn = CASE WHEN @Undo = 0 THEN OldName ELSE NewName END,
                    @ToColumn = CASE WHEN @Undo = 0 THEN NewName ELSE OldName END,
                    @ObjectType = ObjectType FROM @Steps WHERE StepNo = @Step;
                IF @ObjectType = 'COLUMN'
                    UPDATE @Checks SET Definition = REPLACE(Definition, QUOTENAME(@FromColumn), QUOTENAME(@ToColumn));
                SET @Step += 1;
            END;

            SET @Step = CASE WHEN @Undo = 0 THEN 1 ELSE @StepCount END;
            WHILE @Step BETWEEN 1 AND @StepCount
            BEGIN
                SELECT @SourceName = ParentPath + N'.' + QUOTENAME(CASE WHEN @Undo = 0 THEN OldName ELSE NewName END),
                       @TargetName = CASE WHEN @Undo = 0 THEN NewName ELSE OldName END,
                       @ObjectType = ObjectType
                FROM @Steps WHERE StepNo = @Step;
                EXEC @RenameResult = sys.sp_rename @objname = @SourceName, @newname = @TargetName, @objtype = @ObjectType;
                IF @RenameResult <> 0
                    THROW 50005, 'sp_rename returned an error.', 1;
                SET @Step += CASE WHEN @Undo = 0 THEN 1 ELSE -1 END;
            END;

            SET @CheckNo = 1;
            WHILE @CheckNo <= 5
            BEGIN
                SELECT @ObjectId = TableObjectId, @Definition = Definition,
                    @TargetName = CASE WHEN @Undo = 0 THEN NewName ELSE OldName END
                FROM @Checks WHERE CheckNo = @CheckNo;
                SET @DependencySql = N'ALTER TABLE dbo.' + QUOTENAME(OBJECT_NAME(@ObjectId)) +
                    N' WITH CHECK ADD CONSTRAINT ' + QUOTENAME(@TargetName) + N' CHECK ' + @Definition + N';';
                EXEC sys.sp_executesql @DependencySql;
                SET @CheckNo += 1;
            END;

            SET @DependencySql = N'CREATE UNIQUE NONCLUSTERED INDEX ' + QUOTENAME(@PhoneIndexTarget) +
                N' ON dbo.' + QUOTENAME(OBJECT_NAME(@MemberObjectId)) + N' (' + QUOTENAME(@PhoneTargetColumn) + N' ASC)' +
                N' WHERE ' + QUOTENAME(@PhoneTargetColumn) + N' IS NOT NULL' +
                N' WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF,' +
                N' ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF, DATA_COMPRESSION = NONE) ON [PRIMARY];';
            EXEC sys.sp_executesql @DependencySql;
        END;
        SET @Phase += 1;
    END;

    IF EXISTS
    (
        SELECT ObjectId, RecordCount, DataHash FROM #DataSnapshots WHERE Phase = 0
        EXCEPT SELECT ObjectId, RecordCount, DataHash FROM #DataSnapshots WHERE Phase = 1
    )
        THROW 50006, 'Data verification failed; rolling back all renames.', 1;

    SELECT c.object_id, c.column_id, c.system_type_id, c.user_type_id,
           c.max_length, c.precision, c.scale, c.collation_name,
           c.is_nullable, c.is_identity, c.is_computed, c.default_object_id
    INTO #ColumnsAfter
    FROM sys.columns AS c WHERE c.object_id IN (SELECT ObjectId FROM @Tables);
    IF EXISTS (SELECT * FROM #ColumnsBefore EXCEPT SELECT * FROM #ColumnsAfter)
       OR EXISTS (SELECT * FROM #ColumnsAfter EXCEPT SELECT * FROM #ColumnsBefore)
        THROW 50007, 'Column structure changed unexpectedly.', 1;

    SELECT i.object_id, CASE WHEN i.has_filter = 1 THEN -1 ELSE i.index_id END AS index_id, i.type, i.is_unique, i.is_primary_key,
           i.is_unique_constraint, i.is_disabled, i.has_filter,
           i.is_padded, i.fill_factor, i.ignore_dup_key, i.allow_row_locks, i.allow_page_locks, i.data_space_id,
           ic.index_column_id, ic.column_id, ic.key_ordinal,
           ic.is_descending_key, ic.is_included_column
    INTO #IndexesAfter
    FROM sys.indexes AS i
    INNER JOIN sys.index_columns AS ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
    WHERE i.object_id IN (SELECT ObjectId FROM @Tables);
    IF EXISTS (SELECT * FROM #IndexesBefore EXCEPT SELECT * FROM #IndexesAfter)
       OR EXISTS (SELECT * FROM #IndexesAfter EXCEPT SELECT * FROM #IndexesBefore)
        THROW 50008, 'Index structure changed unexpectedly.', 1;

    SELECT o.object_id, o.parent_object_id, o.type INTO #ConstraintsAfter
    FROM sys.objects AS o
    WHERE o.parent_object_id IN (SELECT ObjectId FROM @Tables)
      AND o.type IN ('PK', 'UQ', 'F', 'D');
    IF EXISTS (SELECT * FROM #ConstraintsBefore EXCEPT SELECT * FROM #ConstraintsAfter)
       OR EXISTS (SELECT * FROM #ConstraintsAfter EXCEPT SELECT * FROM #ConstraintsBefore)
        THROW 50009, 'Constraint identity changed unexpectedly.', 1;

    SELECT fk.object_id, fk.parent_object_id, fk.referenced_object_id,
           fk.is_disabled, fk.is_not_trusted, fk.delete_referential_action,
           fk.update_referential_action, fkc.constraint_column_id,
           fkc.parent_column_id, fkc.referenced_column_id
    INTO #ForeignKeysAfter
    FROM sys.foreign_keys AS fk
    INNER JOIN sys.foreign_key_columns AS fkc ON fkc.constraint_object_id = fk.object_id
    WHERE fk.parent_object_id IN (SELECT ObjectId FROM @Tables);
    IF EXISTS (SELECT * FROM #ForeignKeysBefore EXCEPT SELECT * FROM #ForeignKeysAfter)
       OR EXISTS (SELECT * FROM #ForeignKeysAfter EXCEPT SELECT * FROM #ForeignKeysBefore)
        THROW 50010, 'Foreign key relationship changed unexpectedly.', 1;

    IF EXISTS
    (
        SELECT 1 FROM @Checks AS expected
        LEFT JOIN sys.check_constraints AS actual
            ON actual.parent_object_id = expected.TableObjectId
           AND actual.name = CASE WHEN @Undo = 0 THEN expected.NewName ELSE expected.OldName END
        WHERE actual.object_id IS NULL OR actual.definition <> expected.Definition
           OR actual.is_disabled <> 0 OR actual.is_not_trusted <> 0 OR actual.is_not_for_replication <> 0
    )
        THROW 50014, 'Recreated CHECK definitions or validation state differ.', 1;

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.indexes
        WHERE object_id = @MemberObjectId AND name = @PhoneIndexTarget
          AND filter_definition = N'(' + QUOTENAME(@PhoneTargetColumn) + N' IS NOT NULL)'
    )
        THROW 50015, 'Recreated phone index filter differs.', 1;

    IF @Undo = 0 AND EXISTS
    (
        SELECT name FROM sys.objects WHERE object_id IN (SELECT ObjectId FROM @Tables)
            OR parent_object_id IN (SELECT ObjectId FROM @Tables)
        UNION ALL
        SELECT name FROM sys.columns WHERE object_id IN (SELECT ObjectId FROM @Tables)
        UNION ALL
        SELECT name FROM sys.indexes WHERE object_id IN (SELECT ObjectId FROM @Tables)
        EXCEPT
        SELECT name FROM
        (
            SELECT name FROM sys.objects WHERE object_id IN (SELECT ObjectId FROM @Tables)
                OR parent_object_id IN (SELECT ObjectId FROM @Tables)
            UNION ALL SELECT name FROM sys.columns WHERE object_id IN (SELECT ObjectId FROM @Tables)
            UNION ALL SELECT name FROM sys.indexes WHERE object_id IN (SELECT ObjectId FROM @Tables)
        ) AS names WHERE name COLLATE Latin1_General_100_BIN2 NOT LIKE N'%[^ -~]%'
    )
        THROW 50011, 'A non-English object name remains.', 1;

    SELECT OBJECT_NAME(ObjectId) AS TableName, RecordCount, N'PASS' AS DataUnchanged
    FROM #DataSnapshots WHERE Phase = 1 ORDER BY TableName;

    IF @CommitChanges = 1
    BEGIN
        COMMIT TRANSACTION;
        PRINT 'COMMITTED: English-name migration/rollback and data verification completed.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT 'DRY RUN PASSED: all name changes were rolled back.';
    END;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
