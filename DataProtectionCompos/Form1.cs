using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zashita_lab1
{
    public partial class Form1 : Form
    {
        Cryptograph c = new Cryptograph("text");
        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Value = 1;
            byte b;
            int t;
            button1.Click += (s, e) => richTextBox1.Text = c.Crypt(textBox1.Text, (int)numericUpDown1.Value);
            button2.Click += (s, e) => richTextBox1.Text = c.Decrypt(textBox1.Text, (int)numericUpDown1.Value);
        }
    }
}
