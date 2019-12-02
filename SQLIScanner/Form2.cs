using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLIScanner
{
    public partial class Form2 : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public Form2(string captchaurl)
        {
            InitializeComponent();
            pictureBox1.Load(captchaurl);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            MinimizeBox = false;
            button1.Enabled = false;
            
    }
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        //submit
        private void button1_Click(object sender, EventArgs e)
        {

        }
        //switch
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim().Length > 0)
            {
                button1.Enabled = true;
            }
        }
    }
}
