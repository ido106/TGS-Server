
using AngouriMath;
using System.ComponentModel.DataAnnotations;


namespace Domain
{
    public class Node
    {
        public List<Node> Parents { get; set; }
        [Key]
        public string name { get; set; }
        [Key]
        public string typeName { get; set; }
        public DatabaseLibrary.Database.DataType typeData { get; set; }
        public List<KeyValuePair<string, string>> steps { get; set; }
        public Entity Expression { get; set; }
        public string Reason { get; set; }



        public Node(string key, Entity expr, string reason, List<Node> nodeList)
        {
            name = key;
            Expression = expr;
            Reason = reason;
            Parents = nodeList;
            if (key.Length == 2)
            {
                typeName = "ישר";
            }
            else if (key.Length == 3)
            {
                typeName = "זווית";
            }
            steps = new List<KeyValuePair<string, string>>();
        }
        public Node(string key, Entity expr, string reason, Node node)
        {
            name = key;
            Expression = expr;
            Reason = reason;
            Parents = new List<Node>() { node };
            if (key.Length == 2)
            {
                typeName = "ישר";
            }
            else if (key.Length == 3)
            {
                typeName = "זווית";
            }
            steps = new List<KeyValuePair<string, string>>();
        }
        public Node(string key, Entity expr, string reason)
        {
            name = key;
            Expression = expr;
            Reason = reason;
            Parents = new List<Node>();
            if (key.Length == 2)
            {
                typeName = "ישר";
            }
            else if (key.Length == 3)
            {
                typeName = "זווית";
            }
            steps = new List<KeyValuePair<string, string>>();
        }

        public override string ToString()
        {
            return $"key: {name} , type: {typeName} , expr: {Expression} , reason: {Reason}";
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Node))
                return false;
            Node inp = (Node)obj;
            if (inp.Expression == null && this.Expression == null)
                return inp.name == this.name;
            if(inp.Expression == null || this.Expression == null)
                return false;

            return inp.name == this.name && (inp.Expression - this.Expression).Simplify().Equals("0");
        }
        public override int GetHashCode()
        {
            int sum = 0;
            foreach(var c in this.name)
            {
                sum += c.GetHashCode();
            }
            return sum;
        }
    }
}
