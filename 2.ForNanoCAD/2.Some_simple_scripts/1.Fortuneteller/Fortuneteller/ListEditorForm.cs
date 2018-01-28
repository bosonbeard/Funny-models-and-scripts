using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fortuneteller
{
    public partial class ListEditorForm : Form
    {
        private Ball ball;

        public ListEditorForm()
        {
            InitializeComponent();
        }

        public ListEditorForm(Ball ball)
        {
            this.ball = ball;
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DelBtn_Click(object sender, EventArgs e)
        {
            if (Lpredictions.SelectedItem !=null)
            {
                Lpredictions.Items.Remove(Lpredictions.SelectedItem);
            }
        }



        private void AdBtn_Click(object sender, EventArgs e)
        {
            if (textBox.Text!="" | textBox.Text != " ")
            {

                Lpredictions.Items.Add(textBox.Text);
            }

        }

        private void SaceBtn_Click(object sender, EventArgs e)
        {
            ball.predictions = Lpredictions.Items.OfType<String>().ToList();
            this.Close();
        }
    }
}
