using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Book_Management
{
    public partial class RegisterForm : Form
    {
        private readonly MemberRepository _repository = new();
        private readonly TextBox[] _inputBoxes;
        private readonly Dictionary<TextBox, Color> _originalColors = new();
        private string? _checkedId;
        private bool _isBusy;
        public string RegisteredId { get; private set; } = "";


        public RegisterForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void btn_register_Click(object sender, EventArgs e)
        {

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {

        }
    }
}
