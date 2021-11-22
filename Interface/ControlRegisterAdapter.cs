using System.Collections.Generic;
using COGNAV.ARAP;

namespace COGNAV.Interface {
    public class ControlRegisterAdapter : IRegisterAdapter {
        public int ReadRegister(int register, int count, List<byte> args) {
            throw new System.NotImplementedException();
        }

        public int WriteRegister(int register, List<byte> argsOut) {
            throw new System.NotImplementedException();
        }
    }
} 