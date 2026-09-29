# 도서 엑셀 가져오기 (2026-09-29)

`20260929_books.json`은 사용자가 제공한 `도서.xlsx`의 `Sheet1!A2:F68`에서 추출한 67개 도서다.
엑셀의 제목, isbn, 작가, 출판사, 출판연도, 카테고리를 각각 Title, Isbn, Author,
Publisher, PublicationYear, Category에 대응시켰다. Row는 엑셀 원본 행 번호다.
원본 엑셀은 수정하지 않았다.

사용자의 확인에 따라 다음 분류를 변환했다.

- 47행 코스모스: 과학 → 자연과학
- 56행, 62행 ETS 토익 교재: 외국어 → 언어

## 적용 결과

- LibraryDB.dbo.Books에 67행 등록 및 커밋 완료. 관리번호 10~76.
- 기존 6행과 합쳐 총 73행. 기존 데이터와 DB 스키마는 변경하지 않았다.
- 신규 도서는 모두 대출 가능, 대출자·반납예정일 NULL, 조회수 0.
- 필수값, 길이, 연도, 카테고리, ISBN 형식 및 파일/DB의 ISBN 중복 검사 통과.
- 트랜잭션 내부에서 67행의 모든 입력 필드와 초기 상태를 검증했다.
- 커밋 후 별도 연결에서 총 건수, 신규 도서 초기 상태 및 변환 분류를 확인했다.
- .NET Framework 4.8 프로젝트 빌드 성공: 경고 0, 오류 0. 화면 직접 조작은 하지 않았다.

## 스크립트 사용

저장소 루트에서 Windows PowerShell 5.1로 실행한다. 빌드된 프로그램의 기존
DatabaseConfig와 Microsoft.Data.SqlClient를 사용한다.

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/Import-Books.ps1 -DataPath database/imports/20260929_books.json -BinDirectory Book_Management/bin/Debug
```

기본 동작은 읽기 전용 검증이다. 실제 등록은 `-Commit` 옵션으로 수행하며,
중복 확인과 INSERT를 하나의 트랜잭션으로 처리한다. 커밋 모드에서는 도서 테이블을
잠시 잠그므로 다른 도서 변경 요청이 기다릴 수 있다. 기존 ISBN이 있으면 전체 작업을 중단한다.
입력 데이터는 타입과 길이를 지정한 SQL 매개변수로 전달한다.

**위 데이터는 이미 등록되어 있으므로 재실행 시 중복 검사에서 중단되는 것이 정상이다.**
일부만 건너뛰거나 기존 도서를 덮어쓰지 않는다. 실행 도중 검증이나 INSERT에 실패하면
전체 트랜잭션을 롤백한다. 커밋 후의 자동 삭제/복구 기능은 제공하지 않는다.

## 저자 역할 표기 정리 (2026-09-29)

사용자 요청으로 Books의 저자 값 69개를 정리했고, 전체 73개 도서의 저자 표기를 검증했다.
쉼표로 구분된 각 기여자 이름 뒤의 지음, 엮음, 감수, 글, 그림, 원작, 편역, 옮김, 구성과
그 앞의 '외'를 제거했다. 여러 기여자의 이름은 모두 유지했다.
가져오기 JSON의 67개 저자 값에도 동일한 정리를 적용했으며 원본 엑셀은 수정하지 않았다.

- 작업 스크립트: `scripts/Normalize-BookAuthors.ps1`
- 변경 전후 값: `20260929_authors_before_cleanup.json` (BookNumber, Before, After)
- DB 변경은 매개변수화된 UPDATE를 단일 트랜잭션으로 실행했다. 관리번호와 변경 전
  저자가 모두 일치할 때만 수정하며, 영향받은 행 수와 정리 결과를 확인한 뒤 커밋했다.
- 복구가 필요하면 백업의 BookNumber와 After가 현재 값과 일치하는지 확인한 다음
  Before로 되돌린다. 이후 변경된 저자 값을 덮어쓰지 않도록 주의한다.
