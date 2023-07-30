using AngouriMath;
using AngouriMath.Extensions;
using DatabaseLibrary;
using Domain.Lines;
using System.Data;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain.Triangles
{
    public class RightTriangle : Triangle
    {
        //Variables:
        protected Line heightLine { get; set; } = null;
        protected Line baseLine { get; set; } = null;
        protected Line hypotenuseLine { get; set; } = null;
        protected Angle rightAngle { get; set; } = null;

        //Constractor
        public RightTriangle(Database db, string p1, string p2, string p3, Angle rightAngle, string reason) : base(db, p1, p2, p3, reason, true)
        {
            //Main Node
            MainNode.typeName = "משולש ישר זווית";

            if (!AnglesKeys.Contains(rightAngle)) throw new Exception("angle invalid in right trianle");

            //Init spacial lines
            if (!LinesKeys[0].variable.ToString().Contains(rightAngle.variable.ToString()[1]))
            {
                heightLine = (Line)db.FindKey(this.LinesKeys[1]);
                baseLine = (Line)db.FindKey(this.LinesKeys[2]);
                hypotenuseLine = (Line)db.FindKey(this.LinesKeys[0]);
            }
            else if (!LinesKeys[1].variable.ToString().Contains(rightAngle.variable.ToString()[1]))
            {
                heightLine = (Line)db.FindKey(this.LinesKeys[2]);
                baseLine = (Line)db.FindKey(this.LinesKeys[0]);
                hypotenuseLine = (Line)db.FindKey(this.LinesKeys[1]);

            }
            else if (!LinesKeys[2].variable.ToString().Contains(rightAngle.variable.ToString()[1]))
            {
                heightLine = (Line)db.FindKey(this.LinesKeys[0]);
                baseLine = (Line)db.FindKey(this.LinesKeys[1]);
                hypotenuseLine = (Line)db.FindKey(this.LinesKeys[2]);

            }
            else
            {
                throw new Exception("invalid right triangle");
            }
            if (heightLine == null || baseLine == null || hypotenuseLine == null)
                throw new NoNullAllowedException();
            string point1 = rightAngle.ToString()[0].ToString();
            string point2 = rightAngle.ToString()[2].ToString();
            Heights[point1] = new Height((heightLine.PointsKeys.Contains(point1) ? heightLine.ToString() : baseLine.ToString()), reason, MainNode);
            Heights[point2] = new Height((heightLine.PointsKeys.Contains(point2) ? heightLine.ToString() : baseLine.ToString()), reason, MainNode);

            //Init spacial angle - right angle
            this.rightAngle = rightAngle;

            //Update lines
            _db.Update(heightLine, new Node(heightLine.ToString(),
                $"sqrt( {hypotenuseLine.variable} ^ 2 - {baseLine.variable} ^ 2 )",
               "משפט פיתגורס", MainNode), DataType.Equations);

            _db.Update(baseLine, new Node(baseLine.ToString(),
             $"sqrt( {hypotenuseLine.variable}^ 2 - {heightLine.variable} ^ 2 )",
            "משפט פיתגורס", MainNode), DataType.Equations);

            _db.Update(hypotenuseLine, new Node(hypotenuseLine.ToString(),
                $"sqrt( {baseLine.variable} ^ 2 + {heightLine.variable} ^ 2 )",
            "משפט פיתגורס", MainNode), DataType.Equations);

            if (_db.IsTrigo)
            {
                foreach (Angle angle in AnglesKeys)
                {
                    if (!angle.Equals(rightAngle))
                    {
                        Line near;
                        if (heightLine.ToString().Contains(angle.ToString()[1]))
                        {
                            near = heightLine;
                        }
                        else
                        {
                            near = baseLine;
                        }
                        //* (Math.PI / 180)
                        _db.Update(near, new Node(near.ToString(),
                            MathS.Cos(angle.variable) * hypotenuseLine.variable,
                            "לפי הגדרת cos", MainNode), DataType.Equations);

                        _db.Update(hypotenuseLine, new Node(hypotenuseLine.ToString(),
                            MathS.Cos(angle.variable) / near.variable,
                           "לפי הגדרת cos", MainNode), DataType.Equations);

                        _db.Update(angle, new Node(angle.ToString(),
                          MathS.Arccos(near.variable / hypotenuseLine.variable),
                           "לפי הגדרת cos", MainNode), DataType.Equations);


                        /*
                        _db.Update(front, new Node(front.ToString(),
                           MathS.Sin(angle.variable) * hypotenuseLine.variable,
                           "לפי הגדרת sin", MainNode), DataType.Equations);
                        _db.Update(hypotenuseLine, new Node(hypotenuseLine.ToString(),
                           MathS.Sin(angle.variable) / front.variable,
                           "לפי הגדרת sin", MainNode), DataType.Equations);

                        _db.Update(front, new Node(front.ToString(),
                        MathS.Tan(angle.variable) * near.variable,
                          "לפי הגדרת tan", MainNode), DataType.Equations);
                        _db.Update(near, new Node(near.ToString(),
                            MathS.Tan(angle.variable) / front.variable,
                           "לפי הגדרת tan", MainNode), DataType.Equations);


                        _db.Update(angle, new Node(angle.ToString(),
                           front.variable / hypotenuseLine.variable,
                           "לפי הגדרת sin", MainNode), DataType.Equations);
                        _db.Update(angle, new Node(angle.ToString(),
                           front.variable / near.variable,
                           "לפי הגדרת tan", MainNode), DataType.Equations);
                        */
                    }

                }
            }
            //Add this shape to database
            db.AddShape(p1 + p2 + p3, this);

            //Update right angle
            db.Update(rightAngle, new Node(rightAngle.ToString(), "90", reason, MainNode), DataType.Equations);
        }
        //Public:
        public override void UpdateMedian(string p1, string p2, string reason, Node mainParent)
        {
            base.UpdateMedian(p1, p2, reason, mainParent);

            //Init right angle as one letter for ex: <ABC is <B
            string b = rightAngle.ToString()[1].ToString();
            //s:39 The median drawn to hypotenuse equals to half of hypotenuse 
            if (p1 == b || p2 == b)
            {
                string a = rightAngle.ToString()[0].ToString();
                string c = rightAngle.ToString()[2].ToString();
                string d = b == p1 ? p2 : p1;
                Line ad = (Line)_db.FindKey(new Line(a + d));
                Line db = (Line)_db.FindKey(new Line(d + b));
                Line cd = (Line)_db.FindKey(new Line(d + c));
                //s:39
                _db.Update(ad, new Node(ad.ToString(), db.variable, "התיכון ליתר במשולש ישר זווית שווה למחצית היתר", Medians[b]), DataType.Equations);
                _db.Update(db, new Node(db.ToString(), ad.variable, "התיכון ליתר במשולש ישר זווית שווה למחצית היתר", Medians[b]), DataType.Equations);
                _db.Update(ad, new Node(ad.ToString(), cd.variable, "התיכון ליתר במשולש ישר זווית שווה למחצית היתר", Medians[b]), DataType.Equations);
                _db.Update(cd, new Node(cd.ToString(), ad.variable, "התיכון ליתר במשולש ישר זווית שווה למחצית היתר", Medians[b]), DataType.Equations);
            }
        }
        public override void UpdateHeight(string p1, string p2, string reason, Node mainParent)
        {
            if (Heights.ContainsKey(p1))
            {
                if (Heights[p1] != null) return;
            }
            else if (Heights.ContainsKey(p2))
            {
                if (Heights[p2] != null) return;
            }

            /*if (rightAngle.ToString()[0].ToString() == p1 ||
                rightAngle.ToString()[2].ToString() == p1 ||
                rightAngle.ToString()[0].ToString() == p2 ||
                rightAngle.ToString()[2].ToString() == p2)
                throw new Exception("already an height in this right triangle");*/
            base.UpdateHeight(p1, p2, reason, mainParent);

            string a = PointsKeys.Find((p) => p != p1 && p != p2);
            string b = rightAngle.ToString()[1].ToString();
            string c = PointsKeys.Find((p) => p != p1 && p != p2 && p != a);
            string d = p1 == b ? p2 : p1;
            if (a == null || c == null) throw new Exception("error in right triangle - height update");
            Angle a1 = (Angle)_db.FindKey(new Angle(b + a + d));
            Angle b1 = (Angle)_db.FindKey(new Angle(a + b + d));
            Angle b2 = (Angle)_db.FindKey(new Angle(d + b + c));
            Angle c2 = (Angle)_db.FindKey(new Angle(d + c + b));

            //s:41 The height drawn to hypotenuse creates 2 similar triangles
            _db.Update(a1, new Node(a1.ToString(), b2.variable,
                "במשולש ישר זווית הגובה ליתר מחלק את המשולש לשני משולשים שזוויותיהם שוות בהתאמה לזוויות המשולש המקורי", Heights[b]), DataType.Equations);
            _db.Update(b2, new Node(b2.ToString(), a1.variable,
               "במשולש ישר זווית הגובה ליתר מחלק את המשולש לשני משולשים שזוויותיהם שוות בהתאמה לזוויות המשולש המקורי", Heights[b]), DataType.Equations);

            _db.Update(c2, new Node(c2.ToString(), b1.variable,
           "במשולש ישר זווית הגובה ליתר מחלק את המשולש לשני משולשים שזוויותיהם שוות בהתאמה לזוויות המשולש המקורי", Heights[b]), DataType.Equations);
            _db.Update(b1, new Node(b1.ToString(), c2.variable,
               "במשולש ישר זווית הגובה ליתר מחלק את המשולש לשני משולשים שזוויותיהם שוות בהתאמה לזוויות המשולש המקורי", Heights[b]), DataType.Equations);
        }

        public static RightTriangle IsShape(Triangle triangle, Database db)
        {

            RightTriangle rt = db.GetListShape(triangle.ToString()).FirstOrDefault((t) => t is RightTriangle) as RightTriangle;
            if (rt != null)
            {
                triangle.CopyToChild(rt);
                return rt;

            }
            foreach (char point in triangle.ToString())
            {
                rt = CheckByMedian(db, triangle, point.ToString());
                if (rt != null)
                {
                    triangle.CopyToChild(rt);
                    return rt;

                }
                   
            }
            rt = CheckByRevPythagoras(db, triangle);
            if (rt != null)
            {
                triangle.CopyToChild(rt);
                return rt;

            }

            return null;

        }

        //Private:
        /*
         * s:37
         * If this triangle's angles are 30,60,90 
         * then the line across the angle 30 eqauls to half of hypotenuseLine.
         */
        public bool ReverseIsAngle30()
        {
            //Run over the angles
            foreach (Angle angle in AnglesKeys)
            {
                //If angle equals to 30
                Node current = _db.HandleEquations.Equations[angle].ElementAt(0);
                if (current.Expression.ToString() == "30")
                {

                    Line line = heightLine.ToString().Contains(angle.ToString()[1]) ? baseLine : heightLine;
                    Line otherLine = line.Equals(heightLine) ? baseLine : heightLine;
                    Entity expr = 2 * line.variable;

                    _db.UpdateLineBigger(hypotenuseLine, expr, line,
                        "במשולש ישר זווית שזוויותיו הן 30° ו 60° הניצב שמול הזווית של 30° שווה למחצית היתר",
                        new List<Node> { current });

                    //Update lines
                    Entity tempExpr = _db.HandleEquations.Equations[line][0].Expression;

                    if (!tempExpr.Evaled is Number)
                    {
                        Node lineNode = new Node(line.ToString(),
                        $"{otherLine.variable}/sqrt(3)",
                        "הצבה");
                        Entity temp = $"sqrt( {hypotenuseLine.variable} ^ 2 - {otherLine.variable} ^ 2)";
                        lineNode.Parents.Add(_db.HandleEquations.Equations[hypotenuseLine].First((n) => n.Expression.Equals(expr)));
                        lineNode.Parents.Add(_db.HandleEquations.Equations[line].First((n) => n.Expression.Equals(temp.Simplify())));
                        _db.Update(line, lineNode, DataType.Equations);

                        //Update hypotenuse
                        Node hypotenuseLineNode = new Node(hypotenuseLine.ToString(),
                           $"(2*{otherLine.variable})/sqrt(3)",
                           "הצבה");
                        Entity temp3 = hypotenuseLine.variable / 2;
                        hypotenuseLineNode.Parents.Add(_db.HandleEquations.Equations[line].First((n) => n.Expression.Equals(temp3)));
                        Entity temp2 = $"sqrt( {line.variable} ^ 2 + {otherLine.variable} ^ 2)";
                        hypotenuseLineNode.Parents.Add(_db.HandleEquations.Equations[hypotenuseLine].First((n) => n.Expression.Equals(temp2.Simplify())));
                        _db.Update(hypotenuseLine, hypotenuseLineNode, DataType.Equations);
                    }

                    return true;
                }
            }
            return false;
        }
        /*
         * s:38
         * If this triangle has First line that eqauls to half of hypotenuseLine
         * then the angle across this line equals to 30
         */
        public bool IsAngle30()
        {
            //Run over the hypotenuseLine's expression
            foreach (Node n in _db.HandleEquations.Equations[hypotenuseLine])
            {
                //Init lines for checking if they are equal to half of hypotenuse
                Node isLine1Num = _db.HandleEquations.Equations[heightLine].ElementAt(0);
                Node isLine2Num = _db.HandleEquations.Equations[baseLine].ElementAt(0);
                Entity line1 = isLine1Num.Expression.Simplify().IsConstant ? 2 * isLine1Num.Expression.Simplify() : 2 * heightLine.variable;
                Entity line2 = isLine2Num.Expression.Simplify().IsConstant ? 2 * isLine2Num.Expression.Simplify() : 2 * baseLine.variable;

                Entity currentExpr = n.Expression.Simplify();


                if (currentExpr != null)
                {
                    List<Node> addToParents = new List<Node>() { n };

                    //If triangles's hieght equals to half of hypotenuse
                    if (currentExpr.Equals(line1.Simplify()))
                    {
                        Angle crossA = AnglesKeys.Find((a) => a.ToString()[1] != heightLine.ToString()[0] &&
                                                        a.ToString()[1] != heightLine.ToString()[1]);
                        if (isLine1Num.Expression.Simplify().IsConstant)
                            addToParents.Add(isLine1Num);
                        _db.Update(crossA, new Node(
                            crossA.ToString(),
                            30,
                            "אם במשולש ישר זווית אחד מהניצבים שווה למחצית היתר אז הזווית שמול הניצב שווה ל30°",
                            addToParents), DataType.Equations);
                        return true;

                    }
                    //If triangles's base equals to half of hypotenuse
                    else if (currentExpr.Equals(line2.Simplify()))
                    {
                        Angle crossA = AnglesKeys.Find((a) => a.ToString()[1] != baseLine.ToString()[0] &&
                                                        a.ToString()[1] != baseLine.ToString()[1]);

                        if (isLine2Num.Expression.Simplify().IsConstant)
                            addToParents.Add(isLine2Num);
                        _db.Update(crossA, new Node(
                            crossA.ToString(),
                            30,
                            "אם במשולש ישר זווית אחד מהניצבים שווה למחצית היתר אז הזווית שמול הניצב שווה ל30°",
                            addToParents), DataType.Equations);
                        return true;
                    }
                }
            }
            return false;
        }
        /*
         * s:40 
         * If the median equals to half of the line it divided to half
         * then the triangle is First right triangle
         */
        private static RightTriangle CheckByMedian(Database db, Triangle triangle, string point)
        {
            //If triangle has First median from point
            Median median = triangle.GetMedian(point);
            if (median == null) return null;

            //Init triangle (abc) and median (bd) points 
            char a, b, c, d;
            b = point[0];
            d = median.name[0] == b ? median.name[1] : median.name[0];
            a = triangle.ToString()[0] != b ? triangle.ToString()[0] : triangle.ToString()[1];
            c = triangle.ToString()[1] != b && triangle.ToString()[1] != a ? triangle.ToString()[1] : triangle.ToString()[2];
            string strA = a.ToString();
            string strB = b.ToString();
            string strC = c.ToString();
            string strD = d.ToString();

            //Init hypotenuse for checking if median is equal to half of it
            Node isAcNum = db.HandleEquations.Equations[new Line(strA + strC)].ElementAt(0);
            Entity acDividedBy2 = isAcNum.Expression.Simplify().IsConstant ?
                isAcNum.Expression.Simplify() / 2 :
                new Line(strA + strC).variable / 2;

            //Run over all the median's expression
            foreach (Node node in db.HandleEquations.Equations[new Line(median.name)])
            {

                Entity currentExpr = node.Expression.Simplify();

                if (currentExpr.Equals(acDividedBy2.Simplify()))
                {
                    //Init divided lines
                    Line ad = new Line(strA + strD);
                    Line cd = new Line(strC + strD);
                    Node n = new Node(
                                       median.name,
                                       strA + strD + " = " + strC + strD, "כלל המעבר",
                                      new List<Node> { node, db.GetEqualsNode(cd, ad) });


                    //Create new right
                    string reason = "אם במשולש התיכון לאחת מהצלעות שווה למחצית הצלע שאותה הוא חוצה אז המשולש הוא ישר זווית";
                    RightTriangle rt = new RightTriangle(db, strA, strB, strC, new Angle(strA + strB + strC), reason);
                    rt.AddParent(n);

                    //TODO: update ad = db and cd=db? an can do IsoscelesTriangle.istriangle(...)
                    reason = "משולש עם שוקיים שוות הוא משולש שווה שוקיים";
                    new IsoscelesTriangle(db, strA, strB, strD, new Line(strA + strB), reason).AddParent(n);
                    new IsoscelesTriangle(db, strC, strB, strD, new Line(strC + strB), reason).AddParent(n);

                    return rt;
                }

            }
            return null;

        }
        //s: 124
        private static RightTriangle CheckByRevPythagoras(Database db, Triangle triangle)
        {
            List<Line> lines = triangle.LinesKeys;
            for (int i = 0; i < lines.Count; i++)
            {
                foreach (Node node1 in db.HandleEquations.Equations[lines[i]])
                {
                    foreach (Node node2 in db.HandleEquations.Equations[lines[(i + 1) % 3]])
                    {
                        Entity expression = $"({node1.Expression})^2 + ({node2.Expression})^2";
                        foreach (Node node3 in db.HandleEquations.Equations[lines[(i + 2) % 3]])
                        {
                            Entity expression2 = $"({node3.Expression})^2";
                            if (expression.Simplify().Equals(expression2.Simplify()))
                            {
                                Node parent = new Node(expression.ToString(), expression2, "חישוב", new List<Node>() { node1, node2, node3 });
                                //parent.steps.Add(new KeyValuePair<string, string>())
                                List<string> dots = triangle.PointsKeys;
                                string diffrentDot = lines[(i + 1) % 3].ToString().First(c => !lines[i].ToString().Contains(c)).ToString();
                                string commonDot = lines[(i + 1) % 3].ToString().First(c => lines[i].ToString().Contains(c)).ToString();
                                string diffrentDot2 = lines[i].ToString().First(c => !lines[(i + 1) % 3].ToString().Contains(c)).ToString();
                                Angle rightAngle = new Angle(diffrentDot + commonDot + diffrentDot2);
                                RightTriangle rt = new RightTriangle(db, dots[0], dots[1], dots[2], rightAngle, "אם במשולש סכום שטחי הריבועים הבנויים על שתי צלעות המשולש שווה לשטח הריבוע הבנוי על הצלע השלישית אז הוא ישר זווית");
                                rt.AddParent(parent);

                                return rt;
                            }
                        }
                    }
                }

            }
            return null;
        }
        //override
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            // Compare additional properties of Triangle class if any
            RightTriangle otherTriangle = (RightTriangle)obj;
            var otherPoints = otherTriangle.PointsKeys;
            foreach (var point in PointsKeys)
            {
                if (!otherPoints.Contains(point))
                    return false;

            }

            return true;

        }
        public override int GetHashCode()
        {
            // Generate hash code based on additional properties of Triangle class if any
            return PointsKeys[0].GetHashCode() + PointsKeys[1].GetHashCode() + PointsKeys[2].GetHashCode();
        }
    }
}
