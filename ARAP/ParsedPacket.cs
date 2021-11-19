using System.Collections.Generic;

namespace COGNAV.ARAP {
    public class ParsedPacket {
        public ParsedPacket(int register, List<byte> arguments) {
            this.Arguments = arguments;
            this.Register = register;

            this.Error = 0;
            this.IsValid = true;
        }

        public ParsedPacket(int error) {
            this.Arguments = new List<byte>();
            this.Register = -1;

            this.Error = error;
            this.IsValid = false;
        }

        public List<byte> Arguments { get; set; }

        public int Register { get; set; }

        public bool IsValid { get; set; }

        public int Error { get; set; }
    }
}