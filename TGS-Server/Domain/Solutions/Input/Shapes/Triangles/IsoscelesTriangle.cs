using AngouriMath;
using DatabaseLibrary;
using Domain.Lines;
using static DatabaseLibrary.Database;

namespace Domain.Triangles
{
    public class IsoscelesTriangle : Triangle
    {
        //Variables:
        public Line baseSide { get; set; } = null;
        public Line equalSide1 { get; set; } = null;
        public Line equalSide2 { get; set; } = null;
        //Constractor
        public IsoscelesTriangle(Database db, string p1, string p2, string p3, Line baseSide, string reason) : base(db, p1, p2, p3, reason)
        {
            //Main Node
            MainNode.typeName = "משולש שווה שוקיים";

            //Init special lines
            this.baseSide = LinesKeys.Find((l) => l.Equals(baseSide));
            this.equalSide1 = LinesKeys.Find((l) => !l.Equals(baseSide));
            this.equalSide2 = LinesKeys.Find((l) => !l.Equals(baseSide) && !l.Equals(this.equalSide1));

            if (this.baseSide == null || this.equalSide1 == null || this.equalSide2 == null)
                throw new Exception("invalid isosceles triangle's lines");

            //Init special angles
            Angle baseAngle1 = AnglesKeys.Find((a) => a.ToString()[1] == this.baseSide.ToString()[0]);
            Angle baseAngle2 = AnglesKeys.Find((a) => a.ToString()[1] == this.baseSide.ToString()[1]);
            Angle mainAngle = AnglesKeys.Find((a) => a != baseAngle1 && a != baseAngle2);

            if (this.baseSide == null || this.equalSide1 == null || this.equalSide2 == null)
                throw new Exception("invalid isosceles triangle's angles");

            //Update angles
            _db.Update(baseAngle1, new Node(baseAngle1.ToString(), baseAngle2.variable,
                 "זוויות הבסיס במשולש שווה שוקיים שוות זו לזו", MainNode), DataType.Equations);
            _db.Update(baseAngle2, new Node(baseAngle2.ToString(), baseAngle1.variable,
                "זוויות הבסיס במשולש שווה שוקיים שוות זו לזו", MainNode)
                , DataType.Equations);

        
            //Update lines*/
            _db.Update(this.equalSide1, new Node(this.equalSide1.ToString(), this.equalSide2.variable,
                "במשולש שווה שוקיים השוקיים שוות זו לזו", MainNode), DataType.Equations);
            _db.Update(this.equalSide2, new Node(this.equalSide2.ToString(), this.equalSide1.variable,
                "במשולש שווה שוקיים השוקיים שוות זו לזו", MainNode), DataType.Equations);
            //Add this shape to database
            db.AddShape(p1 + p2 + p3, this);
        }
        //Public:
        public override void UpdateHeight(string p1, string p2, string r, Node mainParent)
        {
            if (Heights.ContainsKey(p1))
            {
                if (Heights[p1] != null) return;
            }
            else if (Heights.ContainsKey(p2))
            {
                if (Heights[p2] != null) return;
            }
            string mainPoint = PointsKeys.Find((p) => p != baseSide.ToString()[0].ToString() &&
                                                      p != baseSide.ToString()[1].ToString());
            if (mainPoint == p1 || mainPoint == p2)
            {
                base.UpdateHeight(p1, p2, r, mainParent);
                string reason = "במשולש שווה שוקיים חוצה זווית הראש התיכון לבסיס והגובה לבסיס מתלכדים";
                base.UpdateMedian(p1, p2, reason, Heights[mainPoint]);
                base.UpdateAngleBisector(p1, p2, reason, Heights[mainPoint]);
            }
            else
            {
                base.UpdateHeight(p1, p2, r, mainParent);
            }

        }
        public override void UpdateMedian(string p1, string p2, string r, Node mainParent)
        {
            string mainPoint = PointsKeys.Find((p) => p != baseSide.ToString()[0].ToString() &&
                                                         p != baseSide.ToString()[1].ToString());
            if (mainPoint == p1 || mainPoint == p2)
            {
                base.UpdateMedian(p1, p2, r, mainParent);
                string reason = "במשולש שווה שוקיים חוצה זווית הראש התיכון לבסיס והגובה לבסיס מתלכדים";
                base.UpdateHeight(p1, p2, reason, Medians[mainPoint]);
                base.UpdateAngleBisector(p1, p2, reason, Medians[mainPoint]);
            }
            else
            {
                base.UpdateMedian(p1, p2, r, mainParent);
            }
        }
        public override void UpdateAngleBisector(string p1, string p2, string r, Node mainParent)
        {
            string mainPoint = PointsKeys.Find((p) => p != baseSide.ToString()[0].ToString() &&
                                                       p != baseSide.ToString()[1].ToString());
            if (mainPoint == p1 || mainPoint == p2)
            {

                base.UpdateAngleBisector(p1, p2, r, mainParent);
                string reason = "במשולש שווה- שוקיים חוצה זווית הראש התיכון לבסיס והגובה לבסיס מתלכדים";
                base.UpdateHeight(p1, p2, reason, AngleBisectors[mainPoint]);
                base.UpdateMedian(p1, p2, reason, AngleBisectors[mainPoint]);

            }
            else
            {
                base.UpdateAngleBisector(p1, p2, r, mainParent);
            }
        }


        public static IsoscelesTriangle IsShape(Triangle triangle, Database db)
        {
            IsoscelesTriangle isoscelesTriangle = db.GetListShape(triangle.ToString()).FirstOrDefault((t) => t is IsoscelesTriangle)as IsoscelesTriangle;
            if (isoscelesTriangle != null)
            {
                triangle.CopyToChild(isoscelesTriangle);
                return isoscelesTriangle;

            }
            isoscelesTriangle = CheckByLines(db, triangle);
            if (isoscelesTriangle != null)
            {
                triangle.CopyToChild(isoscelesTriangle);
                return isoscelesTriangle;

            }
            isoscelesTriangle = CheckByAngles(db, triangle);
            if (isoscelesTriangle != null)
            {
                triangle.CopyToChild(isoscelesTriangle);
                return isoscelesTriangle;

            }
            isoscelesTriangle = CheckByAngleBisectoAndHeight(db, triangle);
            if (isoscelesTriangle != null)
            {
                triangle.CopyToChild(isoscelesTriangle);
                return isoscelesTriangle;

            }
            isoscelesTriangle = CheckByAngleBisectoAndMedian(db, triangle);
            if (isoscelesTriangle != null)
            {
                triangle.CopyToChild(isoscelesTriangle);
                return isoscelesTriangle;

            }

            isoscelesTriangle =  CheckByAngleMedianAndHeight(db, triangle);
            if (isoscelesTriangle != null)
            {
                triangle.CopyToChild(isoscelesTriangle);
                return isoscelesTriangle;

            }
            return null;
        }
        //Private:

        /*
         * Given First triangle check if it has 2 equal lines.
         * If it does, return the given triangle as First isosceles triangle.
         * If it dosent, return the origin Triangle.
         */
        private static IsoscelesTriangle CheckByLines(Database db, Triangle triangle)
        {
            //Run over all the triangle's lines
            for (int i = 0; i < triangle.LinesKeys.Count; ++i)
            {
                Line current = triangle.LinesKeys[i];
                //Run over all the current line's exprestions
                foreach (Node node in db.HandleEquations.Equations[current])
                {
                    Entity expr1 = node.Expression.Simplify();
                    Line next = triangle.LinesKeys[(i + 1) % 3];
                    //Check if current line equal to next line
                    foreach (Node node2 in db.HandleEquations.Equations[next])
                    {
                        Entity expr2 = node2.Expression.Simplify();
                        Entity nextName = next.variable;
                        //If the lins are equals
                        if (expr1.Equals(expr2) || expr1.Equals(nextName.Simplify()))
                        {
                            //Init base line
                            Line baseLine = triangle.LinesKeys.Find((l) => l != current && l != next);

                            //Init Isoscele sTriangle
                            IsoscelesTriangle newTriangle =
                            new IsoscelesTriangle(db,
                            triangle.PointsKeys[0],
                            triangle.PointsKeys[1],
                            triangle.PointsKeys[2],
                            baseLine,
                            "משולש עם שוקיים שוות הוא משולש שווה שוקיים");
                            Node equalsLines = new Node(node.name, node2.name, "כלל המעבר", new List<Node> { node, node2 });
                            newTriangle.MainNode.Parents.Add(equalsLines);

                            //Copy all properties
                            triangle.CopyToChild(newTriangle);
                            //return newTriangle;
                            return newTriangle;
                        }
                    }

                }
            }
            return null;
        }
        //s:22
        private static IsoscelesTriangle CheckByAngles(Database db, Triangle triangle)
        {
            //Run over all the triangle's angles
            for (int i = 0; i < triangle.AnglesKeys.Count; ++i)
            {
                Angle current = triangle.AnglesKeys[i];
                //Run over all the current angles's exprestions
                foreach (Node node in db.HandleEquations.Equations[current])
                {
                    Entity expr1 = node.Expression.Simplify();
                    Angle next = triangle.AnglesKeys[(i + 1) % 3];
                    //Check if current angle equal to next angle
                    foreach (Node node2 in db.HandleEquations.Equations[next])
                    {
                        Entity expr2 = node2.Expression.Simplify();
                        Entity nextName = next.variable;
                        //If the angles are equals
                        if (expr1.Equals(expr2) || expr1.Equals(nextName.Simplify()))
                        {
                            //Init head angle
                            Angle headAngle = triangle.AnglesKeys.Find((a) => a != current && a != next);
                            Line baseLine = triangle.LinesKeys.Find((l) => !l.ToString().Contains(headAngle.ToString()[1]));

                            //Init Isoscele Triangle
                            IsoscelesTriangle newTriangle =
                            new IsoscelesTriangle(db,
                            triangle.PointsKeys[0],
                            triangle.PointsKeys[1],
                            triangle.PointsKeys[2],
                            baseLine,
                            "אם שתי זוויות המשולש שוות זו לזו אז המשולש הוא שווה שוקיים");
                            Node equalsLines = new Node(node.name, node2.name, "כלל המעבר", new List<Node> { node, node2 });
                            newTriangle.MainNode.Parents.Add(equalsLines);

                            //Copy all properties
                            triangle.CopyToChild(newTriangle);

                            //return newTriangle;
                            return newTriangle;
                        }
                    }

                }
            }
            //return triangle;
            return null;
        }
        //s:23
        private static IsoscelesTriangle CheckByAngleBisectoAndHeight(Database db, Triangle triangle)
        {
            foreach (string point in triangle.PointsKeys)
            {
                if (triangle.GetAngleBisector(point) == null ||
                    triangle.GetHeight(point) == null)
                {
                    return null;
                }
                AngleBisector angleBisector = triangle.GetAngleBisector(point);
                Height height = triangle.GetHeight(point);
                if (angleBisector.name.Equals(height.name))
                {
                    //Init base line
                    Line baseLine = triangle.LinesKeys.Find((l) => !l.ToString().Contains(point));

                    //Init Isoscele Triangle
                    IsoscelesTriangle newTriangle =
                    new IsoscelesTriangle(db,
                    triangle.PointsKeys[0],
                    triangle.PointsKeys[1],
                    triangle.PointsKeys[2],
                    baseLine,
                    "אם במשולש חוצה זווית מתלכד עם הגובה לצלע שמול הזווית אז המשולש הוא שווה שוקיים");
                    newTriangle.MainNode.Parents.Add(angleBisector);
                    newTriangle.MainNode.Parents.Add(height);
                    newTriangle.MainNode.Parents.Add(triangle.GetMainNode());

                    //Copy all properties
                    triangle.CopyToChild(newTriangle);
                    return newTriangle;
                }
            }
            return null;
        }

        //s:24
        private static IsoscelesTriangle CheckByAngleMedianAndHeight(Database db, Triangle triangle)
        {
            foreach (string point in triangle.PointsKeys)
            {
                if (triangle.GetMedian(point) == null ||
                    triangle.GetHeight(point) == null)
                {
                    return null;
                }
                Median median = triangle.GetMedian(point);
                Height height = triangle.GetHeight(point);
                if (median.name.Equals(height.name))
                {
                    //Init base line
                    Line baseLine = triangle.LinesKeys.Find((l) => !l.ToString().Contains(point));

                    //Init Isoscele Triangle
                    IsoscelesTriangle newTriangle =
                    new IsoscelesTriangle(db,
                    triangle.PointsKeys[0],
                    triangle.PointsKeys[1],
                    triangle.PointsKeys[2],
                    baseLine,
                    "אם במשולש חוצה זווית מתלכד עם הגובה לצלע שמול הזווית אז המשולש הוא שווה שוקיים");
                    newTriangle.MainNode.Parents.Add(median);
                    newTriangle.MainNode.Parents.Add(height);
                    newTriangle.MainNode.Parents.Add(triangle.GetMainNode());

                    //Copy all properties
                    triangle.CopyToChild(newTriangle);
                    return newTriangle;
                }
            }
            return null;
        }
        //s:25
        private static IsoscelesTriangle CheckByAngleBisectoAndMedian(Database db, Triangle triangle)
        {
            foreach (string point in triangle.PointsKeys)
            {
                if (triangle.GetAngleBisector(point) == null ||
                    triangle.GetMedian(point) == null)
                {
                    return null;
                }
                AngleBisector angleBisector = triangle.GetAngleBisector(point);
                Median median = triangle.GetMedian(point);
                if (angleBisector.name.Equals(median.name))
                {
                    //Init base line
                    Line baseLine = triangle.LinesKeys.Find((l) => !l.ToString().Contains(point));

                    //Init Isoscele Triangle
                    IsoscelesTriangle newTriangle =
                    new IsoscelesTriangle(db,
                    triangle.PointsKeys[0],
                    triangle.PointsKeys[1],
                    triangle.PointsKeys[2],
                    baseLine,
                    "אם במשולש חוצה זווית מתלכד עם הגובה לצלע שמול הזווית אז המשולש הוא שווה שוקיים");
                    newTriangle.MainNode.Parents.Add(angleBisector);
                    newTriangle.MainNode.Parents.Add(median);
                    newTriangle.MainNode.Parents.Add(triangle.GetMainNode());

                    //Copy all properties
                    triangle.CopyToChild(newTriangle);
                    return newTriangle;
                }
            }
            return null;
        }
        private void AddPerimeter()
        {
            //  db.HandleEquations.Equations.Add($"perimeter {name}", new Node($"{linesKeys[0]} + {linesKeys[1]} + {linesKeys[2]}", "היקף"));
        }

        private void AddArea()
        {
            // db.HandleEquations.Equations.Add(baseLength* GetHeight() / 2);
        }
        //override
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            // Compare additional properties of Triangle class if any
            IsoscelesTriangle otherTriangle = (IsoscelesTriangle)obj;
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