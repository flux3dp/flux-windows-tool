using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinDHCP;

namespace WinDHCP
{
    public partial class ScanForm : Form
    {
        BasicInterface form1;
        public ScanForm(BasicInterface form1)
        {
            this.form1 = form1;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //form1.scan(textBox1.Text, textBox2.Text);
            this.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
