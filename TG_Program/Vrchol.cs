using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TG_Program
{
    public partial class Vrchol : UserControl
    {
        public Action<Vrchol> clicked;
        public Action<Vrchol> newHranaIniciated;
        public int id;
        public Vrchol(int id)
        {
            this.id = id;
            InitializeComponent();
            label1.Text = "V" + id.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newHranaIniciated.Invoke(this);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            clicked.Invoke(this);
        }
    }
}
