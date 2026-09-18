namespace Book_Management
{
    partial class RegisterForm
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
            this.Title = new System.Windows.Forms.Label();
            this.lName = new System.Windows.Forms.Label();
            this.Name_input = new System.Windows.Forms.TextBox();
            this.Phone = new System.Windows.Forms.Label();
            this.Phone_input = new System.Windows.Forms.TextBox();
            this.Id = new System.Windows.Forms.Label();
            this.Id_input = new System.Windows.Forms.TextBox();
            this.Pw = new System.Windows.Forms.Label();
            this.Pw_input = new System.Windows.Forms.TextBox();
            this.CheckId = new System.Windows.Forms.Button();
            this.Id_check_mark = new System.Windows.Forms.Label();
            this.CheckPw = new System.Windows.Forms.Label();
            this.CheckPw_input = new System.Windows.Forms.TextBox();
            this.btn_register = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_idcheck = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Font = new System.Drawing.Font("휴먼모음T", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Title.Location = new System.Drawing.Point(147, 58);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(145, 30);
            this.Title.TabIndex = 0;
            this.Title.Text = "신규회원가입";
            // 
            // lName
            // 
            this.lName.AutoSize = true;
            this.lName.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lName.Location = new System.Drawing.Point(67, 150);
            this.lName.Name = "lName";
            this.lName.Size = new System.Drawing.Size(44, 23);
            this.lName.TabIndex = 0;
            this.lName.Text = "이름";
            // 
            // Name_input
            // 
            this.Name_input.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name_input.Location = new System.Drawing.Point(210, 140);
            this.Name_input.MaxLength = 50;
            this.Name_input.Name = "Name_input";
            this.Name_input.Size = new System.Drawing.Size(139, 32);
            this.Name_input.TabIndex = 1;
            // 
            // Phone
            // 
            this.Phone.AutoSize = true;
            this.Phone.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Phone.Location = new System.Drawing.Point(67, 210);
            this.Phone.Name = "Phone";
            this.Phone.Size = new System.Drawing.Size(61, 23);
            this.Phone.TabIndex = 0;
            this.Phone.Text = "연락처";
            this.Phone.Click += new System.EventHandler(this.label1_Click);
            // 
            // Phone_input
            // 
            this.Phone_input.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Phone_input.Location = new System.Drawing.Point(210, 200);
            this.Phone_input.MaxLength = 20;
            this.Phone_input.Name = "Phone_input";
            this.Phone_input.Size = new System.Drawing.Size(139, 32);
            this.Phone_input.TabIndex = 1;
            this.Phone_input.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Id
            // 
            this.Id.AutoSize = true;
            this.Id.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Id.Location = new System.Drawing.Point(67, 270);
            this.Id.Name = "Id";
            this.Id.Size = new System.Drawing.Size(61, 23);
            this.Id.TabIndex = 0;
            this.Id.Text = "아이디";
            this.Id.Click += new System.EventHandler(this.label1_Click);
            // 
            // Id_input
            // 
            this.Id_input.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Id_input.Location = new System.Drawing.Point(210, 260);
            this.Id_input.MaxLength = 20;
            this.Id_input.Name = "Id_input";
            this.Id_input.Size = new System.Drawing.Size(139, 32);
            this.Id_input.TabIndex = 1;
            this.Id_input.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Pw
            // 
            this.Pw.AutoSize = true;
            this.Pw.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Pw.Location = new System.Drawing.Point(67, 330);
            this.Pw.Name = "Pw";
            this.Pw.Size = new System.Drawing.Size(78, 23);
            this.Pw.TabIndex = 0;
            this.Pw.Text = "비밀번호";
            this.Pw.Click += new System.EventHandler(this.label1_Click);
            // 
            // Pw_input
            // 
            this.Pw_input.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Pw_input.Location = new System.Drawing.Point(210, 320);
            this.Pw_input.MaxLength = 256;
            this.Pw_input.Name = "Pw_input";
            this.Pw_input.Size = new System.Drawing.Size(139, 32);
            this.Pw_input.TabIndex = 1;
            this.Pw_input.UseSystemPasswordChar = true;
            this.Pw_input.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // CheckId
            // 
            this.CheckId.Location = new System.Drawing.Point(441, 112);
            this.CheckId.Name = "CheckId";
            this.CheckId.Size = new System.Drawing.Size(75, 25);
            this.CheckId.TabIndex = 2;
            this.CheckId.Text = "중복확인";
            this.CheckId.UseVisualStyleBackColor = true;
            // 
            // Id_check_mark
            // 
            this.Id_check_mark.AutoSize = true;
            this.Id_check_mark.BackColor = System.Drawing.Color.White;
            this.Id_check_mark.Font = new System.Drawing.Font("돋움", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Id_check_mark.ForeColor = System.Drawing.Color.Green;
            this.Id_check_mark.Location = new System.Drawing.Point(330, 267);
            this.Id_check_mark.Name = "Id_check_mark";
            this.Id_check_mark.Size = new System.Drawing.Size(16, 16);
            this.Id_check_mark.TabIndex = 3;
            this.Id_check_mark.Text = "✓";
            this.Id_check_mark.Visible = false;
            // 
            // CheckPw
            // 
            this.CheckPw.AutoSize = true;
            this.CheckPw.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.CheckPw.Location = new System.Drawing.Point(67, 385);
            this.CheckPw.Name = "CheckPw";
            this.CheckPw.Size = new System.Drawing.Size(124, 23);
            this.CheckPw.TabIndex = 0;
            this.CheckPw.Text = "비밀번호 확인";
            this.CheckPw.Click += new System.EventHandler(this.label1_Click);
            // 
            // CheckPw_input
            // 
            this.CheckPw_input.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.CheckPw_input.Location = new System.Drawing.Point(210, 380);
            this.CheckPw_input.MaxLength = 256;
            this.CheckPw_input.Name = "CheckPw_input";
            this.CheckPw_input.Size = new System.Drawing.Size(139, 32);
            this.CheckPw_input.TabIndex = 1;
            this.CheckPw_input.UseSystemPasswordChar = true;
            this.CheckPw_input.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btn_register
            // 
            this.btn_register.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_register.Location = new System.Drawing.Point(167, 461);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(125, 40);
            this.btn_register.TabIndex = 4;
            this.btn_register.Text = "회원가입";
            this.btn_register.UseVisualStyleBackColor = true;
            this.btn_register.Click += new System.EventHandler(this.btn_register_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Font = new System.Drawing.Font("휴먼모음T", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_cancel.Location = new System.Drawing.Point(167, 507);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(125, 40);
            this.btn_cancel.TabIndex = 4;
            this.btn_cancel.Text = "취소";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_idcheck
            // 
            this.btn_idcheck.AutoSize = true;
            this.btn_idcheck.Font = new System.Drawing.Font("휴먼모음T", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_idcheck.Location = new System.Drawing.Point(353, 260);
            this.btn_idcheck.Name = "btn_idcheck";
            this.btn_idcheck.Size = new System.Drawing.Size(83, 33);
            this.btn_idcheck.TabIndex = 5;
            this.btn_idcheck.Text = "중복체크";
            this.btn_idcheck.UseVisualStyleBackColor = true;
            // 
            // RegisterForm
            // 
            this.AcceptButton = this.btn_register;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.CancelButton = this.btn_cancel;
            this.ClientSize = new System.Drawing.Size(440, 608);
            this.Controls.Add(this.btn_idcheck);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.Id_check_mark);
            this.Controls.Add(this.CheckId);
            this.Controls.Add(this.CheckPw_input);
            this.Controls.Add(this.CheckPw);
            this.Controls.Add(this.Pw_input);
            this.Controls.Add(this.Pw);
            this.Controls.Add(this.Id_input);
            this.Controls.Add(this.Id);
            this.Controls.Add(this.Phone_input);
            this.Controls.Add(this.Phone);
            this.Controls.Add(this.Name_input);
            this.Controls.Add(this.lName);
            this.Controls.Add(this.Title);
            this.Font = new System.Drawing.Font("휴먼모음T", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "회원가입";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label lName;
        private System.Windows.Forms.TextBox Name_input;
        private System.Windows.Forms.Label Phone;
        private System.Windows.Forms.TextBox Phone_input;
        private System.Windows.Forms.Label Id;
        private System.Windows.Forms.TextBox Id_input;
        private System.Windows.Forms.Label Pw;
        private System.Windows.Forms.TextBox Pw_input;
        private System.Windows.Forms.Button CheckId;
        private System.Windows.Forms.Label Id_check_mark;
        private System.Windows.Forms.Label CheckPw;
        private System.Windows.Forms.TextBox CheckPw_input;
        private System.Windows.Forms.Button btn_register;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_idcheck;
    }
}