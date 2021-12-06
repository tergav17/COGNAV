using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using COGNAV.Interface;

namespace COGNAV.Control {
    public class PathFinder {
        private const int North = 0;
        private const int East = 1;
        private const int South = 2;
        private const int West = 3;

        private const float Spacing = 0.02F;
        private const float Safety = 0.03F;


        /**
         * Calculates the next vector that the robot should move to
         */ 
        public static (float, float, bool) GetNextTrajectory(List<PathNode> path, PointF position, Field f) {
            (float Direction, float Distance, bool DoScan) output = (0F, 0F, false);
            
            if (path == null) return (0, 0, false);
            if (path.Count == 0) return (0, 0, false);
            
            PathNode goal = path[0];

            // Find the goal point to point towards
            for (int i = 1; i < path.Count; i++) {
                // Checks if the current path is traversable
                if (!PathHelper.IsSegmentTraversable(position, new PointF(path[i].X, path[i].Y), f.Obstacles, Safety / 2F)) {
                    if (PathHelper.Distance(position, new PointF(path[i].X, path[i].Y)) < 0.05) {
                        goal = path[i];
                    } else {
                        i = path.Count;
                        output.DoScan = false;
                    }
                } else {
                    // Going into an unknown area triggers a scan
                    if (PathHelper.IsPointKnown(new PointF(path[i].X, path[i].Y), f.ScannedAreas)) {
                        goal = path[i];
                    } else {
                        i = path.Count;
                        output.DoScan = true;
                    }
                }
            }

            // Create trajectory
            output.Direction = PathHelper.GetDirection(goal.X - position.X, goal.Y - position.Y);
            output.Distance = PathHelper.DistanceF(position, new PointF(goal.X, goal.Y));

            return output;
        }

        /**
         * Reduces the generated path to a number of general waypoints
         */
        public static List<PathNode> ReducePath(List<PathNode> path, Field f) {
            List<PathNode> outPath = new List<PathNode>();

            if (path == null) return null;
            if (path.Count == 0) return outPath;

            outPath.Add(path[0]);

            for (int i = 1; i < path.Count; i++) {
                if (i + 1 >= path.Count) {
                    outPath.Add(path[i]);
                } else {
                    PathNode nextNode = path[i + 1];
                    
                    PointF top = new PointF(outPath[^1].X, outPath[^1].Y);
                    PointF next = new PointF(nextNode.X, nextNode.Y);

                    if (!PathHelper.IsSegmentTraversable(top, next, f.Obstacles, Safety / 2F)) {
                        outPath.Add(path[i]);
                    }
                }
            }

            return outPath;
        }

        /**
         * Here it is boys, the A-STAR pathfinding function
         * May god have mercy on our souls
         */
        public static List<PathNode> FindPath(PathNode start, PathNode end, Field f, GraphicConsole console) {

            console.PutStartup("Starting Pathfinding...");
            long startTime = DateTime.Now.Millisecond;
            
            
            List<PathNode> outPath = new List<PathNode>();

            // First things first, create a grid to start with
            List<PathNode> allNodes  = GridBuilder.BuildGrid(f.WestFront, f.NorthFront, f.EastFront, f.SouthFront, Spacing);
            PathNode grid = allNodes[0];
            
            // Remove all invalid nodes
            for (int i = 0; i < allNodes.Count; i++) {
                PathNode n = allNodes[i];
                PointF nodeP = new PointF(n.X, n.Y);

                // Remove if not valid
                if (!PathHelper.IsPointValid(nodeP, f.Obstacles, Safety)) {
                    RemoveNode(n);
                    allNodes.RemoveAt(i);
                    i--;
                }
            }

            // Calculate end node
            PointF startP = new PointF(start.X, start.Y);
            PointF endP = new PointF(end.X, end.Y);

            PathNode endN = null;
            float shortest = Single.MaxValue;

            foreach (PathNode n in allNodes) {
                PointF nodeP = new PointF(n.X, n.Y);

                float distance = Convert.ToSingle(PathHelper.Distance(endP, nodeP));

                if (distance < shortest) {
                    if (PathHelper.IsSegmentTraversable(nodeP, endP, f.Obstacles, Safety)) {
                        shortest = distance;
                        endN = n;
                    }
                }
            }

            // If none was found, no path can be made
            if (endN == null) {
                console.PutError("Can't Find End Node!");
                return null;
            }
            endN.EndPath = end;
            
            // Calculate start node
            PathNode startN = null;
            for (int i = 0; i < 2; i++) {
                // There are two passes to this
                // If a valid node is found instantly, the loop exits
                // Otherwise, another pass is done without traversability checking
                shortest = Single.MaxValue;

                foreach (PathNode n in allNodes) {
                    PointF nodeP = new PointF(n.X, n.Y);

                    float distance = Convert.ToSingle(PathHelper.Distance(startP, nodeP));

                    if (distance < shortest) {
                        //console.PutLine("Checking: " + n.X + ", " + n.Y);
                        
                        if (i == 1 || PathHelper.IsSegmentTraversable(nodeP, startP, f.Obstacles, Safety)) {
                            shortest = distance;
                            startN = n;
                        } else {
                            //console.PutLine("Failed: " + nodeP.X + ", " + nodeP.Y + " to " + startP.X + ", " + startP.Y);
                        }
                    }
                }

                if (startN != null) i++;
            }

            // If none was found, no path can be made
            if (startN == null) {
                console.PutError("Can't Find Start Node!");
                return null;
            }

            startN.Visited = false;
            startN.PathDistance = 0;
            startN.Cost = 0;

            List<PathNode> openNodes = new List<PathNode>();
            openNodes.Add(startN);

            // Start pathfinding
            while (openNodes.Count > 0) {
                PathNode currN = null;
                float minCost = Single.MaxValue;

                // Find the cheapest node
                foreach (PathNode n in openNodes) {
                    if (n.Cost < minCost) {
                        minCost = n.Cost;
                        currN = n;
                    }
                }
                // This can't happen
                if (currN == null) return null;
                
                // Visit it
                currN.Visited = true;
                
                // Check if currN is endN
                if (currN.EndPath != null) {
                    // If no, no pathfinding needed
                    outPath.Add(end);

                    while (!currN.Equals(startN)) {
                        outPath.Insert(0, currN);
                        currN = currN.BackPath;
                    }
                    
                    outPath.Insert(0, startN);

                    long endTime = DateTime.Now.Millisecond;
                    
                    
                    console.PutSuccess("Pathfinding Complete in " + (endTime - startTime) + "ms!");
                    
                    return outPath;
                }
                
                
                // Remove from openNodes
                openNodes.Remove(currN);
                
                // Do up the neighbors
                for (int i = 0; i < 4; i++) {
                    if (currN.NodeMap[i] != null && !currN.NodeMap[i].Visited) {
                        PathNode potN = currN.NodeMap[i];
                        
                        float potDist = currN.PathDistance + 1;
                        float potCost = Convert.ToSingle(PathHelper.Distance(new PointF(potN.X, potN.Y), new PointF(endN.X, endN.Y)));

                        if (potCost < potN.Cost) {
                            potN.Cost = potCost;
                            potN.PathDistance = potDist;
                            potN.BackPath = currN;
                            
                            // If it isn't in open nodes, put it there
                            if (!openNodes.Contains(potN)) openNodes.Add(potN);
                        }
                    }
                }
            }

            // Exhausted all nodes
            console.PutError("Can't Find Path!");
            return null;
        }

        /**
         * Removes a node
         */
        private static void RemoveNode(PathNode n) {
            for (int i = 0; i < 4; i++) {
                if (n.NodeMap[i] != null) {
                    // Calculate mirror direction
                    int mirror = (i + 2) % 4;

                    // Nullify from both sides
                    n.NodeMap[i].NodeMap[mirror] = null;
                    n.NodeMap[i] = null;
                }
            }
        }
    }
}