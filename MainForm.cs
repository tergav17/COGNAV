using System;
using System.Timers;
using System.Windows.Forms;
using COGNAV.EnvGraphics;

namespace COGNAV {
    public partial class MainForm : Form {

        private EnvironmentRedrawHandler _drawOps;
        
        public MainForm() {
            InitializeComponent();

            _drawOps = new EnvironmentRedrawHandler(environmentViewer);
        }
        
        private void redrawTimer_Elapsed(object sender, ElapsedEventArgs e) {
            _drawOps.Redraw();
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

        public EnvironmentRedrawHandler GetEnvironmentRedrawHandler() {
            return _drawOps;
        }
    }
}