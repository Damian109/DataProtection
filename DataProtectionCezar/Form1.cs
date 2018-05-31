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
        Cryptograph c = new Cryptograph("wap12");
        List<MyChar> list;
        string text;
        public Form1()
        {
            InitializeComponent();
            text = c.ReadFile(".txt");
            richTextBox1.Text = text;
            list = c.GetCharsFromText(text);
            dataGridView1.DataSource = list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            text = c.Crypt(richTextBox1.Text, Convert.ToInt32(textBox1.Text));
            dataGridView2.DataSource = c.GetCharsFromText(text);
            richTextBox1.Text = text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!checkBox1.Checked)
            {
                text = c.Decrypt(richTextBox1.Text, Convert.ToInt32(textBox1.Text));
                dataGridView1.DataSource = c.GetCharsFromText(text);
                richTextBox1.Text = text;
            }
            else
            {
                list = c.GetCharsFromText(richTextBox1.Text);
                listBox1.DataSource = c.FindKey(list, richTextBox1.Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            text = c.Decrypt(richTextBox1.Text, Convert.ToInt32(c.sdvig[listBox1.SelectedIndex]));
            richTextBox1.Text = text;
            dataGridView1.DataSource = c.GetCharsFromText(text);
        }
    }
}
