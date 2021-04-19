using Backend.ColoringMap;
using Backend.CSP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleFront
{
    class Program
    {
        static void Main(string[] args)
        {
            ////int h, int w, int n, int seed
            //ProblemMap pm = new ProblemMap(10, 10, 10, 1);
            ////ProblemMap pm = new ProblemMap(4,4,5, 1);
            //pm.RandomizeConnections(ref pm.vertices);
            //int numberOfColors = 5;


            //Dictionary<Vertex, List<int>> domains = new Dictionary<Vertex, List<int>>();

            //foreach (Vertex vertex in pm.vertices)
            //{
            //    List<int> colors = new List<int>();
            //    for (int i = 1; i <= numberOfColors; i++)
            //    {
            //        colors.Add(i);
            //    }
            //    domains.Add(vertex, colors);
            //}


            //CSP<Vertex, int> csp = new CSP<Vertex, int>(pm.vertices, domains);

            //List<Vertex> checkedVertices = new List<Vertex>();
            //// Add Constraints
            //foreach (Vertex vertex in domains.Keys)
            //{
            //    foreach (Vertex neighbour in vertex.Neighbors)
            //    {
            //        if (!checkedVertices.Contains(neighbour))
            //            csp.AddConstraint(new MapColoringConstraint(vertex, neighbour));
            //    }
            //    checkedVertices.Add(vertex);
            //}
            FileStream fs = new FileStream("data.txt", FileMode.Create);
            using StreamWriter writeText = new StreamWriter(fs);
            List<int> BTNodes = new List<int>();
            List<double> BTFirsts = new List<double>();
            List<double> BTTotal = new List<double>();
            List<int> FCNodes = new List<int>();
            List<double> FCFirsts = new List<double>();
            List<double> FCTotal = new List<double>();
            int func1 = 3, func2 = 7;
            foreach (int i in new int[] { func1, func2 })
            {
                Console.WriteLine("\n\ni: " + i);

                switch (i)
                {
                    case 1:
                        writeText.WriteLine("\n\nBacktrackingSearch MVR enabled");
                        writeText.WriteLine("BacktrackingSearch LCV enabled");
                        break;
                    case 2:
                        writeText.WriteLine("\n\nBacktrackingSearch MVR enabled");
                        writeText.WriteLine("BacktrackingSearch LCV disabled");
                        break;
                    case 3:
                        writeText.WriteLine("\n\nBacktrackingSearch MVR disabled");
                        writeText.WriteLine("BacktrackingSearch LCV disabled");
                        break;
                    case 4:
                        writeText.WriteLine("\n\nBacktrackingSearch MVR disabled");
                        writeText.WriteLine("BacktrackingSearch LCV enabled");
                        break;
                    case 5:
                        writeText.WriteLine("\n\nForwardChecking MVR enabled");
                        writeText.WriteLine("ForwardChecking LCV enabled");
                        break;
                    case 6:
                        writeText.WriteLine("\n\nForwardChecking MVR enabled");
                        writeText.WriteLine("ForwardChecking LCV disabled");
                        break;
                    case 7:
                        writeText.WriteLine("\n\nForwardChecking MVR disabled");
                        writeText.WriteLine("ForwardChecking LCV disabled");
                        break;
                    case 8:
                        writeText.WriteLine("\n\nForwardChecking MVR disabled");
                        writeText.WriteLine("ForwardChecking LCV enabled");
                        break;
                    case 9:
                        writeText.WriteLine("\n\n--AC3-- ForwardChecking MVR disabled");
                        writeText.WriteLine("ForwardChecking LCV enabled");
                        break;

                    default:

                        break;
                }

                for (int n = 2; n <= 14; n++)
                {
                    Console.WriteLine("\n\nn: " + n);
                    //int h, int w, int n, int seed
                    ProblemMap pm = new ProblemMap(10, 10, n, 5);
                    //ProblemMap pm = new ProblemMap(4,4,5, 1);
                    pm.RandomizeConnections(ref pm.vertices);
                    int numberOfColors = 4;


                    Dictionary<Vertex, List<int>> domains = new Dictionary<Vertex, List<int>>();

                    foreach (Vertex vertex in pm.vertices)
                    {
                        List<int> colors = new List<int>();
                        for (int j = 1; j <= numberOfColors; j++)
                        {
                            colors.Add(j);
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

                    Tuple<List<Dictionary<Vertex, int>>, int, List<int>> solutions;

                    switch (i)
                    {
                        case 1:
                            solutions = csp.BacktrackingSearch();
                            solutions = csp.BacktrackingSearch();
                            break;
                        case 2:
                            csp.LCVEnabled = false;
                            solutions = csp.BacktrackingSearch();
                            solutions = csp.BacktrackingSearch();
                            break;
                        case 3:
                            csp.MRVEnabled = false;
                            csp.LCVEnabled = false;
                            solutions = csp.BacktrackingSearch();
                            solutions = csp.BacktrackingSearch();
                            break;
                        case 4:
                            csp.LCVEnabled = true;
                            csp.MRVEnabled = false;
                            solutions = csp.BacktrackingSearch();
                            solutions = csp.BacktrackingSearch();
                            break;
                        case 5:
                            csp.MRVEnabled = true;
                            solutions = csp.ForwardChecking();
                            solutions = csp.ForwardChecking();
                            break;
                        case 6:
                            csp.LCVEnabled = false;
                            solutions = csp.ForwardChecking();
                            solutions = csp.ForwardChecking();
                            break;
                        case 7:
                            csp.MRVEnabled = false;
                            csp.LCVEnabled = false;
                            solutions = csp.ForwardChecking();
                            solutions = csp.ForwardChecking();
                            break;
                        case 8:
                            csp.LCVEnabled = true;
                            csp.MRVEnabled = false;
                            solutions = csp.ForwardChecking();
                            solutions = csp.ForwardChecking();
                            break;
                        case 9:
                            csp.MRVEnabled = true;
                            solutions = csp.AC3();
                            solutions = csp.AC3();
                            break;

                        default:
                            solutions = csp.BacktrackingSearch();
                            break;
                    }
                    if (i == func1)
                    {
                        BTNodes.Add(solutions.Item2);
                        BTFirsts.Add(csp.FirstSolutionTime.TotalSeconds);
                        BTTotal.Add(csp.FinishedTime.TotalSeconds);
                    }
                    else if (i == func2)
                    {
                        FCNodes.Add(solutions.Item2);
                        FCFirsts.Add(csp.FirstSolutionTime.TotalSeconds);
                        FCTotal.Add(csp.FinishedTime.TotalSeconds);
                    }
                    //foreach (var vertex in pm.vertices)
                    //{
                    //    writeText.WriteLine($"\nPunkt {vertex.Point.X}:{vertex.Point.Y}");
                    //    foreach (var neighbour in vertex.Neighbors)
                    //    {
                    //        writeText.WriteLine($"{neighbour.Point.X}:{neighbour.Point.Y}");
                    //    }
                    //}
                    //writeText.WriteLine($"ConsistenceCounter\t{solutions.Item2}");
                    //writeText.WriteLine($"Total Time:\t{csp.FinishedTime.TotalSeconds}\nFirst solution time:\t{csp.FirstSolutionTime.TotalSeconds}");
                    //int index = 1;
                    if (solutions.Item1.Count != 0)
                        Console.WriteLine("Found solution");
                }

            }
            writeText.Write("\nn");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{z}");
            }
            writeText.Write("\nBT");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{BTNodes[z - 2]}");
            }
            writeText.Write("\nFC");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{FCNodes[z - 2]}");
            }

            writeText.Write("\n\nFirst Solution Time");
            writeText.Write("\nn");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{z}");
            }
            writeText.Write("\nBT");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{BTFirsts[z - 2]}");
            }
            writeText.Write("\nFC");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{FCFirsts[z - 2]}");
            }

            writeText.Write("\n\nTotal Time");
            writeText.Write("\nn");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{z}");
            }
            writeText.Write("\nBT");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{BTTotal[z - 2]}");
            }
            writeText.Write("\nFC");
            for (int z = 2; z <= BTNodes.Count + 1; z++)
            {
                writeText.Write($"\t{FCTotal[z - 2]}");
            }


        }
    }
}
