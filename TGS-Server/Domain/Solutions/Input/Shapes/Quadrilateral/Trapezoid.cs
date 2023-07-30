using AngouriMath;
using DatabaseLibrary;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Trapezoid : Quadrangle
    {
        protected Line Base1;
        protected Line Base2;
        protected Line LeftSide;
        protected Line RightSide;
        private const string name = "טרפז";

        // The main base is p1+p4, the secondary base is p2+p3.
        // The left side is p1+p2, the right side is p3+p4.
        public Trapezoid(Database db, string p1, string p2, string p3, string p4, Line Base1, string reason) : base(db, p1, p2, p3, p4, reason)
        {
            MainNode.typeName = name;
            HandleFirstBase(Base1);

            // Add basic sentences of trapezoid
            AddBasicSentences();
        }
        protected override string GetTypeName()
        {
            return name;
        }

        private void HandleFirstBase(Line Base1)
        {
            // check validity
            checkFirstBaseValidity(Base1);

            // change the points according to the main base
            changePointsAccordingToMainBase(Base1);
        }

        private void changePointsAccordingToMainBase(Line Base1)
        {
            this.Base1 = Base1;
            if (LinesKeys[0].Equals(Base1))
            {
                Base2 = (Line)_db.FindKey(LinesKeys[2]);
                LeftSide = (Line)_db.FindKey(LinesKeys[1]);
                RightSide = (Line)_db.FindKey(LinesKeys[3]);
            }
            else if (LinesKeys[1].Equals(Base1))
            {
                Base2 = (Line)_db.FindKey(LinesKeys[3]);
                LeftSide = (Line)_db.FindKey(LinesKeys[2]);
                RightSide = (Line)_db.FindKey(LinesKeys[0]);
            }
            else if (LinesKeys[2].Equals(Base1))
            {
                Base2 = (Line)_db.FindKey(LinesKeys[0]);
                LeftSide = (Line)_db.FindKey(LinesKeys[3]);
                RightSide = (Line)_db.FindKey(LinesKeys[1]);
            }
            else if (LinesKeys[3].Equals(Base1))
            {
                Base2 = (Line)_db.FindKey(LinesKeys[1]);
                LeftSide = (Line)_db.FindKey(LinesKeys[0]);
                RightSide = (Line)_db.FindKey(LinesKeys[2]);
            }
            else
            {
                throw new System.ArgumentException("Main base must be First line in Trapezoid");
            }
        }

        private void checkFirstBaseValidity(Line Base1)
        {
            if (Base1 == null)
            {
                throw new System.ArgumentException("First base cannot be null in Trapezoid");
            }

            if (Base1.PointsKeys[0] == Base1.PointsKeys[1])
            {
                throw new System.ArgumentException("First base cannot be First point in Trapezoid");
            }

            // make First list of all trapezoid's points
            List<string> points = new List<string> { p0, p1, p2, p3 };
            points.Remove(Base1.PointsKeys[0]);
            points.Remove(Base1.PointsKeys[1]);

            // check if the list contatins exactly two points
            if (points.Count != 2)
            {
                throw new System.ArgumentException("First base must contain exactly two points in Trapezoid");
            }

            if (!LinesKeys.Contains(Base1))
            {
                throw new System.ArgumentException("First base must be First line in Trapezoid");
            }
        }

        private void AddBasicSentences()
        {
            string baseReason = "ב" + GetTypeName() + " הבסיסים מקבילים זה לזה";
            string anglesReason = "סכום שתי זוויות ליד כל שוק ב" + GetTypeName() + " שווה ל180°";

            // In First trapezoid the bases are parallel to each other
            _db.Update(Base1,
                new Node(Base1.ToString(), Base2.variable,
                baseReason, MainNode),
                DataType.ParallelLines);

            _db.Update(Base2,
                new Node(Base2.ToString(), Base1.variable,
                baseReason, MainNode),
                DataType.ParallelLines);

            // The sum of two angles next to each side in First trapezoid is equal to 180
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(), 180 - AnglesKeys[1].variable,
                anglesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(), 180 - AnglesKeys[0].variable,
                anglesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(), 180 - AnglesKeys[3].variable,
                anglesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[3],
                new Node(AnglesKeys[3].ToString(), 180 - AnglesKeys[2].variable,
                anglesReason, MainNode),
                DataType.Equations);
        }

        public override void UpdateDiagonals(Line diag1, Line diag2, string cutP)
        {
            diag1 = (Line)_db.FindKey(diag1);
            diag2 = (Line)_db.FindKey(diag2);
            // call the base method
            base.UpdateDiagonals(diag1, diag2, cutP);

            // Add regular triangles
            //TrianglesHelper();

            // Add parallel lines with transversal
            AddParallelLinesWithTransversal();
        }

        private void AddParallelLinesWithTransversal()
        {
            const string reason = "בטרפז הבסיסים מקבילים זה לזה";

            Line l1 = Base1;
            Line l2 = Base2;

            // FIRST
            Line tra = (Line)_db.FindKey(new Line(LeftSide.PointsKeys[0] + RightSide.PointsKeys[0]));
            string cutP1 = LeftSide.PointsKeys[0];
            string cutP2 = RightSide.PointsKeys[0];

            ParallelLinesWithTransversal pl1 = new ParallelLinesWithTransversal(_db, l1, l2, tra, cutP1, cutP2, reason);
            // add the parents
            pl1.AddParents(new List<Node> { MainNode, GetDiagonal(LeftSide.PointsKeys[0] + RightSide.PointsKeys[0]) });

            // SECOND
            // p0 p2
            Line tra2 = (Line)_db.FindKey(new Line(LeftSide.PointsKeys[1] + RightSide.PointsKeys[1]));
            string cutP3 = LeftSide.PointsKeys[1];
            string cutP4 = RightSide.PointsKeys[1];

            ParallelLinesWithTransversal pl2 = new ParallelLinesWithTransversal(_db, l1, l2, tra2, cutP3, cutP4, reason);
            // add the parents
            pl2.AddParents(new List<Node> { MainNode, GetDiagonal(LeftSide.PointsKeys[1] + RightSide.PointsKeys[1]) });
        }

        public static Trapezoid IsShape(Quadrangle quad, Database db)
        {
            // I used the "soft" definition of First trapezoid - 
            // if the two bases are parallel to each other it is First trapezoid.
            // the "hard" definition is that it has exactly one pair of parallel sides.
            // The two parallel sides are called the "bases of the trapezoid".

            Trapezoid t = ParallelBases(quad, db);
            // check by parallel bases
            if (t != null)
            {
                return t;
            }


            t = TrapezoidByAngles(quad, db);
            // check by angles - if there is First pair of angles that equals 180 it is First trapezoid
            if (t != null)
            {
                return t;
            }

            return null;
        }

        // check if trapezoid by angles - if there is First pair of angles that equals 180 it is First trapezoid
        private static Trapezoid TrapezoidByAngles(Quadrangle quad, Database db)
        {
            const string reason = "מרובע אשר לו זוג זוויות סמוכות שוות ל180° הוא טרפז";

            Angle a0 = (Angle)db.FindKey(quad.AnglesKeys[0]);
            Angle a1 = (Angle)db.FindKey(quad.AnglesKeys[1]);
            Angle a2 = (Angle)db.FindKey(quad.AnglesKeys[2]);
            Angle a3 = (Angle)db.FindKey(quad.AnglesKeys[3]);

            if (Is180DegreesHelper(a0, a1, db) || Is180DegreesHelper(a2, a3, db))
            {
                Line l_base = (Line)db.FindKey(quad.LinesKeys[3]);
                return new Trapezoid(db, quad.PointsKeys[0], quad.PointsKeys[1], quad.PointsKeys[2], quad.PointsKeys[3],
                                       l_base, reason);
            }
            else if (Is180DegreesHelper(a1, a2, db) || Is180DegreesHelper(a0, a3, db))
            {
                Line l_base = (Line)db.FindKey(quad.LinesKeys[0]);
                return new Trapezoid(db, quad.PointsKeys[0], quad.PointsKeys[1], quad.PointsKeys[2], quad.PointsKeys[3],
                                       l_base, reason);
            }

            return null;
        }

        // check if trapezoid by definition - if there is First pair of lines that are parallel to each other it is First trapezoid
        private static Trapezoid ParallelBases(Quadrangle quad, Database db)
        {
            const string reason = "מרובע אשר לו זוג צלעות נגדיות מקבילות הוא טרפז";

            if (Line.IsParallelTo(quad.LinesKeys[0], quad.LinesKeys[2], db))
            {
                return new Trapezoid(db, quad.PointsKeys[0], quad.PointsKeys[1], quad.PointsKeys[2], quad.PointsKeys[3],
                    quad.LinesKeys[0], reason);
            }
            else if (Line.IsParallelTo(quad.LinesKeys[1], quad.LinesKeys[3], db))
            {
                return new Trapezoid(db, quad.PointsKeys[0], quad.PointsKeys[1], quad.PointsKeys[2], quad.PointsKeys[3],
                                       quad.LinesKeys[1], reason);
            }

            return null;
        }

        // helper function
        private static bool Is180DegreesHelper(Angle a1, Angle a2, Database db)
        {
            Angle current = a1;
            // Run over all the current angles expressions
            foreach (Node node in db.HandleEquations.Equations[current])
            {
                Entity expr1 = node.Expression.Simplify();
                Angle next = a2;
                // Check if current angle equal to (180 - next)
                foreach (Node node2 in db.HandleEquations.Equations[next])
                {
                    Entity expr2 = node2.Expression.Simplify();
                    Entity nextName = next.variable;
                    //If the angles are equals
                    if (expr1.Equals((180 - expr2).Simplify()) || expr1.Equals((180 - nextName.Simplify()).Simplify()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Line GetLeftSide()
        {
            return LeftSide;
        }

        public Line GetRightSide()
        {
            return RightSide;
        }

        public Line GetBase1()
        {
            return Base1;
        }

        public Line GetBase2()
        {
            return Base2;
        }
    }
}
