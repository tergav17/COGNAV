using System.Collections.Generic;

namespace COGNAV.Control {
    public class GridBuilder {

        private const int North = 0;
        private const int East = 1;
        private const int South = 2;
        private const int West = 3;
        
        // Builds a 2D grid of nodes
        public static List<PathNode> BuildGrid(float startX, float startY, float endX, float endY, float space) {
            
            // Create output list
            List<PathNode> output = new List<PathNode>();
            
            // Start node
            PathNode startN = new PathNode(startX, startY);
            output.Add(startN);
            
            // Create initial layer
            float currX = startX;
            PathNode currN = startN;
            
            while (currX <= endX) {

                // Create node
                PathNode newN = new PathNode(currX, startY);
                output.Add(newN);
                
                // Interlink nodes
                currN.NodeMap[East] = newN;
                newN.NodeMap[West] = currN;
                
                // Advance node
                currN = newN;
                
                // Advance X pointer
                currX += space;
            }

            // Advance space, and create base layer node
            startY += space;
            PathNode baseN = startN;
            
            while (startY <= endY) {

                currN = baseN;
                PathNode lastN = null;

                bool cont = true;
                while (cont) {

                    PathNode newN = new PathNode(currN.X, currN.Y + space);
                    output.Add(newN);

                    // Interlink nodes
                    currN.NodeMap[South] = newN;
                    newN.NodeMap[North] = currN;

                    if (lastN != null) {
                        // Interlink these nodes too
                        newN.NodeMap[West] = lastN;
                        lastN.NodeMap[East] = newN;
                    }

                    lastN = newN;

                    // Either stop, or over on to the next node
                    if (currN.NodeMap[East] == null) {
                        cont = false;
                    } else {
                        currN = currN.NodeMap[East];
                    }
                }

                // Move down
                baseN = baseN.NodeMap[South];
                startY += space;
            }

            return output;
        }
    }
}