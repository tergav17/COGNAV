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
            this.environmentViewer = new System.Windows.Forms.PictureBox();
            this.gamepadDivider = new System.Windows.Forms.Label();
            this.gamepadInput = new System.Windows.Forms.Label();
            this.redrawTimer = new System.Timers.Timer();
            ((System.ComponentModel.ISupportInitialize) (this.environmentViewer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.redrawTimer)).BeginInit();
            this.SuspendLayout();
            // 
            // gConsole
            // 
            this.gConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.gConsole.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.gConsole.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.gConsole.Location = new System.Drawing.Point(12, 218);
            this.gConsole.Name = "gConsole";
            this.gConsole.ReadOnly = true;
            this.gConsole.Size = new System.Drawing.Size(413, 244);
            this.gConsole.TabIndex = 2;
            this.gConsole.Text = "";
            // 
            // gamepadSelection
            // 
            this.gamepadSelection.FormattingEnabled = true;
            this.gamepadSelection.Location = new System.Drawing.Point(71, 190);
            this.gamepadSelection.Name = "gamepadSelection";
            this.gamepadSelection.Size = new System.Drawing.Size(126, 21);
            this.gamepadSelection.TabIndex = 3;
            // 
            // leftStickLabel
            // 
            this.leftStickLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.leftStickLabel.Location = new System.Drawing.Point(203, 191);
            this.leftStickLabel.Name = "leftStickLabel";
            this.leftStickLabel.Size = new System.Drawing.Size(109, 20);
            this.leftStickLabel.TabIndex = 4;
            this.leftStickLabel.Text = "Left Stick: 0.00";
            // 
            // rightStickLabel
            // 
            this.rightStickLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.rightStickLabel.Location = new System.Drawing.Point(318, 191);
            this.rightStickLabel.Name = "rightStickLabel";
            this.rightStickLabel.Size = new System.Drawing.Size(106, 20);
            this.rightStickLabel.TabIndex = 5;
            this.rightStickLabel.Text = "Right Stick: 0.00";
            // 
            // environmentViewer
            // 
            this.environmentViewer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.environmentViewer.Location = new System.Drawing.Point(431, 12);
            this.environmentViewer.Name = "environmentViewer";
            this.environmentViewer.Size = new System.Drawing.Size(450, 450);
            this.environmentViewer.TabIndex = 6;
            this.environmentViewer.TabStop = false;
            // 
            // gamepadDivider
            // 
            this.gamepadDivider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gamepadDivider.Location = new System.Drawing.Point(12, 183);
            this.gamepadDivider.Name = "gamepadDivider";
            this.gamepadDivider.Size = new System.Drawing.Size(412, 2);
            this.gamepadDivider.TabIndex = 7;
            // 
            // gamepadInput
            // 
            this.gamepadInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.gamepadInput.Location = new System.Drawing.Point(22, 192);
            this.gamepadInput.Name = "gamepadInput";
            this.gamepadInput.Size = new System.Drawing.Size(43, 19);
            this.gamepadInput.TabIndex = 8;
            this.gamepadInput.Text = "Input:";
            // 
            // redrawTimer
            // 
            this.redrawTimer.Enabled = true;
            this.redrawTimer.Interval = 10D;
            this.redrawTimer.SynchronizingObject = this;
            this.redrawTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.redrawTimer_Elapsed);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 475);
            this.Controls.Add(this.gamepadInput);
            this.Controls.Add(this.gamepadDivider);
            this.Controls.Add(this.environmentViewer);
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
            ((System.ComponentModel.ISupportInitialize) (this.environmentViewer)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.redrawTimer)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Timers.Timer redrawTimer;

        private System.Windows.Forms.Label gamepadInput;

        private System.Windows.Forms.Label gamepadDivider;

        private System.Windows.Forms.PictureBox environmentViewer;

        private System.Windows.Forms.ComboBox gamepadSelection;

        private System.Windows.Forms.Label leftStickLabel;
        private System.Windows.Forms.Label rightStickLabel;

        private System.Windows.Forms.RichTextBox gConsole;

        #endregion
    }
}