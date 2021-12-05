using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using COGNAV.EnvGraphics;
using COGNAV.Interface;
using COGNAV.Specs;

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

            Field testField = new Field();
            
            testField.Obstacles.Add(new Obstacle(0, 0.8F, .2F));

            List<PathNode> path = PathFinder.FindPath(new PathNode(0, 0), new PathNode(0, 1.6F), testField, _graphicConsole);

            path = PathFinder.ReducePath(path, testField);
            
            if (path != null) {
                _graphicConsole.PutSuccess("Found path with distance of " + path.Count + " nodes");
            }
            else _graphicConsole.PutError("Pathfinding failure!");

            while (_run) {
                

                EnvironmentalDataContainer data = new EnvironmentalDataContainer(0, 0, 0);

                _controlRegister.RobotRot = _controlRegister.RobotRot + (_gamepad.RightStick * 1.5F);

                (float X, float Y) displacement = DisplacementHelper.CalculateDisplacement(_gamepad.LeftStick / 100, _controlRegister.RobotRot);

                _controlRegister.RobotX = _controlRegister.RobotX + displacement.X;
                _controlRegister.RobotY = _controlRegister.RobotY + displacement.Y;
                
                data.Rotation = DisplacementHelper.ToRadians(_controlRegister.RobotRot);
                data.X = _controlRegister.RobotX;
                data.Y = _controlRegister.RobotY;

                if (!PathHelper.DoesPointCollide(new PointF(0F, 0.8F),
                    new PointF(_controlRegister.RobotX, _controlRegister.RobotY), Roomba.RoombaRadiusMeters)) {

                    data.Shapes.Add(new EnvironmentShape(0, .8, 0.2, 0.2));

                }

                PathNode last = null;
                if (path != null) foreach (PathNode n in path) {
                    if (last != null) {
                        EnvironmentShape line = new EnvironmentShape(n.X, n.Y, 0F, 0F);
                        line.EndX = last.X;
                        line.EndY = last.Y;
                        line.ShapeGeometry = Shape.Line;
                        line.ShapeColor = Color.Aqua;
                        
                        data.Shapes.Add(line);
                    }

                    last = n;

                }
                
                _drawOps.UpdateData(data);

                Thread.Sleep(10);
            }
        }
    }
}