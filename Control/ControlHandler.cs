using System;
using System.Drawing;
using System.Threading;
using COGNAV.EnvGraphics;
using COGNAV.Interface;

namespace COGNAV.Control {
    public class ControlHandler {

        private GraphicConsole _graphicConsole;
        private ControlRegisterAdapter _controlRegister;
        private EnvironmentRedrawHandler _drawOps;
        private GamepadHandler _gamepad;
        
        private volatile bool _run;

        public ControlHandler(GraphicConsole gc, EnvironmentRedrawHandler rh, GamepadHandler gh) {
            _graphicConsole = gc;
            _drawOps = rh;
            _gamepad = gh;

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
         * Sets the current text of the menu
         */
        
        
        /**
         * Robot control thread, handles navigation and manual control
         */
        private void HandleControl() {

            _graphicConsole.PutStartup("Starting Controller Thread...");

            double count = 0;

            double currX = 0;
            double currY = 0;
            
            while (_run) {

                count += 0.02;

                currY += _gamepad.LeftStick / 10;
                currX += _gamepad.RightStick / 10;

                EnvironmentalDataContainer data = new EnvironmentalDataContainer(0, 0, 0);

                //data.X = currX;
                //data.Y = currY;

                data.X = Math.Sin(count) * 5;
                data.Y = Math.Cos(count) * 5;
                
                _drawOps.UpdateData(data);

                Thread.Sleep(10);
            }
        }
    }
}