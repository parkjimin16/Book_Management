namespace Book_Management
{
    partial class AdminMainForm
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
            this.adminmain = new System.Windows.Forms.Label();
            this.lTitle = new System.Windows.Forms.Label();
            this.lPage = new System.Windows.Forms.Label();
            this.btnBooks = new System.Windows.Forms.Button();
            this.btnMembers = new System.Windows.Forms.Button();
            this.cmbSearchColumn = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvBookList = new System.Windows.Forms.DataGridView();
            this.btnAddBook = new System.Windows.Forms.Button();
            this.btnRequestBook = new System.Windows.Forms.Button();
            this.btnDeleteBook = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookList)).BeginInit();
            this.SuspendLayout();
            // 
            // adminmain
            // 
            this.adminmain.AutoSize = true;
            this.adminmain.Location = new System.Drawing.Point(14, 9);
            this.adminmain.Name = "adminmain";
            this.adminmain.Size = new System.Drawing.Size(38, 12);
            this.adminmain.TabIndex = 0;
            this.adminmain.Text = "label1";
            // 
            // lTitle
            // 
            this.lTitle.AutoSize = true;
            this.lTitle.Location = new System.Drawing.Point(341, 9);
            this.lTitle.Name = "lTitle";
            this.lTitle.Size = new System.Drawing.Size(41, 12);
            this.lTitle.TabIndex = 1;
            this.lTitle.Text = "도서관";
            // 
            // lPage
            // 
            this.lPage.AutoSize = true;
            this.lPage.Location = new System.Drawing.Point(21, 54);
            this.lPage.Name = "lPage";
            this.lPage.Size = new System.Drawing.Size(57, 12);
            this.lPage.TabIndex = 2;
            this.lPage.Text = "도서 관리";
            // 
            // btnBooks
            // 
            this.btnBooks.Location = new System.Drawing.Point(12, 29);
            this.btnBooks.Name = "btnBooks";
            this.btnBooks.Size = new System.Drawing.Size(75, 23);
            this.btnBooks.TabIndex = 3;
            this.btnBooks.Text = "도서 관리";
            this.btnBooks.UseVisualStyleBackColor = true;
            // 
            // btnMembers
            // 
            this.btnMembers.Location = new System.Drawing.Point(93, 29);
            this.btnMembers.Name = "btnMembers";
            this.btnMembers.Size = new System.Drawing.Size(75, 23);
            this.btnMembers.TabIndex = 4;
            this.btnMembers.Text = "회원 관리";
            this.btnMembers.UseVisualStyleBackColor = true;
            // 
            // cmbSearchColumn
            // 
            this.cmbSearchColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchColumn.FormattingEnabled = true;
            this.cmbSearchColumn.Location = new System.Drawing.Point(301, 47);
            this.cmbSearchColumn.Name = "cmbSearchColumn";
            this.cmbSearchColumn.Size = new System.Drawing.Size(58, 20);
            this.cmbSearchColumn.TabIndex = 5;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(365, 47);
            this.txtSearch.MaxLength = 200;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(174, 21);
            this.txtSearch.TabIndex = 6;
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnSearch.Location = new System.Drawing.Point(545, 47);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(66, 21);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "검색";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // dgvBookList
            // 
            this.dgvBookList.AllowUserToAddRows = false;
            this.dgvBookList.AllowUserToDeleteRows = false;
            this.dgvBookList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvBookList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBookList.Location = new System.Drawing.Point(12, 70);
            this.dgvBookList.MultiSelect = false;
            this.dgvBookList.Name = "dgvBookList";
            this.dgvBookList.ReadOnly = true;
            this.dgvBookList.RowHeadersVisible = false;
            this.dgvBookList.RowTemplate.Height = 23;
            this.dgvBookList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBookList.Size = new System.Drawing.Size(599, 368);
            this.dgvBookList.TabIndex = 8;
            // 
            // btnAddBook
            // 
            this.btnAddBook.Location = new System.Drawing.Point(617, 70);
            this.btnAddBook.Name = "btnAddBook";
            this.btnAddBook.Size = new System.Drawing.Size(170, 90);
            this.btnAddBook.TabIndex = 9;
            this.btnAddBook.Text = "신규 도서 등록";
            this.btnAddBook.UseVisualStyleBackColor = true;
            // 
            // btnRequestBook
            // 
            this.btnRequestBook.Location = new System.Drawing.Point(617, 162);
            this.btnRequestBook.Name = "btnRequestBook";
            this.btnRequestBook.Size = new System.Drawing.Size(170, 90);
            this.btnRequestBook.TabIndex = 10;
            this.btnRequestBook.Text = "신규 도서 요청 현황";
            this.btnRequestBook.UseVisualStyleBackColor = true;
            // 
            // btnDeleteBook
            // 
            this.btnDeleteBook.Location = new System.Drawing.Point(618, 252);
            this.btnDeleteBook.Name = "btnDeleteBook";
            this.btnDeleteBook.Size = new System.Drawing.Size(170, 90);
            this.btnDeleteBook.TabIndex = 11;
            this.btnDeleteBook.Text = "도서삭제";
            this.btnDeleteBook.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(617, 348);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(170, 90);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "새로고침";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // AdminMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lPage);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDeleteBook);
            this.Controls.Add(this.btnRequestBook);
            this.Controls.Add(this.btnAddBook);
            this.Controls.Add(this.dgvBookList);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.cmbSearchColumn);
            this.Controls.Add(this.btnMembers);
            this.Controls.Add(this.btnBooks);
            this.Controls.Add(this.lTitle);
            this.Controls.Add(this.adminmain);
            this.Name = "AdminMainForm";
            this.Text = "AdminMainForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvBookList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label adminmain;
        private System.Windows.Forms.Label lTitle;
        private System.Windows.Forms.Label lPage;
        private System.Windows.Forms.Button btnBooks;
        private System.Windows.Forms.Button btnMembers;
        private System.Windows.Forms.ComboBox cmbSearchColumn;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvBookList;
        private System.Windows.Forms.Button btnAddBook;
        private System.Windows.Forms.Button btnRequestBook;
        private System.Windows.Forms.Button btnDeleteBook;
        private System.Windows.Forms.Button btnRefresh;
    }
}