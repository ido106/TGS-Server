
using AngouriMath;
using Domain;
using Domain.Solutions.LocalDB;
using MongoDB.Driver;
using static AngouriMath.Entity;
using static AngouriMath.MathS;

namespace DatabaseLibrary
{
    public class Database
    {
        public enum DataType
        {
            Equations,
            Inequalities,
            ParallelLines,
            Shapes
        }

        public HandleEquations HandleEquations = null;
        public Dictionary<Input, List<Node>> Inequalities = null;
        public Dictionary<Input, List<Node>> ParallelLines = null;
        private Dictionary<string, HashSet<Shape>> Shapes = null;
        private HashSet<string> UnusedPoints;

        public bool IsTrigo;

        public Database(bool IsTrigo = false)
        {
            HandleEquations = new HandleEquations(this);
            Inequalities = new Dictionary<Input, List<Node>>();
            ParallelLines = new Dictionary<Input, List<Node>>();
            Shapes = new Dictionary<string, HashSet<Shape>>(new CustomKeyEqualityComparer());
            UnusedPoints = new HashSet<string>() {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
            };
            this.IsTrigo = IsTrigo;
        }
        private class CustomKeyEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
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

            public int GetHashCode(string obj)
            {
                int sum = 0;
                foreach (char c in obj)
                {
                    sum += c;
                }
                return sum;
            }
        }
        public void Update(Input key, Node value, DataType type)
        {
            if (value != null)
            {
                value.typeData = type;
            }
            if (type == DataType.Equations)
            {
                HandleEquations.Update(key, value);

            }
            else if (type == DataType.Inequalities)
            {
                UpdateInequalities(key, value);
            }
            else if (type == DataType.ParallelLines)
            {
                if (key.GetType() != typeof(Line)) throw new Exception("update parallel lines with no line");
                UpdateParallelLines((Line)key, value);
            }


            /*else if (type == DataType.VerticalLines)
            {
                if (key.GetType() != typeof(Line)) throw new Exception("update vertical lines with no line");
                UpdateVerticalLines((Line)key, value);
            }*/

        }
        //Case: parallel lines
        private void UpdateParallelLines(Line key, Node value)
        {
            
             Line valueLine = new Line(value.Expression.ToString());
            if (!ParallelLines.ContainsKey(valueLine))
            {
                // Node node = new Node(valueLine.ToString(),key.variable,value.Reason);
                ParallelLines.Add(valueLine, new List<Node>());
            }

            if (ParallelLines.ContainsKey(key))
            {
                foreach (Node node in ParallelLines[key])
                {
                    Line current = new Line(node.Expression.ToString());
                    if (ParallelLines[current].FindAll(n => n.Expression.ToString() == valueLine.ToString()).Count == 0)
                    {
                        Node n = new Node(current.ToString(), valueLine.variable, "כלל המעבר", new List<Node>() { node, value });
                        n.typeData = DataType.ParallelLines;
                        ParallelLines[current].Add(n);

                    }
                    if (ParallelLines[valueLine].FindAll(n => n.Expression.ToString() == current.ToString()).Count == 0)
                    {
                        Node n = new Node(valueLine.ToString(), current.variable, "כלל המעבר", new List<Node>() { node, value });
                        n.typeData = DataType.ParallelLines;
                        ParallelLines[valueLine].Add(n);
                    }
                    foreach (Node subNode in ParallelLines[valueLine])
                    {
                        Line subCurrent = new Line(subNode.Expression.ToString());
                        if (!subCurrent.Equals(current) && ParallelLines[current].FindAll(n => n.Expression.ToString() == subCurrent.ToString()).Count == 0)
                        {
                            Node parent = ParallelLines[valueLine].Find(n => n.Expression.ToString() == current.ToString());
                            Node n = new Node(current.ToString(), subCurrent.variable, "כלל המעבר", new List<Node>() { subNode, parent });
                            n.typeData = DataType.ParallelLines;
                            ParallelLines[current].Add(n);
                        }
                      
                    }

                }

                ParallelLines[key].Add(value);
                return;
            }
            ParallelLines.Add(key, new List<Node>() { value });


        }

        //Case: inequalities
        private void UpdateInequalities(Input key, Node value)
        {
            if (value.Expression.ToString().Contains(">") || value.Expression.ToString().Contains("<"))
            {
                if (Inequalities.ContainsKey(key))
                {
                    Inequalities[key].Add(value);
                    return;
                }
                Inequalities.Add(key, new List<Node>() { value });

            }
        }


        //Helpers
        public Variable GetConst()
        {
            return HandleEquations.GetConst();
        }
        public Node GetBiggerNode(Input bigger, Input smaller)
        {
            Input correctBigger = FindKey(bigger);
            Input correctSmaller = FindKey(smaller);
            if (correctBigger == null || correctSmaller == null) return null;
            //If the inputs equal to num
            if (HandleEquations.Equations[correctBigger].Count > 0 && HandleEquations.Equations[correctSmaller].Count > 0)
            {
                if (HandleEquations.Equations[correctBigger][0].Expression.IsConstant && HandleEquations.Equations[correctSmaller][0].Expression.IsConstant)
                {
                    if (Double.Parse(HandleEquations.Equations[correctBigger][0].Expression.ToString()) >
                        Double.Parse(HandleEquations.Equations[correctSmaller][0].Expression.ToString()))
                    {
                        return new Node(correctBigger.ToString(),
                            correctBigger.ToString() + " > " + correctSmaller.ToString(),
                            "כלל המעבר",
                            new List<Node>() { HandleEquations.Equations[correctBigger][0], HandleEquations.Equations[correctSmaller][0] });
                    }
                    return null;

                }
            }

            //TODO: IF GIVEN AB<CD (inequalities)
            if (!Inequalities.Keys.Contains(correctBigger)) return null;
            foreach (Node nodeB in Inequalities[correctBigger])
            {
                string input1 = nodeB.Expression.Solve(correctBigger.variable).Simplify().ToString();
                string clean1 = input1.Substring(1, input1.Length - 2);
                string[] parts = clean1.Split(';');  // split on semicolon
                Entity eB1 = parts[0];
                Entity eB2 = parts[1];

                Entity expr2 = correctBigger.variable > correctSmaller.variable;
                string input2 = expr2.Solve(correctBigger.variable).Simplify().ToString();
                string clean2 = input2.Substring(1, input2.Length - 2);
                string[] parts2 = clean2.Split(';');  // split on semicolon
                Entity eS1 = parts2[0];
                Entity eS2 = parts2[1];

                Node node = new Node(correctBigger.ToString(),
                    correctBigger.ToString() + " > " + correctSmaller.ToString(),
                    "כלל המעבר",
                    new List<Node>() { nodeB });
                node.typeName = "inequalities";

                if (eB1.Simplify().Equals(eS1.Simplify()))
                {
                    if (eB2.Simplify().Equals(eS2.Simplify()))
                    {
                        return nodeB;
                    }
                    if ((eB2 - eS2).EvaluableNumerical)
                    {
                        if (Double.Parse((eB2 - eS2).Simplify().EvalNumerical().ToString()) <= 0)
                        {
                            return node;
                        }
                    }

                }
                if ((eB1 - eS1).EvaluableNumerical)
                {
                    if (Double.Parse((eB1 - eS1).Simplify().EvalNumerical().ToString()) >= 0)
                    {
                        if (eB2.Simplify().Equals(eS2.Simplify()))
                        {
                            return node;
                        }
                        if ((eB2 - eS2).EvaluableNumerical)
                        {
                            if (Double.Parse((eB2 - eS2).Simplify().EvalNumerical().ToString()) >= 0)
                            {
                                return node;
                            }
                        }
                    }
                }

            }
            return null;
        }

        public void UpdateLineBigger(Line l1, Entity expr, Line l2, string reason, List<Node> parents)
        {
            Entity e1 = $"{l1.variable} = {expr}";
            Entity e2 = e1.Solve(l2.variable).Simplify().ToString().Replace("{", "").Replace("}", "");

            Update(l1, new Node(l1.ToString(), expr, reason, parents), DataType.Equations);
            Update(l2, new Node(l2.ToString(), e2, reason, parents), DataType.Equations);

        }

        //Shapes
        public void AddShape(string name, Shape shape)
        {
            if (Shapes.ContainsKey(name))
            {
                Shapes[name].Add(shape);
            }
            else
            {
                Shapes.Add(name, new HashSet<Shape>() { shape });
            }

        }
        public HashSet<Shape> GetListShape(string name)
        {
            if (Shapes.ContainsKey(name))
                return Shapes[name];
            return new HashSet<Shape>();//or null?
        }
        public Dictionary<string, HashSet<Shape>> GetShapes()
        {
            return Shapes;
        }
        public Input FindKey(Input input)
        {
            return HandleEquations.FindKey(input);
        }
        public Node GetEqualsNode(Input input1, Input input2)
        {
            return HandleEquations.GetEqualsNode(input1, input2);

        }
        public bool UpdateInputsEqual(Input i1, Input i2, string reason, List<Node> parents1, List<Node> parents2)
        {
            return HandleEquations.UpdateInputsEqual(i1, i2, reason, parents1, parents2);

        }

        public string GetUnusedPoint()
        {
            string p = UnusedPoints.ElementAt(0);
            if (p == null) throw new Exception("too many points are used");
            UsedPoint(p);
            return p;
        }
        public void UsedPoint(string p)
        {
            if (UnusedPoints.Contains(p))
                UnusedPoints.Remove(p);
        }
        public void UsedPoints(List<string> list)
        {
            foreach (string p in list)
            {
                UsedPoint(p);
            }

        }

    }
}
