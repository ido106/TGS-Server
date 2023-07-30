using AngouriMath;
using DatabaseLibrary;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Parallelogram : Quadrangle
    {
        private const string name = "מקבילית";
        public Parallelogram(Database db, string p1, string p2, string p3, string p4, string reason) : base(db, p1, p2, p3, p4, reason)
        {
            MainNode.typeName = name;

            // Add basic sentences of parallelogram
            AddBasicSentences();

            //DiagonalsCutEachOther("מקום החיתוך של האלכסונים");
        }

        // S: 45
        // S: 59
        public void DiagonalsCutEachOther(string midPoint)
        {
            string diagonalsReason = "האלכסונים ב" + GetTypeName() + " חוצים זה את זה";

            if (midPoint == null || midPoint == "" || midPoint.Length > 1)
            {
                return;
            }

            // get first part of the first diagonal
            Line l1 = (Line)_db.FindKey(new Line(PointsKeys[0] + midPoint));
            // get second part of the first diagonal
            Line l2 = (Line)_db.FindKey(new Line(midPoint + PointsKeys[2]));
            // get first part of the second diagonal
            Line l3 = (Line)_db.FindKey(new Line(PointsKeys[1] + midPoint));
            // get second part of the second diagonal
            Line l4 = (Line)_db.FindKey(new Line(midPoint + PointsKeys[3]));

            // add l1 equal l2 to db
            _db.Update(l1, new Node(l1.ToString(), l2.variable, diagonalsReason, MainNode), DataType.Equations);
            _db.Update(l2, new Node(l2.ToString(), l1.variable, diagonalsReason, MainNode), DataType.Equations);

            // add l3 equal l4 to db
            _db.Update(l3, new Node(l3.ToString(), l4.variable, diagonalsReason, MainNode), DataType.Equations);
            _db.Update(l4, new Node(l4.ToString(), l3.variable, diagonalsReason, MainNode), DataType.Equations);
        }

        private void AddBasicSentences()
        {
            string degreesReason = "כל שתי זוויות נגדיות ב" + GetTypeName() + " שוות זו לזו";
            string adjecentDegreesReason = "סכום כל שתי זוויות סמוכות ב" + GetTypeName() + " שווה ל180°";
            string sidesReason = "כל שתי צלעות נגדיות ב" + GetTypeName() + " שוות זו לזו";
            string oppositeSidesParallel = "כל שתי צלעות נגדיות ב" + GetTypeName() + " מקבילות זו לזו";


            // Any two opposite angles in First parallelogram are equal to each other
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(),
               AnglesKeys[2].variable,
               degreesReason,
               MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
               new Node(AnglesKeys[2].ToString(),
              AnglesKeys[0].variable,
              degreesReason,
              MainNode),
               DataType.Equations);

            _db.Update(AnglesKeys[1],
              new Node(AnglesKeys[1].ToString(),
             AnglesKeys[3].variable,
             degreesReason,
             MainNode),
              DataType.Equations);

            _db.Update(AnglesKeys[3],
               new Node(AnglesKeys[3].ToString(),
              AnglesKeys[1].variable,
              degreesReason,
              MainNode),
               DataType.Equations);

            // Any two opposite sides of First parallelogram are equal to each other
            _db.Update(LinesKeys[0],
                new Node(LinesKeys[0].ToString(), LinesKeys[2].variable,
                sidesReason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[2],
                new Node(LinesKeys[2].ToString(), LinesKeys[0].variable,
                sidesReason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[1],
                new Node(LinesKeys[1].ToString(), LinesKeys[3].variable,
                sidesReason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[3],
                new Node(LinesKeys[3].ToString(), LinesKeys[1].variable,
                sidesReason, MainNode),
                DataType.Equations);

            // The sum of any two adjacent angles in First parallelogram is 180
            _db.Update(AnglesKeys[0],
               new Node(AnglesKeys[0].ToString(),
               180 - AnglesKeys[1].variable,
               adjecentDegreesReason, MainNode),
               DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(),
                180 - AnglesKeys[0].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(),
                180 - AnglesKeys[2].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(),
                180 - AnglesKeys[1].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(),
                180 - AnglesKeys[3].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[3],
                new Node(AnglesKeys[3].ToString(),
                180 - AnglesKeys[2].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[3]
                , new Node(AnglesKeys[3].ToString(),
                180 - AnglesKeys[0].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(),
                180 - AnglesKeys[3].variable,
                adjecentDegreesReason, MainNode),
                DataType.Equations);

            // Any two opposite sides in First parallelogram are parallel
            _db.Update(LinesKeys[0],
                new Node(LinesKeys[0].ToString(),
                LinesKeys[2].variable,
                oppositeSidesParallel, MainNode),
                DataType.ParallelLines);

            _db.Update(LinesKeys[2],
                new Node(LinesKeys[2].ToString(),
                LinesKeys[0].variable,
                oppositeSidesParallel, MainNode),
                DataType.ParallelLines);

            _db.Update(LinesKeys[1],
                new Node(LinesKeys[1].ToString(),
                LinesKeys[3].variable,
                oppositeSidesParallel, MainNode),
                DataType.ParallelLines);

            _db.Update(LinesKeys[3],
                new Node(LinesKeys[3].ToString(),
                LinesKeys[1].variable,
                oppositeSidesParallel, MainNode),
                DataType.ParallelLines);
        }

        protected override string GetTypeName()
        {
            return name;
        }

        public override void UpdateDiagonals(Line diag1, Line diag2, string cutP)
        {
            diag1 = (Line)_db.FindKey(diag1);
            diag2 = (Line)_db.FindKey(diag2);
            // call the base method
            base.UpdateDiagonals(diag1, diag2, cutP);

            // Add parallel lines with transversal
            AddParallelLinesWithTransversal();
        }

        private void AddParallelLinesWithTransversal()
        {
            const string reason = "במקבילית כל שתי צלעות נגדיות מקבילות זו לזו";
            Line l1 = new Line(p0 + p1);
            Line l2 = new Line(p1 + p2);
            Line l3 = new Line(p2 + p3);
            Line l4 = new Line(p3 + p0);

            Line tra1 = (Line)_db.FindKey(new Line(p3 + p1));
            // 1
            ParallelLinesWithTransversal pl1 = new ParallelLinesWithTransversal(_db, l1, l3, tra1, p3, p1, reason);
            // add the parents
            pl1.AddParents(new List<Node> { MainNode, GetDiagonal(p3 + p1) });

            // 2
            ParallelLinesWithTransversal pl2 = new ParallelLinesWithTransversal(_db, l2, l4, tra1, p3, p1, reason);
            // add the parents
            pl2.AddParents(new List<Node> { MainNode, GetDiagonal(p3 + p1) });

            Line tra2 = (Line)_db.FindKey(new Line(p0 + p2));
            // 3
            ParallelLinesWithTransversal pl3 = new ParallelLinesWithTransversal(_db, l1, l3, tra2, p0, p2, reason);
            // add the parents
            pl3.AddParents(new List<Node> { MainNode, GetDiagonal(p0 + p2) });

            // 4
            ParallelLinesWithTransversal pl4 = new ParallelLinesWithTransversal(_db, l2, l4, tra2, p0, p2, reason);
            // add the parents
            pl4.AddParents(new List<Node> { MainNode, GetDiagonal(p0 + p2) });
        }

        public static Parallelogram IsShape(Quadrangle quad, Database db)
        {
            Parallelogram p = IsOppositeLinesEqualParallel(db, quad);
            if (p != null) return p;

            p = IsBothOppositeAnglesEqual(db, quad);
            if (p != null) return p;

            p = IsBothOppositeSidesEqual(db, quad);
            if (p != null) return p;

            p = DiagonalsCutEachOther(db, quad, quad.GetCutPoint());
            if (p != null) return p;

            p = IsEveryTwoAdjecentAnglesEquals180(db, quad);
            if (p != null) return p;

            return null;
        }

        // S: 49
        private static Parallelogram DiagonalsCutEachOther(Database db, Quadrangle quad, string midPoint)
        {
            const string reason = "אם במרובע אלכסונים חוצים זה את זה אז המרובע הוא מקבילית";

            if (midPoint == null || midPoint == "" || midPoint.Length > 1)
            {
                return null;
            }

            // get first part of the first diagonal
            Line l1 = (Line)db.FindKey(new Line(quad.PointsKeys[0] + midPoint));
            // get second part of the first diagonal
            Line l2 = (Line)db.FindKey(new Line(quad.PointsKeys[2] + midPoint));
            // get first part of the second diagonal
            Line l3 = (Line)db.FindKey(new Line(quad.PointsKeys[1] + midPoint));
            // get second part of the second diagonal
            Line l4 = (Line)db.FindKey(new Line(quad.PointsKeys[3] + midPoint));

            if (Line.IsEqualTo(l1, l2, db) && Line.IsEqualTo(l3, l4, db))
            {
                return new Parallelogram(db, quad.PointsKeys[0], quad.PointsKeys[1],
                    quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }
            return null;
        }

        // S: 46
        // Checks if ONE PAIR of the opposite lines are equal and parallel
        private static Parallelogram IsOppositeLinesEqualParallel(Database db, Quadrangle quad)
        {
            const string reason = "אם שתי צלעות נגדיות במרובע שוות לזו ומקבילות זו לזו אז המרובע הוא מקבילית";

            bool firstPairEqualParallel =
                Line.IsEqualTo(quad.LinesKeys[0], quad.LinesKeys[2], db) &&
                Line.IsParallelTo(quad.LinesKeys[0], quad.LinesKeys[2], db);

            bool secondPairEqualParallel =
                Line.IsEqualTo(quad.LinesKeys[1], quad.LinesKeys[3], db) &&
                Line.IsParallelTo(quad.LinesKeys[1], quad.LinesKeys[3], db);

            // only one pair is needed to be equal and parallel
            if (firstPairEqualParallel || secondPairEqualParallel)
            {
                return new Parallelogram(db, quad.PointsKeys[0], quad.PointsKeys[1],
                    quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 47
        // Checks if the both opposite angles are equal
        private static Parallelogram IsBothOppositeAnglesEqual(Database db, Quadrangle quad)
        {
            const string reason = "אם במרובע כל שתי זוויות נגדיות שוות זו לזו אז המרובע הוא מקבילית";

            if (Angle.IsEqualTo(quad.AnglesKeys[0], quad.AnglesKeys[2], db) &&
                Angle.IsEqualTo(quad.AnglesKeys[1], quad.AnglesKeys[3], db))
            {
                return new Parallelogram(db, quad.PointsKeys[0], quad.PointsKeys[1],
                    quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 48
        private static Parallelogram IsBothOppositeSidesEqual(Database db, Quadrangle quad)
        {
            const string reason = "אם במרובע כל שתי צלעות נגדיות שוות זו לזו אז המרובע הוא מקבילית";

            if (Line.IsEqualTo(quad.LinesKeys[0], quad.LinesKeys[2], db) &&
                Line.IsEqualTo(quad.LinesKeys[1], quad.LinesKeys[3], db))
            {
                return new Parallelogram(db, quad.PointsKeys[0], quad.PointsKeys[1],
                    quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }
            return null;
        }

        // S: 50
        private static Parallelogram IsEveryTwoAdjecentAnglesEquals180(Database db, Quadrangle quad)
        {
            const string reason = "אם במרובע סכום כל שתי זוויות סמוכות שווה ל180° אז המרובע הוא מקבילית";

            bool Is180Deg(Angle a1, Angle a2, Database db)
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

            bool pair1 = Is180Deg(quad.AnglesKeys[0], quad.AnglesKeys[1], db);
            bool pair2 = Is180Deg(quad.AnglesKeys[1], quad.AnglesKeys[2], db);
            bool pair3 = Is180Deg(quad.AnglesKeys[2], quad.AnglesKeys[3], db);
            bool pair4 = Is180Deg(quad.AnglesKeys[3], quad.AnglesKeys[0], db);

            if (pair1 && pair2 && pair3 && pair4)
            {
                return new Parallelogram(db, quad.PointsKeys[0], quad.PointsKeys[1],
                    quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }

            return null;
        }
    }
}