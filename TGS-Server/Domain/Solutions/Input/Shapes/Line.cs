
using AngouriMath;
using DatabaseLibrary;
using Domain.Solutions.LocalDB;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain
{
    public class Line : Shape
    {

        public Line(string line) : base()
        {
            if (line == null) throw new ArgumentNullException("line is null");
            if (line.Length != 2) throw new ArgumentException("line has invalid lenght");
            if (line[0] == line[1]) throw new ArgumentException("line has invalid points");
            this.variable = line;
            PointsKeys = new List<string>() { line[0].ToString(), line[1].ToString() };
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Line))
                return false;
            Line inp = (Line)obj;

            if (variable == inp.variable)
                return true;

            if (variable.ToString()[0] == inp.variable.ToString()[1] &&
                variable.ToString()[1] == inp.variable.ToString()[0])
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            int sum = 0;
            foreach (char c in variable.ToString())
            {
                sum += (int)c;
            }
            return sum;
        }
        public override string ToString()
        {
            return this.variable.ToString();
        }
        public void UpdateContinueDot(Database db, string dot, List<Node> parents, Triangle t)
        {
            Line allLine = new Line(PointsKeys[0] + dot);
            Line linePart1 = this;
            Line linePart2 = new Line(PointsKeys[1] + dot);
            db.Update(linePart1, new Node(linePart1.ToString(), allLine.variable - linePart2.variable, "חיסור קטעים", parents), DataType.Equations);
            db.Update(linePart2, new Node(linePart2.ToString(), allLine.variable - linePart1.variable, "חיסור קטעים", parents), DataType.Equations);
            db.Update(allLine, new Node(allLine.ToString(), linePart1.variable + linePart2.variable, "חיבור קטעים", parents), DataType.Equations);

            //check which shapes have this line and had updates.
            //for now i assume it triangle
            t.UpdateExternAngle(this, dot);
        }

        public static bool IsEqualTo(Line l1, Line l2, Database db)
        {
            Line current = (Line)db.FindKey(l1);
            if (!db.HandleEquations.Equations.ContainsKey(current))
            {
                return false;
            }

            // Run over all the current lines expressions
            foreach (Node node in db.HandleEquations.Equations[current])
            {
                Entity expr1 = node.Expression.Simplify();
                Line next = (Line)db.FindKey(l2);
                if (!db.HandleEquations.Equations.ContainsKey(next))
                {
                    return false;
                }

                // Check if current line equals to the next line
                foreach (Node node2 in db.HandleEquations.Equations[next])
                {

                    Entity expr2 = node2.Expression.Simplify();
                    Entity nextName = next.variable;
                    //If the lines are equal
                    if (expr1.Equals(expr2) || expr1.Equals(nextName.Simplify()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static bool IsParallelTo(Line l1, Line l2, Database db)
        {
            Line current = (Line)db.FindKey(l1);
            if (!db.ParallelLines.ContainsKey(current)) return false;

            // Run over all the current lines expressions
            foreach (Node node in db.ParallelLines[current])
            {
                Entity exp1 = node.Expression.Simplify();
                Line next = (Line)db.FindKey(l2);
                if (!db.ParallelLines.ContainsKey(next)) return false;

                // Check if current line is parallel to the next line
                foreach (Node node2 in db.ParallelLines[next])
                {
                    Entity exp2 = node2.Expression.Simplify();
                    Entity nextName = next.variable;
                    // If the lines are parallel
                    if (exp1.Equals(exp2) || exp1.Equals(nextName.Simplify()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
