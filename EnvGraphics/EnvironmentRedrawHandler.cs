using System.Collections.Concurrent;
using System.Drawing;

namespace COGNAV.EnvGraphics {
    public class EnvironmentRedrawHandler {
        
        private System.Windows.Forms.PictureBox _environmentViewer;

        private ConcurrentQueue<EnvironmentalDataContainer> _dataIn = new ConcurrentQueue<EnvironmentalDataContainer>();

        private EnvironmentalDataContainer _currentData = new EnvironmentalDataContainer(0, 0, 0);
        
        private Image _environment = EnvironmentGraphicGenerator.GenerateImage();
        
        public EnvironmentRedrawHandler(System.Windows.Forms.PictureBox ev) {
            _environmentViewer = ev;
        }

        public void UpdateData(EnvironmentalDataContainer data) {
            _dataIn.Enqueue(data);
        }

        public void Redraw() {
            // Store if there is new data, and if a redraw needs to occur
            bool needsRedraw = false;
            
            // Attempt to update the current data with any new data
            while (!_dataIn.IsEmpty) {
                _dataIn.TryDequeue(out _currentData);
                needsRedraw = true;
            }

            // If a redraw is needed, then redraw
            if (needsRedraw) {
                EnvironmentGraphicGenerator.DrawEnvironment(_environment, _currentData);

                _environmentViewer.Image = _environment;
            }
        }

    }
}