using AngouriMath;
using DatabaseLibrary;


namespace Domain.Triangles
{
    public class EquilateralTriangle : Triangle
    {
        //Constractor
        public EquilateralTriangle(Database db, string p1, string p2, string p3, string reason) : base(db, p1, p2, p3, reason)
        {

            //Main Node
            MainNode.typeName = "משולש שווה צלעות";

            //Update all angles
            foreach (Angle angle in this.AnglesKeys)
            {
                db.Update(angle, new Node(angle.ToString(), 60,
                     "במשולש שווה צלעות כל הזוויות שוות (זו לזו) ל60 מעלות", MainNode), Database.DataType.Equations);
            }
            //Update all lines
            for (int i = 0; i < LinesKeys.Count; i++)
            {
                db.Update(LinesKeys[i], new Node(LinesKeys[i].ToString(), LinesKeys[(i + 1) % 3].variable,
                    "במשולש שווה צלעות כל הצלעות שוות זו לזו", MainNode), Database.DataType.Equations);
                db.Update(LinesKeys[i], new Node(LinesKeys[i].ToString(), LinesKeys[(i + 2) % 3].variable,
                   "במשולש שווה צלעות כל הצלעות שוות זו לזו", MainNode), Database.DataType.Equations);

            }
            //Add this shape to database
            db.AddShape(p1 + p2 + p3, this);
        }
        
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
            base.UpdateHeight(p1, p2, r, mainParent);
            string reason = "במשולש שווה שוקיים חוצה זווית הראש התיכון לבסיס והגובה לבסיס מתלכדים";
            base.UpdateMedian(p1, p2, reason, mainParent);
            base.UpdateAngleBisector(p1, p2, reason, mainParent);
        }
        public override void UpdateMedian(string p1, string p2, string r, Node mainParent)
        {
            base.UpdateMedian(p1, p2, r, mainParent);
            string reason = "במשולש שווה שוקיים חוצה זווית הראש התיכון לבסיס והגובה לבסיס מתלכדים";
            base.UpdateHeight(p1, p2, reason, mainParent);
            base.UpdateAngleBisector(p1, p2, reason, mainParent);
        }
        public override void UpdateAngleBisector(string p1, string p2, string r, Node mainParent)
        {
            base.UpdateAngleBisector(p1, p2, r, mainParent);
            string reason = "במשולש שווה שוקיים חוצה זווית הראש התיכון לבסיס והגובה לבסיס מתלכדים";
            base.UpdateHeight(p1, p2, reason, mainParent);
            base.UpdateMedian(p1, p2, reason, mainParent);
        }

        public static EquilateralTriangle IsShape(Triangle triangle, Database db)
        {
            EquilateralTriangle equilateralTriangle = db.GetListShape(triangle.ToString()).FirstOrDefault((t) => t is EquilateralTriangle) as EquilateralTriangle; ;
            if (equilateralTriangle != null)
            {
                triangle.CopyToChild(equilateralTriangle);
                return equilateralTriangle;

            }
            equilateralTriangle = CheckByLines(db, triangle);
            if (equilateralTriangle != null)
            {
                triangle.CopyToChild(equilateralTriangle);
                return equilateralTriangle;

            }

            equilateralTriangle =  CheckByAngles(db, triangle);
            if (equilateralTriangle != null)
            {
                triangle.CopyToChild(equilateralTriangle);
                return equilateralTriangle;

            }
            return null;
        }
        private static EquilateralTriangle CheckByLines(Database db, Triangle triangle)
        {
            List<Line> lines = triangle.LinesKeys;
            Line l1 = lines[0];
            Line l2 = lines[1];
            Line l3 = lines[2];

            Node line12 = db.GetEqualsNode(l1, l2);
            Node line13 = db.GetEqualsNode(l1, l3);
            if (line12 == null && line13 == null) return null;
 List<string> points = triangle.PointsKeys;
            if (line12 != null && line13 != null)
            {
               
                EquilateralTriangle newTriangle =
                    new EquilateralTriangle(db, points[0], points[1], points[2],
                    "משולש שכל צלעותיו שוות הוא משולש שווה צלעות");
                newTriangle.AddParents(new List<Node>() { line12, line13 });
                //Copy all properties
                triangle.CopyToChild(newTriangle);
                //return newTriangle;
                return newTriangle;

            }
            else // only one of them is null
            {
                Node line23 = db.GetEqualsNode(l2, l3);
                if (line23 == null) return null;
                EquilateralTriangle newTriangle =
                    new EquilateralTriangle(db, points[0], points[1], points[2],
                    "משולש שכל צלעותיו שוות הוא משולש שווה צלעות");
                newTriangle.AddParents(new List<Node>()
                {
                    line12==null? line13:line12,line23
                });
                //Copy all properties
                triangle.CopyToChild(newTriangle);
                //return newTriangle;
                return newTriangle;
            }
            
        }
        private static EquilateralTriangle CheckByAngles(Database db, Triangle triangle)
        {
            List<Angle> angles = triangle.AnglesKeys;
            Angle a1 = angles[0];
            Angle a2 = angles[1];
            Angle a3 = angles[2];

            Node angle12 = db.GetEqualsNode(a1, a2);
            Node angle13 = db.GetEqualsNode(a1, a3);
            if (angle12 == null && angle13 == null) return null;

            List<string> points = triangle.PointsKeys;
            if (angle12 != null && angle13 != null)
            {
                EquilateralTriangle newTriangle =
                    new EquilateralTriangle(db, points[0], points[1], points[2],
                    "משולש שבו כל הזוויות שוות הוא משולש שווה צלעות");
                newTriangle.AddParents(new List<Node>() { angle12, angle13 });
                //Copy all properties
                triangle.CopyToChild(newTriangle);
                //return newTriangle;
                return newTriangle;

            }
            else // only one of them is null
            {
                Node angle23 = db.GetEqualsNode(a2, a3);
                if (angle23 == null) return null;
                EquilateralTriangle newTriangle =
                    new EquilateralTriangle(db, points[0], points[1], points[2],
                    "משולש שכל צלעותיו שוות הוא משולש שווה צלעות");
                newTriangle.AddParents(new List<Node>()
                {
                    angle12==null?  angle13: angle12, angle23
                });
                //Copy all properties
                triangle.CopyToChild(newTriangle);
                //return newTriangle;
                return newTriangle;
            }

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
            EquilateralTriangle otherTriangle = (EquilateralTriangle)obj;
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

