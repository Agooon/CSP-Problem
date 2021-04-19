using Backend.ColoringMap;
using Backend.CSP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFront
{
    class Program
    {
        static void Main(string[] args)
        {
            //int h, int w, int n, int seed
            ProblemMap pm = new ProblemMap(3, 3, 3, 1);
            pm.RandomizeConnections(ref pm.vertices);
            int numberOfColors = 2;


            Dictionary<Vertex, List<int>> domains = new Dictionary<Vertex, List<int>>();

            foreach (Vertex vertex in pm.vertices)
            {
                List<int> colors = new List<int>();
                for (int i = 1; i <= numberOfColors; i++)
                {
                    colors.Add(i);
                }
                domains.Add(vertex, colors);
            }


            CSP<Vertex, int> csp = new CSP<Vertex, int>(pm.vertices, domains);

            List<Vertex> checkedVertices = new List<Vertex>();
            // Add Constraints
            foreach (Vertex vertex in domains.Keys)
            {
                foreach (Vertex neighbour in vertex.Neighbors)
                {
                    if (!checkedVertices.Contains(neighbour))
                        csp.AddConstraint(new MapColoringConstraint(vertex, neighbour));
                }
                checkedVertices.Add(vertex);
            }

            var solutions = csp.ForwardChecking();

            int index = 1;
            foreach (var vertex in pm.vertices)
            {
                Console.WriteLine($"\nPunkt {vertex.Point.X}:{vertex.Point.Y}");
                foreach (var neighbour in vertex.Neighbors)
                {
                    Console.WriteLine($"{neighbour.Point.X}:{neighbour.Point.Y}");
                }
            }
            Console.WriteLine($"\nNumber of consistent checks: {solutions.Item2}\n");
            foreach (var solution in solutions.Item1)
            {
                Console.WriteLine("Solution number " + index++ + ": ");
                foreach (var item in solution)
                {
                    Console.WriteLine($"{item.Key.Point.X}:{item.Key.Point.Y} - {item.Value}");
                }
            }

            solutions = csp.BacktrackingSearch();

            index = 1;
            foreach (var vertex in pm.vertices)
            {
                Console.WriteLine($"\nPunkt {vertex.Point.X}:{vertex.Point.Y}");
                foreach (var neighbour in vertex.Neighbors)
                {
                    Console.WriteLine($"{neighbour.Point.X}:{neighbour.Point.Y}");
                }
            }
            Console.WriteLine($"\nNumber of consistent checks: {solutions.Item2}\n");
            foreach (var solution in solutions.Item1)
            {
                Console.WriteLine("Solution number " + index++ + ": ");
                foreach (var item in solution)
                {
                    Console.WriteLine($"{item.Key.Point.X}:{item.Key.Point.Y} - {item.Value}");
                }
            }
            //foreach (var item in pm.vertices)
            //{
            //    Console.WriteLine(item.Point.X + " : " + item.Point.Y);
            //}
            //pm.RandomizeConnections(ref pm.vertices);
            //var verticesAll = pm.SetColors(4);
            ////int ind = 1;

            //for (int i = 0; i < verticesAll.Count; i++)
            //{
            //    Console.WriteLine("\n\n\n\n");
            //    foreach (var item in verticesAll[i])
            //    {
            //        Console.WriteLine("\n" + i + ". point | " + item.Point.X + " : " + item.Point.Y + " | C: " + item.Color);
            //        foreach (var item2 in item.Neighbors)
            //        {
            //            Console.WriteLine(item2.Point.X + " : " + item2.Point.Y + " | C: " + item2.Color);
            //        }
            //    }
            //}

        }
    }
}
