namespace COGNAV.Control {

    public enum ObstacleClass {
        Blip,
        Blob,
        Wide,
        Thin,
        Short,
        Tape,
        Hole
    }

    public class Obstacle {

        public float X { get; set; }

        public float Y { get; set; }
        
        public float Diameter { get; set; }
        
        public ObstacleClass Type { get; set; }

        public Obstacle(float x, float y, float diameter) {
            X = x;
            Y = y;
            Diameter = diameter;

            Type = ObstacleClass.Blip;
        }
    }
}