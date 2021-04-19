using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.CSP
{
    public class CSP<T, U>
    {
        // Timers for mesurment
        public Stopwatch Sw { get; set; } = new Stopwatch();
        public TimeSpan FinishedTime { get; set; }
        public TimeSpan FirstSolutionTime { get; set; }
        public bool MRVEnabled { get; set; } = true;
        public bool LCVEnabled { get; set; } = true;
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
            List<T> unassigned = Variables.Where(x => !assignment.ContainsKey(x)).ToList();
            List<T> assigned = Variables.Where(x => assignment.ContainsKey(x)).ToList();
            if (MRVEnabled)
            {
                Dictionary<T, List<U>> takenValues = new Dictionary<T, List<U>>();
                foreach (var unassignedVal in unassigned)
                {
                    takenValues.Add(unassignedVal, new List<U>());
                    foreach (var constaintsOfVar in Constraints.Where(x => x.Key.Equals(unassignedVal)))
                    {
                        foreach (var constraint in constaintsOfVar.Value)
                        {
                            foreach (var assignedVal in assigned)
                            {
                                if (constraint.Variables.Contains(assignedVal) &&
                                    !takenValues[unassignedVal].Contains(assignment[assignedVal]))
                                {
                                    takenValues[unassignedVal].Add(assignment[assignedVal]);
                                }

                            }
                        }
                    }
                }
                unassigned.Sort(delegate (T x, T y)
                {
                    return -takenValues[x].Count.CompareTo(takenValues[y].Count);
                });
            }
            return unassigned.First();
        }

        public List<U> OrderDomainValues(Dictionary<T, U> assignment, Dictionary<T, List<U>> domains, T variable)
        {
            if (LCVEnabled)
            {
                List<T> unassigned = Variables.Where(x => !assignment.ContainsKey(x)).ToList();
                List<T> assigned = Variables.Where(x => assignment.ContainsKey(x)).ToList();

                // Get value that eliminates the least amount
                Dictionary<U, int> takenCounters = new Dictionary<U, int>();

                foreach (U value in domains[variable])
                {
                    int counter = 0;
                    Dictionary<T, List<U>> takenValues = new Dictionary<T, List<U>>();
                    foreach (var unassignedVal in unassigned)
                    {
                        takenValues.Add(unassignedVal, new List<U>());
                        foreach (var constaintsOfVar in Constraints.Where(x => x.Key.Equals(unassignedVal)))
                        {
                            foreach (var constraint in constaintsOfVar.Value)
                            {
                                foreach (var assignedVal in assigned)
                                {
                                    if (constraint.Variables.Contains(assignedVal) &&
                                        !takenValues[unassignedVal].Contains(assignment[assignedVal]))
                                    {
                                        takenValues[unassignedVal].Add(assignment[assignedVal]);
                                    }

                                }
                            }

                        }
                        foreach (var constaintsOfVar in Constraints.Where(x => x.Key.Equals(unassignedVal)))
                        {
                            if (!takenValues[unassignedVal].Contains(value))
                                takenValues[unassignedVal].Add(value);
                        }
                    }

                    counter += takenValues.Sum(x => x.Value.Count);
                    // Adding the counter for this value
                    takenCounters.Add(value, counter);
                }
                domains[variable].Sort(delegate (U x, U y)
                {
                    return -takenCounters[x].CompareTo(takenCounters[y]);
                });
            }
            return domains[variable];
        }

        public Tuple<List<Dictionary<T, U>>, int, List<int>> BacktrackingSearch()
        {
            int consistenceCounter = 0;
            List<int> solutionsFoundAt = new List<int>();

            FirstSolutionTime = new TimeSpan(0);
            Sw = new Stopwatch();
            Sw.Start();

            var result = new Tuple<List<Dictionary<T, U>>, int, List<int>>(
                BacktrackingSearch(new Dictionary<T, U>(), ref consistenceCounter, solutionsFoundAt),
                consistenceCounter,
                solutionsFoundAt);
            FinishedTime = Sw.Elapsed;
            return result;
        }
        public List<Dictionary<T, U>> BacktrackingSearch(Dictionary<T, U> assignment, ref int consistenceCounter, List<int> solutionsFoundAt)
        {
            // If every variable is assigned -> end
            if (assignment.Count == Variables.Count)
            {
                if (FirstSolutionTime.Ticks == 0)
                    FirstSolutionTime = Sw.Elapsed;
                solutionsFoundAt.Add(consistenceCounter);
                return new List<Dictionary<T, U>> { assignment };
            }

            // Select variable
            T selectedVariable = SelectVariable(assignment);
            List<Dictionary<T, U>> solutions = new List<Dictionary<T, U>>();

            foreach (U value in OrderDomainValues(assignment, Domains, selectedVariable))
            {
                Dictionary<T, U> localAssignment = assignment.ToDictionary(entry => entry.Key,
                                                              entry => entry.Value);
                localAssignment[selectedVariable] = value;

                // If our values match constraints, we continue
                consistenceCounter++;
                if (Consistent(selectedVariable, localAssignment))
                {
                    var result = BacktrackingSearch(localAssignment, ref consistenceCounter, solutionsFoundAt);

                    // If we didn't find the result, we will backtrack
                    if (result.Count != 0)
                        solutions.AddRange(result);
                }
            }
            return solutions;
        }

        public Tuple<List<Dictionary<T, U>>, int, List<int>> ForwardChecking()
        {
            int consistenceCounter = 0;
            Dictionary<T, List<U>> domains = Domains.ToDictionary(entry => entry.Key, entry => entry.Value);
            List<int> solutionsFoundAt = new List<int>();

            FirstSolutionTime = new TimeSpan(0);
            Sw = new Stopwatch();
            Sw.Start();
            var result = new Tuple<List<Dictionary<T, U>>, int, List<int>>(
                ForwardChecking(new Dictionary<T, U>(), domains, ref consistenceCounter, solutionsFoundAt),
                consistenceCounter,
                solutionsFoundAt);
            FinishedTime = Sw.Elapsed;
            return result;
        }
        public List<Dictionary<T, U>> ForwardChecking(Dictionary<T, U> assignment, Dictionary<T, List<U>> domains, ref int consistenceCounter, List<int> solutionsFoundAt)
        {
            // If every variable is assigned -> end
            if (assignment.Count == Variables.Count)
            {
                if (FirstSolutionTime.Ticks == 0)
                    FirstSolutionTime = Sw.Elapsed;
                solutionsFoundAt.Add(consistenceCounter);
                return new List<Dictionary<T, U>> { assignment };
            }

            // Select variable
            T selectedVariable = SelectVariable(assignment);
            List<Dictionary<T, U>> solutions = new List<Dictionary<T, U>>();

            foreach (U value in OrderDomainValues(assignment, domains, selectedVariable))
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


                    var result = ForwardChecking(localAssignment, newDomains, ref consistenceCounter, solutionsFoundAt);

                    // If we didn't find the result, we will backtrack
                    if (result.Count != 0)
                        solutions.AddRange(result);
                }
            }
            return solutions;
        }

        public Tuple<List<Dictionary<T, U>>, int, List<int>> ForwardChecking(Dictionary<T, List<U>> domains)
        {
            int consistenceCounter = 0;
            List<int> solutionsFoundAt = new List<int>();
            
            var result = new Tuple<List<Dictionary<T, U>>, int, List<int>>(
                ForwardChecking(new Dictionary<T, U>(), domains, ref consistenceCounter, solutionsFoundAt),
                consistenceCounter,
                solutionsFoundAt);
            FinishedTime = Sw.Elapsed;
            return result;
        }

        public Tuple<List<Dictionary<T, U>>, int, List<int>> AC3()
        {
            FirstSolutionTime = new TimeSpan(0);
            Sw = new Stopwatch();
            Sw.Start();

            Dictionary<T, List<U>> domains = Domains.ToDictionary(entry => entry.Key, entry => entry.Value);
            List<Tuple<T, T>> arcs = new List<Tuple<T, T>>();

            foreach (var constraintDict in Constraints)
            {
                foreach (var constraint in constraintDict.Value)
                {
                    foreach (var variable in constraint.Variables)
                    {
                        if (!constraintDict.Key.Equals(variable))
                        {
                            arcs.Add(new Tuple<T, T>(constraintDict.Key, variable));
                        }
                    }
                }
            }

            while (arcs.Count != 0)
            {
                var first = arcs.First();
                arcs.Remove(first);

                if (RemoveIncosistentValues(first, ref domains))
                {
                    foreach (var constraint in Constraints[first.Item1])
                    {
                        // Foreach neighbour
                        foreach (var variable in constraint.Variables)
                        {
                            // Getting all variables, expect current var
                            if (!first.Item1.Equals(variable))
                            {
                                var x = new Tuple<T, T>(first.Item1, variable);
                                if (!arcs.Contains(x))
                                    arcs.Add(x);
                            }
                        }
                    }
                }

            }

            return ForwardChecking(domains);
        }

        public bool RemoveIncosistentValues(Tuple<T, T> pair, ref Dictionary<T, List<U>> domains)
        {
            bool removed = false;

            foreach (var x in domains[pair.Item1])
            {
                if (!domains[pair.Item2].Any(z => !x.Equals(z)))
                {
                    domains[pair.Item1].Remove(x);
                    removed = true;
                }
            }
            return removed;
        }

    }
}
