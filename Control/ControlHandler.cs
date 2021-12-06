using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Xml;
using COGNAV.EnvGraphics;
using COGNAV.Interface;
using COGNAV.Specs;

namespace COGNAV.Control {
    public class ControlHandler {

        public volatile int Task = 0;
        public volatile int ManualScan = 0;
        
        
        private readonly GraphicConsole _graphicConsole;
        private readonly ControlRegisterAdapter _controlRegister;
        private readonly EnvironmentRedrawHandler _drawOps;
        private GamepadHandler _gamepad;
        private readonly Field _field;
        private readonly System.Windows.Forms.Label _isrLabel;

        private List<PathNode> _currentPath = new List<PathNode>();
        private (float Direction, float Distance, bool DoScan) _trajectory = (0, 0, false);

        private volatile bool _run;

        public ControlHandler(GraphicConsole gc, EnvironmentRedrawHandler rh, System.Windows.Forms.Label isrLabel, GamepadHandler gh) {
            _graphicConsole = gc;
            _drawOps = rh;
            _gamepad = gh;
            _isrLabel = isrLabel;

            _field = new Field();

            _run = true;

            _controlRegister = new ControlRegisterAdapter(_gamepad, gc);
            
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
         * Sets the current text of the isr label
         */
        private void PutIsrLabel(string label) {
            
            void SafeLabel() {
                _isrLabel.Text = @"Current Instruction:" + '\n' + label;
                
            }
            
            _isrLabel.Invoke((Action) SafeLabel);
        }



        /**
         * Robot control thread, handles navigation and manual control
         */
        private void HandleControl() {

            _graphicConsole.PutStartup("Starting Controller Thread...");

            UpdateGraphics();

            PutIsrLabel("NOP");

            while (Task == 0) Thread.Sleep(10);

            if (Task == 1) {
                
                _graphicConsole.PutStartup("Starting Autonomous Mode...");

                BozoSearch();

                HomeIn();

            }
            
            _graphicConsole.PutStartup("Starting TeleOp Mode...");
            
            DoIsr(5);

            while (_run) {

                if (ManualScan > 0) {
                    DoIsr(3);

                    ManualScan = 0;
                }

                PopAllNewObstacles();
                
                //_graphicConsole.PutLine(_controlRegister.RobotX + ", " + _controlRegister.RobotY);
                
                Thread.Sleep(10);
            }
        }

        // Attempt to get into the box
        public void HomeIn() {
            Obstacle rootThin = null;
            
            foreach (Obstacle obj in _field.Obstacles) {
                if (obj.Type == ObstacleClass.Thin) rootThin = obj;
            }

            if (rootThin == null) {
                _graphicConsole.PutError("There Is No Thin Object!");
                return;
            }

            InvestigateThin(rootThin, true);

            while (CountThin() < 3) {
                InvestigateThin(rootThin, false);
            }

            Obstacle minorThin = null;
            Obstacle majorThin = null;

            float maxDistance = Single.MinValue;
            
            // Find span with largest distance
            foreach (Obstacle obj in _field.Obstacles) {
                if (obj.Type == ObstacleClass.Thin) {
                    
                    foreach (Obstacle objMaj in _field.Obstacles) {
                        if (obj.Type == ObstacleClass.Thin) {
                            float distance = PathHelper.DistanceF(new PointF(obj.X, obj.Y), new PointF(objMaj.X, objMaj.Y));

                            if (distance > maxDistance) {
                                maxDistance = distance;
                                minorThin = obj;
                                majorThin = objMaj;
                            }
                        }
                    }
                }
            }

            if (majorThin == null) return;

            // Calculate centroid
            PointF centroid = new PointF((minorThin.X + majorThin.X) / 2F, (minorThin.Y + majorThin.Y) / 2F);

            // Go to it
            if (NavigateRobot(centroid, false)) {
                DoIsr(7);
            }

        }

        public int CountThin() {
            int output = 0;

            foreach (Obstacle obj in _field.Obstacles)
                if (obj.Type == ObstacleClass.Thin)
                    output++;
            
            return output;
        }

        /**
         * BOZO-SEARCH! Try to find a thin object at any cost
         */
        public void BozoSearch() {
            Stack<Partition> partitionStack = new Stack<Partition>();
            
            // Do a scan to create initial zone
            DoIsr(3);
            _field.ScannedAreas.Add(new Zone(_controlRegister.RobotX, _controlRegister.RobotY, 0.7F));
            PopAllNewObstacles();
            
            // Add this partition
            Partition part = new Partition(_controlRegister.RobotX, _controlRegister.RobotY, 0.6F);
            partitionStack.Push(part);
            _field.Partitions.Add(part);

            while (partitionStack.Count > 0) {
                
                // Look for blobs and visit them
                foreach (Obstacle obj in _field.Obstacles) {
                    if (obj.Type == ObstacleClass.Blob) {
                        VisitBlob(obj);
                    }

                    // We found a thin object, all done with bozosearch
                    if (obj.Type == ObstacleClass.Thin) {
                        _graphicConsole.PutSuccess("Found Thin Object!");
                        return;
                    }
                }

                // Attempt to find a node to travel to
                Partition top = partitionStack.Peek();
                top.UpdateValidity(_field.Partitions);
                int bestNode = -1;
                float lowestScore = Single.MaxValue;

                for (int i = 0; i < Partition.NumBranches; i++) {
                    if (top.Branches[i]) {
                        float score = PathHelper.CalculateScore(top.GetBranchPoint(i), _field);

                        if (score < lowestScore) bestNode = i;
                    }
                }

                if (bestNode == -1) {
                    // There are no available nodes to travel to!
                    // Pop and travel to that partition
                    partitionStack.Pop();

                    if (partitionStack.Count > 0) {
                        PointF destination = new PointF(partitionStack.Peek().X, partitionStack.Peek().Y);

                        NavigateRobot(destination, false);
                    } else {
                        PointF newNode = top.GetBranchPoint(bestNode);

                        if (NavigateRobot(newNode, true)) {
                            
                            // Do a scan to create zone
                            DoIsr(3); 
                            _field.ScannedAreas.Add(new Zone(_controlRegister.RobotX, _controlRegister.RobotY, 0.7F));
                            PopAllNewObstacles();
                            
                            // Add this partition
                            part = new Partition(_controlRegister.RobotX, _controlRegister.RobotY, 0.6F);
                            partitionStack.Push(part);
                            _field.Partitions.Add(part);
                            
                        } else {
                            // Get backtrack point and go there
                            // Zap that node for good measure too
                            PointF backtrack = new PointF(top.X, top.Y);

                            NavigateRobot(backtrack, false);
                            top.ZapBranch(bestNode);
                        }
                    }
                }

            }
            
            _graphicConsole.PutError("Cannot Find Thin Object!");
        }

        private readonly Random _r = new Random();
        
        /**
         * Gets somewhat close to a thin object, and tries to find things around it
         */
        public void InvestigateThin(Obstacle obj, bool far) {

            PointF center = new PointF(obj.X, obj.Y);
            PointF origin = new PointF(_controlRegister.RobotX, _controlRegister.RobotY);

            List<int> validAngles = new List<int>();
            
            int bestAngle = -1;
            float largestDistance = 0;

            for (int i = 0; i < 360; i += 10) {

                PointF orbit = PathHelper.CalculateOrbit(center, 0.3F, i);
                
                // Checks if angle is valid
                if (PathHelper.IsPointValid(orbit, _field.Obstacles, 0.0F)) {
                    float distance = PathHelper.DistanceF(orbit,
                        new PointF(_controlRegister.RobotX, _controlRegister.RobotY));

                    validAngles.Add(i);
                    
                    if (distance > largestDistance) {
                        bestAngle = i;
                        largestDistance = distance;
                    }
                }
            }

            // If we don't want to farthest angle, pick a random one
            if (!far) {
                bestAngle = validAngles[_r.Next() % validAngles.Count];
            }

            // If no best angle is found, return
            if (bestAngle == -1) {

                return;
            }
            
            PointF goal = PathHelper.CalculateOrbit(center, 0.3F, bestAngle);

            // If the robot can navigate there, do a scan
            if (NavigateRobot(goal, false)) {
                
                DoIsr(4);
                _field.ScannedAreas.Add(new Zone(_controlRegister.RobotX, _controlRegister.RobotY, 0.4F));
                PopAllNewObstacles();
            }

            NavigateRobot(origin, false);

        }

        /**
         * Gets somewhat close to a blob, and hits it with a scan
         */
        public void VisitBlob(Obstacle obj) {

            PointF center = new PointF(obj.X, obj.Y);
            PointF origin = new PointF(_controlRegister.RobotX, _controlRegister.RobotY);
            
            int bestAngle = -1;
            float shortestDistance = Single.MaxValue;

            for (int i = 0; i < 360; i += 10) {

                PointF orbit = PathHelper.CalculateOrbit(center, 0.3F, i);
                
                // Checks if angle is valid
                if (PathHelper.IsPointValid(orbit, _field.Obstacles, 0.0F)) {
                    float distance = PathHelper.DistanceF(orbit,
                        new PointF(_controlRegister.RobotX, _controlRegister.RobotY));

                    if (distance < shortestDistance) {
                        bestAngle = i;
                        shortestDistance = distance;
                    }
                }
            }

            // If no best angle is found, just assume wide for now
            if (bestAngle == -1) {
                obj.Type = ObstacleClass.Wide;
                return;
            }
            
            PointF goal = PathHelper.CalculateOrbit(center, 0.3F, bestAngle);

            // If the robot can navigate there, do a scan
            if (NavigateRobot(goal, false)) {
                
                DoIsr(4);
                _field.ScannedAreas.Add(new Zone(_controlRegister.RobotX, _controlRegister.RobotY, 0.4F));
                PopAllNewObstacles();
            } else {
                obj.Type = ObstacleClass.Wide;
            }

            NavigateRobot(origin, false);

        }
        
        



        /**
         * Navigates the robot towards a specific point
         * May stop if a hole is encountered
         */
        public bool NavigateRobot(PointF point, bool stopIfHole) {

            // Run till within 3 cm
            while (PathHelper.Distance(point, new PointF(_controlRegister.RobotX, _controlRegister.RobotY)) > 0.03) {

                // Create the nearest route, return false if impossible
                if (!RouteRobot(point)) return false;

                // Move to current _trajectory, if a hole is found and the stopIfHole flag is set, return false
                if (MoveRobotTowards(_trajectory) && stopIfHole) return false;
                
                // Update graphics after movements
                UpdateGraphics();

            }

            // All done
            return true;
        }

        /**
         * Moves the robot towards a goal
         * Returns true if robot hits a hole/tape
         */
        public bool MoveRobotTowards((float Direction, float Distance, bool DoScan) guide) {

            // Set desired motion values
            _controlRegister.DesiredRot = guide.Direction;
            _controlRegister.DesiredDistance = guide.Distance;

            // Turn and drive
            DoIsr(1);
            UpdateGraphics();
            
            DoIsr(2);
            UpdateGraphics();
            
            // Do scan if needed
            if (guide.DoScan) {
                DoIsr(3);
                _field.ScannedAreas.Add(new Zone(_controlRegister.RobotX, _controlRegister.RobotY, 0.7F));
            }

            // Pop all of the new objects, and see if we hit something bad
            return PopAllNewObstacles();
        }

        /**
         * Pops all new obstacles out the new obstacle queue, and processes them
         */
        public bool PopAllNewObstacles() {
            bool hasWall = false;

            while (!_controlRegister.RobotObstacleQueue.IsEmpty) {
                //_graphicConsole.PutLine("Popping New Object...");
                
                _controlRegister.RobotObstacleQueue.TryDequeue(out Obstacle obstacle);
                if (obstacle == null) continue;

                List<Obstacle> overlap = new List<Obstacle>();

                // Only do overlap checking if the new object is some sort of vision object
                if ((obstacle.Type == ObstacleClass.Blob || obstacle.Type == ObstacleClass.Wide || obstacle.Type == ObstacleClass.Thin)) {

                    foreach (Obstacle obj in _field.Obstacles) {
                        if (PathHelper.DoesPointCollide(new PointF(obstacle.X, obstacle.Y), new PointF(obj.X, obj.Y),
                            (obstacle.Diameter + obj.Diameter) / 2F)) {
                            overlap.Add(obj);
                        }
                    }

                    foreach (Obstacle obj in overlap) {
                        
                        // Any blobs or shorts will just get clobbered
                        if (obj.Type == ObstacleClass.Blob || obj.Type == ObstacleClass.Short) {
                            _field.Obstacles.Remove(obj);
                        }

                        // If the object is wide, the new object will be promoted to a wide object if it is a blob
                        if (obj.Type == ObstacleClass.Wide) {
                            if (obstacle.Type == ObstacleClass.Blob) {
                                obstacle.Type = ObstacleClass.Wide;
                                obstacle.Diameter = obj.Diameter;
                            }

                            _field.Obstacles.Remove(obj);
                        }

                        // If the object is thin, the new object will be promoted to a thin object if it is a blob or wide
                        if (obj.Type == ObstacleClass.Thin) {
                            if (obstacle.Type == ObstacleClass.Blob || obstacle.Type == ObstacleClass.Wide) {
                                obstacle.Type = ObstacleClass.Thin;
                                obstacle.Diameter = obj.Diameter;
                            }

                            _field.Obstacles.Remove(obj);
                        }
                    }
                }
                
                //_graphicConsole.PutLine("New Object Added To Field");
                _field.Obstacles.Add(obstacle);

                if (obstacle.Type == ObstacleClass.Hole || obstacle.Type == ObstacleClass.Tape) hasWall = true;
            }

            // Update graphics for fun
            UpdateGraphics();
            
            return hasWall;
        }

        /**
         * Performs an instruction
         */
        public void DoIsr(int isr) {
            // Disassemble instruction
            if (isr == 1) {
                PutIsrLabel("TUR " + _controlRegister.DesiredRot);
            } else if (isr == 2) {
                PutIsrLabel("DRV " + _controlRegister.DesiredDistance);
            } else if (isr == 3) {
                PutIsrLabel("SCNL");
            } else if (isr == 4) {
                PutIsrLabel("SCNS");
            } else if (isr == 5) {
                PutIsrLabel("MAN");
            } else if (isr == 6) {
                PutIsrLabel("COM");
            } else if (isr == 7) {
                PutIsrLabel("WIN");
            }


            // Send it off
            _controlRegister.Instruction = isr;
            
            // Wait for completion
            while (_controlRegister.Instruction != 0) Thread.Sleep(15);
            
            PutIsrLabel("NOP");
        }

        /**
         * Uses pathfinding to create a route for the robot
         */
        public bool RouteRobot(PointF destination) {

            _currentPath = PathFinder.FindPath(new PathNode(_controlRegister.RobotX, _controlRegister.RobotY), new PathNode(destination.X, destination.Y), _field, _graphicConsole) ??
                           new List<PathNode>();

            if (_currentPath.Count == 0) return false;
            
            _trajectory = PathFinder.GetNextTrajectory(_currentPath,
                new PointF(_controlRegister.RobotX, _controlRegister.RobotY), _field);

            _currentPath = PathFinder.ReducePath(_currentPath, _field);

            return true;
        }

        /**
         * Updates the graphics to the latest state
         */
        public void UpdateGraphics() {
            EnvironmentalDataContainer data = new EnvironmentalDataContainer(0, 0, 0);
            
            data.Rotation = DisplacementHelper.ToRadians(_controlRegister.RobotRot);
            data.X = _controlRegister.RobotX;
            data.Y = _controlRegister.RobotY;
            
            data.Shapes = ObstacleHelper.BuildEnvironmentShapes(_field);

            PathNode last = null;
            if (_currentPath != null) foreach (PathNode n in _currentPath) {
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
        }
    }
}