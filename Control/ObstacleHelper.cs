using System.Collections.Generic;
using System.Drawing;
using COGNAV.EnvGraphics;

namespace COGNAV.Control {
    public class ObstacleHelper {

        public static List<EnvironmentShape> BuildEnvironmentShapes(Field field) {
            List<EnvironmentShape> output = new List<EnvironmentShape>();

            foreach (Obstacle obj in field.Obstacles) {
                if (obj.Type == ObstacleClass.Blip) {
                    // Create Blip Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Square;
                    shape.ShapeColor = Color.Salmon;
                    
                    output.Add(shape);
                } else if (obj.Type == ObstacleClass.Blob) {
                    // Create Blob Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Circle;
                    shape.ShapeColor = Color.Gray;
                    
                    output.Add(shape);
                } else if (obj.Type == ObstacleClass.Wide) {
                    // Create Wide Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Circle;
                    shape.ShapeColor = Color.Silver;
                    
                    output.Add(shape);
                } else if (obj.Type == ObstacleClass.Thin) {
                    // Create Thin Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Circle;
                    shape.ShapeColor = Color.Gold;
                    
                    output.Add(shape);
                } else if (obj.Type == ObstacleClass.Short) {
                    // Create Short Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Circle;
                    shape.ShapeColor = Color.Brown;
                    
                    output.Add(shape);
                }
                else if (obj.Type == ObstacleClass.Hole) {
                    // Create Hole Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Square;
                    shape.ShapeColor = Color.Red;
                    
                    output.Add(shape);
                } else if (obj.Type == ObstacleClass.Tape) {
                    // Create Tape Shape
                    EnvironmentShape shape = new EnvironmentShape(obj.X, obj.Y, obj.Diameter, obj.Diameter);

                    shape.ShapeGeometry = Shape.Cross;
                    shape.ShapeColor = Color.Red;
                    
                    output.Add(shape);
                }
            }

            return output;
        }

    }
}