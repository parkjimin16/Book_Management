namespace Book_Management
{
    partial class LoginForm
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
            this.ID = new System.Windows.Forms.Label();
            this.ID_input = new System.Windows.Forms.TextBox();
            this.PW = new System.Windows.Forms.Label();
            this.PW_input = new System.Windows.Forms.TextBox();
            this.btn_login = new System.Windows.Forms.Button();
            this.btn_register = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ID
            // 
            this.ID.AutoSize = true;
            this.ID.Location = new System.Drawing.Point(307, 287);
            this.ID.Name = "ID";
            this.ID.Size = new System.Drawing.Size(41, 12);
            this.ID.TabIndex = 0;
            this.ID.Text = "아이디";
            // 
            // ID_input
            // 
            this.ID_input.Location = new System.Drawing.Point(366, 282);
            this.ID_input.MaxLength = 50;
            this.ID_input.Name = "ID_input";
            this.ID_input.Size = new System.Drawing.Size(151, 21);
            this.ID_input.TabIndex = 1;
            // 
            // PW
            // 
            this.PW.AutoSize = true;
            this.PW.Location = new System.Drawing.Point(307, 314);
            this.PW.Name = "PW";
            this.PW.Size = new System.Drawing.Size(53, 12);
            this.PW.TabIndex = 0;
            this.PW.Text = "비밀번호";
            // 
            // PW_input
            // 
            this.PW_input.Location = new System.Drawing.Point(366, 309);
            this.PW_input.MaxLength = 256;
            this.PW_input.Name = "PW_input";
            this.PW_input.Size = new System.Drawing.Size(151, 21);
            this.PW_input.TabIndex = 1;
            this.PW_input.UseSystemPasswordChar = true;
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(535, 282);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(75, 23);
            this.btn_login.TabIndex = 2;
            this.btn_login.Text = "로그인";
            this.btn_login.UseVisualStyleBackColor = true;
            // 
            // btn_register
            // 
            this.btn_register.Location = new System.Drawing.Point(535, 309);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(75, 23);
            this.btn_register.TabIndex = 3;
            this.btn_register.Text = "회원가입";
            this.btn_register.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Book_Management.Properties.Resources.zda_icon_white_CzZmuRP6;
            this.pictureBox1.Location = new System.Drawing.Point(226, 112);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(123, 123);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(355, 140);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 64);
            this.label1.TabIndex = 5;
            this.label1.Text = "ZDA도서관";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btn_login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(227)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(929, 466);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ID_input);
            this.Controls.Add(this.PW_input);
            this.Controls.Add(this.btn_login);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.PW);
            this.Controls.Add(this.ID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ZDA도서관-로그인";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ID;
        private System.Windows.Forms.TextBox ID_input;
        private System.Windows.Forms.Label PW;
        private System.Windows.Forms.TextBox PW_input;
        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Button btn_register;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
    }
}