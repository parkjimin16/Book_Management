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
            //using var loginForm = new LoginForm();
            //로그인창 제일 먼저 실행
            while (true)
            {
                LoginMember member;

                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    member = loginForm.CurrentMember;
                }

                if (member == null)
                {
                    return;
                }

                if (member.MemberCode == "01")
                {
                    using var adminForm = new AdminMainForm(member);
                    Application.Run(adminForm);
                    return;
                }

                if (member.MemberCode != "02")
                {
                    return;
                }

                using var userForm = new UserMainForm(member);
                Application.Run(userForm);

                if (!userForm.LogoutRequested)
                {
                    return;
                }
            }
        }
    }
}
