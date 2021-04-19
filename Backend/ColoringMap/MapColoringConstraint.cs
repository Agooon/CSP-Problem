using Backend.CSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ColoringMap
{
    public class MapColoringConstraint : Constraint<Vertex, int>
    {
        public Vertex Vertex1 { get; set; }
        public Vertex Vertex2 { get; set; }
        public MapColoringConstraint(Vertex vertex1, Vertex vertex2) : base(new List<Vertex> { vertex1, vertex2 })
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }
        public override bool Satisfied(Dictionary<Vertex, int> assignment)
        {
            if (!assignment.ContainsKey(Vertex1) || !assignment.ContainsKey(Vertex2))
                return true;
            return assignment[Vertex1] != assignment[Vertex2];
        }
    }
}
