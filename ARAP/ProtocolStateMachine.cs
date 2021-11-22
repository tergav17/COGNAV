using System.Collections.Generic;
using COGNAV.Interface;

namespace COGNAV.ARAP {
    public class ProtocolStateMachine {

        private int _state;
        private int _packetLength;
        private int _targetRegister;

        private int _packetCounter;
        private int _error;
        
        private List<byte> _crcBuffer = new List<byte>();
        private List<byte> _args = new List<byte>();

        private List<byte> _outputBuffer = new List<byte>();
        private Queue<byte> _outputQueue = new Queue<byte>();

        private GraphicConsole _graphicConsole;
        private IRegisterAdapter _registerAdapter;
        
        public ProtocolStateMachine(GraphicConsole gc, IRegisterAdapter ra) {
            // Set up initial condition
            _state = 0;
            _packetLength = 0;
            _targetRegister = 0;
            _packetCounter = 0;
            _error = 0;

            _graphicConsole = gc;
            _registerAdapter = ra;
        }
        
        public ProtocolStateMachine(IRegisterAdapter ra) {
            // Set up initial condition
            _state = 0;
            _packetLength = 0;
            _targetRegister = 0;
            _packetCounter = 0;
            _error = 0;

            _graphicConsole = null;
            _registerAdapter = ra;
        }

        /**
         * Advances the state machine depending on what byte is read, and when
         */
        public void ReceiveByte(byte b) {
            _graphicConsole?.PutLine("Received Byte: " + b);

            // Read Packet Length (Default State) -> Read Packet Register
            if (_state == 0) {
                _state = 1;
                _error = 0;
                _packetLength = b;

                _crcBuffer = new List<byte>();
                _args = new List<byte>();
                
                _crcBuffer.Add(b);

            }
            
            // Read Packet Register -> Read Argument / CRC
            else if (_state == 1) {
                _state = 2;
                _targetRegister = b;
                _packetCounter = 0;
                
                _crcBuffer.Add(b);
            }
            
            // Read Argument / CRC
            else if (_state == 2) {
                _packetCounter++;

                if (_packetCounter > _packetLength) {
                    // Handle CRC stuff
                    byte crc = Protocol.GenerateCyclicCheck(_crcBuffer);
                    
                    if (crc == b) {
                        // If CRC is successful, submit the packet
                        SubmitPacket(BuildPacket());
                        _state = 0;

                    } else {
                        // Otherwise, shit the bed
                        _error = 0x10;
                        // -> Wait For Timeout
                        _state = -1;
                    }

                } else {
                    // Otherwise, do argument stuff with it
                    _args.Add(b);
                    _crcBuffer.Add(b);
                }
            }
        }

        /**
         * Triggered when the communication handler goes too long without receiving data
         */
        public void ReceiveTimeoutCondition() {
            // Timeout error conditions
            if (_state != 0) {
                if (_error == 0) {
                    _error = 0x11;
                }

                _graphicConsole?.PutLine("Transmitting Error: " + _error);
                
                // Create and publish error packet
                _outputBuffer = Protocol.BuildReturnPacket(_error, new List<byte>());
                PublishOutput();
                
                // Reset state
                _state = 0;
            }
        }

        /**
         * Handle an incoming packet
         */
        public void SubmitPacket(ParsedPacket packet) {
            if (packet.Register == 0) {
                // Resend operation, just republish the last message
                PublishOutput();
            } else if (packet.Register == 128) {
                // Create and publish ping packet
                _outputBuffer = Protocol.BuildReturnPacket(0, new List<byte>());
                PublishOutput();
            } else {
                _graphicConsole?.PutLine("Received Packet With Length: " + packet.Arguments.Count);

                // Check if it is a read or a write
                if (packet.Register < 128) {
                    // Read condition
                    if (packet.Arguments.Count != 1) {
                        // There is no requested count argument, fail!
                        _outputBuffer = Protocol.BuildReturnPacket(0x43, new List<byte>());
                        PublishOutput();
                    }
                    else {
                        // Read from the register, and then send it out
                        List<byte> outArgs = new List<byte>();
                        
                        int status = _registerAdapter.ReadRegister(packet.Register, packet.Arguments[0], outArgs);
                        
                        _outputBuffer = Protocol.BuildReturnPacket(status, outArgs);
                        PublishOutput();
                    }
                } else {
                    // Write condition

                    // Write to register, and then return
                    int status = _registerAdapter.WriteRegister(packet.Register - 128, packet.Arguments);
                        
                    _outputBuffer = Protocol.BuildReturnPacket(status, new List<byte>());
                    PublishOutput();
                }
            }
        }

        /**
         * Build a packet using current information
         */
        public ParsedPacket BuildPacket() {
            return new ParsedPacket(_targetRegister, _args);
        }

        /**
         * Push all values in the output buffer onto the output queue to be send out
         */
        public void PublishOutput() {
            foreach (byte b in _outputBuffer) _outputQueue.Enqueue(b);
        }

        /**
         * Checks if there is anything in the output queue
         */
        public bool HasOutput() {
            return _outputQueue.Count > 0;
        }

        /**
         * Pops the first element out of the queue
         */
        public byte SendOutput() {
            return _outputQueue.Dequeue();
        }

    }
}