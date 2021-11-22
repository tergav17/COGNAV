using System.Collections.Concurrent;
using System.Threading;

namespace COGNAV.Interface {

    public class GraphicConsole {

        private readonly System.Windows.Forms.RichTextBox _gConsoleBox;

        private volatile bool _run;
        
        private readonly ConcurrentQueue<string> _lineQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<char> _charQueue = new ConcurrentQueue<char>();

        /**
         * Allows for multiple threads to easily interface with a RichTextBox as a rudimentary console
         */
        public GraphicConsole(System.Windows.Forms.RichTextBox gc) {
            _gConsoleBox = gc;

            Thread consoleThread = new Thread(new ThreadStart(this.HandleConsole));

            _run = true;
            
            consoleThread.Start();
        }

        /**
         * Method to queue in a line to display
         */
        public void PutLine(string str) {
            _lineQueue.Enqueue(str);
        }

        /**
         * Method to queue in a char to display
         */
        public void PutChar(char ch) {
            _charQueue.Enqueue(ch);
        }

        /**
         * Terminate handler thread
         */
        public void Terminate() {
            _run = false;
        }

        /**
         * Adds line to the text box
         */
        private void AddLine(string str) {
            string prior = _gConsoleBox.Text;

            _gConsoleBox.Text = prior + str + '\n';
        }
        
        /**
         * Adds char to text box
         */
        private void AddChar(char ch) {
            string prior = _gConsoleBox.Text;

            _gConsoleBox.Text = prior + ch;
        }

        /**
         * Constantly polls the line queue, and puts any lines found into the text box
         */
        private void HandleConsole() {
            // Crude method of doing polling, running with it anyways

            while (_run) {
                Thread.Sleep(10);

                while (_lineQueue.TryDequeue(out var line)) { 
                    AddLine(line); 
                }

                while (_charQueue.TryDequeue(out var ch)) {
                    AddChar(ch);
                }
            }
        }

    }
}