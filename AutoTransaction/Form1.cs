using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoTransaction
{
    public partial class Form1 : Form
    {
        public Form1()
        {   
            
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.A_param[0] = Convert.ToDouble(A1.Text);
            MainForm.A_param[1] = Convert.ToDouble(A2.Text);
            MainForm.A_param[2] = Convert.ToDouble(A3.Text);
            MainForm.A_param[3] = Convert.ToDouble(A4.Text);
            MainForm.A_param[4] = Convert.ToDouble(A5.Text);
            MainForm.B_param[0] = Convert.ToDouble(B1.Text);
            MainForm.B_param[1] = Convert.ToDouble(B2.Text);
            MainForm.B_param[2] = Convert.ToDouble(B3.Text);
            MainForm.C_param[0] = Convert.ToDouble(C1.Text);
            MainForm.C_param[1] = Convert.ToDouble(C2.Text);
            MainForm.C_param[2] = Convert.ToDouble(C3.Text);
            MainForm.C_param[3] = Convert.ToDouble(C4.Text);
            MainForm.SaveInifile();
            
            MessageBox.Show("保存成功");
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            A1.Text = MainForm.A_param[0].ToString();
            AA2.Text = A2.Text = MainForm.A_param[1].ToString();
            AA3.Text = A3.Text = MainForm.A_param[2].ToString();
            A4.Text = MainForm.A_param[3].ToString();
            AA5.Text = A5.Text = MainForm.A_param[4].ToString();
            B1.Text = MainForm.B_param[0].ToString();
            B2.Text = MainForm.B_param[1].ToString();
            B3.Text = MainForm.B_param[2].ToString();
            C1.Text = MainForm.C_param[0].ToString();
            C2.Text = MainForm.C_param[1].ToString();
            C3.Text = MainForm.C_param[2].ToString();
            C4.Text = MainForm.C_param[3].ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
