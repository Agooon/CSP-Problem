using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.CSP
{
    public class CSP<T, U>
    {
        public List<T> Variables { get; set; }
        public Dictionary<T, List<U>> Domains { get; set; }
        public Dictionary<T, List<Constraint<T, U>>> Constraints { get; set; }
        public CSP(List<T> vars, Dictionary<T, List<U>> domains)
        {
            Variables = vars;
            Domains = domains;

            Constraints = new Dictionary<T, List<Constraint<T, U>>>();

            foreach (T variable in Variables)
            {
                Constraints[variable] = new List<Constraint<T, U>>();
                if (!Domains.ContainsKey(variable))
                    throw new Exception("Variable doesn't have any domains");
            }
        }

        public void AddConstraint(Constraint<T, U> constraint)
        {
            foreach (T variable in constraint.Variables)
            {
                if (!Variables.Contains(variable))
                    throw new Exception("Variable is in constraint and not in CSP");
                else
                {
                    Constraints[variable].Add(constraint);
                }
            }
        }

        public bool Consistent(T variable, Dictionary<T, U> assignment)
        {
            foreach (var constraint in Constraints[variable])
            {
                if (!constraint.Satisfied(assignment))
                    return false;
            }
            return true;
        }
        public T SelectVariable(Dictionary<T, U> assignment)
        {
            // Get all variables from CSP, that don't have assigment
            List<T> unassigned = Variables.Where(x => !assignment.ContainsKey(x)).ToList();

            // Get every possible domain value of the first unassigned variable
            return unassigned.First();
        }

        public Tuple<List<Dictionary<T, U>>, int> BacktrackingSearch()
        {
            int consistenceCounter = 0;
            return new Tuple<List<Dictionary<T, U>>, int>(BacktrackingSearch(new Dictionary<T, U>(), ref consistenceCounter), consistenceCounter);
        }
        public List<Dictionary<T, U>> BacktrackingSearch(Dictionary<T, U> assignment, ref int consistenceCounter)
        {
            // If every variable is assigned -> end
            if (assignment.Count == Variables.Count)
            {
                return new List<Dictionary<T, U>> { assignment };
            }

            // Select variable
            T first = SelectVariable(assignment);
            List<Dictionary<T, U>> solutions = new List<Dictionary<T, U>>();

            foreach (U value in Domains[first])
            {
                Dictionary<T, U> localAssignment = assignment.ToDictionary(entry => entry.Key,
                                                              entry => entry.Value);
                localAssignment[first] = value;

                // If our values match constraints, we continue
                consistenceCounter++;
                if (Consistent(first, localAssignment))
                {
                    var result = BacktrackingSearch(localAssignment, ref consistenceCounter);

                    // If we didn't find the result, we will backtrack
                    if (result.Count != 0)
                        solutions.AddRange(result);
                }
            }
            return solutions;
        }

        public Tuple<List<Dictionary<T, U>>, int> ForwardChecking()
        {
            int consistenceCounter = 0;
            Dictionary<T, List<U>> domains = Domains.ToDictionary(entry => entry.Key, entry => entry.Value);
            return new Tuple<List<Dictionary<T, U>>, int>(ForwardChecking(new Dictionary<T, U>(), domains, ref consistenceCounter), consistenceCounter);
        }
        public List<Dictionary<T, U>> ForwardChecking(Dictionary<T, U> assignment, Dictionary<T, List<U>> domains, ref int consistenceCounter)
        {
            // If every variable is assigned -> end
            if (assignment.Count == Variables.Count)
            {
                return new List<Dictionary<T, U>> { assignment };
            }

            // Select variable
            T selectedVariable = SelectVariable(assignment);
            List<Dictionary<T, U>> solutions = new List<Dictionary<T, U>>();

            foreach (U value in domains[selectedVariable])
            {
                Dictionary<T, U> localAssignment = assignment.ToDictionary(entry => entry.Key,
                                                              entry => entry.Value);
                localAssignment[selectedVariable] = value;

                // If our values match constraints, we continue
                consistenceCounter++;
                if (Consistent(selectedVariable, localAssignment))
                {
                    var newDomains = domains.ToDictionary(entry => entry.Key, entry => new List<U>(entry.Value));
                    foreach (var keyConstraint in Constraints.Where(x => x.Key.Equals(selectedVariable)
                                                            && x.Value.Any(x => x.Variables.Contains(selectedVariable))))
                    {
                        foreach (var constraint in keyConstraint.Value)
                        {
                            foreach (var variable in constraint.Variables)
                            {
                                if (!variable.Equals(selectedVariable))
                                    newDomains[variable].Remove(value);
                            }
                        }
                    }
                    var result = ForwardChecking(localAssignment, newDomains, ref consistenceCounter);

                    // If we didn't find the result, we will backtrack
                    if (result.Count != 0)
                        solutions.AddRange(result);
                }
            }
            return solutions;
        }

    }
}
