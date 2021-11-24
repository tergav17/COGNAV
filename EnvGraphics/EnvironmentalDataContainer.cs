﻿namespace COGNAV.EnvGraphics {
    public class EnvironmentalDataContainer {
        
        public double X { get; set; }
        public double Y { get; set; }
        public double Rotation { get; set; }

        public EnvironmentalDataContainer(double x, double y, double rot) {
            this.X = x;
            this.Y = y;
            this.Rotation = rot;
        }
    }
}