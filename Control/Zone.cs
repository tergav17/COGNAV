namespace COGNAV.Control {
    public class Zone {
     
        public float X { get; set; }
        
        public float Y { get; set; }
        
        public float Radius { get; set; }

        public Zone(float x, float y, float radius) {
            X = x;
            Y = y;
            Radius = radius;
        }

    }
}