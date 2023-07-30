using DatabaseLibrary;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Deltoid : Quadrangle
    {
        private IsoscelesTriangle _triangle1;
        private IsoscelesTriangle _triangle2;
        private string secondaryDiagonal;
        private string mainDiagonal;
        private const string name = "דלתון";


        // secondaryDiagonal is the diagonal that is the base of the two isosceles triangles, and
        // mainDiagonal is the main diagonal of the deltoid
        public Deltoid(Database db, string p1, string p2, string p3, string p4, string reason,
            string secondaryDiagonal) : base(db, p1, p2, p3, p4, reason)
        {
            MainNode.typeName = name;

            // check parameters validity
            CheckValidity(secondaryDiagonal);

            this.secondaryDiagonal = secondaryDiagonal;
            AddMainDiag();

            // advanced sentences
            // diag2CutHeadAngles() #26
            // MainDiagonalCutSecondaryDiagonalInHalf() #27
            // DiagonalsArePerpendicular() #28
        }

        protected override string GetTypeName()
        {
            return name;
        }

        private void AddMainDiag()
        {
            // get the points from PointsKeys that doesnt appear in the secondary diagonal
            // make First list of strings
            List<string> points = new List<string> { p0, p1, p2, p3 };
            points.Remove(secondaryDiagonal[0].ToString());
            points.Remove(secondaryDiagonal[1].ToString());

            mainDiagonal = points[0] + points[1];
        }

        public override void UpdateDiagonals(Line diag1, Line diag2, string cutP)
        {
            const string isoscelesTrianglesReason = "דלתון הוא מרובע המורכב משני משולשים שווי שוקיים";
            const string rightTriangleReason = "האלכסון הראשי בדלתון מאונך לאלכסון המשנה";

            diag1 = (Line)_db.FindKey(diag1);
            diag2 = (Line)_db.FindKey(diag2);
            // call the base method
            base.UpdateDiagonals(diag1, diag2, cutP);


            // Add the isosceles triangles
            AddIsoscelesTriangles(isoscelesTrianglesReason);

            // Add the right triangles
            AddRightTriangles(rightTriangleReason);

            // Add the sides triangles
            AddSidesTriangles();
        }

        // Comprehensive sentences
        private void AddSidesTriangles()
        {
            const string fact = "";

            // base
            string p0 = mainDiagonal[0].ToString();
            string p1 = mainDiagonal[1].ToString();

            // top
            string v1 = secondaryDiagonal[0].ToString();
            string v2 = secondaryDiagonal[1].ToString();

            // make First list of the shape and the diagonals nodes
            List<Node> parents = new List<Node> { MainNode, GetDiagonal(secondaryDiagonal) };

            // make two triangles
            Triangle t1 = new Triangle(_db, p0, p1, v1, fact);
            t1.AddParents(parents);

            Triangle t2 = new Triangle(_db, p0, p1, v2, fact);
            t2.AddParents(parents);
        }
        private void AddIsoscelesTriangles(string reason)
        {
            Line tBase = (Line)_db.FindKey(new Line(secondaryDiagonal[0].ToString() + secondaryDiagonal[1].ToString()));
            // Make isocles triangles
            string p1Base = tBase.PointsKeys[0];
            string p2Base = tBase.PointsKeys[1];
            string p3 = mainDiagonal[0].ToString();
            string p4 = mainDiagonal[1].ToString();


            // make First list of the shape and the diagonals nodes
            List<Node> parents = new List<Node> { this.GetMainNode(), _diagonalsLinesCut.GetMainNode() };

            // add triangles to db
            _triangle1 = new IsoscelesTriangle(_db, p1Base, p2Base, p3, tBase, reason);
            _triangle1.AddParents(parents);

            _triangle2 = new IsoscelesTriangle(_db, p1Base, p2Base, p4, tBase, reason);
            _triangle2.AddParents(parents);
        }

        private void AddRightTriangles(string reason)
        {
            // make First list of the shape and the diagonals nodes
            List<Node> parents = new List<Node> { this.GetMainNode(), _diagonalsLinesCut.GetMainNode() };

            // make right triangles
            Angle a1 = (Angle)_db.FindKey(new Angle(p0 + _cutP + p1));
            Triangle t1 = new RightTriangle(_db, p0, _cutP, p1, a1, reason);
            t1.AddParents(parents);

            Angle a2 = (Angle)_db.FindKey(new Angle(p1 + _cutP + p2));
            Triangle t2 = new RightTriangle(_db, p1, _cutP, p2, a2, reason);
            t2.AddParents(parents);

            Angle a3 = (Angle)_db.FindKey(new Angle(p2 + _cutP + p3));
            Triangle t3 = new RightTriangle(_db, p2, _cutP, p3, a3, reason);
            t3.AddParents(parents);

            Angle a4 = (Angle)_db.FindKey(new Angle(p3 + _cutP + p0));
            Triangle t4 = new RightTriangle(_db, p3, _cutP, p0, a4, reason);
            t4.AddParents(parents);
        }

        // END Comprehensive sentences

        public static Deltoid IsShape(Quadrangle quad, Database db)
        {
            Deltoid t = CheckShapeByDefinition(quad, db);
            if (t != null)
            {
                return t;
            }

            return null;
        }

        private static Deltoid CheckShapeByDefinition(Quadrangle quad, Database db)
        {
            Deltoid d = CheckShapeByDefinitionHelper(quad, db, quad.PointsKeys[0] + quad.PointsKeys[2]);
            if (d != null)
            {
                return d;
            }

            d = CheckShapeByDefinitionHelper(quad, db, quad.PointsKeys[1] + quad.PointsKeys[3]);
            if (d != null)
            {
                return d;
            }

            return null;
        }

        private static Deltoid CheckShapeByDefinitionHelper(Quadrangle quad, Database db, string SecondaryBase)
        {
            const string reason = "מרובע שבו שני זוגות של צלעות סמוכות שוות הוא דלתון";

            // check validity of the secondary base
            if (SecondaryBase == null || SecondaryBase.Length != 2 || SecondaryBase[0] == SecondaryBase[1])
                throw new Exception("The secondary base is not valid in CheckShapeByDefinition, Deltoid");

            // chec if secondary base contains two points of the quadrilateral
            if (!quad.PointsKeys.Contains(SecondaryBase[0].ToString()) ||
                !quad.PointsKeys.Contains(SecondaryBase[1].ToString()))
                throw new Exception("The secondary base does not contain two points from the quad in CheckShapeByDefinition, Deltoid");

            // get the points that are not in the secondary base
            List<string> otherPoints = new List<string>();
            foreach (string point in quad.PointsKeys)
            {
                if (!SecondaryBase.Contains(point))
                    otherPoints.Add(point);
            }

            // check that otherPoints contains two points
            if (otherPoints.Count != 2)
                throw new Exception("otherPoints does not contain two points in CheckShapeByDefinition, Deltoid");

            string p1 = otherPoints[0];
            string p2 = otherPoints[1];

            // get the lines of the first triangle
            Line AB = (Line)db.FindKey(new Line(p1 + SecondaryBase[0]));
            Line BC = (Line)db.FindKey(new Line(p1 + SecondaryBase[1]));

            Line CD = (Line)db.FindKey(new Line(p2 + SecondaryBase[0]));
            Line DA = (Line)db.FindKey(new Line(p2 + SecondaryBase[1]));

            // check if AB=BC and CD=DA
            if (!Line.IsEqualTo(AB, BC, db) || !Line.IsEqualTo(CD, DA, db))
                return null;

            Deltoid delt = new Deltoid(db, quad.PointsKeys[0], quad.PointsKeys[1], quad.PointsKeys[2], quad.PointsKeys[3],
                reason, SecondaryBase);

            return delt;
        }

        // S: 28
        // The diagonals of First deltoid are perpendicular
        public void DiagonalsArePerpendicular(string midPoint)
        {
            const string reason = "האלכסון הראשי בדלתון מאונך לאלכסון המשנה";

            // Get the nodes of the two diagonals
            Node diag1Node = GetDiagonal(mainDiagonal);
            Node diag2Node = GetDiagonal(secondaryDiagonal);
            // make First list of the two nodes
            List<Node> diagNodes = new List<Node>() { diag1Node, diag2Node };

            // check validity of the mid point
            if (midPoint == null || midPoint.Length != 1)
                throw new Exception("The mid point is not valid in DiagonalsArePerpendicular, Deltoid");

            // get the angles
            Angle E1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + midPoint + PointsKeys[1]));
            Angle E2 = (Angle)_db.FindKey(new Angle(PointsKeys[1] + midPoint + PointsKeys[2]));
            Angle E3 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + midPoint + PointsKeys[3]));
            Angle E4 = (Angle)_db.FindKey(new Angle(PointsKeys[3] + midPoint + PointsKeys[0]));

            // update that each angle is 90 degrees
            _db.Update(E1, new Node(E1.ToString(), 90, reason, diagNodes), DataType.Equations);
            _db.Update(E2, new Node(E2.ToString(), 90, reason, diagNodes), DataType.Equations);
            _db.Update(E3, new Node(E3.ToString(), 90, reason, diagNodes), DataType.Equations);
            _db.Update(E4, new Node(E4.ToString(), 90, reason, diagNodes), DataType.Equations);
        }

        // S: 27
        // The main diagonal cuts the secondary diagonal in half
        public void MainDiagonalCutSecondaryDiagonalInHalf(string midPoint)
        {
            const string reason = "האלכסון הראשי בדלתון חוצה את אלכסון המשנה";

            // Get the nodes of the two diagonals
            Node diag1Node = GetDiagonal(mainDiagonal);
            Node diag2Node = GetDiagonal(secondaryDiagonal);
            // make First list of the two nodes
            List<Node> diagNodes = new List<Node>() { diag1Node, diag2Node };

            // check validity of the mid point
            if (midPoint == null || midPoint.Length != 1)
                throw new Exception("The mid point is not valid in MainDiagonalCutSecondaryDiagonalInHalf, Deltoid");

            // get the first part of the secondary diagonal
            Line DE = (Line)_db.FindKey(new Line(midPoint + secondaryDiagonal[0]));
            // get the second part of the secondary diagonal
            Line EB = (Line)_db.FindKey(new Line(midPoint + secondaryDiagonal[1]));

            // update DE = EB
            _db.Update(DE, new Node(DE.ToString(), EB.variable, reason, diagNodes), DataType.Equations);
            _db.Update(EB, new Node(EB.ToString(), DE.variable, reason, diagNodes), DataType.Equations);

        }

        // S: 26
        // The main diagonal in Deltoid crosses the head angles
        public void diag2CutHeadAngles()
        {
            const string reason = "האלכסון הראשי בדלתון חוצה את זוויות הראש";

            // get the diag2 Node
            Node diag2Node = GetDiagonal(mainDiagonal);

            //_db.Update(A1, new Node(A1.ToString(), A2.variable, "האלכסונים ב" + GetTypeName() + " חוצים את זוויות ה" + GetTypeName(), diag02Node), DataType.Equations);

            // get the first head angle
            Angle A1 = (Angle)_db.FindKey(new Angle(mainDiagonal + secondaryDiagonal[0]));
            Angle A2 = (Angle)_db.FindKey(new Angle(mainDiagonal + secondaryDiagonal[1]));
            // update that the angles are equal
            _db.Update(A1, new Node(A1.ToString(), A2.variable, reason, diag2Node), DataType.Equations);
            _db.Update(A2, new Node(A2.ToString(), A1.variable, reason, diag2Node), DataType.Equations);

            // get the second head angle
            Angle B1 = (Angle)_db.FindKey(new Angle(secondaryDiagonal[0] + mainDiagonal));
            Angle B2 = (Angle)_db.FindKey(new Angle(secondaryDiagonal[1] + mainDiagonal));
            // update that the angles are equal
            _db.Update(B1, new Node(B1.ToString(), B2.variable, reason, diag2Node), DataType.Equations);
            _db.Update(B2, new Node(B2.ToString(), B1.variable, reason, diag2Node), DataType.Equations);
        }

        private void CheckValidity(string triangleBaseDiagonal)
        {
            // each diagonals has to be First line, i.e length 2
            if (triangleBaseDiagonal == null || triangleBaseDiagonal.Length != 2)
                throw new Exception("The diagonal " + triangleBaseDiagonal + " is not of length 2");

            // check if each diagonal has two different points
            if (triangleBaseDiagonal[0] == triangleBaseDiagonal[1])
                throw new Exception("The diagonal " + triangleBaseDiagonal + " has two identical points");

            // check that the points make the triangleBaseDiagonal and diag2
            if ((PointsKeys[0] + PointsKeys[2] != triangleBaseDiagonal) &&
                (PointsKeys[2] + PointsKeys[0] != triangleBaseDiagonal) &&
                (PointsKeys[1] + PointsKeys[3] != triangleBaseDiagonal) &&
                (PointsKeys[3] + PointsKeys[1] != triangleBaseDiagonal))
            {
                throw new Exception("The diagonal " + triangleBaseDiagonal + " is not in the deltoid");
            }
        }
    }
}