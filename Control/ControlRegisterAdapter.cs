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
        public ConcurrentQueue<Obstacle> RobotObstacleQueue = new ConcurrentQueue<Obstacle>();
        
        public ControlRegisterAdapter(GamepadHandler gamepad) {
            _gamepad = gamepad;
        }

        public int ReadRegister(int register, int count, List<byte> argsOut) {
            
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

            // Update rotational position register
            if (register == 2) {
                if (argsIn.Count > 3) {
                    RobotRot = ToFloat(argsIn);
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