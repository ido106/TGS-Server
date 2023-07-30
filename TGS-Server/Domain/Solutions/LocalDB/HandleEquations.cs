using AngouriMath;
using DatabaseLibrary;
using Domain.Triangles;
using NCalc;
using static AngouriMath.Entity;

namespace Domain.Solutions.LocalDB
{
    public class HandleEquations
    {

        public Dictionary<Input, List<Node>> Equations = null;
        private HashSet<string> Constacts = new HashSet<string>()
                {
                     "a", "b", "c", "d", "f", "g", "h", "j", "k", "l",
                "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" 
                };
        private HashSet<string> UnusedConstacts = new HashSet<string>() 
                {
                     "a", "b", "c", "d", "f", "g", "h", "j", "k", "l",
                "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
                };

        private Database database = null;

        public HandleEquations(Database db)
        {
            database = db;
            Equations = new Dictionary<Input, List<Node>>();
        }

        //Case: equationsin 
        public bool Update(Input input, Node value)
        {
            Input key = FindKey(input);

            if (Equations.ContainsKey(key))
            {
                if (value == null) return false;
                value.name = key.ToString();
                if (Equations[key].Count == 1)
                {
                    if (Equations[key][0].Expression.Evaled is Number)
                    {
                        
                        return false;
                    }
                        
                }
            }
            else
            {
                if (value == null)
                {
                    Equations.Add(key, new List<Node>());
                    return false;
                }
                value.name = key.ToString();
                Equations.Add(key, new List<Node>());

            }
            if (!Equations[key].Contains(value))
                return ReduceVariables(key, value);
            return false;

        }
        private bool ReduceVariables(Input key, Node value)
        {
            // Handle expressions containing variable numbers
            value = HandleExprContainVarNumber(key, value);

            // Check if the equation is already a number
            if (HandleIsExprNumber(key, value))
                return true;

            // Extract variables and create the value equation
            HashSet<Variable> varsValue = value.Expression.Vars.ToHashSet();
            Entity valueEquation = $"{value.Expression} - {key.variable}";

            // Retrieve the nodes associated with the key
            var nodes = Equations[key];
            var nodesLen = nodes.Count;

            // Iterate over all the key's nodes
            for (int i = 0; i < nodesLen; ++i)
            {
                // Get the current node
                var node = nodes[i];
                Entity nodeEquation = $"{node.Expression} - {key.variable}";

                // Extract variables from the node's expression
                HashSet<Variable> varsNode = node.Expression.Vars.ToHashSet();

                // Find common variables between the value and node expressions
                HashSet<Variable> commonVars = new HashSet<Variable>(varsValue);
                commonVars.IntersectWith(varsNode);

                // Process each common variable
                foreach (Variable variable in commonVars)
                {
                    // Solve the equation for the current common variable
                    Entity variableValue = nodeEquation.SolveEquation(variable);
                    string variableValueString = variableValue.ToString();
                    int commaIndex = variableValueString.IndexOf(",");
                    string result = (commaIndex != -1) ? variableValueString.Substring(0, commaIndex).Replace("{", "").Replace("}", "")
                        : variableValueString.Replace("{", "").Replace("}", "");

                    // Skip if the result is empty
                    if (result.Trim() == "")
                        continue;

                    // Substitute the variable value in the value equation
                    variableValue = result;
                    Entity resultExpr = valueEquation.Substitute(variable, variableValue).Simplify();

                    // Extract variables from the resulting expression
                    HashSet<Variable> resultVars = resultExpr.Vars.ToHashSet();

                    // Check if the resulting expression still contains the key variable
                    if (resultVars.Contains(key.variable))
                    {
                        // Solve the resulting expression for the key variable
                        resultExpr = resultExpr.SolveEquation(key.variable).Simplify();
                        string resultString = resultExpr.ToString().Replace("{", "").Replace("}", "");

                        // Skip if the result is empty
                        if (resultString.Trim() == "")
                            continue;

                        // Check if the result contains multiple values
                        if (resultString.Contains(","))
                        {
                            // Add the value to the key's nodes if not already present and return false
                            if (!Equations[key].Contains(value))
                                Equations[key].Add(value);
                            return false;
                        }

                        // Update the result expression
                        resultExpr = resultString;

                        // Check if the result expression is not equal to zero
                        if (resultExpr != "0")
                        {
                            // Simplify the result expression
                            resultExpr = resultExpr.Simplify();

                            // Create an updated node with the reduced variable
                            Node updatedNode = new Node(key.ToString(), resultExpr, "כלל המעבר", new List<Node>() { node, value });

                            // Add the value to the key's nodes if not already present
                            if (!Equations[key].Contains(value))
                                Equations[key].Add(value);

                            // Update the key with the updated node
                            if (Update(key, updatedNode))
                                return true;
                        }
                        else
                        {
                            // Add the value to the key's nodes if not already present and return false
                            if (!Equations[key].Contains(value))
                                Equations[key].Add(value);
                            return false;
                        }
                    }
                    else
                    {
                        // Process each variable in the resulting expression
                        foreach (Variable rstVar in resultVars)
                        {
                            // Break the loop if the variable is already in the common variables
                            if (commonVars.Contains(rstVar))
                                break;

                            // Solve the resulting expression for the current variable
                            Entity rstVal = resultExpr.SolveEquation(rstVar).Simplify();
                            string rstString = rstVal.ToString();
                            rstVal = rstString.Replace("{", "").Replace("}", "");

                            // Create a node for the variable with the updated value
                            Node rstVarNode = new Node(rstVar.ToString(), rstVal, "כלל המעבר", new List<Node>() { node, value });

                            // Find the desired input associated with the updated value
                            Input desiredInput = Equations.Keys.FirstOrDefault(input => input.ToString() == rstVal);

                            // Check if the desired input is not updated yet
                            if (desiredInput == null)
                            {
                                string rstVarStr = rstVar.ToString();
                                // Case: line
                                if (rstVarStr.Count() == 2)
                                {
                                    desiredInput = new Line(rstVarStr);
                                }
                                // Case: angle
                                else // == 3
                                {
                                    desiredInput = new Angle(rstVarStr);
                                }
                            }

                            // Add the value to the key's nodes if not already present
                            if (!Equations[key].Contains(value))
                                Equations[key].Add(value);

                            // Update the desired input with the updated node
                            if (Update(desiredInput, rstVarNode))
                                return true;
                        }
                    }
                } // End of loop over variables
            } // End of loop over all the key's nodes

            // Add the value to the key's nodes if not already present
            if (!Equations[key].Contains(value))
                Equations[key].Add(value);

            return false;
        }
        private bool HandleIsExprNumber(Input key, Node value)
        {
            if (IsExprConst(value.Expression))
            {
                if (value.Expression.Evaled is Number)
                {
                    Equations[key] = new List<Node>() { value };
                    if (key is Angle && value.Expression.EvalNumerical() == 90)
                    {
                        bool hasRightTriangle = database.GetListShape(key.ToString()).OfType<RightTriangle>().Any();
                        if (!hasRightTriangle)
                        {
                            RightTriangle newTriangle = new RightTriangle(database,
                            key.ToString()[0].ToString(),
                            key.ToString()[1].ToString(),
                            key.ToString()[2].ToString(),
                            (Angle)key, "משולש עם זווית בת 90 מעלות הוא משולש ישר זווית");
                            newTriangle.AddParent(value);

                        }
                        
                        //Update heights at triangles
                        Line part1 = new Line(key.ToString()[0].ToString() + key.ToString()[1].ToString());
                        Line part2 = new Line(key.ToString()[2].ToString() + key.ToString()[1].ToString());

                        var shapes = database.GetShapes().Values
                            .SelectMany(x => x)
                            .OfType<TwoLinesCut>()
                            .Where(s => s.GetCutPoint().Equals(key.ToString()[1].ToString())).ToList();
                        foreach (var s in shapes)
                        {
                            Line line1 = s.GetLine1();
                            Line line2 = s.GetLine2();
                            string str10 = line1.ToString()[0].ToString();
                            string str11 = line1.ToString()[1].ToString();
                            string str20 = line2.ToString()[0].ToString();
                            string str21 = line2.ToString()[1].ToString();
                            string cutP = s.GetCutPoint();
                            Line l1 = null, l2 = null, l3 = null, l4 = null;
                            if (str10 != cutP)
                                l1 = new Line(str10 + cutP);
                            if (str11 != cutP)
                                l2 = new Line(str11 + cutP);
                            if (str20 != cutP)
                                l3 = new Line(str20 + cutP);
                            if (str21 != cutP)
                                l4 = new Line(str21 + cutP);

                            UpdateTriangleHeight(part1, l1, l2, l3, l4, line1, line2, value);
                            UpdateTriangleHeight(part2, l1, l2, l3, l4, line1, line2, value);


                        }
                    }
                }
                else if (!Equations[key].Contains(value))
                {
                    Equations[key].Add(value);

                }

                //Update all the nodes in Equations that contian key.variable
                for (int j = 0; j < Equations.Count; ++j)
                {
                    var pair = Equations.ElementAt(j);
                    if (pair.Key != key)
                    {
                        for (int i = 0; i < pair.Value.Count; ++i)
                        {
                            Node node = pair.Value[i];
                            if (node.Expression.Vars.Contains(key.variable))
                            {
                                Entity expr = node.Expression.Substitute(key.variable, value.Expression).Simplify();
                               
                                    Node updateNode = new Node(node.name, expr, "כלל המעבר", new List<Node>() { node, value });
                                    //there is amultipule value with the same vals:
                                    pair.Value[i] = updateNode;
                                    if (HandleIsExprNumber(pair.Key, pair.Value[i])) break;
                         
                            }
                        }
                    }
                }
                return true;
            }
            return false;

        }
        private void UpdateTriangleHeight(Line part1,Line l1,Line l2,Line l3,Line l4, Line line1, Line line2,Node value)
        {
            if (part1.Equals(l1))
            {
                string triangleName = line2.ToString() + part1.ToString()[0].ToString();
                List<Shape> list = database.GetListShape(triangleName).ToList();
                list.Reverse();
                list.ForEach((t) =>
                ((Triangle)t).UpdateHeight(l1.ToString()[0].ToString(),
                                            l1.ToString()[1].ToString(),
                                            "על פי הגדרת גובה",
                                            value));

            }
            else if (part1.Equals(l2))
            {
                string triangleName = line2.ToString() + part1.ToString()[0].ToString();
                List<Shape> list = database.GetListShape(triangleName).ToList();
                list.Reverse();
                list.ForEach((t) =>
                ((Triangle)t).UpdateHeight(l2.ToString()[0].ToString(),
                                            l2.ToString()[1].ToString(),
                                            "על פי הגדרת גובה",
                                            value));
            }
            else if (part1.Equals(l3))
            {
                string triangleName = line1.ToString() + part1.ToString()[0].ToString();
                List<Shape> list = database.GetListShape(triangleName).ToList();
                list.Reverse();
                list.ForEach((t) =>
                ((Triangle)t).UpdateHeight(l3.ToString()[0].ToString(),
                                            l3.ToString()[1].ToString(),
                                            "על פי הגדרת גובה",
                                            value));
            }
            else if (part1.Equals(l4))
            {
                string triangleName = line1.ToString() + part1.ToString()[0].ToString();
                List<Shape> list = database.GetListShape(triangleName).ToList();
                list.Reverse();
                list.ForEach((t) =>
                ((Triangle)t).UpdateHeight(l4.ToString()[0].ToString(),
                                            l4.ToString()[1].ToString(),
                                            "על פי הגדרת גובה",
                                            value));
            }
        }
        private Node HandleExprContainVarNumber(Input key, Node value)
        {
            HandleInputEquals(key, value);
            Entity expr = value.Expression;
            HashSet<Variable> vars = expr.Vars.ToHashSet();
            foreach (Variable variable in vars)
            {
                Input input = Equations.Keys.FirstOrDefault(input => input.ToString() == variable.ToString());
                if (input != null)
                {
                    if (Equations[input].Count > 0)
                    {
                        Node parent = Equations[input][0];
                        Entity num = parent.Expression;
                        if (IsExprConst(num))
                        {
                            Entity newExpr = value.Expression.Substitute(variable, num).Simplify();
                            if (!newExpr.Equals("0"))
                            {
                                Node newNode = new Node(value.name, newExpr, "הצבה", new List<Node> { value, parent });
                                value = newNode;
                            }

                        }
                    }
                }
            }
            return value;
        }
        private void HandleInputEquals(Input key, Node value)
        {
            Entity expr = value.Expression;
            HashSet<Variable> vars = expr.Vars.ToHashSet();
            vars.RemoveWhere((v) => Constacts.Contains(v.ToString()));
            if (vars.Count != 1) return;
            //equal to const
            if (vars.ElementAt(0).ToString().Length==1) return;
            string eqlInpStr = vars.ElementAt(0).ToString();

            if (vars.ElementAt(0).ToString().Trim() != expr.ToString().Trim()) return;


         
            string p1, p2, p3, reason;
            Line baseSide;

            if (key is Line)
            {
                Line lineInput = (Line)key;
                // Handle Line-specific logic using the lineInput variable
                Line eqlInpLine = new Line(eqlInpStr);
                if (!eqlInpLine.Equals(lineInput))
                {
                    HashSet<string> differentPoints = new HashSet<string>
                    {
                    lineInput.PointsKeys[0],
                    lineInput.PointsKeys[1],
                    eqlInpLine.PointsKeys[0],
                    eqlInpLine.PointsKeys[1]
                    };
                    if (differentPoints.Count == 3)
                    {
                        p1 = differentPoints.ElementAt(0);
                        p2 = differentPoints.ElementAt(1);
                        p3 = differentPoints.ElementAt(2);
                        reason = "משולש עם שוקיים שוות הוא משולש שווה שוקיים";
                        baseSide = new Line(p3 + (eqlInpLine.PointsKeys.Contains(p1) ? p2 : p1));
                        bool hasIsoscelesTriangle = database.GetListShape(p1 + p2 + p3).OfType<IsoscelesTriangle>().Any();
                        if (!hasIsoscelesTriangle)
                        {
                            //i choose to add it first
                            //Equations[key].Add(value);
                            IsoscelesTriangle newTriangle = new IsoscelesTriangle(database, p1, p2, p3, baseSide, reason);
                            newTriangle.AddParent(value);
                            //delete the node that lines equals from newTriangle or add it before
                            Equations[key].Remove(value);
                        }
                    }
                }
            }
            else if (key is Angle)
            {
                Angle angleInput = (Angle)key;
                // Handle Angle-specific logic using the angleInput variable
                Angle eqlInpAngle = new Angle(eqlInpStr);
                if (!eqlInpAngle.Equals(angleInput))
                {
                    HashSet<char> differentPoints = new HashSet<char>
                    {
                    angleInput.ToString()[0],
                    angleInput.ToString()[1],
                    angleInput.ToString()[2],
                    eqlInpAngle.ToString()[0],
                    eqlInpAngle.ToString()[1],
                    eqlInpAngle.ToString()[2]
                    };
                    if (differentPoints.Count == 3)
                    {
                        p1 = differentPoints.ElementAt(0).ToString();
                        p2 = differentPoints.ElementAt(1).ToString();
                        p3 = differentPoints.ElementAt(2).ToString();
                        reason = "אם שתי זוויות המשולש שוות זו לזו אז המשולש הוא שווה שוקיים";
                        baseSide = new Line(angleInput.ToString()[1].ToString() + eqlInpAngle.ToString()[1].ToString());
                        bool hasIsoscelesTriangle = database.GetListShape(p1 + p2 + p3).OfType<IsoscelesTriangle>().Any();
                        if (!hasIsoscelesTriangle)
                        {
                            IsoscelesTriangle newTriangle = new IsoscelesTriangle(database, p1, p2, p3, baseSide, reason);
                            newTriangle.AddParent(value);
                            //delete the node that angles equals from newTriangle or add it before
                            Equations[key].Remove(value);
                        }
                    }


                }

            }


        }
        public Variable GetConst()
        {
            if (UnusedConstacts.Count > 0)
            {
                Variable v = UnusedConstacts.ElementAt(0);
                UnusedConstacts.Remove(v.ToString());
                return v;

            }
            return null;

        }
        private bool IsExprConst(Entity expr)
        {
            if (expr.Evaled is Number)
            {
                return true;
            }
            foreach (Variable variable in expr.Vars)
            {
                if (!variable.Evaled is Number || !Constacts.Contains(variable.ToString()))
                    return false;
            }
            return true;
        }

        //public helpers
        public Input FindKey(Input input)
        {
            //DataType type
            /*if (type == DataType.Inequalities)
            {
                return Inequalities.Keys.First((i) => i.Equals(input));
            }*/
            if (Equations.ContainsKey(input))
                return Equations.Keys.First((i) => i.Equals(input));
            //if (ParallelLines.ContainsKey(input))
             //   return ParallelLines.Keys.First((i) => i.Equals(input));
            return input;
        }

        /*
         * Assume that if input1 = input2 then input2 = input1 in the database
         */
        public Node GetEqualsNode(Input input1, Input input2)
        {
            Input correctInput2 = FindKey(input2);
            Input correctInput1 = FindKey(input1);
            if (correctInput1.Equals(correctInput2))
            {
                return new Node(correctInput2.ToString(), correctInput1.variable, "");
            }
            Node result1 = Equations[correctInput1].Find((n) => n.Expression.Simplify().Equals(correctInput2.variable));
            if (result1 != null)
                return result1;
            Node result2 = Equations[correctInput2].Find((n) => n.Expression.Simplify().Equals(correctInput1.variable));
            if (result2 != null)
                return result2;

            //Input equals to num
            if (Equations[correctInput1].Count > 0 && Equations[correctInput2].Count > 0)


                //Input equals to num or same expr (like constact)
                foreach (Node node1 in Equations[correctInput1])
                {
                    foreach (Node node2 in Equations[correctInput2])
                    {
                        if (node1.Expression.Equals(node2.Expression))
                        {
                            return new Node(correctInput2.ToString(), correctInput1.variable, "כלל המעבר",
                                         new List<Node>() { Equations[correctInput1][0], Equations[correctInput2][0] });
                        }
                    }
                }
            return null;

        }
        public bool UpdateInputsEqual(Input i1, Input i2, string reason, List<Node> parents1, List<Node> parents2)
        {
            Input correct1 = FindKey(i1);
            Input correct2 = FindKey(i2);
            if (correct1 == null || correct2 == null) return false;
            Update(correct1, new Node(correct1.ToString(), correct2.variable, reason, parents1));
            Update(correct2, new Node(correct2.ToString(), correct1.variable, reason, parents2));
            return true;

        }

        //Handle node.expr is First number
        public void UpdateNodeIsNumber(Input key, Node node)
        {
            // Update the expr in all nodes in the Equations field that contain key.variable
            for (int i = 0; i < Equations.Count; ++i)
            {
                var equationPair = Equations.ElementAt(i);
                foreach (Node equationNode in equationPair.Value)
                {
                    if (equationNode.Expression.ToString().Contains(key.variable.ToString()))
                    {
                        // Substitute the variable with the number in the expression
                        Node newNodeToUpdate = new Node(equationNode.name,
                             equationNode.Expression.Substitute(key.variable, node.Expression).Simplify(),
                            "כלל המעבר",
                            new List<Node>() { equationNode, node });

                        if (newNodeToUpdate.Expression is Number)
                        {
                            Equations[equationPair.Key] = new List<Node>() { newNodeToUpdate };
                            UpdateNodeIsNumber(equationPair.Key, newNodeToUpdate);
                        }
                    }
                }
            }
        }
    }
}
