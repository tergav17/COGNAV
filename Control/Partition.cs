using System;
using System.Collections.Generic;
using System.Drawing;

namespace COGNAV.Control {
    public class Partition {

        public const int NumBranches = 24;
        public const int Spread = 3;
        
        public float X { get; set; }
        
        public float Y { get; set; }
        
        public float Radius { get; set; }

        public bool[] Branches = new bool[NumBranches];

        public Partition(float x, float y, float radius) {
            X = x;
            Y = y; 
            Radius = radius;

            for (int i = 0; i < NumBranches; i++) Branches[i] = true;
        }

        /**
         * Zaps a zone on the branches array, plus an amount of spread
         */
        public void ZapBranch(int branch) {
            // Zap all branches
            for (int i = 0; i < Spread; i++) {
                Branches[(branch + i) % NumBranches] = false;
                Branches[(branch - i) % NumBranches] = false;
            }

        }

        /**
         * Checks if any points are overlapping, and disables them
         */
        public void UpdateValidity(List<Partition> partitions) {
            for (int i = 0; i < NumBranches; i++) {
                if (IsOverlapping(GetBranchPoint(i), partitions)) Branches[i] = false;
            }
        }

        /**
         * Calculates the position point of a branch
         */
        public PointF GetBranchPoint(int branch) {
            return PathHelper.CalculateOrbit(new PointF(X, Y), Radius,
                Convert.ToSingle((360.0 / NumBranches) * branch));
        }

        /**
         * Checks if a point is overlapping with an existing partition
         */
        public bool IsOverlapping(PointF branchL, List<Partition> partitions) {

            foreach (Partition part in partitions) {
                if (part != this && PathHelper.DoesPointCollide(branchL, new PointF(part.X, part.Y), part.Radius))
                    return true;
            }

            return false;
        }

    }
}