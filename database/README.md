# LibraryDB 영문 이름 변경

`20260928_english_names.sql`은 현재 프로젝트의 한글 DB 식별자를 영문으로 변경한다.
자료형, NULL 허용 여부, 기본값, IDENTITY, 실제 데이터, PK/UQ/FK 연결 관계는 유지한다.
신규 도서 요청은 `BookRequests`에만 저장하며 `Books`로 자동 이동하지 않는다.

## 변경 이름

| 이전 테이블 | 변경 테이블 | 컬럼 |
|---|---|---|
| 도서 | Books | BookNumber, Title, Author, Publisher, PublicationYear, Category, Isbn, IsAvailable, BorrowerLoginId, ViewCount, DueDate |
| 신규도서 | BookRequests | Title, Author, Publisher, PublicationYear, Category, Isbn |
| 회원 | Members | MemberNumber, Name, Phone, LoginId, Password, MemberCode |

키·제약조건은 `PK_Books`, `PK_BookRequests`, `PK_Members`, `UQ_Members_LoginId`,
`FK_Books_Members_BorrowerLoginId`, `CK_Books_LoanState`, `CK_Books_PublicationYear`,
`CK_Books_ViewCount`, `CK_BookRequests_PublicationYear`, `CK_Members_MemberCode`,
`DF_Books_IsAvailable`, `DF_Books_ViewCount`, `DF_Members_MemberCode`로 변경한다.
독립 인덱스는 `IX_Books_ViewCount`, `UX_Members_Phone`이며 PK/UQ 인덱스는 키 이름을 따른다.

`BorrowerLoginId`는 `Members.LoginId`를 참조한다. `DueDate`는 반납 예정일이다.
`BookRequests.Isbn`은 기존처럼 기본 키이고 별도 요청 번호를 추가하지 않는다.

## 의존성 처리

현재 SQL Server는 CHECK 및 필터 인덱스가 참조하는 컬럼의 직접 이름 변경을 막는다.
따라서 CHECK 5개와 연락처 필터 인덱스 1개만 트랜잭션 안에서 제거한 후 같은 조건으로 재생성한다.
CHECK는 기존처럼 활성화·신뢰 상태를 유지하며, 조건식을 대괄호 식별자 단위로 변환하여 검증한다.
재생성되는 CHECK는 테이블 수준 제약조건으로 정의되어 기존 컬럼 수준 제약의
`parent_column_id`와 `object_id`는 달라질 수 있지만 검사 규칙은 동일하다.
전화번호 인덱스는 고유성, `IS NOT NULL` 필터, 키 순서, 파일그룹 및 설정을 유지한다.
PK/UQ/FK/DEFAULT는 기존 객체의 이름만 변경한다.

관련 SQL 모듈·동의어가 추가되거나 CHECK/필터 인덱스 설정이 달라지면 스크립트는 중단한다.
실행 전 외부 프로그램이나 별도 SQL의 기존 이름 참조도 확인해야 한다.

## 실행 순서

프로그램을 종료하고 DB와 변경할 소스 파일을 백업한다. 현재 연결은 `localhost`,
Windows 통합 인증이며 명령은 저장소 루트에서 실행한다. SQLCMD 변수 두 개를 반드시 지정한다.
UTF-8 입력을 위해 `-f 65001`, SQL 오류 종료를 위해 `-b`를 유지한다.

1. 세 Repository의 영문 SQL과 한국어 결과 별칭을 함께 적용한다.
2. `dotnet build Book_Management\Book_Management.csproj --no-restore`로 빌드한다.
3. 이름 변경을 시험한다. 이 명령은 검증 후 전체 롤백한다.

```powershell
sqlcmd -S localhost -d LibraryDB -E -C -b -f 65001 -v Undo="0" CommitChanges="0" -i database\20260928_english_names.sql
```

4. 시험이 성공하면 실제 적용한다.

```powershell
sqlcmd -S localhost -d LibraryDB -E -C -b -f 65001 -v Undo="0" CommitChanges="1" -i database\20260928_english_names.sql
```

이미 적용한 방향으로 다시 실행하면 대상 이름 충돌로 중단한다. 커밋 후 반복 적용하는 스크립트가 아니다.
모든 변경은 하나의 트랜잭션이며, 실패하면 해당 실행 전체를 롤백한다.
변경 전후 모든 행을 고정 컬럼 별칭과 PK 순서로 직렬화하여 SHA-256으로 비교한다.
비밀번호를 포함한 실제 데이터나 해시값은 출력하지 않는다. 컬럼/인덱스 구조와 FK 관계도 비교한다.

5. 실제 Repository 조회를 검증한다. Windows PowerShell 5.1로 실행한다.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts\Verify-EnglishSchema.ps1
```

스크립트는 조회·검색·대출 목록·실패 로그인과 한국어 결과 바인딩을 확인한다.
조회수를 증가시키거나 등록·수정·삭제·대출·반납을 실행하지 않는다.

## 되돌리기

프로그램을 종료하고 먼저 되돌리기를 시험한다.

```powershell
sqlcmd -S localhost -d LibraryDB -E -C -b -f 65001 -v Undo="1" CommitChanges="0" -i database\20260928_english_names.sql
```

복원이 필요할 때만 `CommitChanges="1"`로 실행하고 세 Repository도 변경 전 백업으로 복구한 뒤 재빌드한다.
이름 복원은 변경 이후 추가된 데이터도 보존한다. DB 전체 백업 복원은 백업 이후 데이터도 되돌리므로 별도 판단이 필요하다.

## 이번 작업의 백업

- DB COPY_ONLY/CHECKSUM 백업: `C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\Backup\LibraryDB_before_english_names_20260928_150250.bak`
- `RESTORE VERIFYONLY ... WITH CHECKSUM` 통과.
- 변경 전 소스: `C:\Users\admin\AppData\Local\Temp\BookManagement-EnglishSchema-20260928-150250`
- 소스 백업은 Temp에 있으므로 장기 보관이 필요하면 별도 위치에 복사한다.

## 소스와 화면

물리 DB 이름은 영문이며 세 Repository에서 한국어 `AS` 별칭을 사용한다.
따라서 기존 `DataPropertyName`, `DataRow` 접근, 화면의 한국어 표시를 유지한다.
관리자 검색 콤보박스는 허용 목록 검증 후 실제 영문 컬럼명으로 매핑한다.
현재 대상 프레임워크는 .NET Framework 4.8, SQL 클라이언트는 Microsoft.Data.SqlClient이다.

## 적용 및 검증 결과 (2026-09-28)

- 실제 DB에 영문 이름 변경을 커밋했다. 테이블 3개, 컬럼 23개, 키·제약조건 13개, 인덱스 6개의 이름이 영문이다. 인덱스 수에는 PK/UQ 인덱스가 포함된다.
- 변경 전후 데이터 비교 통과: Books 3행, BookRequests 2행, Members 2행의 모든 값이 동일하다.
- 정방향 시험과 역방향 시험 모두 통과했다. 역방향 시험은 롤백했으므로 현재 DB 이름은 영문이다.
- `DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS, NO_INFOMSGS`에서 위반 항목이 없었다.
- `dotnet build Book_Management\Book_Management.csproj --no-restore --verbosity minimal` 통과: 오류 0개, 기존 nullable 관련 CS8632 경고 9개.
- 실제 Repository 읽기 검증 35개 통과: 관리자 목록·검색 21개, 사용자 검색·상세·대출 목록 11개, 회원 중복 확인·존재하지 않는 계정 로그인 3개.
- 화면을 직접 조작하는 통합 테스트와 등록·수정·삭제·대출·반납 등의 쓰기 기능 실행은 하지 않았다. 프로그램을 다시 실행한 뒤 이 흐름은 별도로 확인할 수 있다.
