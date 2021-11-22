using System.Collections.Generic;

namespace COGNAV.ARAP {
    public interface IRegisterAdapter {
        int ReadRegister(int register, int count, List<byte> args);
        int WriteRegister(int register, List<byte> argsOut);
    }
}