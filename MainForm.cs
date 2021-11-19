using System;
using System.Windows.Forms;

namespace COGNAV {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            //throw new System.NotImplementedException();
        }

        private void button1_Click(object sender, EventArgs e) {
            label1.Text = @"Eat Shit, and Piss ur pants";
        }
    }
}