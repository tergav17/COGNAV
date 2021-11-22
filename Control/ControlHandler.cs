using System.Threading;
using COGNAV.Interface;

namespace COGNAV.Control {
    public class ControlHandler {

        private GraphicConsole _graphicConsole;
        private ControlRegisterAdapter _controlRegister;
        
        private volatile bool _run;

        public ControlHandler(GraphicConsole gc) {
            _graphicConsole = gc;

            _run = true;

            _controlRegister = new ControlRegisterAdapter();
            
            Thread controlThread = new Thread(new ThreadStart(this.HandleControl));
            
            controlThread.Start();
        }

        /**
         * Return the control register adapter for the Control Handler
         */
        public ControlRegisterAdapter GetRegisterAdapter() {
            return _controlRegister;
        }

        /**
         * Terminate handler thread
         */
        public void Terminate() {
            _run = false;
        }
        
        /**
         * Robot control thread, handles navigation and manual control
         */
        private void HandleControl() {
            while (_run) {
                Thread.Sleep(10);
            }
        }
    }
}