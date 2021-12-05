using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Numerics;
using COGNAV.Specs;

namespace COGNAV.Control {
    public static class PathHelper {


        /**
         * Gets the angle of a line starting at 0,0 and going to X,Y
         */
        public static float GetDirection(float x, float y) {
            Double dir = (Math.Atan2(x, y) * (180 / Math.PI));

            if (dir < 0) dir += 360;
            
            return Convert.ToSingle(dir);
        }

        /**
         * Calculate the point in orbit of another object
         */
        public static PointF CalculateOrbit(PointF center, float radius, float angle) {

            float rad = ToRadians(angle);
            
            center.X = center.X + Convert.ToSingle(radius * Math.Sin(rad));
            center.Y = center.Y + Convert.ToSingle(radius * Math.Cos(rad));

            return center;
        }

        /**
         * Detects if a point has been scanned in or not
         */
        public static bool IsPointKnown(PointF point, List<Zone> zones) {
            foreach (Zone zone in zones) {
                if (DoesPointCollide(point, new PointF(zone.X, zone.Y),  zone.Radius)) return false;
            }

            return true;
        }


        /**
         * Detects if the Roomba can traverse a specified path 
         */
        public static bool IsSegmentTraversable(PointF start, PointF end, List<Obstacle> obstacles, float offset) {
            foreach (Obstacle obj in obstacles) {
                if (DoesSegmentCollide(start, end, new PointF(obj.X, obj.Y),  (obj.Diameter / 2F) + offset + Roomba.RoombaRadiusMeters)) return false;
            }

            return true;
        }
        
        /**
         * Detects if the Roomba can be at a point
         */
        public static bool IsPointValid(PointF point, List<Obstacle> obstacles, float offset) {
            foreach (Obstacle obj in obstacles) {
                if (DoesPointCollide(point, new PointF(obj.X, obj.Y),  (obj.Diameter / 2F) + offset + Roomba.RoombaRadiusMeters)) return false;
            }

            return true;
        }

        /**
         * Detects if a line segment collides with a circle
         * Me when math :( 
         */
        public static bool DoesSegmentCollide(PointF start, PointF end, PointF circle, float radius) {
            Vector2 d = new Vector2(end.X - start.X, end.Y - start.Y);
            Vector2 f = new Vector2(start.X - circle.X, start.Y - circle.Y);

            // Math my beloved
            float a = Vector2.Dot(d, d);
            float b = 2F * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - (radius * radius);

            // calculate discriminant
            float disc = (b * b) - (4 * a * c);

            
            if (!(disc < 0)) {
                // There may be a collision, but not for sure
                disc = Convert.ToSingle(Math.Sqrt(disc));
                
                float t1 = (-b - disc)/(2 * a);
                float t2 = (-b + disc)/(2 * a);

                // Collision cirCUMstances
                // They each have a different meaning, but we don't care
                if (t1 >= 0 && t1 <= 1) return true;
                if (t2 >= 0 && t2 <= 1) return true;

            }

            // No collision! Yay
            return false;
        }

        /**
         * Checks if a point collides with a circle
         */
        public static bool DoesPointCollide(PointF point, PointF circle, float radius) {
            double dist = Distance(point, circle);

            return (dist <= radius);
        }

        /**
         * Calculates distance
         */
        public static double Distance(PointF start, PointF end) {
            return Math.Sqrt(((end.X - start.X) * (end.X - start.X)) + ((end.Y - start.Y) * (end.Y - start.Y)));
        }

        /**
         * Calculates float distance
         */
        public static float DistanceF(PointF start, PointF end) {
            return Convert.ToSingle(Distance(start, end));
        }


        /**
         * Calculates radians
         */
        public static float ToRadians(float degrees) {
            return DisplacementHelper.ToRadians(degrees);
        }

    }
}