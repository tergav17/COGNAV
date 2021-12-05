using System;
using System.Collections.Generic;

namespace COGNAV.Control {
    public class PathNode {
        
        public PathNode BackPath { get; set; }
        
        public PathNode EndPath { get; set; }
        
        public float X { get; set; }
        
        public float Y { get; set; }
        
        public float PathDistance { get; set; }

        public float Cost { get; set; }
        
        public bool Visited { get; set; }
        
        public PathNode[] NodeMap = new PathNode[4];

        public PathNode(float x, float y) {
            X = x;
            Y = y;
            Visited = false;

            // Make sure all neighbors are null
            for (int i = 0; i < 4; i++) NodeMap[i] = null;

            EndPath = null;
            BackPath = null;
            
            Cost = Single.MaxValue;
        }

    }
}