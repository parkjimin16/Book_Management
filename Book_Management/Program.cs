using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Book_Management
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            using var loginForm = new LoginForm();
            //로그인창 제일 먼저 실행
            DialogResult result = loginForm.ShowDialog();

            // 로그인하지 않고 창을 닫으면 프로그램 종료
            if (result != DialogResult.OK)
            {
                return;
            }

            LoginMember? member = loginForm.CurrentMember;

            if (member == null)
            {
                return;
            }

            if (member.MemberCode == "01")
            {
                using var mainForm = new AdminMainForm(member);
                Application.Run(mainForm);
            }

            else if (member.MemberCode == "02")
            {
                using var mainForm = new UserMainForm(member);
                Application.Run(mainForm);
            }
        }
    }
}
