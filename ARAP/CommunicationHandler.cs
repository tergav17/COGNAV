using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using COGNAV.Interface;

namespace COGNAV.ARAP {
    public class CommunicationHandler {

        private GraphicConsole _graphicConsole;

        private volatile bool _run;

        private ProtocolStateMachine _psm;
        
        public CommunicationHandler(GraphicConsole gc, ControlRegisterAdapter cra) {
            _graphicConsole = gc;
            _psm = new ProtocolStateMachine(cra);

            Thread communicationThread = new Thread(new ThreadStart(this.HandleCommunication));

            _run = true;
            
            communicationThread.Start();
        }

        /**
         * Terminate handler thread
         */
        public void Terminate() {
            _run = false;
        }

        /**
         * Connects to socket, and continually watches for incoming information
         */
        private void HandleCommunication() {
            _graphicConsole.PutLine("Starting Communication Thread...");

            // Create IP endpoint at 192.168.1.1:288
            IPAddress ipAddr = System.Net.IPAddress.Parse("192.168.1.1");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 288);
            
            
            
            // Build socket
            Socket connection = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            while (_run) {

                try {
                    
                    // Attempt to connect to socket
                    _graphicConsole.PutLine("Attempting Connection At " + endPoint.ToString()); 
                    connection.Connect(endPoint);
                    _graphicConsole.PutLine("Connection Established");

                    // Wait counter
                    int bypasses = 0;
                    
                    // Socket handler loop
                    while (_run) {
                        
                        // Check and see if the socket has received any information
                        int av = connection.Available;
                        if (av > 0) {
                            // Collect it, and pass it on to the state machine
                            byte[] buf = new byte[1];

                            connection.Receive(buf, 1, SocketFlags.None);

                            _psm.ReceiveByte(buf[0]);

                            bypasses++;

                        } else {
                            Thread.Sleep(10);
                            bypasses++;

                            // Too much time has been spent without receiving a packet, tell that state machine about that
                            if (bypasses > 100) {
                                _psm.ReceiveTimeoutCondition();
                                bypasses = 0;
                            }
                        }

                        // If the state machine has any outgoing information...
                        while (_psm.HasOutput()) {
                            byte[] buf = new byte[1];
                            buf[0] = _psm.SendOutput();

                            // Send it off!
                            connection.Send(buf, 1, SocketFlags.None);
                        }
                    }
                }
                catch (SocketException) {
                    // Connection failure, clean up and attempt to reconnect
                    _graphicConsole.PutLine("Connection Failure");
                    if (connection.Connected) connection.Disconnect(true);
                }

                Thread.Sleep(100);
            }
        }
    }
}