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

        private void MainForm_FormClosing(Object sender, FormClosingEventArgs e) {
            Application.Exit();
            Environment.Exit(0);
        }

        public System.Windows.Forms.RichTextBox GetGraphicConsole() {
            return gConsole;
        }

        public System.Windows.Forms.Label GetLeftStickLabel() {
            return leftStickLabel;
        }

        public System.Windows.Forms.Label GetRightStickLabel() {
            return rightStickLabel;
        }

        public System.Windows.Forms.ComboBox GetGamepadMenu() {
            return gamepadSelection;
        }
    }
}