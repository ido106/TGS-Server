using AngouriMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AngouriMath.Entity;

namespace Domain
{
    public class Constact : Input
    {
        public Variable variable { get; set; }

        public Constact(Variable var)
        {
            variable = var;
        }
        bool Input.Equals(object obj)
        {
            if (variable.Equals(obj)) return true;
            return false;
        }
        int Input.GetHashCode()
        {
            return variable.GetHashCode();
        }
        string Input.ToString()
        {
            return variable.ToString();
        }


    }
}
