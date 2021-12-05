using System.Collections.Generic;
using System.Drawing;

namespace COGNAV.Control {
    public class Field {

        public List<Obstacle> Obstacles = new List<Obstacle>();

        public List<Zone> ScannedAreas = new List<Zone>();

        public List<Partition> Partitions = new List<Partition>();

        public float NorthFront = -2;
        public float EastFront = 2;
        public float SouthFront = 2;
        public float WestFront = -2;

    }
}