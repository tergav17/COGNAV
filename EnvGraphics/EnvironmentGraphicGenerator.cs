using System;
using System.Drawing;
using COGNAV.Specs;

namespace COGNAV.EnvGraphics {
    public static class EnvironmentGraphicGenerator {

        /**
         * Constant land
         */
        
        // Number of squares wide
        private const int Scale = 14;
        
        // Represented meters per square
        private const double MetersPerSquare = 0.10;
        
        // Environment pixel width/height
        private const int EWidth = 450;
        private const int EHeight = 450;
        
        // Adds extra lines out of bounds
        private const int Extension = 3;
        
        /*
         * Derivative Constants
         */
        private const int WidthScalar = EWidth / Scale;
        private const int HeightScalar = EHeight / Scale;
        private const double CmPerSquare = MetersPerSquare * 100;
        private const double RobotRadius = Roomba.RoombaRadiusCm;

        public static int DrawEnvironment(Image img, EnvironmentalDataContainer data) {
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen gridPen = new Pen(Color.LightGreen, 2);

            Pen robotPen = new Pen(Color.LightGray, 3);

            try {

                using var graphics = Graphics.FromImage(img);
                
                graphics.FillRectangle(blackBrush, 0, 0, EWidth, EHeight);

                /*
                Point start = Rotate(new Point(0, 0), data.Rotation);
                Point end = Rotate(new Point(EWidth, 0), data.Rotation);
                
                graphics.DrawLine(gridPen, start, end);
                */
                
                
                // Draw grid lines
                for (int i = ((int) ((data.Y / MetersPerSquare) * HeightScalar) % HeightScalar) - (HeightScalar * Extension); i < EHeight + (HeightScalar * Extension); i += HeightScalar) {
                    Point start = Rotate(new Point(-(WidthScalar * Extension), i), data.Rotation);
                    Point end = Rotate(new Point(EWidth + (WidthScalar * Extension), i), data.Rotation);
                    
                    graphics.DrawLine(gridPen,start, end);
                }
            
                for (int i = ((int) (-(data.X / MetersPerSquare) * WidthScalar) % WidthScalar) - (WidthScalar * Extension); i < EWidth + (WidthScalar * Extension); i += WidthScalar) {
                    Point start = Rotate(new Point(i, -(HeightScalar * Extension)), data.Rotation);
                    Point end = Rotate(new Point(i, EHeight + (HeightScalar * Extension)), data.Rotation);
                    
                    graphics.DrawLine(gridPen,start, end);
                }
                
                // Draw all shapes
                foreach (EnvironmentShape shape in data.Shapes) {

                    // Create pen
                    Pen shapePen = new Pen(shape.ShapeColor, 3);

                    // Calculate integer location of object to be drawn
                    Point iPoint = new Point((int) (((shape.X / MetersPerSquare) - (data.X / MetersPerSquare)) * WidthScalar), (int) -(((shape.Y / MetersPerSquare) - (data.Y / MetersPerSquare)) * HeightScalar));
                    Point iPointEnd = new Point((int) (((shape.EndX / MetersPerSquare) - (data.X / MetersPerSquare)) * WidthScalar), (int) -(((shape.EndY / MetersPerSquare) - (data.Y / MetersPerSquare)) * HeightScalar));
                    
                    iPoint.X = iPoint.X + (EWidth / 2);
                    iPoint.Y = iPoint.Y + (EHeight / 2);
                    
                    iPointEnd.X = iPointEnd.X + (EWidth / 2);
                    iPointEnd.Y = iPointEnd.Y + (EHeight / 2);

                    int hw  = (int) (((shape.Width / MetersPerSquare) * WidthScalar) / 2);
                    int hh = (int) (((shape.Height / MetersPerSquare) * HeightScalar) / 2);
                    
                    // Rotate that point
                    Point dPoint = Rotate(iPoint, data.Rotation);
                    Point dPointEnd = Rotate(iPointEnd, data.Rotation);

                    // Do the actual drawing
                    if (shape.ShapeGeometry == Shape.Circle) {
                        graphics.DrawEllipse(shapePen, dPoint.X - hw, dPoint.Y - hh, hw * 2, hh * 2);
                    } else if (shape.ShapeGeometry == Shape.Cross) {
                        graphics.DrawLine(shapePen, dPoint.X - hw, dPoint.Y - hh, dPoint.X + hw, dPoint.Y + hh);
                        graphics.DrawLine(shapePen, dPoint.X - hw, dPoint.Y + hh, dPoint.X + hw, dPoint.Y - hh);
                    } else if (shape.ShapeGeometry == Shape.Square) {
                        graphics.DrawRectangle(shapePen, dPoint.X - hw, dPoint.Y - hh, hw * 2, hh * 2);
                    } else if (shape.ShapeGeometry == Shape.Line) {
                        graphics.DrawLine(shapePen, dPoint, dPointEnd);
                    }

                    shapePen.Dispose();
                }

                // Draw robot
                int robotWidth = (int) (WidthScalar * ((RobotRadius*2) / CmPerSquare));
                int robotHeight = (int) (HeightScalar * ((RobotRadius*2) / CmPerSquare));
                graphics.DrawEllipse(robotPen, (EWidth / 2) - (robotWidth / 2), (EHeight / 2) - (robotHeight / 2), robotWidth, robotHeight);
                graphics.DrawLine(robotPen, (EWidth / 2), (EHeight / 2), (EWidth / 2), (EHeight / 2) - robotHeight/2);
                

            } catch {

                return -1;

            }

            return 0;


        }

        /**
         * Creates a blank image to the specifications of the environment viewer
         */
        public static Image GenerateImage() {
            Image img = new Bitmap(EWidth, EHeight);
            
            return img;
        }

        /**
         * Rotates a point around the middle of the environment
         */
        public static Point Rotate(Point p, double rot) {
            
            // Translate mid to 0,0
            double dx = p.X - (EWidth / 2.0);
            double dy = p.Y - (EHeight / 2.0);

            // Rotate
            double dxR = (dx * Math.Cos(-rot)) - (dy * Math.Sin(-rot));
            double dyR = (dx * Math.Sin(-rot)) + (dy * Math.Cos(-rot));
                
            // Translate back to mid
            dx = dxR +  (EWidth / 2.0);
            dy = dyR +  (EHeight / 2.0);    
            
            return new Point((int) dx, (int) dy);
        }
    }
}