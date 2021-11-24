using System.Drawing;

namespace COGNAV.EnvGraphics {
    public class EnvironmentGraphicGenerator {

        private const int EWidth = 450;
        private const int EHeight = 450;
        private const int Scale = 10;

        private const int WidthScalar = EWidth / Scale;
        private const int HeightScalar = EHeight / Scale;

        public static int DrawEnvironment(Image img, EnvironmentalDataContainer data) {
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen gridPen = new Pen(Color.LightGreen, 2);

            try {

                using var graphics = Graphics.FromImage(img);
                
                graphics.FillRectangle(blackBrush, 0, 0, EWidth, EHeight);
            
                // Draw grid lines
                for (int i = (int) (data.Y * HeightScalar) % HeightScalar; i < EHeight; i += HeightScalar) {
                    graphics.DrawLine(gridPen, 0, i, EWidth, i);
                }
            
                for (int i = (int) (data.X * WidthScalar) % WidthScalar; i < EWidth; i += WidthScalar) {
                    graphics.DrawLine(gridPen, i, 0, i, EHeight);
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
    }
}