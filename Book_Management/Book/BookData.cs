using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Management
{
    public class BookData
    {
        public int BookNumber { get; set; }

        public string Title { get; set; } = "";

        public string Author { get; set; } = "";

        public string Publisher { get; set; } = "";

        public short PublicationYear { get; set; }

        public string Category { get; set; } = "";

        public string Isbn { get; set; } = "";
    }
}
