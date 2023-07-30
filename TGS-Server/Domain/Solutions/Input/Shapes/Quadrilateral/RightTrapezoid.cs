using AngouriMath;
using DatabaseLibrary;
using Domain.Triangles;
using static DatabaseLibrary.Database;

namespace Domain.Quadrilateral
{
    public class RightTrapezoid : Trapezoid
    {
        Angle _rightAngle1 = null;
        Angle _rightAngle2 = null;
        private const string name = "טרפז ישר זווית";

        public RightTrapezoid(Database db, string p1, string p2, string p3, string p4, string reason,
            Line Base1, Angle rightAngle1, Angle rightAngle2) :
            base(db, p1, p2, p3, p4, Base1, reason)
        {
            MainNode.typeName = name;

            // check validity
            CheckRightAnglesValidty(rightAngle1);

            _rightAngle1 = (Angle)_db.FindKey(rightAngle1);
            _rightAngle2 = (Angle)_db.FindKey(rightAngle2);
            //MakeRightAngle2(Base1);

            // Add basic sentences of right trapezoid
            AddBasicSentces();
        }

        override protected string GetTypeName()
        {
            return name;
        }

        private void AddBasicSentces()
        {
            const string reason = "טרפז ישר זווית הוא טרפז עם שתי זוויות השוות ל90° כל אחת";

            // Update right angles
            _db.Update(_rightAngle1, new Node(_rightAngle1.ToString(), 90, reason, MainNode), DataType.Equations);
            _db.Update(_rightAngle2, new Node(_rightAngle2.ToString(), 90, reason, MainNode), DataType.Equations);
        }

        private bool IsRightAngleInBase(Line Base1)
        {
            string a0 = GetAngleByLetter(Base1.PointsKeys[0]).ToString();
            string a1 = GetAngleByLetter(Base1.PointsKeys[1]).ToString();

            if (_rightAngle1.ToString() != a0 && _rightAngle1.ToString() != a1)
            {
                return false;
            }

            return true;
        }


        private void MakeRightAngle2(Line Base1)
        {
            if (!IsRightAngleInBase(Base1))
            {
                HandleNotInBaseHelper(Base1);
                return;
            }

            HandleInBaseHelper(Base1);
        }

        private void HandleInBaseHelper(Line Base1)
        {
            if (Base1.ToString() == LinesKeys[0].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[0].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[1]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[1].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[0]);
                }
            }
            else if (Base1.ToString() == LinesKeys[1].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[1].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[2]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[2].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[1]);
                }
            }
            else if (Base1.ToString() == LinesKeys[2].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[2].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[3]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[3].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[2]);
                }
            }
            else if (Base1.ToString() == LinesKeys[3].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[3].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[0]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[0].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[3]);
                }
            }
            throw new Exception("Illegal right base");
        }
        private void HandleNotInBaseHelper(Line Base1)

        {
            /**
            Angle baseFirstAng = GetAngleByLetter(Base1.PointsKeys[0]);
            Angle baseSecondAng = GetAngleByLetter(Base1.PointsKeys[1]);
            _rightAngle1 = (Angle)_db.FindKey(_rightAngle1);
            */

            if (Base1.ToString() == LinesKeys[0].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[2].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[1]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[3].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[0]);
                }
            }
            else if (Base1.ToString() == LinesKeys[1].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[3].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[2]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[0].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[1]);
                }
            }
            else if (Base1.ToString() == LinesKeys[2].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[1].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[2]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[0].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[3]);
                }
            }
            else if (Base1.ToString() == LinesKeys[3].ToString())
            {
                if (_rightAngle1.ToString() == AnglesKeys[1].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[0]);
                }
                else if (_rightAngle1.ToString() == AnglesKeys[2].ToString())
                {
                    _rightAngle2 = (Angle)_db.FindKey(AnglesKeys[3]);
                }
            }
            //throw new Exception("Illegal right base");
        }

        private Angle GetAngleByLetter(string letter)
        {
            string a0 = (AnglesKeys[0].ToString())[1].ToString();
            string a1 = (AnglesKeys[1].ToString())[1].ToString();
            string a2 = (AnglesKeys[2].ToString())[1].ToString();
            string a3 = (AnglesKeys[3].ToString())[1].ToString();

            if (a0 == letter)
            {
                return (Angle)_db.FindKey(AnglesKeys[0]);
            }
            else if (a1 == letter)
            {
                return (Angle)_db.FindKey(AnglesKeys[1]);
            }
            else if (a2 == letter)
            {
                return (Angle)_db.FindKey(AnglesKeys[2]);
            }
            else if (a3 == letter)
            {
                return (Angle)_db.FindKey(AnglesKeys[3]);
            }
            else
            {
                throw new Exception("There is no angle with " + letter + " as the mid char");
            }
        }

        private void CheckRightAnglesValidty(Angle rightAngle1)
        {
            rightAngle1 = (Angle)_db.FindKey(rightAngle1);

            if (!AnglesKeys.Contains(rightAngle1))
            {
                throw new Exception("The angle " + rightAngle1 + " is not in the trapezoid");
            }
        }

        public override void UpdateDiagonals(Line diag1, Line diag2, string cutP)
        {
            diag1 = (Line)_db.FindKey(diag1);
            diag2 = (Line)_db.FindKey(diag2);
            // call the base method
            base.UpdateDiagonals(diag1, diag2, cutP);

            // Add right triangles that created by the diagonals
            AddRightTriangleHelper();
        }

        private void AddRightTriangleHelper()
        {
            // Add right triangles that created by the diagonals
            const string reason = "בטרפז ישר זווית השוק מאונכת לבסיסים";

            // triangle 1
            string t1p1 = _rightAngle1.ToString()[0].ToString();
            string t1p2 = _rightAngle1.ToString()[1].ToString();
            string t1p3 = _rightAngle1.ToString()[2].ToString();
            Triangle t1 = new RightTriangle(_db, t1p1, t1p2, t1p3, _rightAngle1, reason);
            // create the node list for t1
            string diag1 = _rightAngle1.ToString()[0].ToString() + _rightAngle1.ToString()[2].ToString();
            List<Node> t1Nodes = new List<Node> { MainNode, GetDiagonal(diag1) };
            t1.AddParents(t1Nodes);

            // triangle 2
            string t2p1 = _rightAngle2.ToString()[0].ToString();
            string t2p2 = _rightAngle2.ToString()[1].ToString();
            string t2p3 = _rightAngle2.ToString()[2].ToString();
            Triangle t2 = new RightTriangle(_db, t2p1, t2p2, t2p3, _rightAngle2, reason);
            // create the node list for t2
            string diag2 = _rightAngle2.ToString()[0].ToString() + _rightAngle2.ToString()[2].ToString();
            List<Node> t2Nodes = new List<Node> { MainNode, GetDiagonal(diag2) };
            t2.AddParents(t2Nodes);
        }

        public static new RightTrapezoid IsShape(Quadrangle quad, Database db)
        {
            // if not trapezoid, return false
            Trapezoid t = Trapezoid.IsShape(quad, db);
            if (t == null) return null;

            // definition
            RightTrapezoid rt = Has90DegreeBaseAngle(t, db);
            if (rt != null) return rt;

            return null;
        }

        // Definition - there is no number in the list
        private static RightTrapezoid Has90DegreeBaseAngle(Trapezoid t, Database db)
        {
            const string reason = "טרפז ישר זווית הוא טרפז עם שתי זוויות השוות ל90° כל אחת";

            // check right angle (90 degrees)
            Angle a0 = t.AnglesKeys[0];
            Angle a1 = t.AnglesKeys[1];
            Angle a2 = t.AnglesKeys[2];
            Angle a3 = t.AnglesKeys[3];

            // if one pair of the angles is 90 degrees, return true
            if (is90Degrees(a0, db) && is90Degrees(a1, db))
            {
                return new RightTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                    reason, t.GetBase1(), a0, a1);
            }
            else if (is90Degrees(a1, db) && is90Degrees(a2, db))
            {
                return new RightTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                    reason, t.GetBase1(), a1, a2);
            }
            else if (is90Degrees(a2, db) && is90Degrees(a3, db))
            {
                return new RightTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                    reason, t.GetBase1(), a2, a3);
            }
            else if (is90Degrees(a3, db) && is90Degrees(a0, db))
            {
                return new RightTrapezoid(db, t.PointsKeys[0], t.PointsKeys[1], t.PointsKeys[2], t.PointsKeys[3],
                    reason, t.GetBase1(), a3, a0);
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
