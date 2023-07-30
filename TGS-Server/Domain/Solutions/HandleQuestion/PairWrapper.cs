using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class PairWrapper<A, B>
    {
        public A First { get; set; }
        public B Second { get; set; }
        public PairWrapper(A first, B second)
        {
            this.First = first;
            Second = second;
        }
    }

}
