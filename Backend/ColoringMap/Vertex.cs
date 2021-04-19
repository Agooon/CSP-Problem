using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ColoringMap
{
    public class Vertex
    {
        public Point Point { get; set; }

        public List<Vertex> Neighbors { get; set; } = new List<Vertex>();

        public Vertex(Point point)
        {
            Point = point;
        }

        public List<Point> GetPossibleConnections(ref List<Vertex> remaingVertices, ref List<LinearParameters> usedFunctions)
        {
            List<Point> possiblePoints = new List<Point>();

            foreach (Vertex vertex in remaingVertices)
            {
                if (vertex != this)
                {
                    // Checking if vertex isn't already connected
                    if (!Neighbors.Contains(vertex))
                    {
                        LinearParameters currFun = new LinearParameters(Point, vertex.Point);

                        bool crosses = false;

                        foreach (LinearParameters func in usedFunctions)
                        {
                            crosses = currFun.Crosses(func);
                            if (crosses)
                                break;
                        }
                        if (!crosses)
                            possiblePoints.Add(vertex.Point);
                    }
                }

            }
            return possiblePoints;
        }
    }
}
