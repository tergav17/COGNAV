using System;
using System.Collections.Generic;

namespace COGNAV.ARAP {
    public class Protocol {

        // CRC polynomial for CRC-8-Bluetooth implementation
        private const int PolyMask = 0b110100111;

        public static List<byte> BuildReturnPacket(int status, List<byte> contents) {
            List<byte> packet = new List<byte>();
            
            // Calculate and add size of packet to packet. Include byte count, status, and CRC
            packet.Add(Convert.ToByte(CalculateSize(contents)));
            
            // Add status to packet
            packet.Add(Convert.ToByte(status));

            // Add all of the bytes 
            foreach (byte b in contents) {
                packet.Add(b);
            }
            
            // Generate and add the CRC for the packet
            packet.Add(GenerateCyclicCheck(packet));

            return packet;
        }

        /**
         * Calculates the size of an outgoing packet, includes the byte count, status, and CRC
         */
        public static int CalculateSize(List<byte> contents) {
            return 3 + contents.Count;
        }

        /**
         * Generates a CRC value based on an incoming message
         * Lowest index is assumed to be the first byte received
         */
        public static byte GenerateCyclicCheck(List<byte> message) {

            int crc = 0;
            
            foreach (int b in message) {

                // Create a copy of this byte for usage
                int by = b;
                
                for (int i = 0; i < 8; i++) {
                    // Get the highest bit the the byte
                    int last = by & 0x80;
                    
                    // Shift the current byte up
                    by = by << 1;

                    // Shift it onto the CRC
                    crc = (crc << 1) | last;

                    // Check bit 9 to see if it is on, if so do invert CRC bits
                    if ((crc & 0x100) == 1) {
                        // XOR polynomial mask onto CRC
                        crc = crc ^ PolyMask;
                    }
                    
                    // Just in case, slice up the CRC
                    crc = crc & 0xFF;
                }
            }

            return Convert.ToByte(crc);
        }


    }
}