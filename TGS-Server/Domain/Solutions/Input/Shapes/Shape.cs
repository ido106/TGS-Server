using DatabaseLibrary;
using Domain.Triangles;
using static AngouriMath.Entity;

namespace Domain
{
    public class Shape : Input
    {
        public List<string> PointsKeys { get; set; }
        public List<Line> LinesKeys { get; set; }
        public List<Angle> AnglesKeys { get; set; }
        public Variable variable { get; set; }
        public Node MainNode { get; set; } = null;
        protected Database _db { get; set; }
        protected Node Area = null;
        protected Node Perimeter = null;
        public override string ToString()
        {
            return variable.ToString();
        }
        public Node GetMainNode()
        {
            return MainNode;
        }
        public void AddParent(Node node)
        {
            MainNode.Parents.Add(node);
        }
        public void AddParents(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {

                MainNode.Parents.Add(node);
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is Shape)) return false;

            Shape s = (Shape)obj;
            string y = s.ToString();
            string x = this.ToString();
            int indexY = y.IndexOf(x[0]);

            if (indexY < 0) return false;
            int lenY = y.Length;
            bool isEql = true;
            foreach (var cx in x)
            {
                if (cx != y[(indexY) % lenY])
                {
                    isEql = false;
                    break;
                }
                ++indexY;
            }
            if (isEql)
                return true;
            //Check for rev
            char[] charArray = y.ToCharArray();
            Array.Reverse(charArray);
            y = new string(charArray);

            indexY = y.IndexOf(x[0]);
            if (indexY < 0) return false;
            lenY = y.Length;
            isEql = true;
            foreach (var cx in x)
            {
                if (cx != y[(indexY) % lenY])
                {
                    isEql = false;
                    break;
                }
                ++indexY;
            }
            return isEql;


        }
        public override int GetHashCode()
        {
            int sum = 0;
            foreach (char c in this.ToString())
            {
                sum += c;
            }
            return sum;
        }

    
    }

}
