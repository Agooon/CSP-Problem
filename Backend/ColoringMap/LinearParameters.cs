using Backend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ColoringMap
{
    public class LinearParameters
    {
        public LinearParameters(Point start, Point end)
        {
            if (start.X != end.X)
            {
                A = (double)(start.Y - end.Y) / (double)(start.X - end.X);
                B = (double)start.Y - A * (double)start.X;
            }
            else
            {
                A = double.PositiveInfinity;
                B = double.NegativeInfinity;
            }
            // For checking if

            if (end.X >= start.X)
            {
                StartX = start.X;
                EndX = end.X;

                StartY = start.Y;
                EndY = end.Y;
            }
            else
            {
                StartX = end.X;
                EndX = start.X;

                if (StartX == EndX)
                {
                    if (end.Y >= start.Y)
                    {
                        StartY = start.Y;
                        EndY = end.Y;
                    }
                    else
                    {
                        StartY = end.Y;
                        EndY = start.Y;
                    }

                }
                else
                {
                    StartY = end.Y;
                    EndY = start.Y;
                }


            }
        }
        public double A { get; set; }
        public double B { get; set; }
        public double StartX { get; set; }
        public double EndX { get; set; }

        public double StartY { get; set; }
        public double EndY { get; set; }

        public bool Crosses(LinearParameters function)
        {
            // Same function, need to check X values
            if (A == function.A)
            {
                if (B == function.B)
                {
                    if (StartX > function.StartX)
                        return (StartX < function.EndX);

                    else if (StartX < function.StartX)
                        return (function.StartX < EndX);

                    else
                    {
                        // The same X value
                        if (A == double.PositiveInfinity)
                        {
                            if (StartY > function.StartY)
                                return !(StartY >= function.EndY);

                            else if (StartY < function.StartY)
                                return !(function.StartY >= EndY);

                            // Because minimum length cannot be 0
                            else
                                return true;
                        }
                        else 
                            return false;
                    }
                }
                // They will never cross with different B and the same A
                return false;
            }
            else
            {
                double crossX;
                if (A == double.PositiveInfinity)
                {
                    // one or the functions: x=Start.X
                    crossX = StartX;

                    if (crossX >= function.StartX && crossX <= function.EndX)
                    {
                        double yValue = crossX * function.A + function.B;

                        if (EndY >= StartY)
                            return (yValue > StartY && yValue < EndY);
                        else
                            return (yValue > EndY && yValue < StartY);

                    }
                    else
                        return false;

                }
                else if (function.A == double.PositiveInfinity)
                {
                    // one or the functions: x=Start.X
                    crossX = function.StartX;

                    if (crossX >= StartX && crossX <= EndX)
                    {
                        double yValue = crossX * A + B;
                        if (function.EndY >= function.StartY)
                            return (yValue > function.StartY && yValue < function.EndY);
                        else
                            return (yValue > function.EndY && yValue < function.StartY);
                    }
                    else
                        return false;
                }
                // If none of the funtions 
                else
                {
                    // both functions: y = Ax + B
                    crossX = Math.Round((function.B - B) / (A - function.A),Globals.RoundVal);

                    if ((crossX > function.StartX && crossX < function.EndX) && (crossX > StartX && crossX < EndX))
                        return true;
                    else
                        return false;
                }

            }

        }
    }
}
