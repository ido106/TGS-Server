using AngouriMath;
using DatabaseLibrary;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Rectangle : Parallelogram
    {
        private const string name = "מלבן";
        public Rectangle(Database db, string p1, string p2, string p3, string p4, string reason) : base(db, p1, p2, p3, p4, reason)
        {
            MainNode.typeName = name;

            // Add basic sentences of rectangle
            AddBasicSentences();

            // advanced sentences
            //UpdateDiagonals("נקודת אמצע");
        }

        private void AddBasicSentences()
        {
            string reason = "כל אחת מזווית ב" + GetTypeName() + " היא בת 90°";
            // Each of the angles of the rectangle is 90 degrees
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
            AddRightTriangles();

            // Add isosceles triangles
            AddIsoscelesTriangles();

            // Update Angles
            UpdateAngles();
        }

        // im not sure it is neccesary because of the ParallelLinesWithTransversal but doing it anyway
        // A1 = 90-A2 etc.
        private void UpdateAngles()
        {
            const string reason = "כל אחת מזוויות המלבן היא בת 90°";

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

        private void AddIsoscelesTriangles()
        {
            const string reason = "האלכסונים במלבן חוצים זה את זה ושווים זה לזה";

            List<Node> parents = new List<Node> { MainNode, _diagonalsLinesCut.GetMainNode() };

            // make first triangle
            Line t1_base = (Line)_db.FindKey(new Line(p0 + p1));
            Triangle t1 = new IsoscelesTriangle(_db, p0, p1, _cutP, t1_base, reason);
            t1.AddParents(parents);

            // make second triangle
            Line t2_base = (Line)_db.FindKey(new Line(p1 + p2));
            Triangle t2 = new IsoscelesTriangle(_db, p1, p2, _cutP, t2_base, reason);
            t2.AddParents(parents);

            // make third triangle
            Line t3_base = (Line)_db.FindKey(new Line(p2 + p3));
            Triangle t3 = new IsoscelesTriangle(_db, p2, p3, _cutP, t3_base, reason);
            t3.AddParents(parents);

            // make fourth triangle
            Line t4_base = (Line)_db.FindKey(new Line(p3 + p0));
            Triangle t4 = new IsoscelesTriangle(_db, p3, p0, _cutP, t4_base, reason);
            t4.AddParents(parents);
        }

        private void AddRightTriangles()
        {
            const string reason = "כל אחת מזוויות המלבן היא בת 90°";

            // make first triangle
            Angle a1 = (Angle)_db.FindKey(new Angle(p0 + p1 + p2));
            Triangle t1 = new RightTriangle(_db, p0, p1, p2, a1, reason);
            List<Node> t1_nodes = new List<Node> { MainNode, GetDiagonal(p0 + p2) };
            t1.AddParents(t1_nodes);

            // make second triangle
            Angle a2 = (Angle)_db.FindKey(new Angle(p1 + p2 + p3));
            Triangle t2 = new RightTriangle(_db, p1, p2, p3, a2, reason);
            List<Node> t2_nodes = new List<Node> { MainNode, GetDiagonal(p1 + p3) };
            t2.AddParents(t2_nodes);

            // make third triangle
            Angle a3 = (Angle)_db.FindKey(new Angle(p2 + p3 + p0));
            Triangle t3 = new RightTriangle(_db, p2, p3, p0, a3, reason);
            List<Node> t3_nodes = new List<Node> { MainNode, GetDiagonal(p2 + p0) };
            t3.AddParents(t3_nodes);

            // make fourth triangle
            Angle a4 = (Angle)_db.FindKey(new Angle(p3 + p0 + p1));
            Triangle t4 = new RightTriangle(_db, p3, p0, p1, a4, reason);
            List<Node> t4_nodes = new List<Node> { MainNode, GetDiagonal(p3 + p1) };
            t4.AddParents(t4_nodes);
        }

        // S:53
        // The diagonals of First rectangle are equal and cut each other in half
        public void UpdateDiagonals(string midPoint)
        {
            const string equalDiagonals = "האלכסונים במלבן שווים זה לזה";
            const string cutDiagonals = "האלכסונים במלבן חוצים זה את זה";

            Line diag1 = (Line)_db.FindKey(new Line(PointsKeys[0] + PointsKeys[2]));
            Line diag2 = (Line)_db.FindKey(new Line(PointsKeys[1] + PointsKeys[3]));

            // update diagonals are equal
            _db.Update(diag1, new Node(diag1.ToString(), diag2.variable, equalDiagonals, MainNode), DataType.Equations);
            _db.Update(diag2, new Node(diag2.ToString(), diag1.variable, equalDiagonals, MainNode), DataType.Equations);

            // update diagonals cut each other in half
            // get first part of the first diagonal
            Line l1 = (Line)_db.FindKey(new Line(PointsKeys[0] + midPoint));
            // get second part of the first diagonal
            Line l2 = (Line)_db.FindKey(new Line(midPoint + PointsKeys[2]));
            // get first part of the second diagonal
            Line l3 = (Line)_db.FindKey(new Line(PointsKeys[1] + midPoint));
            // get second part of the second diagonal
            Line l4 = (Line)_db.FindKey(new Line(midPoint + PointsKeys[3]));

            // add l1 equal l2 to db
            _db.Update(l1, new Node(l1.ToString(), l2.variable, cutDiagonals, MainNode), DataType.Equations);
            _db.Update(l2, new Node(l2.ToString(), l1.variable, cutDiagonals, MainNode), DataType.Equations);

            // add l3 equal l4 to db
            _db.Update(l3, new Node(l3.ToString(), l4.variable, cutDiagonals, MainNode), DataType.Equations);
            _db.Update(l4, new Node(l4.ToString(), l3.variable, cutDiagonals, MainNode), DataType.Equations);
        }

        public static new Rectangle IsShape(Quadrangle quad, Database db)
        {
            Parallelogram p = Parallelogram.IsShape(quad, db);
            if (p == null) return null;

            Rectangle r = Has90Degrees(p, db);
            if (r != null) return r;

            r = IsEqualDiagonals(p, db);
            if (r != null) return r;

            return null;
        }

        // S: 54
        private static Rectangle Has90Degrees(Parallelogram p, Database db)
        {
            const string reason = "אם במקבילית יש זווית ישרה אז המקבילית היא מלבן";

            // if one of the angles is 90 degrees, return true
            foreach (Angle angle in p.AnglesKeys)
            {
                if (is90Degrees(angle, db))
                {
                    return new Rectangle(db, p.PointsKeys[0], p.PointsKeys[1], p.PointsKeys[2], p.PointsKeys[3], reason);
                }
            }

            return null;
        }

        // S: 55
        private static Rectangle IsEqualDiagonals(Parallelogram p, Database db)
        {
            const string reason = "אם במקבילית אלכסונים שווים זה לזה אז המקבילית היא מלבן";

            Line diag1 = new Line(p.PointsKeys[0] + p.PointsKeys[2]);
            Line diag2 = new Line(p.PointsKeys[1] + p.PointsKeys[3]);

            if (Line.IsEqualTo(diag1, diag2, db))
            {
                return new Rectangle(db, p.PointsKeys[0], p.PointsKeys[1], p.PointsKeys[2], p.PointsKeys[3], reason);
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
