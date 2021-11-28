using System;
using System.Drawing;

namespace COGNAV.EnvGraphics {
    public class EnvironmentGraphicGenerator {

        private const int EWidth = 450;
        private const int EHeight = 450;
        private const int Scale = 10;

        private const int WidthScalar = EWidth / Scale;
        private const int HeightScalar = EHeight / Scale;

        private const int Extension = 2;

        public static int DrawEnvironment(Image img, EnvironmentalDataContainer data) {
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen gridPen = new Pen(Color.LightGreen, 2);

            try {

                using var graphics = Graphics.FromImage(img);
                
                graphics.FillRectangle(blackBrush, 0, 0, EWidth, EHeight);

                /*
                Point start = Rotate(new Point(0, 0), data.Rotation);
                Point end = Rotate(new Point(EWidth, 0), data.Rotation);
                
                graphics.DrawLine(gridPen, start, end);
                */
                
                
                // Draw grid lines
                for (int i = ((int) (data.Y * HeightScalar) % HeightScalar) - (HeightScalar * Extension); i < EHeight + (HeightScalar * Extension); i += HeightScalar) {
                    Point start = Rotate(new Point(-(WidthScalar * Extension), i), data.Rotation);
                    Point end = Rotate(new Point(EWidth + (WidthScalar * Extension), i), data.Rotation);
                    
                    graphics.DrawLine(gridPen,start, end);
                }
            
                for (int i = ((int) (data.X * WidthScalar) % WidthScalar) - (WidthScalar * Extension); i < EWidth + (WidthScalar * Extension); i += WidthScalar) {
                    Point start = Rotate(new Point(i, -(HeightScalar * Extension)), data.Rotation);
                    Point end = Rotate(new Point(i, EHeight + (HeightScalar * Extension)), data.Rotation);
                    
                    graphics.DrawLine(gridPen,start, end);
                }
                

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
            double dxR = (dx * Math.Cos(rot)) - (dy * Math.Sin(rot));
            double dyR = (dx * Math.Sin(rot)) + (dy * Math.Cos(rot));
                
            // Translate back to mid
            dx = dxR +  (EWidth / 2.0);
            dy = dyR +  (EHeight / 2.0);    
            
            return new Point((int) dx, (int) dy);
        }
    }
}