using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using COGNAV.ARAP;
using COGNAV.Control;

namespace COGNAV.Interface {
    public class ControlRegisterAdapter : IRegisterAdapter {
        
        
        private GamepadHandler _gamepad;

        public volatile float RobotX;
        public volatile float RobotY;
        public volatile float RobotRot;

        public volatile float DesiredRot;
        public volatile float DesiredDistance;

        public volatile int Instruction;

        private float _objectAngle = 0;
        private float _objectDistance = 0;
        private float _objectWidth = 0;
        public ConcurrentQueue<Obstacle> RobotObstacleQueue = new ConcurrentQueue<Obstacle>();
        
        public ControlRegisterAdapter(GamepadHandler gamepad) {
            _gamepad = gamepad;
            RobotX = 0;
            RobotY = 0;
            RobotRot = 0;

            DesiredRot = 0;
            DesiredDistance = 0;

            Instruction = 0;
        }

        public int ReadRegister(int register, int count, List<byte> argsOut) {

            // Get the instruction
            if (register == 1) {
                if (count > 0) {
                    argsOut.Add(Convert.ToByte(Instruction & 0xFF));
                }
            }

            // Get the desired rotational position
            if (register == 2) {

                if (count > 0) {
                    
                    // Convert stick into byte array
                    byte[] bytes = BitConverter.GetBytes(DesiredRot);

                    // And send it out
                    foreach (byte by in bytes) {
                        argsOut.Add(by);
                    }
                }

                return 0;
            }
            
            // Get the desired linear displacement
            if (register == 3) {

                if (count > 0) {
                    
                    // Convert stick into byte array
                    byte[] bytes = BitConverter.GetBytes(DesiredDistance);

                    // And send it out
                    foreach (byte by in bytes) {
                        argsOut.Add(by);
                    }
                }

                return 0;
            }

            // Get the left analog control stack
            if (register == 8) {

                if (count > 0) {
                    
                    // Convert stick into byte array
                    byte[] bytes = BitConverter.GetBytes(_gamepad.LeftStick);

                    // And send it out
                    foreach (byte by in bytes) {
                        argsOut.Add(by);
                    }
                }

                return 0;
            }

            // Get the right analog control stick
            if (register == 9) {
                
                if (count > 0) {
                    
                    // Convert stick into byte array
                    byte[] bytes = BitConverter.GetBytes(_gamepad.RightStick);

                    // And send it out
                    foreach (byte by in bytes) {
                        argsOut.Add(by);
                    }
                }

                return 0;
            }

            return 0x44;
        }

        public int WriteRegister(int register, List<byte> argsIn) {

            if (register == 1) {
                // Instruction acknowledge 
                
                Instruction = 0;

            } else if (register == 2) {
                // Update rotational position register
                if (argsIn.Count > 3) {
                    RobotRot = ToFloat(argsIn);
                }

                return 0;
                
            } else if (register == 3) {
                // Update linear displacement register
                if (argsIn.Count > 3) {
                    float displacement = ToFloat(argsIn);

                    (float x, float y) = DisplacementHelper.CalculateDisplacement(displacement, RobotRot);

                    RobotX = RobotX + x;
                    RobotY = RobotY + y;
                }
                
                return 0;
            } else if (register == 4) {
                // Update object angle register
                if (argsIn.Count > 3) {
                    _objectAngle = ToFloat(argsIn);
                }

                return 0;
            } else if (register == 5) {
                // Update object distance register
                if (argsIn.Count > 3) {
                    _objectDistance = ToFloat(argsIn);
                }

                return 0;
            } else if (register == 6) {
                // Update object angle register
                if (argsIn.Count > 3) {
                    _objectWidth = ToFloat(argsIn);
                }

                return 0;
            } else if (register == 7) {
                // Update object type register
                if (argsIn.Count > 0) {
                    byte objState = argsIn[0];

                    (float x, float y) = DisplacementHelper.CalculateDisplacement(_objectDistance, _objectAngle);

                    Obstacle obj = new Obstacle(RobotX + x, RobotY + y, _objectWidth);

                    if (objState == 0x00) obj.Type = ObstacleClass.Blip;
                    if (objState == 0x01) obj.Type = ObstacleClass.Blob;
                    if (objState == 0x02) obj.Type = ObstacleClass.Wide;
                    if (objState == 0x03) obj.Type = ObstacleClass.Thin;
                    if (objState == 0x04) obj.Type = ObstacleClass.Short;
                    if (objState == 0x05) obj.Type = ObstacleClass.Tape;
                    if (objState == 0x06) obj.Type = ObstacleClass.Hole;
                }

                return 0;
            }

            return 0x44;
        } 

        private float ToFloat(List<byte> argsIn) {
            byte[] floatArray = new byte[4];

            for (int i = 0; i < 4; i++) floatArray[i] = argsIn[i];

            return BitConverter.ToSingle(floatArray);
        }
    }
} 