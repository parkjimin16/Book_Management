using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Book_Management
{
    public partial class AdminMainForm : Form
    {
        public AdminMainForm()
        {
            InitializeComponent();
        }

        public AdminMainForm(LoginMember member) : this()
        {
            if (member.MemberCode != "01")
            {
                throw new InvalidOperationException(
                    "관리자 계정이 아닙니다.");
            }

            adminmain.Text =
                $"관리자 {member.Name}님, 환영합니다.";
        }
    }
}
