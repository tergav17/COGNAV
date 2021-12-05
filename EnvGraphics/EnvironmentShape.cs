using System.Drawing;

namespace COGNAV.EnvGraphics {

    public enum Shape {
        Circle,
        Cross,
        Square,
        Line
    }

    public class EnvironmentShape {

        public static Shape Circle = Shape.Circle;
        public static Shape Cross = Shape.Cross;
        public static Shape Square = Shape.Square;
        public static Shape Line = Shape.Line;
        
        public Shape ShapeGeometry { get; set; }
        
        public Color ShapeColor { get; set; }
        
        public double X { get; set; }
        
        public double Y { get; set; }
        
        public double EndX { get; set; }
        
        public double EndY { get; set; }
        
        public double Width { get; set; }
        
        public double Height { get; set; }

        public EnvironmentShape(double x, double y, double width, double height) {
            X = x;
            Y = y;
            EndX = x;
            EndY = y;
            
            Width = width;
            Height = height;

            ShapeGeometry = Circle;
            ShapeColor = Color.Red;
        }

    }
}