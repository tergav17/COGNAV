using System.Collections.Generic;
using System.Drawing;

namespace COGNAV.Control {
    public class Field {

        public List<Obstacle> Obstacles = new List<Obstacle>();

        public List<PointF> NucleationPoints = new List<PointF>();

        public double NorthFront = 2;
        public double EastFront = 2;
        public double SouthFront = -2;
        public double WestFront = -2;

    }
}