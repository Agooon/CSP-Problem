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

        public List<Point> NeightborPoints { get; set; } = new List<Point>();

        public int Color { get; set; } = -1;

        public Vertex(Point point)
        {
            Point = point;
        }

        public Vertex(Vertex v)
        {
            Point = new Point(v.Point);
            Color = v.Color;
            NeightborPoints = v.NeightborPoints.Select(x => new Point(x)).ToList();
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


        public bool SetColors(int k, int color, ref List<Vertex> vertices)
        {

            for (int i = 0; i < Neighbors.Count; i++)
            {
                if (Neighbors[i].Color == color)
                    return false;
            }
            Color = color;

            for (int i = 0; i < Neighbors.Count; i++)
            {
                for (int j = 1; j <= k; j++)
                {
                    if (Neighbors[i].Color != -1)
                        break;
                    if (Neighbors[i].SetColors(k, j, ref vertices))
                        break;    
                }
                if (Finished(ref vertices))
                    return true;
            }

            return false;
        }

        public bool Finished(ref List<Vertex> vertices)
        {
            foreach (var vertex in vertices)
                if (vertex.Color == -1)
                    return false;
            return true;
        }
    }
}
