using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.CSP
{
    public abstract class Constraint<T, U>
    {   
        public List<T> Variables { get; set; }

        public Constraint(List<T> vars)
        {
            Variables = vars;
        }

        public abstract bool Satisfied(Dictionary<T, U> assignment);
    }
}
