namespace Book_Management
{
    partial class UserMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.usermain = new System.Windows.Forms.Label();
            this.dgvBooks = new System.Windows.Forms.DataGridView();
            this.colTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPublisher = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLoanStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDueDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlFilters = new System.Windows.Forms.Panel();
            this.txtYear = new System.Windows.Forms.TextBox();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.cmbAvailability = new System.Windows.Forms.ComboBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.cmbSearchType = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnLoans = new System.Windows.Forms.Button();
            this.btnRequest = new System.Windows.Forms.Button();
            this.lblPage = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlList = new System.Windows.Forms.Panel();
            this.pnlLoanActions = new System.Windows.Forms.Panel();
            this.btnReturn = new System.Windows.Forms.Button();
            this.btnRefreshLoans = new System.Windows.Forms.Button();
            this.pnlRequest = new System.Windows.Forms.Panel();
            this.txtRequestPublisher = new System.Windows.Forms.TextBox();
            this.cmbRequestCategory = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveRequest = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRequestAuthor = new System.Windows.Forms.TextBox();
            this.txtRequestIsbn = new System.Windows.Forms.TextBox();
            this.txtRequestTitle = new System.Windows.Forms.TextBox();
            this.txtRequestYear = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).BeginInit();
            this.pnlFilters.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlLoanActions.SuspendLayout();
            this.pnlRequest.SuspendLayout();
            this.SuspendLayout();
            // 
            // usermain
            // 
            this.usermain.AutoSize = true;
            this.usermain.Location = new System.Drawing.Point(10, 9);
            this.usermain.Name = "usermain";
            this.usermain.Size = new System.Drawing.Size(38, 12);
            this.usermain.TabIndex = 1;
            this.usermain.Text = "label1";
            // 
            // dgvBooks
            // 
            this.dgvBooks.AllowUserToAddRows = false;
            this.dgvBooks.AllowUserToDeleteRows = false;
            this.dgvBooks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBooks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBooks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTitle,
            this.colAuthor,
            this.colPublisher,
            this.colYear,
            this.colCategory,
            this.colLoanStatus,
            this.colDueDate});
            this.dgvBooks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBooks.Location = new System.Drawing.Point(0, 0);
            this.dgvBooks.MultiSelect = false;
            this.dgvBooks.Name = "dgvBooks";
            this.dgvBooks.ReadOnly = true;
            this.dgvBooks.RowHeadersVisible = false;
            this.dgvBooks.RowTemplate.Height = 23;
            this.dgvBooks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBooks.Size = new System.Drawing.Size(773, 269);
            this.dgvBooks.TabIndex = 2;
            // 
            // colTitle
            // 
            this.colTitle.DataPropertyName = "제목";
            this.colTitle.HeaderText = "제목";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colTitle.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colAuthor
            // 
            this.colAuthor.DataPropertyName = "저자";
            this.colAuthor.HeaderText = "저자";
            this.colAuthor.Name = "colAuthor";
            this.colAuthor.ReadOnly = true;
            this.colAuthor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colPublisher
            // 
            this.colPublisher.DataPropertyName = "출판사";
            this.colPublisher.HeaderText = "출판사";
            this.colPublisher.Name = "colPublisher";
            this.colPublisher.ReadOnly = true;
            this.colPublisher.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colYear
            // 
            this.colYear.DataPropertyName = "발행연도";
            this.colYear.HeaderText = "발행연도";
            this.colYear.Name = "colYear";
            this.colYear.ReadOnly = true;
            this.colYear.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCategory
            // 
            this.colCategory.DataPropertyName = "카테고리";
            this.colCategory.HeaderText = "카테고리";
            this.colCategory.Name = "colCategory";
            this.colCategory.ReadOnly = true;
            this.colCategory.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colLoanStatus
            // 
            this.colLoanStatus.DataPropertyName = "대출여부";
            this.colLoanStatus.HeaderText = "대출 가능 여부";
            this.colLoanStatus.Name = "colLoanStatus";
            this.colLoanStatus.ReadOnly = true;
            this.colLoanStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colDueDate
            // 
            this.colDueDate.DataPropertyName = "반납일";
            dataGridViewCellStyle8.Format = "yyyy-MM-dd";
            this.colDueDate.DefaultCellStyle = dataGridViewCellStyle8;
            this.colDueDate.HeaderText = "반납 예정일";
            this.colDueDate.Name = "colDueDate";
            this.colDueDate.ReadOnly = true;
            this.colDueDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(374, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(41, 12);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "도서관";
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(713, 4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 3;
            this.btnLogout.Text = "로그아웃";
            this.btnLogout.UseVisualStyleBackColor = true;
            // 
            // pnlFilters
            // 
            this.pnlFilters.Controls.Add(this.txtYear);
            this.pnlFilters.Controls.Add(this.txtKeyword);
            this.pnlFilters.Controls.Add(this.cmbAvailability);
            this.pnlFilters.Controls.Add(this.cmbCategory);
            this.pnlFilters.Controls.Add(this.cmbSearchType);
            this.pnlFilters.Location = new System.Drawing.Point(12, 32);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(776, 34);
            this.pnlFilters.TabIndex = 4;
            this.pnlFilters.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlFilters_Paint);
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(418, 4);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(93, 21);
            this.txtYear.TabIndex = 1;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(94, 3);
            this.txtKeyword.MaxLength = 200;
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(309, 21);
            this.txtKeyword.TabIndex = 1;
            // 
            // cmbAvailability
            // 
            this.cmbAvailability.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAvailability.FormattingEnabled = true;
            this.cmbAvailability.Location = new System.Drawing.Point(648, 4);
            this.cmbAvailability.Name = "cmbAvailability";
            this.cmbAvailability.Size = new System.Drawing.Size(121, 20);
            this.cmbAvailability.TabIndex = 0;
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(521, 4);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(121, 20);
            this.cmbCategory.TabIndex = 0;
            // 
            // cmbSearchType
            // 
            this.cmbSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchType.FormattingEnabled = true;
            this.cmbSearchType.Location = new System.Drawing.Point(3, 3);
            this.cmbSearchType.Name = "cmbSearchType";
            this.cmbSearchType.Size = new System.Drawing.Size(90, 20);
            this.cmbSearchType.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(12, 72);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 32);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "검색";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnLoans
            // 
            this.btnLoans.Location = new System.Drawing.Point(138, 72);
            this.btnLoans.Name = "btnLoans";
            this.btnLoans.Size = new System.Drawing.Size(120, 32);
            this.btnLoans.TabIndex = 5;
            this.btnLoans.Text = "대출/반납";
            this.btnLoans.UseVisualStyleBackColor = true;
            // 
            // btnRequest
            // 
            this.btnRequest.Location = new System.Drawing.Point(264, 72);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(120, 32);
            this.btnRequest.TabIndex = 5;
            this.btnRequest.Text = "신규 도서 요청";
            this.btnRequest.UseVisualStyleBackColor = true;
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Location = new System.Drawing.Point(13, 107);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(57, 12);
            this.lblPage.TabIndex = 0;
            this.lblPage.Text = "검색 결과";
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.pnlList);
            this.pnlContent.Controls.Add(this.pnlRequest);
            this.pnlContent.Location = new System.Drawing.Point(15, 126);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(773, 314);
            this.pnlContent.TabIndex = 1;
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.dgvBooks);
            this.pnlList.Controls.Add(this.pnlLoanActions);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 0);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(773, 314);
            this.pnlList.TabIndex = 0;
            this.pnlList.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlList_Paint);
            // 
            // pnlLoanActions
            // 
            this.pnlLoanActions.Controls.Add(this.btnReturn);
            this.pnlLoanActions.Controls.Add(this.btnRefreshLoans);
            this.pnlLoanActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLoanActions.Location = new System.Drawing.Point(0, 269);
            this.pnlLoanActions.Name = "pnlLoanActions";
            this.pnlLoanActions.Size = new System.Drawing.Size(773, 45);
            this.pnlLoanActions.TabIndex = 3;
            // 
            // btnReturn
            // 
            this.btnReturn.Location = new System.Drawing.Point(181, 3);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(148, 39);
            this.btnReturn.TabIndex = 0;
            this.btnReturn.Text = "선택도서반납";
            this.btnReturn.UseVisualStyleBackColor = true;
            // 
            // btnRefreshLoans
            // 
            this.btnRefreshLoans.Location = new System.Drawing.Point(0, 3);
            this.btnRefreshLoans.Name = "btnRefreshLoans";
            this.btnRefreshLoans.Size = new System.Drawing.Size(148, 39);
            this.btnRefreshLoans.TabIndex = 0;
            this.btnRefreshLoans.Text = "새로고침";
            this.btnRefreshLoans.UseVisualStyleBackColor = true;
            // 
            // pnlRequest
            // 
            this.pnlRequest.Controls.Add(this.txtRequestPublisher);
            this.pnlRequest.Controls.Add(this.cmbRequestCategory);
            this.pnlRequest.Controls.Add(this.label6);
            this.pnlRequest.Controls.Add(this.label4);
            this.pnlRequest.Controls.Add(this.label5);
            this.pnlRequest.Controls.Add(this.label3);
            this.pnlRequest.Controls.Add(this.label1);
            this.pnlRequest.Controls.Add(this.btnSaveRequest);
            this.pnlRequest.Controls.Add(this.label2);
            this.pnlRequest.Controls.Add(this.txtRequestAuthor);
            this.pnlRequest.Controls.Add(this.txtRequestIsbn);
            this.pnlRequest.Controls.Add(this.txtRequestTitle);
            this.pnlRequest.Controls.Add(this.txtRequestYear);
            this.pnlRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRequest.Location = new System.Drawing.Point(0, 0);
            this.pnlRequest.Name = "pnlRequest";
            this.pnlRequest.Size = new System.Drawing.Size(773, 314);
            this.pnlRequest.TabIndex = 1;
            // 
            // txtRequestPublisher
            // 
            this.txtRequestPublisher.Location = new System.Drawing.Point(236, 111);
            this.txtRequestPublisher.MaxLength = 100;
            this.txtRequestPublisher.Name = "txtRequestPublisher";
            this.txtRequestPublisher.Size = new System.Drawing.Size(100, 21);
            this.txtRequestPublisher.TabIndex = 4;
            // 
            // cmbRequestCategory
            // 
            this.cmbRequestCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRequestCategory.FormattingEnabled = true;
            this.cmbRequestCategory.Location = new System.Drawing.Point(236, 165);
            this.cmbRequestCategory.Name = "cmbRequestCategory";
            this.cmbRequestCategory.Size = new System.Drawing.Size(100, 20);
            this.cmbRequestCategory.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(174, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "ISBN";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(174, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "발행연도";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(174, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "카테고리";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(174, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "출판사";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "제목";
            // 
            // btnSaveRequest
            // 
            this.btnSaveRequest.Location = new System.Drawing.Point(219, 246);
            this.btnSaveRequest.Name = "btnSaveRequest";
            this.btnSaveRequest.Size = new System.Drawing.Size(75, 23);
            this.btnSaveRequest.TabIndex = 5;
            this.btnSaveRequest.Text = "요청등록";
            this.btnSaveRequest.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(174, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "저자";
            // 
            // txtRequestAuthor
            // 
            this.txtRequestAuthor.Location = new System.Drawing.Point(236, 84);
            this.txtRequestAuthor.MaxLength = 100;
            this.txtRequestAuthor.Name = "txtRequestAuthor";
            this.txtRequestAuthor.Size = new System.Drawing.Size(100, 21);
            this.txtRequestAuthor.TabIndex = 4;
            // 
            // txtRequestIsbn
            // 
            this.txtRequestIsbn.Location = new System.Drawing.Point(236, 191);
            this.txtRequestIsbn.MaxLength = 13;
            this.txtRequestIsbn.Name = "txtRequestIsbn";
            this.txtRequestIsbn.Size = new System.Drawing.Size(100, 21);
            this.txtRequestIsbn.TabIndex = 4;
            // 
            // txtRequestTitle
            // 
            this.txtRequestTitle.Location = new System.Drawing.Point(236, 57);
            this.txtRequestTitle.MaxLength = 200;
            this.txtRequestTitle.Name = "txtRequestTitle";
            this.txtRequestTitle.Size = new System.Drawing.Size(100, 21);
            this.txtRequestTitle.TabIndex = 4;
            // 
            // txtRequestYear
            // 
            this.txtRequestYear.Location = new System.Drawing.Point(236, 138);
            this.txtRequestYear.MaxLength = 4;
            this.txtRequestYear.Name = "txtRequestYear";
            this.txtRequestYear.Size = new System.Drawing.Size(100, 21);
            this.txtRequestYear.TabIndex = 4;
            // 
            // UserMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.usermain);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.pnlFilters);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnLoans);
            this.Controls.Add(this.btnRequest);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.pnlContent);
            this.Name = "UserMainForm";
            this.Text = "UserMainForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).EndInit();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.pnlLoanActions.ResumeLayout(false);
            this.pnlRequest.ResumeLayout(false);
            this.pnlRequest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label usermain;
        private System.Windows.Forms.DataGridView dgvBooks;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnLoans;
        private System.Windows.Forms.Button btnRequest;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.TextBox txtYear;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.ComboBox cmbAvailability;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.ComboBox cmbSearchType;
        private System.Windows.Forms.Panel pnlRequest;
        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.Panel pnlLoanActions;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.Button btnRefreshLoans;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAuthor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPublisher;
        private System.Windows.Forms.DataGridViewTextBoxColumn colYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLoanStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDueDate;
        private System.Windows.Forms.TextBox txtRequestAuthor;
        private System.Windows.Forms.TextBox txtRequestPublisher;
        private System.Windows.Forms.TextBox txtRequestYear;
        private System.Windows.Forms.TextBox txtRequestTitle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRequestCategory;
        private System.Windows.Forms.Button btnSaveRequest;
        private System.Windows.Forms.TextBox txtRequestIsbn;
    }
}