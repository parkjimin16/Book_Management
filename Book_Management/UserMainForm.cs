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
    public partial class UserMainForm : Form
    {
        public UserMainForm()
        {
            InitializeComponent();
        }
        public UserMainForm(LoginMember member) : this()
        {
            if (member.MemberCode != "02")
            {
                throw new InvalidOperationException(
                    "일반사용자 계정이 아닙니다.");
            }

            usermain.Text =
                $"{member.Name}님, 환영합니다.";
        }
    }
}
