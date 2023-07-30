using AngouriMath;
using Antlr4.Runtime.Atn;
using DatabaseLibrary;
using Domain.Triangles;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain.Solutions
{
    public class HandleProve
    {
        private Question question;
        private Database database;
        public HandleProve(Question question, Database database)
        {
            this.question = question;
            this.database = database;
        }
        //Get the input that we want to prove 
        public List<Node> GetProvable()
        {
            List<Node> provenList = new List<Node>();
            List<Input> needToProveList = new List<Input>();
            List<PairWrapper<string, List<string>>> proveData = question.GetProveData();

            //Triangles' case:
            ProveCaseTriangle(proveData, needToProveList, provenList);


            GenerateProve(proveData, needToProveList, provenList);
            return provenList;

        }
       
        private void ProveCaseTriangle(List<PairWrapper<string, List<string>>> proveData, List<Input> needToProveList, List<Node> provenList)
        {
            foreach (var p in proveData)
            {
                switch (p.First)
                {
                    case "RightTriangle":
                        {
                            var s = database.GetListShape(p.Second[0]).FirstOrDefault(shape => shape is RightTriangle);
                            if (s != null)
                            {
                                provenList.Add(s.MainNode);
                            }
                            else
                            {
                                s = database.GetListShape(p.Second[0]).FirstOrDefault(shape => shape is Triangle);
                                if (s == null) throw new Exception("triangle do not exist");
                                RightTriangle t = RightTriangle.IsShape((Triangle)s, database);
                                if (t != null)
                                {
                                    provenList.Add(t.MainNode);
                                }
                                else
                                {
                                    var anglesKeys = s.AnglesKeys;
                                    var linesKeys = s.LinesKeys;
                                    needToProveList.AddRange(anglesKeys);
                                    needToProveList.AddRange(linesKeys);
                                    needToProveList = needToProveList.Distinct().ToList();

                                }
                            }
                        }
                        break;
                    case "IsoscelesTriangle":
                        {
                            var s = database.GetListShape(p.Second[0]).FirstOrDefault(shape => shape is IsoscelesTriangle);
                            if (s != null)
                            {
                                provenList.Add(s.MainNode);
                            }
                            else
                            {
                                s = database.GetListShape(p.Second[0]).FirstOrDefault(shape => shape is Triangle);
                                if (s == null) throw new Exception("triangle do not exist");
                                IsoscelesTriangle t = IsoscelesTriangle.IsShape((Triangle)s, database);
                                if (t != null)
                                {
                                    provenList.Add(t.MainNode);
                                }
                                else
                                {
                                    var anglesKeys = s.AnglesKeys;
                                    var linesKeys = s.LinesKeys;
                                    needToProveList.AddRange(anglesKeys);
                                    needToProveList.AddRange(linesKeys);
                                    needToProveList = needToProveList.Distinct().ToList();

                                }
                            }
                        }
                        break;
                    case "EquilateralTriangle":
                        {
                            var s = database.GetListShape(p.Second[0]).FirstOrDefault(shape => shape is IsoscelesTriangle);
                            if (s != null)
                            {
                                provenList.Add(s.MainNode);
                            }
                            else
                            {
                                s = database.GetListShape(p.Second[0]).FirstOrDefault(shape => shape is Triangle);
                                if (s == null) throw new Exception("triangle do not exist");
                                IsoscelesTriangle t = IsoscelesTriangle.IsShape((Triangle)s, database);
                                if (t != null)
                                {
                                    provenList.Add(t.MainNode);
                                }
                                else
                                {
                                    var anglesKeys = s.AnglesKeys;
                                    var linesKeys = s.LinesKeys;
                                    needToProveList.AddRange(anglesKeys);
                                    needToProveList.AddRange(linesKeys);
                                    needToProveList = needToProveList.Distinct().ToList();

                                }
                            }
                        }
                        break;
                    case "Equation_Line_Expr":
                        {
                            if (p.Second.Count != 2)
                                throw new Exception("Need 2 inputs input for Equation_Line_Expr prove");

                            var l1 = (Line)database.FindKey(new Line(p.Second[0]));
                            var s = database.GetShapes().First(pair => pair.Key.Contains(l1.ToString()[0]) &&
                            pair.Key.Contains(l1.ToString()[1]) &&
                            pair.Value.ElementAt(0).LinesKeys != null &&
                            pair.Value.ElementAt(0).AnglesKeys != null).Value.ElementAt(0);
                            var expr = p.Second[1];
                            var answerRoot = database.HandleEquations.Equations[l1].Find(node => node.Expression.Equals(expr));
                            if (answerRoot != null)
                            {
                                provenList.Add(answerRoot);
                            }
                            else
                            {
                                var anglesKeys = s.AnglesKeys;
                                var linesKeys = s.LinesKeys;
                                needToProveList.AddRange(anglesKeys);
                                needToProveList.AddRange(linesKeys);
                                needToProveList = needToProveList.Distinct().ToList();
                            }
                        }
                        break;
                    case "Equation_Angle_Expr":
                        {
                            if (p.Second.Count != 2)
                                throw new Exception("Need 2 inputs input for Equation_Angle_Expr prove");

                            var a1 = (Angle)database.FindKey(new Angle(p.Second[0]));
                            var s = database.GetShapes().First(pair => pair.Key.Contains(a1.ToString()[0]) &&
                            pair.Key.Contains(a1.ToString()[1]) && 
                            pair.Key.Contains(a1.ToString()[2]) &&
                            pair.Value.ElementAt(0).LinesKeys != null &&
                            pair.Value.ElementAt(0).AnglesKeys != null).Value.ElementAt(0);
                            var expr = p.Second[1];
                            var answerRoot = database.HandleEquations.Equations[a1].Find(node => node.Expression.Equals(expr));
                            if (answerRoot != null)
                            {
                                provenList.Add(answerRoot);
                            }
                            else
                            {
                                var anglesKeys = s.AnglesKeys;
                                var linesKeys = s.LinesKeys;
                                needToProveList.AddRange(anglesKeys);
                                needToProveList.AddRange(linesKeys);
                                needToProveList = needToProveList.Distinct().ToList();
                            }
                        }
                        break;
                    case "Area_Triangle":
                    case "Perimeter_Triangle":
                        {
                            var triangleType = Type.GetType(p.First);
                            if (triangleType != null)
                            {
                                var t = (Triangle)database.GetListShape(p.Second[0]).FirstOrDefault(shape => triangleType.IsInstanceOfType(shape));
                                if (t != null)
                                {
                                    var resultNode = (p.First == "Area_Triangle") ? t.CalculateArea() : t.CalculatePerimeter();
                                    if (resultNode != null)
                                    {
                                        provenList.Add(resultNode);
                                    }
                                    else
                                    {
                                        var anglesKeys = t.AnglesKeys;
                                        var linesKeys = t.LinesKeys;
                                        needToProveList.AddRange(anglesKeys);
                                        needToProveList.AddRange(linesKeys);
                                        needToProveList = needToProveList.Distinct().ToList();
                                    }
                                }
                            }
                        }
                        break;
                    case "TrianglesCongruent":
                        {
                            if (p.Second.Count != 2)
                                throw new Exception("Need 2 triangles input for triangles congruent prove");

                            var node = ProveTrianglesCongruent(p.Second, needToProveList);
                            if (node != null)
                            {
                                provenList.Add(node);
                            }
                        }
                        break;
                    case "TriangleSimilarity":
                        {
                            if (p.Second.Count != 2)
                                throw new Exception("Need 2 triangles input for triangles similarity prove");

                            var node = ProveTriangleSimilarity(p.Second, needToProveList);
                            if (node != null)
                            {
                                provenList.Add(node);
                            }
                        }
                        break;
                    case "ParallelLines":
                        {
                            if (p.Second.Count != 2)
                                throw new Exception("Need 2 lines input for parallel lines prove");
                            var l1 = (Line)database.FindKey(new Line(p.Second[0]));
                            var l2 = (Line)database.FindKey(new Line(p.Second[1]));
                            //if already parallel
                            if (database.ParallelLines.ContainsKey(l1))
                            {
                                Node node = database.ParallelLines[l1].FirstOrDefault((n) => n.Expression.Equals(l2.variable));
                                if (node != null)
                                {
                                    provenList.Add(node);
                                    break;
                                }
                            }
                            needToProveList.Add(l1);
                            needToProveList.Add(l2);
                        }
                        break;
                }
            }
        }
         //if the input's value not proved yet, genarate to prove
        private void GenerateProve(List<PairWrapper<string, List<string>>> proveData, List<Input> needToProveList, List<Node> provenList)
        {
            int len = needToProveList.Count;
            int inputIndex = 0;
            List<HashSet<Triangle>> listPair = proveData.FindAll(pair => pair.First.Equals("TrianglesCongruent"))
                 .ConvertAll(pair =>
                 {
                     Triangle t1 = (Triangle)database.GetShapes()[pair.Second[0]].ElementAt(0);
                     Triangle t2 = (Triangle)database.GetShapes()[pair.Second[1]].ElementAt(0);
                     return new HashSet<Triangle>() { t1, t2 };
                 });
            HashSet<Triangle> triangles = GetUniqueTriangles();
            HashSet<RightTriangle> rightTriangles = database.GetShapes().Values
           .SelectMany(shapeList => shapeList.OfType<RightTriangle>())
           .ToHashSet();
            //Run over all the unproven
            while (needToProveList.Count > 0 && inputIndex < needToProveList.Count)
            {
                Input input = needToProveList[inputIndex];
                Input correctInp = database.FindKey(input);

                // Is proved
                needToProveList.Clear();
                ProveCaseTriangle(proveData, needToProveList, provenList);

                if (needToProveList.Count == 0 || !needToProveList[inputIndex].Equals(input))
                {
                    ++inputIndex;
                    continue;
                }

                // *************************** 1 ********************************
                Variable x = database.GetConst();
                if (x == null)
                    break;

                Node parent = new Node(correctInp.ToString(), x, "הצבה");
                // From input to const
                database.Update(correctInp, parent, DataType.Equations);
                // From const to input
                ReturnFromConstToInput(correctInp, x);

                // Is proved
                needToProveList.Clear();
                ProveCaseTriangle(proveData, needToProveList, provenList);

                if (needToProveList.Count == 0 || needToProveList[inputIndex] != input)
                {
                    ++inputIndex;
                    continue;
                }

                // *************************** 2 ********************************

             

                for (int j = 0; j < triangles.Count; ++j)
                {
                    for (int k = j + 1; k < triangles.Count; ++k)
                    {
                        Triangle t1 = triangles.ElementAt(j);
                        Triangle t2 = triangles.ElementAt(k);

                        HashSet<Triangle> elem = new HashSet<Triangle>() { t1, t2 };
                        if (listPair.Contains(elem))
                            continue;

                        else
                        {
                            Node node = TrianglesCongruent.IsTrianglesCongruent(database, t1, t2);
                           // if (node == null)
                           // {
                                // TriangleSimilarity.IsTriangleSimilarity(database, t1, t2);
                           // }
                            
                        }
                    }
                }
                // Is proved
                needToProveList.Clear();
                ProveCaseTriangle(proveData, needToProveList, provenList);

                if (needToProveList.Count == 0 || needToProveList[inputIndex] != input)
                {
                    ++inputIndex;
                    continue;
                }

                // *************************** 3 ********************************
                ProcessMissingTriangleShapes(rightTriangles);

                // Is proved
                needToProveList.Clear();
                ProveCaseTriangle(proveData, needToProveList, provenList);

                if (needToProveList.Count == 0 || needToProveList[inputIndex] != input)
                {
                    ++inputIndex;
                    continue;
                }

                // *************************** 4 ********************************
                //activate special func
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
                if( isChange){
                    // Is proved
                    needToProveList.Clear();
                    ProveCaseTriangle(proveData, needToProveList, provenList);

                    if (needToProveList.Count == 0 || needToProveList[inputIndex] != input)
                    {
                        ++inputIndex;
                        continue;
                    }
                }
                ++inputIndex;

                //activate special func for ParallelLines
                if (checkForParallel())
                {    
                    // Is proved
                    needToProveList.Clear();
                    ProveCaseTriangle(proveData, needToProveList, provenList);

                    if (needToProveList.Count == 0 || needToProveList[inputIndex] != input)
                    {
                        ++inputIndex;
                        continue;
                    }

                }
            /*
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
                        {twoLineCut1.GetLine1(),twoLineCut1.GetLine2() };
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
                // Is proved
                needToProveList.Clear();
                ProveCaseTriangle(proveData, needToProveList, provenList);

                if (needToProveList.Count == 0 || needToProveList[inputIndex] != input)
                {
                    ++inputIndex;
                    continue;
                }*/
            }

            if (needToProveList.Count > 0)
                throw new Exception("cannot prove");
        }
        private void ProcessMissingTriangleShapes(HashSet<RightTriangle> rightTriangles)
        {
            List<Type> typesToCheck = new List<Type>
            {
                typeof(IsoscelesTriangle),
                typeof(EquilateralTriangle),
                //typeof(RightIsoscelesTriangle),
                typeof(RightTriangle)
            };

            var shapesCollection = database.GetShapes().Values;

            foreach (var shapeList in shapesCollection)
            {
                if (shapeList.ElementAt(0) is Triangle)
                {
                    Triangle mainTriangle = (Triangle)shapeList.ElementAt(0);
                    List<Type> missingTypes = typesToCheck.Except(shapeList.Select(s => s.GetType())).ToList();

                    foreach (var type in missingTypes)
                    {
                        
                        // Find the static method named "IsShape" with appropriate parameters
                        var isShapeMethod = type.GetMethod("IsShape", BindingFlags.Static | BindingFlags.Public);

                        // Invoke the static method if it exists
                        if (isShapeMethod != null)
                        {
                            if (type == typeof(RightTriangle))
                            {
                                RightTriangle rightTriangleResult = (RightTriangle)isShapeMethod.Invoke(null, new object[] { mainTriangle, database });
                                if (rightTriangleResult != null)
                                    rightTriangles.Add(rightTriangleResult);
                            }
                            else
                            {
                                Shape shapeResult = (Shape)isShapeMethod.Invoke(null, new object[] { mainTriangle, database });
                            }
                        }
                       
                    }
                }
            }
        }
        private HashSet<Triangle> GetUniqueTriangles()
        {
            var shapeValues = database.GetShapes().Values;
            HashSet<Triangle> uniqueTriangles = new HashSet<Triangle>(shapeValues.Count);

            foreach (var shapeList in shapeValues)
            {
                Triangle triangle = shapeList.OfType<Triangle>().FirstOrDefault();
                if (triangle != null)
                {
                    uniqueTriangles.Add(triangle);
                }
            }

            return uniqueTriangles;
        }
        private Node ProveTrianglesCongruent(List<string> inputs, List<Input> needToProveList)
        {
            Node node = null;
            var shapes = database.GetShapes();

            if (shapes.TryGetValue(inputs[0], out var shapeList1) && shapes.TryGetValue(inputs[1], out var shapeList2))
            {
                if (shapeList1.ElementAt(0) is Triangle t1 && shapeList2.ElementAt(0) is Triangle t2)
                {
                    node = TrianglesCongruent.IsTrianglesCongruent(database, t1, t2);

                    if (node == null)
                    {
                        CollectMissingKeys(t1, needToProveList);
                        CollectMissingKeys(t2, needToProveList);

                        needToProveList = needToProveList.Distinct().ToList();
                    }
                }
            }

            return node;
        }
        private Node ProveTriangleSimilarity(List<string> inputs, List<Input> needToProveList)
        {
            Node node = null;
            var shapes = database.GetShapes();

            if (shapes.TryGetValue(inputs[0], out var shapeList1) && shapes.TryGetValue(inputs[1], out var shapeList2))
            {
                if (shapeList1.ElementAt(0) is Triangle t1 && shapeList2.ElementAt(0) is Triangle t2)
                {
                    node = TriangleSimilarity.IsTriangleSimilarity(database, t1, t2);

                    if (node == null)
                    {
                        CollectMissingKeys(t1, needToProveList);
                        CollectMissingKeys(t2, needToProveList);

                        needToProveList = needToProveList.Distinct().ToList();
                    }
                }
            }

            return node;
        }
        private void CollectMissingKeys(Triangle triangle, List<Input> needToProveList)
        {
            needToProveList.AddRange(triangle.AnglesKeys);
            needToProveList.AddRange(triangle.LinesKeys);
        }
        private void ReturnFromConstToInput(Input correctInp, Variable x)
        {
            var equations = database.HandleEquations.Equations;

            foreach (var nodeList in equations.Values)
            {
                RemoveDuplicateNodes(nodeList);

                foreach (var node in nodeList)
                {
                    if (node.Expression.Vars.Contains(x))
                    {
                        node.Expression = node.Expression.Substitute(x, correctInp.variable).Simplify();
                    }
                }
            }

            if (equations.TryGetValue(correctInp, out var correctInpNodes))
            {
                foreach (var node in correctInpNodes)
                {
                    if (node.Expression.Vars.Contains(correctInp.variable))
                    {
                        Node newNode = new Node(correctInp.ToString(), node.Expression, "כלל המעבר", node.Parents);

                        newNode.Expression = (node.Expression - correctInp.variable).SolveEquation(correctInp.variable)
                            .ToString().Replace("{", "").Replace("}", "");
                        newNode.Expression = newNode.Expression.Simplify();

                        if (!newNode.Expression.ToString().Trim().Equals("0"))
                        {
                            database.Update(correctInp, newNode, DataType.Equations);

                            if (newNode.Expression.Evaled is Number)
                                break;
                        }
                    }
                }
            }
        }
        private void RemoveDuplicateNodes(List<Node> nodeList)
        {
            HashSet<Node> uniqueNodes = new HashSet<Node>(nodeList);
            nodeList.Clear();
            nodeList.AddRange(uniqueNodes);
        }
        private bool checkForParallel()
        {
            //try to check parallel by angles
            HashSet<TwoLinesCut> twoLineCutShapes = new HashSet<TwoLinesCut>(
                        database.GetShapes().Values.SelectMany(x => x)
                            .OfType<TwoLinesCut>());

            int len = twoLineCutShapes.Count;
            for (int i = 0; i < len; ++i)
            {
                var shape1 = twoLineCutShapes.ElementAt(i);
                for (int j = i+1; j < len; ++j)
                {
                    var shape2 = twoLineCutShapes.ElementAt(j);
                    Line otherLine1 = null, otherLine2 = null, trav = null; ;
                    if (shape1.GetLine1().Equals(shape2.GetLine1()))
                    {
                        trav = shape1.GetLine1();
                        otherLine1 = shape1.GetLine2();
                        otherLine2 = shape2.GetLine2();
                    }
                    if (shape1.GetLine1().Equals(shape2.GetLine2()))
                    {
                        trav = shape1.GetLine1();
                        otherLine1 = shape1.GetLine2();
                        otherLine2 = shape2.GetLine1();
                    }
                    if (shape1.GetLine2().Equals(shape2.GetLine1()))
                    {
                        trav = shape1.GetLine2();
                        otherLine1 = shape1.GetLine1();
                        otherLine2 = shape2.GetLine2();
                    }
                    if (shape1.GetLine2().Equals(shape2.GetLine2()))
                    {
                        trav = shape1.GetLine2();
                        otherLine1 = shape1.GetLine1();
                        otherLine2 = shape2.GetLine1();
                    }
                    if (otherLine1 != null && otherLine2 != null && !otherLine1.Equals(otherLine2))
                    {
                        string side1 = "l", side2 = "l";
                        Angle a1 = null, a2 = null;
                        {
                            string str1 = otherLine1.ToString()[0].ToString(), str2 = otherLine1.ToString()[1].ToString();
                            if (str1 != shape1.GetCutPoint())
                                a1 = new Angle(str1 + shape1.GetCutPoint() + shape2.GetCutPoint());
                            else
                            {
                                a1 = new Angle(str2 + shape1.GetCutPoint() + shape2.GetCutPoint());
                                side1 = "r";
                            }

                        }
                        {
                            string str1 = otherLine2.ToString()[0].ToString(), str2 = otherLine2.ToString()[1].ToString();
                            if (str1 != shape2.GetCutPoint())
                                a2 = new Angle(str1 + shape2.GetCutPoint() + shape1.GetCutPoint());
                            else
                            {
                                a2 = new Angle(str2 + shape2.GetCutPoint() + shape1.GetCutPoint());
                                side2 = "r";
                            }

                        }
                        if (a1 != null && a2 != null)
                        {
                            //check if the angles complete to 180
                            if (side2 == side1)
                            {
                                a1 = (Angle)database.FindKey(a1);
                                a2 = (Angle)database.FindKey(a2);

                                List<Node> nodes1 = database.HandleEquations.Equations[a1];
                                List<Node> nodes2 = database.HandleEquations.Equations[a2];
                                foreach (Node n1 in nodes1)
                                {
                                    foreach (Node n2 in nodes2)
                                    {
                                        Entity check = n1.Expression + n2.Expression - 180;
                                        if (check.Simplify().Equals("0"))
                                        {
                                            Node parent = new Node(a1.ToString(), 180 - a2.variable, "חישוב", new List<Node>() { n1, n2 });
                                            Node n = new Node(otherLine1.ToString(), otherLine2.variable,
                                                "נתונים שני ישרים הנחתכים על ידי ישר שלישי. אם קיים זוג אחד של זוויות חד צדדיות פנימיות שסכומן שווה ל- 180 אז הישרים מקבילים",
                                                parent);
                                            database.Update(otherLine1, n, DataType.ParallelLines);
                                            database.Update(otherLine2, new Node(otherLine2.ToString(), otherLine1.variable,
                                                "נתונים שני ישרים הנחתכים על ידי ישר שלישי. אם קיים זוג אחד של זוויות חד צדדיות פנימיות שסכומן שווה ל- 180 אז הישרים מקבילים",
                                                parent), DataType.ParallelLines);
                                            new ParallelLinesWithTransversal(database, shape1, shape2, "").AddParent(n);
                                            return true;
                                           
                                        }
                                    }
                                }
                            }
                            //check if the angles are equals
                            else
                            {
                                Node parent = database.GetEqualsNode(a1, a2);
                                if (parent != null)
                                {
                                    Node n = new Node(otherLine1.ToString(), otherLine2.variable,
                                                "נתונים שני ישרים הנחתכים על ידי ישר שלישי. אם קיים זוג אחד של זוויות מתחלפות שוות זו לזו אז הישרים מקבילים",
                                                parent);
                                    database.Update(otherLine1, n, DataType.ParallelLines);
                                    database.Update(otherLine2, new Node(otherLine2.ToString(), otherLine1.variable,
                                        "נתונים שני ישרים הנחתכים על ידי ישר שלישי. אם קיים זוג אחד של זוויות מתחלפות שוות זו לזו אז הישרים מקבילים",
                                        parent), DataType.ParallelLines);
                                    new ParallelLinesWithTransversal(database, shape1, shape2, "").AddParent(n);
                                    return true;

                                }
                            }
                        }
                    }
                }
            }
            return false;
        }


    }
}
