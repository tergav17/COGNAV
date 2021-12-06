using System;

namespace COGNAV.Control {
    public class DisplacementHelper {
        
        public static (float,float) CalculateDisplacement(float dis, float angle) {
            (float XOut, float YOut) output = (0, 0);

            float rAngle = ToRadians(angle);

            output.XOut = Convert.ToSingle((dis * Math.Sin(rAngle)));
            output.YOut = Convert.ToSingle((dis * Math.Cos(rAngle)));

            return output;
        }

        public static float ToRadians(float val) {
            return Convert.ToSingle((Math.PI / 180) * val);
        }
    }
}