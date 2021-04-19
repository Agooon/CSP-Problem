using Backend.ColoringMap;
using System;

namespace ConsoleFront
{
    class Program
    {
        static void Main(string[] args)
        {
            //int h, int w, int n, int seed
            ProblemMap pm = new ProblemMap(5, 5, 5, 2);
            int x = 1;

            var x1 = new Point(3, 4);
            var x2 = new Point(0, 2);

            var xd = new LinearParameters(x2, x1);
            foreach (var item in pm.vertices)
            {
                Console.WriteLine(item.Point.X + " : " + item.Point.Y);
            }
            pm.RandomizeConnections(ref pm.vertices);
            var verticesAll = pm.SetColors(3);
            //int ind = 1;
            
            for (int i = 0; i < verticesAll.Count; i++)
            {
                Console.WriteLine("\n\n\n\n");
                foreach (var item in verticesAll[i])
                {
                    Console.WriteLine("\n" + i + ". point | " + item.Point.X + " : " + item.Point.Y + " | C: " + item.Color);
                    foreach (var item2 in item.Neighbors)
                    {
                        Console.WriteLine(item2.Point.X + " : " + item2.Point.Y + " | C: " + item2.Color);
                    }
                }
            }
            
        }
    }
}
