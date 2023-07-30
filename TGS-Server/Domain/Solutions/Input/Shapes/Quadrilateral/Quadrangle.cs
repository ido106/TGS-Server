using DatabaseLibrary;
using Domain.Nodes;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class Quadrangle : Shape
    {
        protected Dictionary<string, Diagonal> Diagonals { get; set; } = null;
        protected string _cutP = null;
        protected TwoLinesCut _diagonalsLinesCut { get; set; } = null;
        public List<Line> Diag1Split { get; set; } = null;
        public List<Line> Diag2Split { get; set; } = null;
        private const string name = "מרובע";

        // for convenience
        protected string p0, p1, p2, p3;
        public Quadrangle(Database db, string p1, string p2, string p3, string p4, string reason) : base()
        {
            // init parameters and basic sentences
            Init(db, p1, p2, p3, p4, reason);

        }

        private void Init(Database db, string p1, string p2, string p3, string p4, string reason)
        {
            const string degreesReason = "סכום הזוויות הפנימיות במרובע שווה ל360°";
            //Shape name
            variable = p1 + p2 + p3 + p4;

            this.p0 = p1;
            this.p1 = p2;
            this.p2 = p3;
            this.p3 = p4;

            //Main Node
            MainNode = new Node(variable.ToString(), null, reason);
            MainNode.typeName = name;

            //Init variables
            PointsKeys = new List<string> { p1, p2, p3, p4 };
            LinesKeys = new List<Line> { (Line)db.FindKey(new Line(p1 + p2)), (Line)db.FindKey(new Line(p2 + p3)),
                (Line)db.FindKey(new Line(p3 + p4)), (Line)db.FindKey(new Line(p4 + p1)) };
            AnglesKeys = new List<Angle> { (Angle)db.FindKey(new Angle(p4 + p1 + p2)), (Angle)db.FindKey(new Angle(p1 + p2 + p3)),
                (Angle)db.FindKey(new Angle(p2 + p3 + p4)), (Angle) db.FindKey(new Angle(p3 + p4 + p1)), };
            _db = db;

            // Update angles
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(), 360 - AnglesKeys[1].variable - AnglesKeys[2].variable - AnglesKeys[3].variable,
                degreesReason, MainNode), DataType.Equations);

            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(), 360 - AnglesKeys[0].variable - AnglesKeys[2].variable - AnglesKeys[3].variable,
                degreesReason, MainNode), DataType.Equations);

            _db.Update(AnglesKeys[2],
                new Node(AnglesKeys[2].ToString(), 360 - AnglesKeys[0].variable - AnglesKeys[1].variable - AnglesKeys[3].variable,
                degreesReason, MainNode), DataType.Equations);

            _db.Update(AnglesKeys[3],
                new Node(AnglesKeys[3].ToString(), 360 - AnglesKeys[0].variable - AnglesKeys[1].variable - AnglesKeys[2].variable,
                degreesReason, MainNode), DataType.Equations);

            // Update lines
            foreach (Line line in LinesKeys)
            {
                _db.Update(line, null, DataType.Equations);
            }
        }

        protected virtual string GetTypeName()
        {
            return name;
        }



        public virtual void UpdateDiagonals(Line diag1, Line diag2, string cutP)
        {
            diag1 = (Line)_db.FindKey(diag1);
            diag2 = (Line)_db.FindKey(diag2);

            checkValidity(diag1, diag2, cutP);
            _cutP = cutP;

            // Add the diagonals to the diagonals dictionary
            addDiagonalsHelper(diag1, diag2);

            // add the two lines cut
            addTwoLinesCutHelper(diag1, diag2, cutP);

            // update mid angles
            updateMidAnglesHelper();

            // update splitted diagonals
            updateSplitDiagHelper(diag1, diag2, cutP);
        }

        private void updateSplitDiagHelper(Line diag1, Line diag2, string cutP)
        {
            // init the lists
            Diag1Split = new List<Line>();
            Diag2Split = new List<Line>();

            Diag1Split.Add((Line)_db.FindKey(new Line(diag1.PointsKeys[0] + cutP)));
            Diag1Split.Add((Line)_db.FindKey(new Line(diag1.PointsKeys[1] + cutP)));

            Diag2Split.Add((Line)_db.FindKey(new Line(diag2.PointsKeys[0] + cutP)));
            Diag2Split.Add((Line)_db.FindKey(new Line(diag2.PointsKeys[1] + cutP)));
        }

        private void updateMidAnglesHelper()
        {
            const string fact = "";

            List<Node> parents = new List<Node> { MainNode, _diagonalsLinesCut.GetMainNode() };

            // A1 A2
            Angle A1 = (Angle)_db.FindKey(new Angle(p1 + p0 + p2));
            Angle A1_match = (Angle)_db.FindKey(new Angle(p1 + p0 + _cutP));
            _db.Update(A1, new Node(A1.ToString(), A1_match.variable, fact, parents), DataType.Equations);
            _db.Update(A1_match, new Node(A1_match.ToString(), A1.variable, fact, parents), DataType.Equations);

            Angle A2 = (Angle)_db.FindKey(new Angle(PointsKeys[3] + PointsKeys[0] + PointsKeys[2]));
            Angle A2_match = (Angle)_db.FindKey(new Angle(PointsKeys[3] + PointsKeys[0] + _cutP));
            _db.Update(A2, new Node(A2.ToString(), A2_match.variable, fact, parents), DataType.Equations);
            _db.Update(A2_match, new Node(A2_match.ToString(), A2.variable, fact, parents), DataType.Equations);

            // B1 B2
            Angle B1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[1] + PointsKeys[3]));
            Angle B1_match = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[1] + _cutP));
            _db.Update(B1, new Node(B1.ToString(), B1_match.variable, fact, parents), DataType.Equations);
            _db.Update(B1_match, new Node(B1_match.ToString(), B1.variable, fact, parents), DataType.Equations);

            Angle B2 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[1] + PointsKeys[3]));
            Angle B2_match = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[1] + _cutP));
            _db.Update(B2, new Node(B2.ToString(), B2_match.variable, fact, parents), DataType.Equations);
            _db.Update(B2_match, new Node(B2_match.ToString(), B2.variable, fact, parents), DataType.Equations);

            // C1 C2
            Angle C1 = (Angle)_db.FindKey(new Angle(PointsKeys[1] + PointsKeys[2] + PointsKeys[0]));
            Angle C1_match = (Angle)_db.FindKey(new Angle(PointsKeys[1] + PointsKeys[2] + _cutP));
            _db.Update(C1, new Node(C1.ToString(), C1_match.variable, fact, parents), DataType.Equations);
            _db.Update(C1_match, new Node(C1_match.ToString(), C1.variable, fact, parents), DataType.Equations);

            Angle C2 = (Angle)_db.FindKey(new Angle(PointsKeys[3] + PointsKeys[2] + PointsKeys[0]));
            Angle C2_match = (Angle)_db.FindKey(new Angle(PointsKeys[3] + PointsKeys[2] + _cutP));
            _db.Update(C2, new Node(C2.ToString(), C2_match.variable, fact, parents), DataType.Equations);
            _db.Update(C2_match, new Node(C2_match.ToString(), C2.variable, fact, parents), DataType.Equations);

            // D1 D2
            Angle D1 = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[3] + PointsKeys[1]));
            Angle D1_match = (Angle)_db.FindKey(new Angle(PointsKeys[0] + PointsKeys[3] + _cutP));
            _db.Update(D1, new Node(D1.ToString(), D1_match.variable, fact, parents), DataType.Equations);
            _db.Update(D1_match, new Node(D1_match.ToString(), D1.variable, fact, parents), DataType.Equations);

            Angle D2 = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[3] + PointsKeys[1]));
            Angle D2_match = (Angle)_db.FindKey(new Angle(PointsKeys[2] + PointsKeys[3] + _cutP));
            _db.Update(D2, new Node(D2.ToString(), D2_match.variable, fact, parents), DataType.Equations);
            _db.Update(D2_match, new Node(D2_match.ToString(), D2.variable, fact, parents), DataType.Equations);
        }

        private void addTwoLinesCutHelper(Line diag1, Line diag2, string cutP)
        {
            string shapePoints = PointsKeys[0] + PointsKeys[1] + PointsKeys[2] + PointsKeys[3];
            // create the two lines cut
            string diagsReason = "הישרים" + diag1.ToString() + " ו-" + diag2.ToString() +
                "הם אלכסונים ב" + GetTypeName() + " " + shapePoints + "ונחתכים בנקודה " + cutP;

            _diagonalsLinesCut = new TwoLinesCut(_db, diag1, diag2, cutP, diagsReason);
            _diagonalsLinesCut.AddParent(MainNode);
        }

        private void addDiagonalsHelper(Line diag1, Line diag2)
        {
            if (Diagonals == null)
            {
                Diagonals = new Dictionary<string, Diagonal>();
            }

            string shapePoints = PointsKeys[0] + PointsKeys[1] + PointsKeys[2] + PointsKeys[3];

            string diag1Reason = "הישר " + diag1.ToString() + " הוא אלכסון ב" + GetTypeName() + " " + shapePoints;
            string diag2Reason = "הישר " + diag2.ToString() + " הוא אלכסון ב" + GetTypeName() + " " + shapePoints;

            // add diagonal if not exists
            if (!Diagonals.ContainsKey(diag1.ToString()))
            {
                Diagonals.Add(diag1.ToString(), new Diagonal(diag1.ToString(), diag1Reason));
            }

            // do the same with diag2
            if (!Diagonals.ContainsKey(diag2.ToString()))
            {

                Diagonals.Add(diag2.ToString(), new Diagonal(diag2.ToString(), diag2Reason));

            }
        }

        private void checkValidity(Line diag1, Line diag2, string cutP)
        {
            // check diag1, diag2, and cutPoint validty
            if (diag1 == null || diag2 == null || cutP == null || cutP.Length != 1)
            {
                throw new Exception($"invalid diagonals or cutP in Quadrangle");
            }

            string diag1p1 = diag1.PointsKeys[0];
            string diag1p2 = diag1.PointsKeys[1];
            string diag2p1 = diag2.PointsKeys[0];
            string diag2p2 = diag2.PointsKeys[1];

            // check the diag1 and diag2 points are in the shape
            if (!PointsKeys.Contains(diag1p1) || !PointsKeys.Contains(diag1p2) ||
                !PointsKeys.Contains(diag2p1) || !PointsKeys.Contains(diag2p2))
            {
                throw new Exception($"PointsKeys does not contains one or more diagonal point");
            }

            // check that all points are different
            // create First data structure that does not allow duplicates
            HashSet<string> points = new HashSet<string> { diag1p1, diag1p2, diag2p1, diag2p2 };
            // check that the size of the set is 4
            if (points.Count != 4)
            {
                throw new Exception($"one or more diagonal points are the same");
            }
        }


        public Diagonal GetDiagonal(string p)
        {
            if (Diagonals == null)
                throw new Exception($"Diagonals is null in quadrangle");

            if (p == null || p.Length != 2) return null;

            if (Diagonals.ContainsKey(p))
            {
                return Diagonals[p];
            }

            string reversedDiagonal = p[1].ToString() + p[0].ToString();
            if (Diagonals.ContainsKey(reversedDiagonal))
            {
                return Diagonals[reversedDiagonal];
            }
            return null;
        }

        public string GetCutPoint()
        {
            return _cutP;
        }
    }
}
