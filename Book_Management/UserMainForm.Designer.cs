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
            this.usermain = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // usermain
            // 
            this.usermain.AutoSize = true;
            this.usermain.Location = new System.Drawing.Point(381, 219);
            this.usermain.Name = "usermain";
            this.usermain.Size = new System.Drawing.Size(38, 12);
            this.usermain.TabIndex = 1;
            this.usermain.Text = "label1";
            // 
            // UserMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.usermain);
            this.Name = "UserMainForm";
            this.Text = "UserMainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label usermain;
    }
}