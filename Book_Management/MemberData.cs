using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Management
{
    public class MemberData
    {
        public int MemberNumber { get; set; }

        public string Name { get; set; } = "";

        public string Phone { get; set; } = "";

        public string LoginId { get; set; } = "";

        public enum MemberUpdateResult
        {
            Success,
            DuplicatePhone,
            NotFound
        }

        public enum MemberDeleteResult
        {
            Success,
            HasLoans,
            NotFound
        }
    }
}
