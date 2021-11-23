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
            this.gamepadSelection = new System.Windows.Forms.ComboBox();
            this.leftStickLabel = new System.Windows.Forms.Label();
            this.rightStickLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // gConsole
            // 
            this.gConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.gConsole.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.gConsole.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.gConsole.Location = new System.Drawing.Point(12, 39);
            this.gConsole.Name = "gConsole";
            this.gConsole.ReadOnly = true;
            this.gConsole.Size = new System.Drawing.Size(543, 372);
            this.gConsole.TabIndex = 2;
            this.gConsole.Text = "";
            // 
            // gamepadSelection
            // 
            this.gamepadSelection.FormattingEnabled = true;
            this.gamepadSelection.Location = new System.Drawing.Point(12, 12);
            this.gamepadSelection.Name = "gamepadSelection";
            this.gamepadSelection.Size = new System.Drawing.Size(126, 21);
            this.gamepadSelection.TabIndex = 3;
            // 
            // leftStickLabel
            // 
            this.leftStickLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.leftStickLabel.Location = new System.Drawing.Point(144, 12);
            this.leftStickLabel.Name = "leftStickLabel";
            this.leftStickLabel.Size = new System.Drawing.Size(109, 20);
            this.leftStickLabel.TabIndex = 4;
            this.leftStickLabel.Text = "Left Stick: 0.00";
            // 
            // rightStickLabel
            // 
            this.rightStickLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.rightStickLabel.Location = new System.Drawing.Point(259, 12);
            this.rightStickLabel.Name = "rightStickLabel";
            this.rightStickLabel.Size = new System.Drawing.Size(152, 20);
            this.rightStickLabel.TabIndex = 5;
            this.rightStickLabel.Text = "Right Stick: 0.00";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 422);
            this.Controls.Add(this.rightStickLabel);
            this.Controls.Add(this.leftStickLabel);
            this.Controls.Add(this.gamepadSelection);
            this.Controls.Add(this.gConsole);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "COGNAV Control Panel";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.ComboBox gamepadSelection;

        private System.Windows.Forms.Label leftStickLabel;
        private System.Windows.Forms.Label rightStickLabel;

        private System.Windows.Forms.RichTextBox gConsole;

        #endregion
    }
}