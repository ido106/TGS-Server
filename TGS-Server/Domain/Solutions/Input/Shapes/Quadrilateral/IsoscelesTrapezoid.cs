using DatabaseLibrary;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class IsoscelesTrapezoid : Trapezoid
    {
        private const string name = "טרפז שווה שוקיים";
        public IsoscelesTrapezoid(Database db, string p1, string p2, string p3, string p4, Line Base1, string reason) : base(db, p1, p2, p3, p4, Base1, reason)
        {
            MainNode.typeName = name;

            // Add basic sentences of isosceles trapezoid
            AddBasicSentces();

            // advanced sentences
            // EqualDiagonals() #83
            // OppositeAnglesEquals180() #84
        }

        private void AddBasicSentces()
        {
            string reason = "כל שתי זוויות בסיס ב " + GetTypeName() + " שוות זו לזו";

            // The angles of the bases in isosceles trapezoid are equal
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(), AnglesKeys[3].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[3],
                new Node(AnglesKeys[3].ToString(), AnglesKeys[0].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(), AnglesKeys[2].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(), AnglesKeys[1].variable,
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

            // Add isosceles triangles that created by the diagonals
            AddIsoscelesTiangleHelper();
        }

        private void AddIsoscelesTiangleHelper()
        {
            const string reason = "בטרפז שווה שוקיים האלכסונים שווים זה לזה";

            // create the first triangle - p1p2 and cutP
            Line baseSide1 = new Line(p1 + p2);
            Triangle t1 = new IsoscelesTriangle(_db, p1, p2, _cutP, baseSide1, reason);
            t1.AddParents(new List<Node> { MainNode, _diagonalsLinesCut.GetMainNode() });

            // create the second triangle - p3p0 and cutP
            Line baseSide2 = new Line(p3 + p0);
            Triangle t2 = new IsoscelesTriangle(_db, p3, p0, _cutP, baseSide2, reason);
            t2.AddParents(new List<Node> { MainNode, _diagonalsLinesCut.GetMainNode() });
        }

        // S:83
        // Equal diagonals
        public void EqualDiagonals()
        {
            const string reason = "האלכסונים בטרפז שווה שוקיים שווים זה לזה";

            // Get the nodes of the two diagonals
            Node diag1Node = GetDiagonal(PointsKeys[0] + PointsKeys[2]);
            Node diag2Node = GetDiagonal(PointsKeys[1] + PointsKeys[3]);
            // make First list of the two nodes
            List<Node> diagNodes = new List<Node>() { diag1Node, diag2Node };

            // get line of the diagonals
            Line diag1 = (Line)_db.FindKey(new Line(PointsKeys[0] + PointsKeys[2]));
            Line diag2 = (Line)_db.FindKey(new Line(PointsKeys[1] + PointsKeys[3]));

            // _db.Update(DE, new Node(DE.ToString(), EB.variable, "האלכסון הראשי בדלתון חוצה את אלכסון המשנה", diagNodes), DataType.Equations);
            // update the diagonals are equal to each other
            _db.Update(diag1, new Node(diag1.ToString(), diag2.variable, reason, diagNodes), DataType.Equations);
            _db.Update(diag2, new Node(diag2.ToString(), diag1.variable, reason, diagNodes), DataType.Equations);
        }

        // S: 84
        // The sum of any two opposite angles in an isosceles trapezoid equals 180 degrees
        public void OppositeAnglesEquals180()
        {
            string reason = "סכום כל שתי זוויות נגדיות ב " + GetTypeName() + " שוות ל180°";
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(), 180 - AnglesKeys[2].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(), 180 - AnglesKeys[0].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(), 180 - AnglesKeys[3].variable,
                reason, MainNode),
                DataType.Equations);

            _db.Update(AnglesKeys[3],
                new Node(AnglesKeys[3].ToString(), 180 - AnglesKeys[1].variable,
                reason, MainNode),
                DataType.Equations);
        }

        public static new IsoscelesTrapezoid IsShape(Quadrangle quad, Database db)
        {
            // if not trapezoid so false
            Trapezoid t = Trapezoid.IsShape(quad, db);
            if (t == null) return null;

            IsoscelesTrapezoid IsosTrapez = IsSidesEqual(t, db);
            if (IsosTrapez != null) return IsosTrapez; // By definition

            IsosTrapez = IsBaseAnglesAreEqual(t, db);
            if (IsosTrapez != null) return IsosTrapez; // #85

            IsosTrapez = IsEqualDiagonals(t, db);
            if (IsosTrapez != null) return IsosTrapez; // #86

            return null;
        }

        // Check by definition - if two sides are equal so it is an isosceles trapezoid
        private static IsoscelesTrapezoid IsSidesEqual(Trapezoid t, Database db)
        {
            const string reason = "טרפז שבו השוקיים שוות זו לזו הוא טרפז שווה שוקיים";

            Line side1 = (Line)db.FindKey(t.GetLeftSide());
            Line side2 = (Line)db.FindKey(t.GetRightSide());

            if (Line.IsEqualTo(side1, side2, db))
            {
                return new IsoscelesTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                    t.GetBase1(), reason);

            }

            return null;
        }

        // S: 85
        private static IsoscelesTrapezoid IsBaseAnglesAreEqual(Trapezoid t, Database db)
        {
            const string reason = "אם בטרפז זוויות שליד אחד הבסיסים שוות זו לזו אז הוא טרפז שווה שוקיים";

            Angle a0 = (Angle)db.FindKey(new Angle(t.AnglesKeys[0].ToString()));
            Angle a1 = (Angle)db.FindKey(new Angle(t.AnglesKeys[1].ToString()));
            Angle a2 = (Angle)db.FindKey(new Angle(t.AnglesKeys[2].ToString()));
            Angle a3 = (Angle)db.FindKey(new Angle(t.AnglesKeys[3].ToString()));

            if (Angle.IsEqualTo(a0, a3, db) || Angle.IsEqualTo(a1, a2, db))
            {
                return new IsoscelesTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                    t.GetBase1(), reason);
            }

            return null;
        }

        // S: 86
        private static IsoscelesTrapezoid IsEqualDiagonals(Trapezoid t, Database db)
        {
            const string reason = "אם בטרפז האלכסונים שווים זה לזה אז הוא טרפז שווה שוקיים";
            Line diag1 = (Line)db.FindKey(new Line(t.PointsKeys[0] + t.PointsKeys[2]));
            Line diag2 = (Line)db.FindKey(new Line(t.PointsKeys[1] + t.PointsKeys[3]));

            if (Line.IsEqualTo(diag1, diag2, db))
            {
                return new IsoscelesTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                                       t.GetBase1(), reason);
            }

            return null;
        }
    }
}
