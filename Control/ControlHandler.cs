using System;
using System.Collections.Generic;
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
        private Field _field;
        
        private volatile bool _run;

        public ControlHandler(GraphicConsole gc, EnvironmentRedrawHandler rh, GamepadHandler gh) {
            _graphicConsole = gc;
            _drawOps = rh;
            _gamepad = gh;

            _field = new Field();

            _run = true;

            _controlRegister = new ControlRegisterAdapter(_gamepad);
            
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

            while (_run) {

                count += 0.01;

                EnvironmentalDataContainer data = new EnvironmentalDataContainer(0, 0, 0);

                //data.X = 10 * Math.Cos(count);
                //data.Y = 10 * Math.Sin(count);
                
                //data.Rotation = Math.Sin(count / 10);

                _controlRegister.RobotRot = _controlRegister.RobotRot + (_gamepad.RightStick * 1.5F);

                (float X, float Y) displacement = DisplacementHelper.CalculateDisplacement(_gamepad.LeftStick / 10, _controlRegister.RobotRot);

                _controlRegister.RobotX = _controlRegister.RobotX + displacement.X;
                _controlRegister.RobotY = _controlRegister.RobotY + displacement.Y;
                
                data.Rotation = DisplacementHelper.ToRadians(_controlRegister.RobotRot);
                data.X = _controlRegister.RobotX;
                data.Y = _controlRegister.RobotY;
                
                data.Shapes.Add(new EnvironmentShape(0, 1, 1, 1) );
                
                _drawOps.UpdateData(data);

                Thread.Sleep(10);
            }
        }
    }
}