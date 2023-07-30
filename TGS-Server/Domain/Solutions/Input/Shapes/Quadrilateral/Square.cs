using AngouriMath;
using DatabaseLibrary;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Square : Rhombus
    {
        private const string name = "ריבוע";
        public Square(Database db, string p1, string p2, string p3, string p4, string reason) : base(db, p1, p2, p3, p4, reason)
        {
            MainNode.typeName = name;

            // add basic sentences of square
            AddBasicSentences();

            // advanced sentences
            // DiagonalsCutEqualEachOther("נקודת אמצע"); S#68
            // EqualDiagonals(); S#69
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

            // Make isosceles triangles
            MakeIsoscelesTriangles();

            // Update angles
            UpdateAngles();
        }

        private void MakeIsoscelesTriangles()
        {
            const string reason = "האלכסונים בריבוע חוצים זה את זה ושווים זה לזה";

            // list of nodes with the parents
            List<Node> parents = new List<Node> { MainNode, _diagonalsLinesCut.GetMainNode() };

            // make triangles
            Line b1 = (Line)_db.FindKey(new Line(p0 + p3));
            Triangle t1 = new IsoscelesTriangle(_db, p0, _cutP, p3, b1, reason);
            t1.AddParents(parents);

            Line b2 = (Line)_db.FindKey(new Line(p0 + p1));
            Triangle t2 = new IsoscelesTriangle(_db, p0, _cutP, p1, b2, reason);
            t2.AddParents(parents);

            Line b3 = (Line)_db.FindKey(new Line(p1 + p2));
            Triangle t3 = new IsoscelesTriangle(_db, p1, _cutP, p2, b3, reason);
            t3.AddParents(parents);

            Line b4 = (Line)_db.FindKey(new Line(p2 + p3));
            Triangle t4 = new IsoscelesTriangle(_db, p2, _cutP, p3, b4, reason);
            t4.AddParents(parents);
        }

        // im not sure it is neccesary because of the ParallelLinesWithTransversal but doing it anyway
        // A1 = 90-A2 etc.
        private void UpdateAngles()
        {
            const string reason = "כל אחת מזוויות הריבוע היא בת 90°";
            List<Node> parents_A_C = new List<Node> { MainNode, GetDiagonal(p0 + p2) };
            // A1 A2
            Angle A1 = (Angle)_db.FindKey(new Angle(PointsKeys[1] + PointsKeys[0] + PointsKeys[2]));
            Angle A2 = (Angle)_db.FindKey(new Angle(PointsKeys[3] + PointsKeys[0] + PointsKeys[2]));
            _db.Update(A2, new Node(A2.ToString(), 90 - A1.variable, reason, parents_A_C), DataType.Equations);
            _db.Update(A1, new Node(A1.ToString(), 90 - A2.variable, reason, parents_A_C), DataType.Equations);

            // C1 C2
            Angle C1 = (Angle)_db.FindKey(new Angle(PointsKeys[1] + PointsKeys[2] + PointsKeys[0]));
            Angle C2 = (Angle)_db.FindKey(new Angle(PointsKeys[3] + PointsKeys[2] + PointsKeys[0]));
            _db.Update(C2, new Node(C2.ToString(), 90 - C1.variable, reason, parents_A_C), DataType.Equations);
            _db.Update(C1, new Node(C1.ToString(), 90 - C2.variable, reason, parents_A_C), DataType.Equations);

            List<Node> parents_B_D = new List<Node> { MainNode, GetDiagonal(p1 + p3) };
            // B1 B2
            Angle B1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[1] + PointsKeys[3]));
            Angle B2 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[1] + PointsKeys[3]));
            _db.Update(B2, new Node(B2.ToString(), 90 - B1.variable, reason, parents_B_D), DataType.Equations);
            _db.Update(B1, new Node(B1.ToString(), 90 - B2.variable, reason, parents_B_D), DataType.Equations);

            // D1 D2
            Angle D1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[3] + PointsKeys[1]));
            Angle D2 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[3] + PointsKeys[1]));
            _db.Update(D2, new Node(D2.ToString(), 90 - D1.variable, reason, parents_B_D), DataType.Equations);
            _db.Update(D1, new Node(D1.ToString(), 90 - D2.variable, reason, parents_B_D), DataType.Equations);
        }

        // S: 69
        // Equal Diagonals AC=BD
        public void EqualDiagonals()
        {
            const string reason = "האלכסונים בריבוע שווים זה לזה";

            Line diag1 = (Line)_db.FindKey(new Line(PointsKeys[0] + PointsKeys[2]));
            Line diag2 = (Line)_db.FindKey(new Line(PointsKeys[1] + PointsKeys[3]));

            // update diagonals are equal
            _db.Update(diag1, new Node(diag1.ToString(), diag2.variable, reason, MainNode), DataType.Equations);
            _db.Update(diag2, new Node(diag2.ToString(), diag1.variable, reason, MainNode), DataType.Equations);
        }

        // S: 68
        // AM = BM = CM = DM
        public void DiagonalsCutEqualEachOther(string midPoint)
        {
            string reason = "האלכסונים ב" + GetTypeName() + " חוצים זה את זה ושווים זה לזה";

            if (midPoint == null || midPoint.Length != 1)
                throw new Exception("midPoint must be First single char, DiagonalsCutEachOther() in Square shape");

            Node diag02Node = GetDiagonal(PointsKeys[0] + PointsKeys[2]);
            Node diag13Node = GetDiagonal(PointsKeys[1] + PointsKeys[3]);
            // make First list of the two diagonals
            List<Node> diagonals = new List<Node>() { diag02Node, diag13Node };

            Line AM = (Line)_db.FindKey(new Line(PointsKeys[0] + midPoint));
            Line BM = (Line)_db.FindKey(new Line(PointsKeys[1] + midPoint));
            Line CM = (Line)_db.FindKey(new Line(PointsKeys[2] + midPoint));
            Line DM = (Line)_db.FindKey(new Line(PointsKeys[3] + midPoint));

            // update AM to be equal to all others
            _db.Update(AM, new Node(AM.ToString(), BM.variable, reason, diagonals), DataType.Equations);
            _db.Update(AM, new Node(AM.ToString(), CM.variable, reason, diagonals), DataType.Equations);
            _db.Update(AM, new Node(AM.ToString(), DM.variable, reason, diagonals), DataType.Equations);

            // update BM to be equal to all others
            _db.Update(BM, new Node(BM.ToString(), AM.variable, reason, diagonals), DataType.Equations);
            _db.Update(BM, new Node(BM.ToString(), CM.variable, reason, diagonals), DataType.Equations);
            _db.Update(BM, new Node(BM.ToString(), DM.variable, reason, diagonals), DataType.Equations);

            // update CM to be equal to all others
            _db.Update(CM, new Node(CM.ToString(), AM.variable, reason, diagonals), DataType.Equations);
            _db.Update(CM, new Node(CM.ToString(), BM.variable, reason, diagonals), DataType.Equations);
            _db.Update(CM, new Node(CM.ToString(), DM.variable, reason, diagonals), DataType.Equations);

            // update DM to be equal to all others
            _db.Update(DM, new Node(DM.ToString(), AM.variable, reason, diagonals), DataType.Equations);
            _db.Update(DM, new Node(DM.ToString(), BM.variable, reason, diagonals), DataType.Equations);
            _db.Update(DM, new Node(DM.ToString(), CM.variable, reason, diagonals), DataType.Equations);
        }

        private void AddBasicSentences()
        {
            string reason = "כל אחת מזווית ב" + GetTypeName() + " היא בת 90°";

            // Each of the angles of the square is 90 degrees
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(),
                90,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(),
                90,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(),
                90,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[3],
                new Node(AnglesKeys[3].ToString(),
                90,
                reason, MainNode),
                DataType.Equations);
        }

        public static new Square IsShape(Quadrangle quad, Database db)
        {
            Square s = HasEqualEdgesAnd90Degrees(quad, db);
            if (s != null) return s; // #72

            s = EqualVerticalCutDiagonals(quad, db, quad.GetCutPoint());
            if (s != null) return s; // #73

            s = DiagonalsWithDegree(quad, db, quad.GetCutPoint());
            if (s != null) return s; // #74

            s = IsParallelogramAndDiagonalsEqualAndVertical(quad, db, quad.GetCutPoint());
            if (s != null) return s; // #75

            s = IsRhombusAndDiagonalsEqual(quad, db, quad.GetCutPoint());
            if (s != null) return s; // #76

            s = IsRhombusAndHas90Degrees(quad, db, quad.GetCutPoint());
            if (s != null) return s; // #77

            Rectangle r = Rectangle.IsShape(quad, db);
            if (r == null) return null;

            s = IsDiagonalsVertical(r, db, quad.GetCutPoint());
            if (s != null) return s; // #78

            s = HasEqualEdgesPair(r, db);
            if (s != null) return s; // #79

            s = IsDiagonalsCutDegree(r, db);
            if (s != null) return s; // #80

            return null;
        }

        // S: 80
        // if it is rectangle and one of the diagonals cut the degrees in half so it's First square
        private static Square IsDiagonalsCutDegree(Rectangle r, Database db)
        {
            const string reason = "אם במלבן אחד מהאלכסונים חוצה זווית המלבן אז הוא ריבוע";

            // check if one of the diagonals cut the degrees in half
            // get first angle - diag02
            Angle A1 = (Angle)db.FindKey(new Angle(r.PointsKeys[2] + r.PointsKeys[0] + r.PointsKeys[1]));
            Angle A2 = (Angle)db.FindKey(new Angle(r.PointsKeys[2] + r.PointsKeys[0] + r.PointsKeys[3]));

            // get second angle - diag02
            Angle C1 = (Angle)db.FindKey(new Angle(r.PointsKeys[0] + r.PointsKeys[2] + r.PointsKeys[3]));
            Angle C2 = (Angle)db.FindKey(new Angle(r.PointsKeys[0] + r.PointsKeys[2] + r.PointsKeys[1]));

            // get first angle - diag13
            Angle B1 = (Angle)db.FindKey(new Angle(r.PointsKeys[0] + r.PointsKeys[1] + r.PointsKeys[3]));
            Angle B2 = (Angle)db.FindKey(new Angle(r.PointsKeys[2] + r.PointsKeys[1] + r.PointsKeys[3]));

            // get second angle - diag13
            Angle D1 = (Angle)db.FindKey(new Angle(r.PointsKeys[1] + r.PointsKeys[3] + r.PointsKeys[2]));
            Angle D2 = (Angle)db.FindKey(new Angle(r.PointsKeys[1] + r.PointsKeys[3] + r.PointsKeys[0]));

            if (Angle.IsEqualTo(A1, A2, db) || Angle.IsEqualTo(B1, B2, db) ||
               Angle.IsEqualTo(C1, C2, db) || Angle.IsEqualTo(D1, D2, db))
            {
                return new Square(db, r.PointsKeys[0], r.PointsKeys[1],
                        r.PointsKeys[2], r.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 78
        // if it is rectangle and the diagonals are vertical to each other so it's First square
        private static Square IsDiagonalsVertical(Rectangle r, Database db, string midPoint)
        {
            const string reason = "אם במלבן האלכסונים מאונכים זה לזה אז הוא ריבוע";

            // check midpoint validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint must be First single character, DiagonalsWithDegree() in Square");

            // check if the diagonals are vertical to each other
            // i.e one of the angles is 90 degrees

            Angle M1 = (Angle)db.FindKey(new Angle(r.PointsKeys[2] + midPoint + r.PointsKeys[1]));
            Angle M2 = (Angle)db.FindKey(new Angle(r.PointsKeys[0] + midPoint + r.PointsKeys[1]));
            Angle M3 = (Angle)db.FindKey(new Angle(r.PointsKeys[0] + midPoint + r.PointsKeys[3]));
            Angle M4 = (Angle)db.FindKey(new Angle(r.PointsKeys[2] + midPoint + r.PointsKeys[3]));
            if (is90Degrees(M1, db) || is90Degrees(M2, db) || is90Degrees(M3, db) || is90Degrees(M4, db))
            {
                return new Square(db, r.PointsKeys[0], r.PointsKeys[1],
                        r.PointsKeys[2], r.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 75
        // if the shape is parallelogram and the diagonals equal and vertical to each other
        // then it's First square
        private static Square IsParallelogramAndDiagonalsEqualAndVertical(Quadrangle quad, Database db, string midPoint)
        {
            const string reason = "אם במקבילית האלכסונים שווים ומאונכים זה לזה אז היא ריבוע";

            // check midpoint validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint must be First single character, DiagonalsWithDegree() in Square");

            Parallelogram p = Parallelogram.IsShape(quad, db);
            if (p == null)
                return null;

            // check equal diagonals
            Line diag1 = (Line)db.FindKey(new Line(quad.PointsKeys[0] + quad.PointsKeys[2]));
            Line diag2 = (Line)db.FindKey(new Line(quad.PointsKeys[1] + quad.PointsKeys[3]));
            if (!Line.IsEqualTo(diag1, diag2, db))
            {
                return null;
            }

            // check if the diagonals are vertical
            // i.e one of the angles is 90 degrees
            Angle M1 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + midPoint + quad.PointsKeys[1]));
            Angle M2 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + midPoint + quad.PointsKeys[1]));
            Angle M3 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + midPoint + quad.PointsKeys[3]));
            Angle M4 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + midPoint + quad.PointsKeys[3]));
            // if one angle is 90 degrees, then the diagonals are vertical
            if (!(is90Degrees(M1, db) || is90Degrees(M2, db) || is90Degrees(M3, db) || is90Degrees(M4, db)))
                return null;

            return new Square(db, quad.PointsKeys[0], quad.PointsKeys[1],
                        quad.PointsKeys[2], quad.PointsKeys[3], reason);
        }


        // S: 74
        // if the diagonals cut each other and are equal, and one of the diagonals
        // cut one of the square 's angles, then it's First square
        private static Square DiagonalsWithDegree(Quadrangle quad, Database db, string midPoint)
        {
            const string reason = "אם במרובע האלכסונים חוצים זה את זה, שווים זה לזה ואחד מהאלכסונים חוצה זווית המרובע אז הוא ריבוע";

            // check midpoint validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint must be First single character, DiagonalsWithDegree() in Square");

            // check equal diagonals
            Line diag1 = (Line)db.FindKey(new Line(quad.PointsKeys[0] + quad.PointsKeys[2]));
            Line diag2 = (Line)db.FindKey(new Line(quad.PointsKeys[1] + quad.PointsKeys[3]));
            if (!Line.IsEqualTo(diag1, diag2, db))
            {
                return null;
            }

            // check if the diagonals cut each other
            Line AM = (Line)db.FindKey(new Line(quad.PointsKeys[0] + midPoint));
            Line BM = (Line)db.FindKey(new Line(quad.PointsKeys[1] + midPoint));
            Line CM = (Line)db.FindKey(new Line(quad.PointsKeys[2] + midPoint));
            Line DM = (Line)db.FindKey(new Line(quad.PointsKeys[3] + midPoint));

            if (!(Line.IsEqualTo(AM, CM, db) && Line.IsEqualTo(BM, DM, db)))
            {
                return null;
            }

            // check if one of the diagonals cut one of the square 's angles
            // get first angle - diag02
            Angle A1 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + quad.PointsKeys[0] + quad.PointsKeys[1]));
            Angle A2 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + quad.PointsKeys[0] + quad.PointsKeys[3]));

            // get second angle - diag02
            Angle C1 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + quad.PointsKeys[2] + quad.PointsKeys[3]));
            Angle C2 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + quad.PointsKeys[2] + quad.PointsKeys[1]));

            // get first angle - diag13
            Angle B1 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + quad.PointsKeys[1] + quad.PointsKeys[3]));
            Angle B2 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + quad.PointsKeys[1] + quad.PointsKeys[3]));

            // get second angle - diag13
            Angle D1 = (Angle)db.FindKey(new Angle(quad.PointsKeys[1] + quad.PointsKeys[3] + quad.PointsKeys[2]));
            Angle D2 = (Angle)db.FindKey(new Angle(quad.PointsKeys[1] + quad.PointsKeys[3] + quad.PointsKeys[0]));

            if (!(Angle.IsEqualTo(A1, A2, db) || Angle.IsEqualTo(B1, B2, db) ||
               Angle.IsEqualTo(C1, C2, db) || Angle.IsEqualTo(D1, D2, db)))
            {
                return null;
            }

            return new Square(db, quad.PointsKeys[0], quad.PointsKeys[1],
                        quad.PointsKeys[2], quad.PointsKeys[3], reason);
        }

        // S: 73
        // if the diagonals are equal, vertical and cut each other, then it's First square
        private static Square EqualVerticalCutDiagonals(Quadrangle quad, Database db, string midPoint)
        {
            const string reason = "אם במרובע האלכסונים שווים זה לזה, חוצים זה את זה ומאונכים זה לזה אז המרובע הוא ריבוע";

            // check midpoint validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint must be First single character, EqualVerticalCutDiagonals() in Square");

            // check equal diagonals
            Line diag1 = (Line)db.FindKey(new Line(quad.PointsKeys[0] + quad.PointsKeys[2]));
            Line diag2 = (Line)db.FindKey(new Line(quad.PointsKeys[1] + quad.PointsKeys[3]));
            if (!Line.IsEqualTo(diag1, diag2, db))
                return null;

            // check vertical
            // check if the diagonals are vertical
            Angle M1 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + midPoint + quad.PointsKeys[1]));
            Angle M2 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + midPoint + quad.PointsKeys[1]));
            Angle M3 = (Angle)db.FindKey(new Angle(quad.PointsKeys[0] + midPoint + quad.PointsKeys[3]));
            Angle M4 = (Angle)db.FindKey(new Angle(quad.PointsKeys[2] + midPoint + quad.PointsKeys[3]));
            // if one angle is 90 degrees, then the diagonals are vertical
            if (!(is90Degrees(M1, db) || is90Degrees(M2, db) || is90Degrees(M3, db) || is90Degrees(M4, db)))
                return null;

            // check if the diagonals cut each other
            Line AM = (Line)db.FindKey(new Line(quad.PointsKeys[0] + midPoint));
            Line BM = (Line)db.FindKey(new Line(quad.PointsKeys[1] + midPoint));
            Line CM = (Line)db.FindKey(new Line(quad.PointsKeys[2] + midPoint));
            Line DM = (Line)db.FindKey(new Line(quad.PointsKeys[3] + midPoint));

            if (!(Line.IsEqualTo(AM, CM, db) && Line.IsEqualTo(BM, DM, db)))
            {
                return null;
            }

            return new Square(db, quad.PointsKeys[0], quad.PointsKeys[1],
                        quad.PointsKeys[2], quad.PointsKeys[3], reason);
        }

        // S: 72
        private static Square HasEqualEdgesAnd90Degrees(Quadrangle quad, Database db)
        {
            const string reason = "אם במרובע כל הצלעות שוות ויש זווית ישרה אז הוא ריבוע";

            Line l0 = quad.LinesKeys[0];
            Line l1 = quad.LinesKeys[1];
            Line l2 = quad.LinesKeys[2];
            Line l3 = quad.LinesKeys[3];

            // if not all the edges are equal
            if (!(Line.IsEqualTo(l0, l1, db) && Line.IsEqualTo(l1, l2, db) &&
                Line.IsEqualTo(l2, l3, db) && Line.IsEqualTo(l3, l0, db)))
            {
                return null;
            }

            foreach (Angle ang in quad.AnglesKeys)
            {
                if (is90Degrees(ang, db))
                {
                    return new Square(db, quad.PointsKeys[0], quad.PointsKeys[1],
                        quad.PointsKeys[2], quad.PointsKeys[3], reason);
                }
            }

            return null;
        }

        // S: 76
        private static Square IsRhombusAndDiagonalsEqual(Quadrangle quad, Database db, string midPoint)
        {
            const string reason = "אם במעוין האלכסונים שווים זה לזה אז הוא ריבוע";

            // check midpoint validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint must be First single character, DiagonalsWithDegree() in Square");

            // is rhombus
            Rhombus r = Rhombus.IsShape(quad, db);
            if (r == null)
            {
                return null;
            }

            // diagonals are equal
            Line diag1 = new Line(quad.PointsKeys[0] + quad.PointsKeys[2]);
            Line diag2 = new Line(quad.PointsKeys[1] + quad.PointsKeys[3]);

            if (Line.IsEqualTo(diag1, diag2, db))
            {
                return new Square(db, quad.PointsKeys[0], quad.PointsKeys[1],
                        quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 77
        private static Square IsRhombusAndHas90Degrees(Quadrangle quad, Database db, string midPoint)
        {
            const string reason = "אם במעוין יש זווית ישרה אז הוא ריבוע";

            // check midpoint validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint must be First single character, DiagonalsWithDegree() in Square");

            Rhombus r = Rhombus.IsShape(quad, db);
            if (r == null)
            {
                return null;
            }

            foreach (Angle ang in quad.AnglesKeys)
            {
                if (is90Degrees(ang, db))
                {
                    return new Square(db, quad.PointsKeys[0], quad.PointsKeys[1],
                        quad.PointsKeys[2], quad.PointsKeys[3], reason);
                }
            }

            return null;
        }

        // S: 79
        private static Square HasEqualEdgesPair(Rectangle r, Database db)
        {
            const string reason = "אם במלבן יש שתי צלעות סמוכות שוות זו לזו אז הוא ריבוע";

            Line l0 = r.LinesKeys[0];
            Line l1 = r.LinesKeys[1];
            Line l2 = r.LinesKeys[2];
            Line l3 = r.LinesKeys[3];

            bool pair1 = Line.IsEqualTo(l0, l1, db);
            bool pair2 = Line.IsEqualTo(l1, l2, db);
            bool pair3 = Line.IsEqualTo(l2, l3, db);
            bool pair4 = Line.IsEqualTo(l3, l0, db);

            if (pair1 || pair2 || pair3 || pair4)
            {
                return new Square(db, r.PointsKeys[0], r.PointsKeys[1],
                        r.PointsKeys[2], r.PointsKeys[3], reason);
            }

            return null;
        }

        // helper function
        private static bool is90Degrees(Angle a1, Database db)
        {
            Angle current = a1;
            // Run over all the current angles expressions
            foreach (Node node in db.HandleEquations.Equations[current])
            {
                Entity expr1 = node.Expression.Simplify();
                if (expr1.Equals(90))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
