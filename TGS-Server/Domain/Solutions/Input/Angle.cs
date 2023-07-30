using AngouriMath;
using DatabaseLibrary;
using Domain.Solutions.LocalDB;
using static AngouriMath.Entity;

namespace Domain
{
    public class Angle : Input
    {

        public Variable variable { get; set; }
        public Angle(Variable angle)
        {
            if (angle == null) throw new ArgumentNullException("angle is null");
            if (angle.ToString().Length != 3 ) throw new ArgumentException("angle has invalid lenght");
            if (angle.ToString()[0] == angle.ToString()[1] ||
                angle.ToString()[0] == angle.ToString()[2] ||
                angle.ToString()[1] == angle.ToString()[2]) throw new ArgumentException("angle has invalid points");
            this.variable = angle;
        }
        public override string ToString()
        {
            return variable.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Angle))
                return false;
            Angle inp = (Angle)obj;

            if (variable == inp.variable)
                return true;

            if (variable.ToString()[0] == inp.variable.ToString()[2] &&
                variable.ToString()[1] == inp.variable.ToString()[1] &&
                variable.ToString()[2] == inp.variable.ToString()[0])
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

        public static bool IsEqualTo(Angle a1, Angle a2, Database db)
        {
            Angle current = (Angle)db.FindKey(a1);
            if (!db.HandleEquations.Equations.ContainsKey(current))
            {
                return false;
            }

            // Run over all the current angles expressions
            foreach (Node node in db.HandleEquations.Equations[current])
            {
                Entity expr1 = node.Expression.Simplify();
                Angle next = (Angle)db.FindKey(a2);
                if (!db.HandleEquations.Equations.ContainsKey(next))
                {
                    return false;
                }

                // Check if current angle equal to next angle
                foreach (Node node2 in db.HandleEquations.Equations[next])
                {
                    Entity expr2 = node2.Expression.Simplify();
                    Entity nextName = next.variable;
                    //If the angles are equals
                    if (expr1.Equals(expr2) || expr1.Equals(nextName.Simplify()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
