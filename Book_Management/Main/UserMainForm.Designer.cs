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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.label7 = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).BeginInit();
            this.pnlFilters.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlLoanActions.SuspendLayout();
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
            dataGridViewCellStyle7.Format = "yyyy-MM-dd";
            this.colDueDate.DefaultCellStyle = dataGridViewCellStyle7;
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
            this.lblTitle.Size = new System.Drawing.Size(65, 12);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "ZDA도서관";
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
            this.pnlFilters.Controls.Add(this.label7);
            this.pnlFilters.Controls.Add(this.txtYear);
            this.pnlFilters.Controls.Add(this.txtKeyword);
            this.pnlFilters.Controls.Add(this.cmbAvailability);
            this.pnlFilters.Controls.Add(this.cmbCategory);
            this.pnlFilters.Controls.Add(this.cmbSearchType);
            this.pnlFilters.Location = new System.Drawing.Point(12, 32);
            this.pnlFilters.Name = "pnlFilters";
            this.pnlFilters.Size = new System.Drawing.Size(776, 34);
            this.pnlFilters.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(409, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "발행연도";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtYear
            // 
            this.txtYear.Location = new System.Drawing.Point(468, 6);
            this.txtYear.MaxLength = 4;
            this.txtYear.Name = "txtYear";
            this.txtYear.Size = new System.Drawing.Size(111, 21);
            this.txtYear.TabIndex = 1;
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(94, 5);
            this.txtKeyword.MaxLength = 200;
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(309, 21);
            this.txtKeyword.TabIndex = 1;
            // 
            // cmbAvailability
            // 
            this.cmbAvailability.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAvailability.FormattingEnabled = true;
            this.cmbAvailability.Location = new System.Drawing.Point(701, 7);
            this.cmbAvailability.Name = "cmbAvailability";
            this.cmbAvailability.Size = new System.Drawing.Size(68, 20);
            this.cmbAvailability.TabIndex = 0;
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(585, 7);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(110, 20);
            this.cmbCategory.TabIndex = 0;
            // 
            // cmbSearchType
            // 
            this.cmbSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchType.FormattingEnabled = true;
            this.cmbSearchType.Location = new System.Drawing.Point(3, 6);
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
            // 
            // pnlLoanActions
            // 
            this.pnlLoanActions.AutoScroll = true;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UserMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZDA도서관";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBooks)).EndInit();
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.pnlLoanActions.ResumeLayout(false);
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
        private System.Windows.Forms.Label label7;
    }
}