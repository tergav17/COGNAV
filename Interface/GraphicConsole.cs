using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;

namespace COGNAV.Interface {

    public class GraphicConsole {

        private readonly System.Windows.Forms.RichTextBox _gConsoleBox;

        /**
         * Allows for multiple threads to easily interface with a RichTextBox as a rudimentary console
         */
        public GraphicConsole(System.Windows.Forms.RichTextBox gc) {
            _gConsoleBox = gc;
        }

        /**
         * Method to put in a line to display
         */
        public void PutLine(string str) {
            //_lineQueue.Enqueue(str);

            void SafeLine() {
                AddLine(str, Color.LightGray);
            }
            
            _gConsoleBox.Invoke((Action) SafeLine);
        }
        
        /**
         * Method to put in a error to display
         */
        public void PutError(string str) {
            //_lineQueue.Enqueue(str);

            void SafeLine() {
                AddLine(str, Color.Red);
            }
            
            _gConsoleBox.Invoke((Action) SafeLine);
        }

        /**
         * Method to put in a success to display
         */
        public void PutSuccess(string str) {
            //_lineQueue.Enqueue(str);

            void SafeLine() {
                AddLine(str, Color.Lime);
            }
            
            _gConsoleBox.Invoke((Action) SafeLine);
        }
        
        /**
         * Method to put in a success to display
         */
        public void PutStartup(string str) {
            //_lineQueue.Enqueue(str);

            void SafeLine() {
                AddLine(str, Color.Aqua);
            }
            
            _gConsoleBox.Invoke((Action) SafeLine);
        }

        /**
         * Adds line to the text box
         */
        private void AddLine(string str, Color c) {
            var pos = _gConsoleBox.TextLength;

            _gConsoleBox.AppendText(str + '\n');

            _gConsoleBox.SelectionStart = pos;
            _gConsoleBox.SelectionLength = str.Length + 1;
            _gConsoleBox.SelectionColor = c;
            
            _gConsoleBox.ScrollToCaret();

            _gConsoleBox.SelectionLength = 0;
        }

    }
}