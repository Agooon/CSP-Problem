using Backend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ColoringMap
{
    public class ProblemMap
    {
        private readonly int height;
        private readonly int width;

        public List<Vertex> vertices = new List<Vertex>();

        public Random Rnd { get; set; }

        public ProblemMap()
        {
            height = Globals.Height;
            width = Globals.Width;
            Rnd = new Random(Globals.Seed);
            RandomizePoints(Globals.NumbeOfPoints);
        }

        public ProblemMap(int h, int w, int n)
        {
            height = h;
            width = w;
            Rnd = new Random();
            if (h * w < n)
                RandomizePoints(h * w);
            else
                RandomizePoints(n);
        }

        public ProblemMap(int h, int w, int n, int seed)
        {
            height = h;
            width = w;
            Rnd = new Random(seed);
            if (h * w < n)
                RandomizePoints(h * w);
            else
                RandomizePoints(n);
        }


        private void RandomizePoints(int n)
        {
            List<Point> remainingPoints = new List<Point>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    remainingPoints.Add(new Point(i, j));
                }
            }

            // From Remaining points we are choosing our n points
            for (int i = 0; i < n; i++)
            {
                int randIndex = Rnd.Next(0, remainingPoints.Count);
                Vertex newVertex = new Vertex(remainingPoints[randIndex]);
                remainingPoints.RemoveAt(randIndex);
                vertices.Add(newVertex);
            }
        }

        public void RandomizeConnections(ref List<Vertex> givenVertices)
        {
            int verticesLeft = givenVertices.Count;

            List<Vertex> remaingVertices = givenVertices.Select(x => x).ToList();
            Vertex currentVertex;

            Point[] possibleConnections;
            Point choosenPoint;

            List<LinearParameters> usedFunctions = new List<LinearParameters>();

            while (verticesLeft > 0)
            {
                // From remaing vertices choose one vertex
                currentVertex = remaingVertices[Rnd.Next(0, verticesLeft)];

                // Get possible connections
                possibleConnections = currentVertex.GetPossibleConnections(ref remaingVertices, ref usedFunctions).ToArray();

                // If none of connections can be made, we are deleting the current vertex
                if (possibleConnections.Length == 0)
                {
                    remaingVertices.Remove(currentVertex);
                    verticesLeft--;
                }
                // From possible points, we are choosing the shortest one
                else
                {

                    choosenPoint = currentVertex.Point.GetClosestPoint(ref possibleConnections);

                    Vertex choosenVertex = remaingVertices.FirstOrDefault(x => x.Point.Equals(choosenPoint));

                    choosenVertex.Neighbors.Add(currentVertex);
                    choosenVertex.NeightborPoints.Add(currentVertex.Point);

                    currentVertex.Neighbors.Add(choosenVertex);
                    currentVertex.NeightborPoints.Add(choosenPoint);

                    // Adding new Functions to used ones 
                    usedFunctions.Add(new LinearParameters(currentVertex.Point, choosenPoint));

                    if (possibleConnections.Length == 1)
                    {
                        remaingVertices.Remove(currentVertex);
                        verticesLeft--;
                    }
                }
            }
        }

        public List<List<Vertex>> SetColors(int k)
        {
            List<List<Vertex>> verticesAll = new List<List<Vertex>>();

            for (int i = 0; i < vertices.Count; i++)
            {
                verticesAll.Add(new List<Vertex>());
                for (int j = 0; j < vertices.Count; j++)
                {
                    verticesAll[i].Add(new Vertex(vertices[j]));
                }
                // Setting up same connections
                for (int j = 0; j < vertices.Count; j++)
                {
                    foreach (Point point in verticesAll[i][j].NeightborPoints)
                    {
                        verticesAll[i][j].Neighbors.Add(verticesAll[i].FirstOrDefault(x => x.Point.Equals(point)));
                    }
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                // For each starting vertex we are looking for solution
                var currentStartingVects = verticesAll[i];
                for (int j = 1; j <= k; j++)
                {
                    verticesAll[i][i].SetColors(k, j, ref currentStartingVects);
                }

            }

            return verticesAll;
        }



    }
}
