using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SC.Math
{
    public class Line
    {

        private static bool onSegment(Vector2 p, Vector2 q, Vector2 r)
        {
            if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
                q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private static int orientation(Vector2 p, Vector2 q, Vector2 r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
            // for details of below formula. 
            float val = (q.y - p.y) * (r.x - q.x) -
                      (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0;  // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        public static bool DoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
            {
            // Find the four orientations needed for general and 
            // special cases 
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }

        public static Vector2 GetLineIntersection(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            // Line AB represented as a1x + b1y = c1  
            double a1 = B.y - A.y;
            double b1 = A.x - B.x;
            double c1 = a1 * (A.x) + b1 * (A.y);

            // Line CD represented as a2x + b2y = c2  
            double a2 = D.y - C.y;
            double b2 = C.x - D.x;
            double c2 = a2 * (C.x) + b2 * (C.y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                // The lines are parallel. This is simplified  
                // by returning a pair of FLT_MAX  
                return Vector2.positiveInfinity;
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                return new Vector2((float)x,(float) y);
            }
        }

        public static System.Func<float, float> CalculateLinearRegressionX(Vector2 p_point1, Vector2 p_point2)
        {
            float m = CalculateSlopeM(p_point1, p_point2);
            float b = CalculateSlopeB(p_point1, m);

            return (y) => (y - b) / m;
        }

        public static System.Func<float, float> CalculateLinearRegressionY(Vector2 p_point1, Vector2 p_point2) {
            float m = CalculateSlopeM(p_point1, p_point2);
            float b = CalculateSlopeB(p_point1, m);

            return (x) => (m*x) + b;
        }

        public static float CalculateSlopeM(Vector2 p_point1, Vector2 p_point2) {
            return (p_point2.y - p_point1.y) / (p_point2.x - p_point1.x);
        }

        public static float CalculateSlopeB(Vector2 point, float m)
        {
            return -(point.x * m) + point.y;
        }



    }
}
