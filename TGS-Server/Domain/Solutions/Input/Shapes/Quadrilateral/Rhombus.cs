using AngouriMath;
using DatabaseLibrary;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Rhombus : Parallelogram
    {
        private const string name = "מעוין";
        public Rhombus(Database db, string p1, string p2, string p3, string p4, string reason) : base(db, p1, p2, p3, p4, reason)
        {
            MainNode.typeName = name;

            // Add basic sentences of rhombus
            AddBasicSentences();

            // advanced sentences
            // public void DiagonalsCutTheDegrees()
        }

        override protected string GetTypeName()
        {
            return name;
        }

        public override void UpdateDiagonals(Line diag1, Line diag2, string cutP)
        {
            diag1 = (Line)_db.FindKey(diag1);
            diag2 = (Line)_db.FindKey(diag2);
            // call the base method
            base.UpdateDiagonals(diag1, diag2, cutP);

            // Make the right triangles
            MakeRightTriangles();
        }

        private void MakeRightTriangles()
        {
            const string reason = "האלכסונים במעוין מאונכים זה לזה";

            // the the list of nodes
            List<Node> parents = new List<Node> { MainNode, _diagonalsLinesCut.GetMainNode() };

            // make the right triangles
            Angle a1 = (Angle)_db.FindKey(new Angle(p0 + _cutP + p3));
            Triangle t1 = new RightTriangle(_db, p0, _cutP, p3, a1, reason);
            t1.AddParents(parents);

            Angle a2 = (Angle)_db.FindKey(new Angle(p0 + _cutP + p1));
            Triangle t2 = new RightTriangle(_db, p0, _cutP, p1, a2, reason);
            t2.AddParents(parents);

            Angle a3 = (Angle)_db.FindKey(new Angle(p1 + _cutP + p2));
            Triangle t3 = new RightTriangle(_db, p1, _cutP, p2, a3, reason);
            t3.AddParents(parents);

            Angle a4 = (Angle)_db.FindKey(new Angle(p2 + _cutP + p3));
            Triangle t4 = new RightTriangle(_db, p2, _cutP, p3, a4, reason);
            t4.AddParents(parents);
        }

        // S: 61
        public void VerticalDiagonals(string midPoint)
        {
            string reason = "האלכסונים ב" + GetTypeName() + " מאונכים זה לזה";

            // check if the midPoint is valid
            if (midPoint == null || midPoint.Length != 1)
                throw new Exception("midPoint is not valid in VerticalDiagonals, rhombus class");

            Node diag02Node = GetDiagonal(PointsKeys[0] + PointsKeys[2]);
            Node diag13Node = GetDiagonal(PointsKeys[1] + PointsKeys[3]);
            // make First list of the two diagonals
            List<Node> diagonals = new List<Node>() { diag02Node, diag13Node };

            // update first angle to be 90 degrees
            Angle M1 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + midPoint + PointsKeys[1]));
            _db.Update(M1, new Node(M1.ToString(), 90, reason, diagonals), DataType.Equations);

            // update second angle to be 90 degrees
            Angle M2 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + midPoint + PointsKeys[1]));
            _db.Update(M2, new Node(M2.ToString(), 90, reason, diagonals), DataType.Equations);

            // update third angle to be 90 degrees
            Angle M3 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + midPoint + PointsKeys[3]));
            _db.Update(M3, new Node(M3.ToString(), 90, reason, diagonals), DataType.Equations);

            // update fourth angle to be 90 degrees
            Angle M4 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + midPoint + PointsKeys[3]));
            _db.Update(M4, new Node(M4.ToString(), 90, reason, diagonals), DataType.Equations);
        }

        // S: 60
        public void DiagonalsCutTheDegrees()
        {
            string reason = "האלכסונים ב" + GetTypeName() + " חוצים את זוויות ה" + GetTypeName();

            Node diag02Node = GetDiagonal(PointsKeys[0] + PointsKeys[2]);
            Node diag13Node = GetDiagonal(PointsKeys[1] + PointsKeys[3]);

            // update first angle - diag02
            Angle A1 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[0] + PointsKeys[1]));
            Angle A2 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[0] + PointsKeys[3]));
            _db.Update(A1, new Node(A1.ToString(), A2.variable, reason, diag02Node), DataType.Equations);
            _db.Update(A2, new Node(A2.ToString(), A1.variable, reason, diag02Node), DataType.Equations);

            // update second angle - diag02
            Angle C1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[2] + PointsKeys[3]));
            Angle C2 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[2] + PointsKeys[1]));
            _db.Update(C1, new Node(C1.ToString(), C2.variable, reason, diag02Node), DataType.Equations);
            _db.Update(C2, new Node(C2.ToString(), C1.variable, reason, diag02Node), DataType.Equations);

            // update first angle - diag13
            Angle B1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[1] + PointsKeys[3]));
            Angle B2 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[1] + PointsKeys[3]));
            _db.Update(B1, new Node(B1.ToString(), B2.variable, reason, diag13Node), DataType.Equations);
            _db.Update(B2, new Node(B2.ToString(), B1.variable, reason, diag13Node), DataType.Equations);

            // update second angle - diag13
            Angle D1 = (Angle)_db.FindKey(new Angle(PointsKeys[1] + PointsKeys[3] + PointsKeys[2]));
            Angle D2 = (Angle)_db.FindKey(new Angle(PointsKeys[1] + PointsKeys[3] + PointsKeys[0]));
            _db.Update(D1, new Node(D1.ToString(), D2.variable, reason, diag13Node), DataType.Equations);
            _db.Update(D2, new Node(D2.ToString(), D1.variable, reason, diag13Node), DataType.Equations);
        }

        private void AddBasicSentences()
        {
            string reason = "כל הצלעות ב" + GetTypeName() + " שוות זו לזו";
            // All the sides of the rhombus are equal to each other
            // first side
            _db.Update(LinesKeys[0],
                new Node(LinesKeys[0].ToString(), LinesKeys[1].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[0],
                new Node(LinesKeys[0].ToString(), LinesKeys[2].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[0],
                new Node(LinesKeys[0].ToString(), LinesKeys[3].variable,
                reason, MainNode),
                DataType.Equations);

            // second side
            _db.Update(LinesKeys[1],
                new Node(LinesKeys[1].ToString(), LinesKeys[0].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[1],
                new Node(LinesKeys[1].ToString(), LinesKeys[2].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[1],
                new Node(LinesKeys[1].ToString(), LinesKeys[3].variable,
                reason, MainNode),
                DataType.Equations);

            // third side
            _db.Update(LinesKeys[2],
                new Node(LinesKeys[2].ToString(), LinesKeys[0].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[2],
                new Node(LinesKeys[2].ToString(), LinesKeys[1].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[2],
                new Node(LinesKeys[2].ToString(), LinesKeys[3].variable,
                reason, MainNode),
                DataType.Equations);

            // fourth side
            _db.Update(LinesKeys[3],
                new Node(LinesKeys[3].ToString(), LinesKeys[0].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[3],
                new Node(LinesKeys[3].ToString(), LinesKeys[1].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(LinesKeys[3],
                new Node(LinesKeys[3].ToString(), LinesKeys[2].variable,
                reason, MainNode),
                DataType.Equations);
        }

        public static new Rhombus IsShape(Quadrangle quad, Database db)
        {
            Rhombus r = IsAllEdgesEqual(quad, db);
            if (r != null) return r;

            Parallelogram p = Parallelogram.IsShape(quad, db);
            if (p == null) return null;

            r = hasEqualAdjecentSides(p, db);
            if (r != null) return r; // #63

            r = IsVerticalDiagonals(p, db, quad.GetCutPoint());
            if (r != null) return r; // #64

            r = IsAnglesCutsEqual(p, db);
            if (r != null) return r; // #65

            return null;
        }

        // S:65
        // diagonals cut the angles in half
        private static Rhombus IsAnglesCutsEqual(Parallelogram p, Database db)
        {
            const string reason = "אם במקבילית האלכסונים חוצים את זוויות המקבילית אז המקבילית היא מעוין";

            // get first angle - diag02
            Angle A1 = (Angle)db.FindKey(new Angle(p.PointsKeys[2] + p.PointsKeys[0] + p.PointsKeys[1]));
            Angle A2 = (Angle)db.FindKey(new Angle(p.PointsKeys[2] + p.PointsKeys[0] + p.PointsKeys[3]));

            // get second angle - diag02
            Angle C1 = (Angle)db.FindKey(new Angle(p.PointsKeys[0] + p.PointsKeys[2] + p.PointsKeys[3]));
            Angle C2 = (Angle)db.FindKey(new Angle(p.PointsKeys[0] + p.PointsKeys[2] + p.PointsKeys[1]));

            // get first angle - diag13
            Angle B1 = (Angle)db.FindKey(new Angle(p.PointsKeys[0] + p.PointsKeys[1] + p.PointsKeys[3]));
            Angle B2 = (Angle)db.FindKey(new Angle(p.PointsKeys[2] + p.PointsKeys[1] + p.PointsKeys[3]));

            // get second angle - diag13
            Angle D1 = (Angle)db.FindKey(new Angle(p.PointsKeys[1] + p.PointsKeys[3] + p.PointsKeys[2]));
            Angle D2 = (Angle)db.FindKey(new Angle(p.PointsKeys[1] + p.PointsKeys[3] + p.PointsKeys[0]));

            if (Angle.IsEqualTo(A1, A2, db) && Angle.IsEqualTo(B1, B2, db) &&
               Angle.IsEqualTo(C1, C2, db) && Angle.IsEqualTo(D1, D2, db))
            {
                return new Rhombus(db, p.PointsKeys[0], p.PointsKeys[1], p.PointsKeys[2], p.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 64
        private static Rhombus IsVerticalDiagonals(Parallelogram p, Database db, string midPoint)
        {
            const string reason = "אם במקבילית האלכסונים מאונכים זה לזה אז המקבילית היא מעוין";

            // check validity
            if (midPoint == null)
                return null;
            if (midPoint.Length != 1)
                throw new Exception("midPoint is not First single char, IsVerticalDiagonals() in Rhombus");

            // check if the diagonals are vertical
            Angle M1 = (Angle)db.FindKey(new Angle(p.PointsKeys[2] + midPoint + p.PointsKeys[1]));
            Angle M2 = (Angle)db.FindKey(new Angle(p.PointsKeys[0] + midPoint + p.PointsKeys[1]));
            Angle M3 = (Angle)db.FindKey(new Angle(p.PointsKeys[0] + midPoint + p.PointsKeys[3]));
            Angle M4 = (Angle)db.FindKey(new Angle(p.PointsKeys[2] + midPoint + p.PointsKeys[3]));

            // check if all the angles are 90 degrees
            if (is90Degrees(M1, db) && is90Degrees(M2, db) && is90Degrees(M3, db) && is90Degrees(M4, db))
            {
                return new Rhombus(db, p.PointsKeys[0], p.PointsKeys[1], p.PointsKeys[2], p.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 62
        private static Rhombus IsAllEdgesEqual(Quadrangle quad, Database db)
        {
            const string reason = "אם במרובע כל צלעותיו שוות זו לזו אז המרובע הוא מעוין";

            Line l0 = quad.LinesKeys[0];
            Line l1 = quad.LinesKeys[1];
            Line l2 = quad.LinesKeys[2];
            Line l3 = quad.LinesKeys[3];

            if (Line.IsEqualTo(l0, l1, db) && Line.IsEqualTo(l1, l2, db) &&
                Line.IsEqualTo(l2, l3, db) && Line.IsEqualTo(l3, l0, db))
            {
                return new Rhombus(db, quad.PointsKeys[0], quad.PointsKeys[1], quad.PointsKeys[2], quad.PointsKeys[3], reason);
            }

            return null;
        }

        // S: 63
        private static Rhombus hasEqualAdjecentSides(Parallelogram p, Database db)
        {
            const string reason = "אם במקבילית יש שתי צלעות סמוכות שוות זו לזו אז המקבילית היא מעוין";

            // have to also be First parallelogram - checked once outside for all sentences
            Line l0 = p.LinesKeys[0];
            Line l1 = p.LinesKeys[1];
            Line l2 = p.LinesKeys[2];
            Line l3 = p.LinesKeys[3];

            bool pair1 = Line.IsEqualTo(l0, l1, db);
            bool pair2 = Line.IsEqualTo(l1, l2, db);
            bool pair3 = Line.IsEqualTo(l2, l3, db);
            bool pair4 = Line.IsEqualTo(l3, l0, db);

            if (pair1 || pair2 || pair3 || pair4)
            {
                return new Rhombus(db, p.PointsKeys[0], p.PointsKeys[1], p.PointsKeys[2], p.PointsKeys[3], reason);
            }

            return null;
        }

        // helper function
        private static bool is90Degrees(Angle a1, Database db)
        {
            Angle current = (Angle)db.FindKey(a1);
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
