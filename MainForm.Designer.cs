using System.Windows.Forms;

namespace COGNAV {
    partial class MainForm {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.gConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // gConsole
            // 
            this.gConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.gConsole.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.gConsole.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.gConsole.Location = new System.Drawing.Point(12, 12);
            this.gConsole.Name = "gConsole";
            this.gConsole.ReadOnly = true;
            this.gConsole.Size = new System.Drawing.Size(543, 406);
            this.gConsole.TabIndex = 2;
            this.gConsole.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 430);
            this.Controls.Add(this.gConsole);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "COGNAV Control Panel";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.RichTextBox gConsole;

        #endregion
    }
}