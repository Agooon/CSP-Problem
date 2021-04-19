using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ColoringMap
{
    public class Point : IEquatable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }
        
        public bool Equals(Point other)
        {
            if (other == null)
                return false;

            if (other.X == X && other.Y == Y)
                return true;
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Point point = obj as Point;
            if (point == null)
                return false;
            else
                return Equals(point);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }

        public double CalculateDisctance(ref Point point)
        {
            return Math.Sqrt( Math.Pow((X - point.X),2) + Math.Pow((Y - point.Y),2));
        }

        public ref Point GetClosestPoint(ref Point[] points)
        {
            int bestInd = 0;
            double bestDist = CalculateDisctance(ref points[0]);
            for (int i = 1; i < points.Length; i++)
            {
                double currDist = CalculateDisctance(ref points[i]);
                if (bestDist > currDist)
                {
                    bestDist = currDist;
                    bestInd = i;
                }
            }
            return ref points[bestInd];
        }
    }
}
