using DatabaseLibrary;
using Domain.Triangles;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain.Solutions
{
    public class Solution
    {
        private Database _db { get; set; }
        private List<Input> _find { get; set; } = null;

        private List<Node> _prove { get; set; } = null;

      
        
        public Solution(Database database, List<Input> find, List<Node> prove)
        {
            _db = database; 
            _find = find != null ? find : new List<Input>();
            _prove = prove != null ? prove : new List<Node>();
            if ((find == null && prove == null) || _db == null)
            {
                throw new Exception("invalid solution's argument");
            }

        }
        private void ReverseBFS(Node root, Dictionary<string, string> answer)
        {
            if (root == null) return;

            Queue<Node> queue = new Queue<Node>();
            Stack<Node> stack = new Stack<Node>();

            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();
                stack.Push(current);

                foreach (Node child in current.Parents)
                {
                    queue.Enqueue(child);
                }
            }

            while (stack.Count > 0)
            {
                Node current = stack.Pop();

                if (current.typeName == "inequalities")
                {
                    answer.TryAdd(current.Expression.ToString(), current.Reason);
                }

                else if (current.Expression != null)
                {
                    var currentExpr = current.Expression.ToString();
                    if (_db.IsTrigo && current.Expression.Evaled is Number)
                        
                    {
                        current.Expression = current.Expression.ToString()
                        .Replace("arccos", $"(180 / {Math.PI}) * arcc")
                        .Replace("arcsin", $"(180 / {Math.PI}) * arcs")
                        .Replace("sin(", $"sin(({Math.PI}/180)*")
                        .Replace("cos(", $"cos(({Math.PI}/180)*")
                        .Replace("arcc", "arccos")
                        .Replace("arcs", "arcsin");
                        var eval = current.Expression.Evaled as Number;
                        var dec = ((decimal)eval);
                        currentExpr = dec.ToString("0.000");

                    }
                    foreach (KeyValuePair<string, string> step in current.steps)
                    {
                        answer.TryAdd(current.name + $" {GetStrByType(current.typeData)} " + step.Key, step.Value);
                    }
                    answer.TryAdd(current.name + $" {GetStrByType(current.typeData)} " + currentExpr, current.Reason);
                }
                else if (current.typeName != null)
                {

                    answer.TryAdd(current.typeName + " " + current.name, current.Reason);
                }
                else//type == null
                {
                    answer.TryAdd(current.name, current.Reason);
                }
            }
        }
        private string GetStrByType(DataType dt)
        {
            switch(dt)
            {
                case DataType.Equations:
                    return "=";
                case DataType.ParallelLines:
                    return "||";
                    //case inequalities
                    //case TrianglesCongruent
            }
            return null;
        }
      
        public Dictionary<string, string> GetSolution()
        {
            Dictionary<string, string> answer = new Dictionary<string, string>();
            foreach (Input inp in _find)
            {
                Node root = _db.HandleEquations.Equations[inp][0];
                ReverseBFS(root, answer);

            }
            foreach (Node root in _prove)
            {
                ReverseBFS(root, answer);
            }


            return answer;
        }
    }
}
