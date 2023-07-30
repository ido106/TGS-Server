
using AngouriMath;
using Antlr4.Runtime.Misc;
using DatabaseLibrary;
using Domain.Triangles;
using System.Collections.Generic;
using System.Linq;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;
using static Domain.Solutions.Question;

namespace Domain.Solutions
{
    public class HandleQuestion
    {
        public Database database;
        private Question question;
        public HandleQuestion(Question q,bool isTrigo)
        {
            question = q;
            database = new Database(isTrigo);
        }
        //Helper for TwoLinesCut
        public Dictionary<string, string> GetSolution()
        {
            HandleGiven();
            List<Input> finds = new HandleFind(question, database).GetSolvable();
            List<Node> proves = new HandleProve(question, database).GetProvable();
            return new Solution(database, finds, proves).GetSolution();
        }
        private void HandleGiven()
        {
            Dictionary<Input, Variable> toConst = new Dictionary<Input, Variable>();
            List<PairWrapper<string, List<string>>> givanData = question.GetGivenData();
            string givenReason = "נתון";
            //Triangles' case:
            foreach (PairWrapper<string, List<string>> given in givanData)
            {
                switch (given.First)
                {
                    case "TwoLinesCut":
                        // Code for "TwoLinesCut" case
                        {
                            Line l1 = (Line)database.FindKey(new Line(given.Second[0]));
                            Line l2 = (Line)database.FindKey(new Line(given.Second[1]));
                            string p1 = given.Second[2];
                            TwoLinesCut tlc = new TwoLinesCut(database, l1, l2, p1, givenReason);
                            List<TwoLinesCut> shapes = new List<TwoLinesCut>();
                            shapes.AddRange( database.GetShapes().Values.SelectMany(x => x).OfType<TwoLinesCut>()
                                      .Where((s)=>! s.ToString().GroupBy(c => c).Any(g => g.Count() > 1)));
                            foreach (var shape in shapes)
                            {
                                if (shape != tlc)
                                {
                                    char c = shape.GetCutPoint()[0];
                                    List<string> str = new List<string>();
                                    str.AddRange(shape.GetLine1().PointsKeys);
                                    str.AddRange(shape.GetLine2().PointsKeys);

                                    if (c == l1.ToString()[0] && str.Contains(l1.ToString()[1].ToString()))
                                    {
                                        var line = shape.LinesKeys.Find(l => l.PointsKeys.Contains(l1.ToString()[1].ToString()));
                                        //tlc.CreateAnglesFromPoints(line, c.ToString());
                                        new TwoLinesCut(database,
                                         line,
                                         tlc.LinesKeys.Find(l => !l.PointsKeys.Contains(l1.ToString()[1].ToString())),
                                         tlc.GetCutPoint(),
                                         "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        string e = line.PointsKeys.Find(p => p != l1.ToString()[1].ToString());
                                        line = (Line)database.FindKey(
                                            new Line(e + tlc.GetCutPoint()));
                                        new TwoLinesCut(database,
                                            line,
                                            shape.LinesKeys.Find(l => !l.PointsKeys.Contains(e)),
                                            shape.GetCutPoint(),
                                            "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        continue;
                                    }
                                    if (c == l1.ToString()[1] && str.Contains(l1.ToString()[0].ToString()))
                                    {
                                        var line = shape.LinesKeys.Find(l => l.PointsKeys.Contains(l1.ToString()[0].ToString()));
                                        //tlc.CreateAnglesFromPoints(line, c.ToString());
                                        new TwoLinesCut(database,
                                         line,
                                         tlc.LinesKeys.Find(l => !l.PointsKeys.Contains(l1.ToString()[0].ToString())),
                                         tlc.GetCutPoint(),
                                         "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        string e = line.PointsKeys.Find(p => p != l1.ToString()[0].ToString());
                                        line = (Line)database.FindKey(
                                            new Line(e + tlc.GetCutPoint()));
                                        new TwoLinesCut(database,
                                            line,
                                            shape.LinesKeys.Find(l => !l.PointsKeys.Contains(e)),
                                            shape.GetCutPoint(),
                                            "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        continue;
                                    }
                                    if (c == l2.ToString()[0] && str.Contains(l2.ToString()[1].ToString()))
                                    {
                                        var line = shape.LinesKeys.Find(l => l.PointsKeys.Contains(l2.ToString()[1].ToString()));
                                        //tlc.CreateAnglesFromPoints(line, tlc.GetCutPoint());
                                        new TwoLinesCut(database,
                                          line,
                                          tlc.LinesKeys.Find(l => !l.PointsKeys.Contains(l2.ToString()[1].ToString())),
                                          tlc.GetCutPoint(),
                                          "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        string e = line.PointsKeys.Find(p => p != l2.ToString()[1].ToString());
                                        line = (Line)database.FindKey(
                                            new Line(e + tlc.GetCutPoint()));
                                        //tlc.CreateAnglesFromPoints(line, c.ToString());//tlc or shape? or create new object

                                        new TwoLinesCut(database,
                                           line,
                                           shape.LinesKeys.Find(l => !l.PointsKeys.Contains(e)),
                                           c.ToString(),
                                           "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        continue;
                                    }
                                    if (c == l2.ToString()[1] && str.Contains(l2.ToString()[0].ToString()))
                                    {
                                        var line = shape.LinesKeys.Find(l => l.PointsKeys.Contains(l2.ToString()[0].ToString()));
                                        //tlc.CreateAnglesFromPoints(line, c.ToString());
                                        new TwoLinesCut(database,
                                         line,
                                         tlc.LinesKeys.Find(l => !l.PointsKeys.Contains(l2.ToString()[0].ToString())),
                                         tlc.GetCutPoint(),
                                         "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        string e = line.PointsKeys.Find(p => p != l2.ToString()[0].ToString());
                                        line = (Line)database.FindKey(
                                            new Line(e + tlc.GetCutPoint()));
                                        new TwoLinesCut(database,
                                            line,
                                            shape.LinesKeys.Find(l => !l.PointsKeys.Contains(e)),
                                            shape.GetCutPoint(),
                                            "המשכי צלעות").AddParents(new List<Node>() { shape.MainNode, tlc.MainNode });
                                        continue;
                                    }

                                }
                            }
                            database.UsedPoints(l1.PointsKeys);
                            database.UsedPoints(l2.PointsKeys);
                            database.UsedPoint(p1);

                        }
                        break;
                    case "ParallelLinesWithTransversal":
                        //Code for "ParallelLinesWithTransversal" case
                        {
                            Line l1 = (Line)database.FindKey(new Line(given.Second[0]));
                            Line l2 = (Line)database.FindKey(new Line(given.Second[1]));
                            Line l3 = (Line)database.FindKey(new Line(given.Second[2]));
                            string p1 = given.Second[3];
                            string p2 = given.Second[4];
                            new ParallelLinesWithTransversal(database, l1, l2, l3, p1, p2, givenReason);
                            database.UsedPoints(l1.PointsKeys);
                            database.UsedPoints(l2.PointsKeys);
                            database.UsedPoints(l3.PointsKeys);
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                        }
                        break;
                    case "Triangle":
                        // Code for "Triangle" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string p3 = given.Second[2];
                            new Triangle(database, p1, p2, p3, givenReason);
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                            database.UsedPoint(p3);
                        }
                        break;
                    case "RightTriangle":
                        // Code for "RightTriangle" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string p3 = given.Second[2];
                            Angle rightAngle = (Angle)database.FindKey(new Angle(given.Second[3]));
                            new RightTriangle(database, p1, p2, p3, rightAngle, givenReason);
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                            database.UsedPoint(p3);
                        }
                        break;
                    case "IsoscelesTriangle":
                        // Code for "IsoscelesTriangle" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string p3 = given.Second[2];
                            Line baseSide = (Line)database.FindKey(new Line(given.Second[3]));
                            new IsoscelesTriangle(database, p1, p2, p3, baseSide, givenReason);
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                            database.UsedPoint(p3);
                         
                        }
                        break;
                    case "EquilateralTriangle":
                        // Code for "EquilateralTriangle" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string p3 = given.Second[2];
                            EquilateralTriangle t = new EquilateralTriangle(database, p1, p2, p3, givenReason);
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                            database.UsedPoint(p3);

                            Line l1 = t.LinesKeys[0];
                            Line l2 = t.LinesKeys[1];
                            Line l3 = t.LinesKeys[2];
                            Angle a1 = t.AnglesKeys[0];
                            Angle a2 = t.AnglesKeys[1];
                            Angle a3 = t.AnglesKeys[2];
                            if (!toConst.ContainsKey(l1))
                                toConst.Add(l1, database.GetConst());
                            if (!toConst.ContainsKey(l2))
                                toConst.Add(l2, database.GetConst());
                            if (!toConst.ContainsKey(l3))
                                toConst.Add(l3, database.GetConst());
                            if (!toConst.ContainsKey(a1))
                                toConst.Add(a1, database.GetConst());
                            if (!toConst.ContainsKey(a2))
                                toConst.Add(a2, database.GetConst());
                            if (!toConst.ContainsKey(a3))
                                toConst.Add(a3, database.GetConst());
                        }
                        break;
                    case "Height_Triangle":
                        // Code for "Height" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string name = given.Second[2] + given.Second[3] + given.Second[4];

                            HashSet<Triangle> trianglesToUpdate = new HashSet<Triangle>();

                            foreach (Shape shape in database.GetListShape(name))
                            {
                                if (shape is Triangle triangle)
                                {
                                    trianglesToUpdate.Add(triangle);
                                }
                            }

                            foreach (Triangle triangle in trianglesToUpdate)
                            {
                                triangle.UpdateHeight(p1, p2, givenReason, triangle.MainNode);
                            }
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);

                        }
                        break;
                    case "Median_Triangle":
                        // Code for "Median" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string name = given.Second[2] + given.Second[3] + given.Second[4];
                            HashSet<Triangle> trianglesToUpdate = new HashSet<Triangle>();

                            foreach (Shape shape in database.GetListShape(name))
                            {
                                if (shape is Triangle triangle)
                                {
                                    trianglesToUpdate.Add(triangle);
                                }
                            }

                            foreach (Triangle triangle in trianglesToUpdate)
                            {
                                triangle.UpdateMedian(p1, p2, givenReason, triangle.MainNode);
                            }
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                        }
                        break;
                    case "AngleBisector_Triangle":
                        // Code for "AngleBisector" case
                        {
                            string p1 = given.Second[0];
                            string p2 = given.Second[1];
                            string name = given.Second[2] + given.Second[3] + given.Second[4];
                            HashSet<Triangle> trianglesToUpdate = new HashSet<Triangle>();

                            foreach (Shape shape in database.GetListShape(name))
                            {
                                if (shape is Triangle triangle)
                                {
                                    trianglesToUpdate.Add(triangle);
                                }
                            }

                            foreach (Triangle triangle in trianglesToUpdate)
                            {
                                triangle.UpdateAngleBisector(p1, p2, givenReason, triangle.MainNode);
                            }
                            database.UsedPoint(p1);
                            database.UsedPoint(p2);
                        }
                        break;
                    case "ExternAngle_Triangle":
                        // Code for "ExternAngle" case
                        {
                            string p1 = given.Second[0];
                            Line l1 = (Line)database.FindKey(new Line(given.Second[1]));
                            string name = given.Second[2] + given.Second[3] + given.Second[4];
                            HashSet<Triangle> trianglesToUpdate = new HashSet<Triangle>();

                            foreach (Shape shape in database.GetListShape(name))
                            {
                                if (shape is Triangle triangle)
                                {
                                    trianglesToUpdate.Add(triangle);
                                }
                            }

                            foreach (Triangle triangle in trianglesToUpdate)
                            {
                                triangle.UpdateExternAngle(l1, p1);
                            }
                            database.UsedPoint(p1);
                        }
                        break;
                    case "Equation_Line_Expr":
                        // Code for "Equation_Line_Expr" case
                        {
                            Line l1 = (Line)database.FindKey(new Line(given.Second[0]));
                            Entity expr = given.Second[1];
                            string name1 = database.GetShapes().Keys.First((name) => name.Contains(l1.ToString()[0]) && name.Contains(l1.ToString()[1]));
                            Node mainNode1 = database.GetListShape(name1).ElementAt(0).MainNode;
                            List<Node> parents = new List<Node>() { mainNode1 };
                            
                            foreach (var variable in expr.Vars)
                            {
                                 expr = expr.Substitute(variable, database.FindKey(new Line(variable.ToString())).variable);

                                Line l2 = (Line)database.FindKey(new Line(variable.ToString()));
                                string name2 = database.GetShapes().Keys.First((name) => name.Contains(l2.ToString()[0]) && name.Contains(l2.ToString()[1]));
                                Node mainNode2 = database.GetListShape(name2).ElementAt(0).MainNode;
                                parents.Add(mainNode2);
                                if (!toConst.ContainsKey(l2))
                                    toConst.Add(l2, database.GetConst());

                            }
                            expr = expr - l1.variable;
                            foreach (var variable in expr.Vars)
                            {
                                Entity l2Expr = expr.SolveEquation(variable).ToString().Replace("{", "").Replace("}", "");
                                database.Update(new Line(variable.ToString()), new Node(variable.ToString(), l2Expr.Simplify(), givenReason, parents), Database.DataType.Equations);

                            }

                            //database.Update(l1, new Node(l1.ToString(), expr, givenReason, parents), Database.DataType.Equations);
                     
                            if(!toConst.ContainsKey(l1))
                            toConst.Add(l1, database.GetConst());
                          
                        }
                        break;
                   
                    case "Equation_Angle_Expr":
                        // Code for "Equation_Angle_Expr" case
                        {
                            Angle a1 = (Angle)database.FindKey(new Angle(given.Second[0]));
                            Entity expr = given.Second[1];
                            string name1 = database.GetShapes().Keys.First((name) => name.Contains(a1.ToString()[0]) &&
                            name.Contains(a1.ToString()[1]) &&
                            name.Contains(a1.ToString()[2]));
                            Node mainNode1 = database.GetListShape(name1).ElementAt(0).MainNode;
                            List<Node> parents = new List<Node>() { mainNode1 };

                            foreach (var variable in expr.Vars)
                            {
                                expr = expr.Substitute(variable, database.FindKey(new Angle(variable.ToString())).variable);

                                Angle a2 = (Angle)database.FindKey(new Angle(variable.ToString()));
                                string name2 = database.GetShapes().Keys.First((name) => name.Contains(a2.ToString()[0]) &&
                                name.Contains(a2.ToString()[1]) &&
                                name.Contains(a2.ToString()[1]));
                                Node mainNode2 = database.GetListShape(name2).ElementAt(0).MainNode;
                                parents.Add(mainNode2);
                                if (!toConst.ContainsKey(a2))
                                    toConst.Add(a2, database.GetConst());

                            }

                            expr = expr - a1.variable;
                            foreach (var variable in expr.Vars)
                            {
                                Entity a2Expr = expr.SolveEquation(variable).ToString().Replace("{", "").Replace("}", "");
                                database.Update(new Angle(variable.ToString()), new Node(variable.ToString(), a2Expr.Simplify(), givenReason, parents), Database.DataType.Equations);

                            }

                            if (!toConst.ContainsKey(a1))
                                toConst.Add(a1, database.GetConst());

                        }
                        break;
                    case "Inequalities":
                        // Code for "Inequalities" case
                        break;
                    case "ParallelLines":
                        {
                            // Code for "ParallelLines" case
                            Line l1 = (Line)database.FindKey(new Line(given.Second[0]));
                            Line l2 = (Line)database.FindKey(new Line(given.Second[1]));
                            database.Update(l1, new Node(l1.ToString(), l2.variable, givenReason), DataType.ParallelLines);
                            database.Update(l2, new Node(l2.ToString(), l1.variable, givenReason), DataType.ParallelLines);
                        }
                        break;
                    default:
                        {
                            // Code for default case (if none of the above cases match)
                            throw new Exception("Given is not in the right format");

                        }
                }
            }

            {
                List<TwoLinesCut> shapes = new List<TwoLinesCut>();
                shapes.AddRange(database.GetShapes().Values.SelectMany(x => x).OfType<TwoLinesCut>());

                foreach (var shape in shapes)
                {
                    string ab, cd;
                    char a, b, c, d, e;
                    e = shape.GetCutPoint()[0];
                    ab = shape.GetLine1().ToString();
                    cd = shape.GetLine2().ToString();

                    a = ab[0];
                    b = ab[1];
                    c = cd[0];
                    d = cd[1];

                    HashSet<Angle> angles = database.HandleEquations.Equations.Keys.OfType<Angle>().ToHashSet();
                    SameAngles(shape, angles, a, b, c, d, e);

                }

            }
            foreach (var pair in toConst)
            {
                Input correctInp = pair.Key;
                Variable x = pair.Value;
                Node parent = new Node(correctInp.ToString(), x, "הצבה");
                //  From input to const
                database.Update(correctInp, parent, DataType.Equations);
                //  From const to input
                ReturnFromConstToInput(correctInp, x);

            }

        }
        private void RemoveDuplicateNodes(List<Node> nodeList)
        {
            HashSet<Node> uniqueNodes = new HashSet<Node>(nodeList);
            nodeList.Clear();
            nodeList.AddRange(uniqueNodes);
        }
        private void ReturnFromConstToInput(Input correctInp, Variable x)
        {
            foreach (var nodeList in database.HandleEquations.Equations.Values)
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
            foreach (var node in database.HandleEquations.Equations[correctInp])
            {
                if (node.Expression.Vars.Contains(correctInp.variable))
                {
                    Node newNode = new Node(correctInp.ToString(), node.Expression, "כלל המעבר", node.Parents);


                    newNode.Expression = (node.Expression - correctInp.variable).SolveEquation(correctInp.variable)
                        .ToString().Replace("{", "").Replace("}", "");
                    newNode.Expression = newNode.Expression.Simplify();
                    if (!newNode.Expression.ToString().Trim().Equals("0"))
                    {
                        //only f angleStr number (?)
                        database.Update(correctInp, newNode, DataType.Equations);
                        if (newNode.Expression.Evaled is Number) break;

                    }
                }

            }
        }
        private void SameAngles(Shape shape, HashSet<Angle> angles, char a, char b, char c, char d, char e)
        {
            foreach (Angle angle in angles)
            {
                string angleStr = angle.ToString();
                if (b != a && b != e)
                {
                    if (angleStr[1] == b && angleStr[2] == a || angleStr[1] == b && angleStr[2] == e)
                    {
                        try
                        {
                            Angle newAngle1 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + b.ToString() + a.ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + b.ToString() + e.ToString()));
                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle1.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle1,
                               new Node(newAngle1.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }

                }
                if (c != e && c != d)
                {
                    if (angleStr[1] == c && angleStr[2] == d || angleStr[1] == c && angleStr[2] == e)
                    {
                        try
                        {
                            Angle newAngle1 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + c.ToString() + d.ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + c.ToString() + e.ToString()));
                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle1.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle1,
                               new Node(newAngle1.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }
                }
                if (d != e && d != c)
                {
                    if (angleStr[1] == d && angleStr[2] == e || angleStr[1] == d && angleStr[2] == c)
                    {
                        try
                        {
                            Angle newAngle1 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + d.ToString() + e.ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + d.ToString() + c.ToString()));
                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle1.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle1,
                               new Node(newAngle1.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }
                }
                if (a != b && a != e)
                {
                    if (angleStr[1] == a && angleStr[2] == e || angleStr[1] == a && angleStr[2] == b)
                    {
                        try
                        {
                            Angle newAngle = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + a.ToString() + b.ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(angleStr[0].ToString() + a.ToString() + e.ToString()));

                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle,
                               new Node(newAngle.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }
                }
                if (e != c && d != c)
                {
                    if (angleStr[0] == e && angleStr[1] == c || angleStr[0] == d && angleStr[1] == c)
                    {
                        try
                        {
                            Angle newAngle = (Angle)database.FindKey(new Angle(e.ToString() + c.ToString() + angleStr[2].ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(d.ToString() + c.ToString() + angleStr[2].ToString()));

                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle,
                               new Node(newAngle.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }
                }
                if (d != e && d != c)
                {
                    if (angleStr[0] == e && angleStr[1] == d || angleStr[0] == c && angleStr[1] == d)
                    {
                        try
                        {
                            Angle newAngle = (Angle)database.FindKey(new Angle(e.ToString() + d.ToString() + angleStr[2].ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(c.ToString() + d.ToString() + angleStr[2].ToString()));

                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle,
                               new Node(newAngle.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }
                }
                if (b != e && a != b)
                {
                    if (angleStr[0] == e && angleStr[1] == b || angleStr[0] == a && angleStr[1] == b)
                    {
                        try
                        {
                            Angle newAngle = (Angle)database.FindKey(new Angle(a.ToString() + b.ToString() + angleStr[2].ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(e.ToString() + b.ToString() + angleStr[2].ToString()));

                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle,
                               new Node(newAngle.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch { }

                    }
                }
                if (a != e && b != a)
                {
                    if (angleStr[0] == e && angleStr[1] == a || angleStr[0] == b && angleStr[1] == a)
                    {
                        try
                        {
                            Angle newAngle = (Angle)database.FindKey(new Angle(e.ToString() + a.ToString() + angleStr[2].ToString()));
                            Angle newAngle2 = (Angle)database.FindKey(new Angle(b.ToString() + a.ToString() + angleStr[2].ToString()));

                            database.Update(newAngle2,
                                new Node(newAngle2.ToString(),
                                newAngle.variable,
                                "אותה זווית",
                                shape.MainNode), DataType.Equations);
                            database.Update(newAngle,
                               new Node(newAngle.ToString(),
                               newAngle2.variable,
                               "אותה זווית",
                               shape.MainNode), DataType.Equations);
                            continue;
                        }
                        catch
                        {

                        }

                    }
                }

            }
        }
    }
}
