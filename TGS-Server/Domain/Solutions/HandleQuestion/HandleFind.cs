using AngouriMath;
using AngouriMath.Extensions;
using Antlr4.Runtime.Misc;
using DatabaseLibrary;
using Domain.Triangles;
using System.Reflection;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain.Solutions
{
    public class HandleFind
    {
        private Question question;
        private Database database;

        public HandleFind(Question question, Database database)
        {
            this.question = question;
            this.database = database;
        }
        //Get the input that we want to solve 
        public List<Input> GetSolvable()
        {
            List<Input> needToFindList = new List<Input>();
            List<PairWrapper<string, List<string>>> findData = question.GetFindData();

            foreach (PairWrapper<string, List<string>> find in findData)
            {
                switch (find.First)
                {
                    case "Angle":
                        needToFindList.Add(database.FindKey(new Angle(find.Second[0])));
                        break;
                    case "Line":
                        needToFindList.Add(database.FindKey(new Line(find.Second[0])));
                        break;
                }
            }

            GenerateFind(needToFindList);
            return needToFindList;
        }
        //if the input's value not found yet, genarate to find the input's value
        private void GenerateFind(List<Input> needToFindList)
        {
            HashSet<Triangle> triangles = GetUniqueTriangles();

            List<HashSet<Triangle>> trianglesList = new List<HashSet<Triangle>>();

            List<RightTriangle> rightTriangles = database.GetShapes().Values
                .SelectMany(shapeList => shapeList.OfType<RightTriangle>())
                .ToList();
            Dictionary<Input, string> inputConstDic = new Dictionary<Input, string>();

            // Step 1: Generate additional inputs
            for (int i = 0; i < needToFindList.Count; i++)
            {
                Input input = needToFindList[i];
                if (IsFound(input))
                    continue;

                List<Input> needToFindListGenerate = new List<Input>();

                // Retrieve the shape from the database based on the input
                var s = database.GetShapes().First(pair => pair.Key.Contains(input.ToString()[0]) &&
                    pair.Key.Contains(input.ToString()[1]) &&
                    (input.ToString().Count() == 3 ? pair.Key.Contains(input.ToString()[2]) : true) &&
                    pair.Value.ElementAt(0).AnglesKeys != null &&
                    pair.Value.ElementAt(0).LinesKeys != null).Value.ElementAt(0);

                if (s == null)
                    throw new Exception("Input does not exist in a shape");

                // Generate a list of additional inputs to find
                needToFindListGenerate.AddRange(s.AnglesKeys);
                needToFindListGenerate.AddRange(s.LinesKeys);

                // Assign constant values to the additional inputs
                foreach (var inp in needToFindListGenerate)
                {
                    Variable x = null;
                    if (inputConstDic.ContainsKey(inp))
                        x = inputConstDic[inp];
                    else
                    {
                        x = database.GetConst();
                        if (x == null)
                            break;
                        else
                            inputConstDic.Add(inp, x.ToString());
                    }
                    Node parent = new Node(inp.ToString(), x, "הצבה");
                    database.Update(inp, parent, DataType.Equations);
                    ReturnFromConstToInput(inp, x);
                }
                if (IsFound(input))
                    continue;

                // Step 2: Check congruence of triangles
                for (int j = 0; j < triangles.Count; ++j)
                {
                    for (int k = j + 1; k < triangles.Count; ++k)
                    {
                        Triangle t1 = triangles.ElementAt(j);
                        Triangle t2 = triangles.ElementAt(k);
                        HashSet<Triangle> elem = new HashSet<Triangle>() { t1, t2 };
                        if (!trianglesList.Contains(elem))
                        {
                            Node node = TrianglesCongruent.IsTrianglesCongruent(database, t1, t2);
                            if (node == null)
                            {
                                // TriangleSimilarity.IsTriangleSimilarity(database, t1, t2);
                            }
                            else
                            {
                                trianglesList.Add(elem);
                            }
                        }
                    }
                }

                if (IsFound(input))
                    continue;

                // Step 3: Process missing triangle shapes
                ProcessMissingTriangleShapes(rightTriangles);

                if (IsFound(input))
                    continue;

                // Step 4: Activate special functions for RightTriangle
                bool isChange = false;
                foreach (RightTriangle rightTriangle in rightTriangles)
                {
                    if (rightTriangle.IsAngle30() == false)
                    {
                        if (rightTriangle.ReverseIsAngle30())
                        {
                            isChange = true;
                        }
                    }
                    else
                    {
                        isChange = true;
                    }
                }
                if (isChange)
                {
                    IsFound(input);
                }

                // Activate special functions for ParallelLines
                HashSet<TwoLinesCut> twoLineCutShapes = new HashSet<TwoLinesCut>(
                    database.GetShapes().Values.SelectMany(x => x)
                    .OfType<TwoLinesCut>()
                    .Where(s => database.ParallelLines.ContainsKey(s.GetLine1()) ||
                                database.ParallelLines.ContainsKey(s.GetLine2()))
                );

                for (int j = 0; j < twoLineCutShapes.Count; ++j)
                {
                    TwoLinesCut twoLineCut1 = twoLineCutShapes.ElementAt(j);
                    for (int k = j + 1; k < twoLineCutShapes.Count; ++k)
                    {
                        TwoLinesCut twoLineCut2 = twoLineCutShapes.ElementAt(k);
                        HashSet<Line> lines = new HashSet<Line>()
                { twoLineCut1.GetLine1(), twoLineCut1.GetLine2() };
                        Line l1 = lines.ElementAt(0), l2 = lines.ElementAt(1), common = null;

                        if (!lines.Add(twoLineCut2.GetLine1()))
                        {
                            common = twoLineCut2.GetLine1();
                            l1 = l1.Equals(common) ? l2 : l1;
                            l2 = twoLineCut2.GetLine2();
                        }
                        if (!lines.Add(twoLineCut2.GetLine2()))
                        {
                            common = twoLineCut2.GetLine2();
                            l1 = l1.Equals(common) ? l2 : l1;
                            l2 = twoLineCut2.GetLine1();
                        }
                        if (lines.Count != 3) continue;
                        if (database.ParallelLines.ContainsKey(l1))
                        {
                            var node = database.ParallelLines[l1].Find((n) => n.Expression.Equals(database.FindKey(l2).variable));  //l1 || l2

                            if (node != null)
                            {
                                string p1 = twoLineCut1.GetCutPoint();
                                string p2 = twoLineCut2.GetCutPoint();

                                if (common.ToString().Contains(p1))
                                {
                                    common = new Line(p1 + (common.ToString()[0] == p1[0] ? common.ToString()[1].ToString() : common.ToString()[0].ToString()));
                                }
                                if (common.ToString().Contains(p2))
                                {
                                    common = new Line(p2 + (common.ToString()[0] == p2[0] ? common.ToString()[1].ToString() : common.ToString()[0].ToString()));
                                }
                                ParallelLinesWithTransversal plwt =
                                    new ParallelLinesWithTransversal(database, l1, l2, common, twoLineCut1.GetCutPoint(), twoLineCut2.GetCutPoint(), "");
                                plwt.AddParent(node);
                            }
                        }
                    }
                }
            }

            // ******************** FINAL ATTEMPT **************************
            // Check if all values in `needToFindList` have been found
            int count = 0;
            foreach (var inp in needToFindList)
            {
                if (IsFound(inp))
                    count++;
            }
            if (count == needToFindList.Count) return;

            // Assign constant values to remaining inputs
            foreach (var inp in inputConstDic.Keys)
            {
                Variable y = inputConstDic[inp];
                if (y == null)
                    break;

                Node parent = new Node(inp.ToString(), y, "הצבה");
                database.Update(inp, parent, DataType.Equations);
                ReturnFromConstToInput(inp, y);
            }

            // Check if all values in `needToFindList` have been found after assigning constants
            count = 0;
            foreach (var inp in needToFindList)
            {
                if (IsFound(inp))
                    count++;
            }
            if (count != needToFindList.Count)
                throw new Exception("Cannot find all values");

            //if (needToFindList.Count > 0) { throw new Exception("Cannot find all values"); }
        }

        private void ProcessMissingTriangleShapes(List<RightTriangle> rightTriangles)
        {
            // Define the types of triangles to check for
            List<Type> typesToCheck = new List<Type>
            {
                typeof(IsoscelesTriangle),
                typeof(EquilateralTriangle),
                //typeof(RightIsoscelesTriangle),
                typeof(RightTriangle)
            };

            // Get the collection of shapes from the database
            var shapesCollection = database.GetShapes().Values;

            // Iterate over each shape list in the collection
            foreach (var shapeList in shapesCollection)
            {
                // Check if the first element of the shape list is a triangle
                if (shapeList.ElementAt(0) is Triangle)
                {
                    // Get the main triangle from the shape list
                    Triangle mainTriangle = (Triangle)shapeList.ElementAt(0);

                    // Find the missing types by comparing the shape list with the types to check
                    List<Type> missingTypes = typesToCheck.Except(shapeList.Select(s => s.GetType())).ToList();

                    // Iterate over each missing type
                    foreach (var type in missingTypes)
                    {
                        // Find the static method named "IsShape" with appropriate parameters
                        var isShapeMethod = type.GetMethod("IsShape", BindingFlags.Static | BindingFlags.Public);

                        // Invoke the static method if it exists
                        if (isShapeMethod != null)
                        {
                            if (type == typeof(RightTriangle))
                            {
                                // Invoke the "IsShape" method for RightTriangle and add the result to the list of right triangles
                                RightTriangle rightTriangleResult = (RightTriangle)isShapeMethod.Invoke(null, new object[] { mainTriangle, database });
                                if (rightTriangleResult != null)
                                    rightTriangles.Add(rightTriangleResult);
                            }
                            else
                            {
                                // Invoke the "IsShape" method for other triangle types
                                Shape shapeResult = (Shape)isShapeMethod.Invoke(null, new object[] { mainTriangle, database });
                            }
                        }
                    }
                }
            }
        }

        private bool IsFound(Input input)
        {
            // Check if the expression of the equation has been evaluated to a Number
            return database.HandleEquations.Equations[input][0].Expression.Evaled is Number;
        }

        private void ReturnFromConstToInput(Input input, Variable x)
        {
            var equations = database.HandleEquations.Equations;

            // Iterate over each equation pair in the equations collection
            foreach (var pair in equations)
            {
                // Remove duplicate nodes from the equation list
                RemoveDuplicateNodes(pair.Value);

                // Iterate over each node in the equation list
                foreach (var node in pair.Value)
                {
                    // Check if the node's expression contains the constant variable
                    if (node.Expression.Vars.Contains(x))
                    {
                        // Substitute the constant variable with the original input variable and simplify the expression
                        node.Expression = node.Expression.Substitute(x, input.variable).Simplify();
                    }
                }
            }

            // Check if the equations collection contains the input variable
            if (equations.TryGetValue(input, out var inputNodes))
            {
                // Create a copy of the input nodes collection
                var inputNodesCopy = inputNodes.ToList();

                // Iterate over each node in the input nodes copy
                foreach (var node in inputNodesCopy)
                {
                    // Check if the node's expression contains the input variable
                    if (node.Expression.Vars.Contains(input.variable))
                    {
                        // Create a new node with the original input variable and modified expression
                        Node newNode = new Node(input.ToString(), node.Expression, "כלל המעבר", node.Parents);

                        // Solve the equation to remove the input variable and simplify the expression
                        string expressionString = (node.Expression - input.variable).SolveEquation(input.variable)
                            .ToString().Replace("{", "").Replace("}", "");
                        newNode.Expression = expressionString.Simplify();

                        // Update the equation with the new node
                        if (!newNode.Expression.ToString().Trim().Equals("0"))
                        {
                            database.Update(input, newNode, DataType.Equations);

                            // Break the loop if the new node's expression evaluates to a Number
                            if (newNode.Expression.Evaled is Number)
                                break;
                        }
                    }
                }
            }
        }

        private void RemoveDuplicateNodes(List<Node> nodeList)
        {
            // Create a HashSet to store unique nodes
            HashSet<Node> uniqueNodes = new HashSet<Node>(nodeList);

            // Clear the original nodeList
            nodeList.Clear();

            // Add the unique nodes back to the nodeList
            nodeList.AddRange(uniqueNodes);
        }

        /// <summary>
        /// Retrieves a set of unique triangles from the shapes in the database.
        /// </summary>
        /// <returns>A HashSet containing the unique triangles.</returns>
        private HashSet<Triangle> GetUniqueTriangles()
        {
            HashSet<Triangle> uniqueTriangles = new HashSet<Triangle>();

            foreach (var shapeList in database.GetShapes().Values)
            {
                foreach (var shape in shapeList)
                {
                    if (shape is Triangle triangle)
                    {
                        uniqueTriangles.Add(triangle);
                        break;
                    }
                }
            }

            return uniqueTriangles;
        }

        /// </summary>
        /// <param name="correctInp">The correct input.</param>
        /// <param name="parent">The parent node.</param>
        /// <param name="x">The variable.</param>
        private void UpdateConst(Input correctInp, Node parent, Variable x)
        {
            var values = database.HandleEquations.Equations;

            foreach (var pair in values)
            {
                foreach (var node in pair.Value)
                {
                    Entity expr = node.Expression;
                    HashSet<Variable> vars = expr.Vars.ToHashSet();

                    if (vars.Contains(correctInp.variable))
                    {
                        node.Parents.Add(parent);
                        node.steps.Add(new KeyValuePair<string, string>(node.Expression.ToString(), node.Reason.ToString()));
                        node.Expression = expr.Substitute(correctInp.variable, x);

                        if (vars.Count == 1)
                        {
                            UpdateConst(pair.Key, node, x);
                        }
                    }
                }
            }
        }

    }
}
