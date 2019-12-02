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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            MessageBox.Show(this, "To be implemented...");
            //System.Diagnostics.Process.Start("http://www.microsoft.com");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(int.Parse(label3.Text) != 0)
            {
                int num = int.Parse(label3.Text) - 1;
                label3.Text = num.ToString();
            } else
            {
                this.Close();
            }
        }
    }
}
