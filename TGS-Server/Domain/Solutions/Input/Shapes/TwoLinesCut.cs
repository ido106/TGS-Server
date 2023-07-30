using AngouriMath;
using DatabaseLibrary;
using Domain.Triangles;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain
{
    public class TwoLinesCut : Shape
    {
        protected Line line1 { get; set; }
        protected Line line2 { get; set; }
        protected string cutP { get; set; }
        //Constractor 
        public TwoLinesCut(Database db, Line l1, Line l2, string cutP, string reason) : base()
        {
            //Init variables
            _db = db;
            MainNode = new Node($"{cutP} נחתכים בנקודה {l1.variable},{l2.variable} הישרים", null, reason);
            MainNode.typeName = "שני ישרים הנחתכים בנקודה";
            variable = l1.ToString() +cutP+ l2.ToString();
            line1 = l1;
            line2 = l2;
            this.cutP = cutP;
            PointsKeys = new List<string>() { l1.PointsKeys[0], l1.PointsKeys[1], l2.PointsKeys[0], l2.PointsKeys[1] };
            PointsKeys = PointsKeys.Distinct().ToList();
            LinesKeys = new List<Line>() {(Line)_db.FindKey(l1), (Line)_db.FindKey(l2) };
            foreach(Line l in LinesKeys)
            {
                _db.Update(l, null, DataType.Equations);
            }
            Angle a1, a2, a3, a4;
            AnglesKeys = new List<Angle>();
            try
            {
               a1 = (Angle)_db.FindKey(new Angle(l1.ToString()[0].ToString() + cutP + l2.ToString()[0].ToString()));
                AnglesKeys.Add(a1);
            }
            catch
            {
                a1 = null;
            }
            try
            {
                a2 = (Angle)_db.FindKey(new Angle(l1.ToString()[0].ToString() + cutP + l2.ToString()[1].ToString()));
                AnglesKeys.Add(a2);
            }
            catch
            {
                a2 = null;
            }
            try
            {
                a3 = (Angle)_db.FindKey(new Angle(l1.ToString()[1].ToString() + cutP + l2.ToString()[0].ToString()));
                AnglesKeys.Add(a3);
            }
            catch
            {
                a3 = null;
            }
            try
            {
                a4 = (Angle)_db.FindKey(new Angle(l1.ToString()[1].ToString() + cutP + l2.ToString()[1].ToString()));
                AnglesKeys.Add(a4);
            }
            catch
            {
                a4 = null;
            }
            Complete180(a1, a3);
            Complete180(a1, a2);
            Complete180(a2, a4);
            Complete180(a3, a4);
            vertical(a1, a4);
            vertical(a2, a3);
            Line part1, part2, part3, part4;
            try
            {
                part1 = (Line)_db.FindKey(new Line(l1.ToString()[0].ToString() + cutP));

            }
            catch
            {
                part1 = null;
            }
            try
            {
                part2 = (Line)_db.FindKey(new Line(l2.ToString()[0].ToString() + cutP));

            }
            catch
            {
                part2 = null;
            }
            try
            {
                part3 = (Line)_db.FindKey(new Line(cutP + l1.ToString()[1].ToString()));

            }
            catch
            {
                part3 = null;
            }
            try
            {
                part4 = (Line)_db.FindKey(new Line(cutP + l2.ToString()[1].ToString()));

            }
            catch
            {
                part4 = null;
            }
            if (part1 != null && part3 != null)
            {
                _db.Update(l1, new Node(LinesKeys[0].ToString(), part1.variable + part3.variable, "חיבור צלעות", MainNode), DataType.Equations);
                _db.Update(part1, new Node(part1.ToString(), LinesKeys[0].variable - part3.variable, "חיסור צלעות", MainNode), DataType.Equations);
                _db.Update(part3, new Node(part3.ToString(), LinesKeys[0].variable - part1.variable, "חיסור צלעות", MainNode), DataType.Equations);

            }
            if (part2 != null && part4 != null)
            {
                _db.Update(l2, new Node(LinesKeys[1].ToString(), part2.variable + part4.variable, "חיבור צלעות", MainNode), DataType.Equations);
                _db.Update(part2, new Node(part2.ToString(), LinesKeys[1].variable - part4.variable, "חיסור צלעות", MainNode), DataType.Equations);
                _db.Update(part4, new Node(part4.ToString(), LinesKeys[1].variable - part2.variable, "חיסור צלעות", MainNode), DataType.Equations);

            }
            try
            {
                new Triangle(db, l1.ToString()[0].ToString(), cutP, l2.ToString()[0].ToString(), "").AddParent(MainNode);

            }
            catch
            {

            }
            try
            {
                new Triangle(db, l1.ToString()[0].ToString(), cutP, l2.ToString()[1].ToString(), "").AddParent(MainNode);

            }
            catch
            {

            }
            try
            {
                new Triangle(db, l1.ToString()[1].ToString(), cutP, l2.ToString()[0].ToString(), "").AddParent(MainNode);

            }
            catch
            {

            }
            try
            {
                new Triangle(db, l1.ToString()[1].ToString(), cutP, l2.ToString()[1].ToString(), "").AddParent(MainNode);

            }
            catch
            {

            }

            /*string ab = l1.ToString();
           string ef = l2.ToString();

          if (ab.Intersect(ef).Any())
           {
               Triangle t = new Triangle(_db, PointsKeys[0], PointsKeys[1], PointsKeys[2],"");
               t.AddParent(MainNode);
           }
           else
           {
               //Update angles and lines
           UpdateAnglesAndLines(ab, ef);

           }

           //  */
           CreateAnglesFromPoints(l1, cutP);
           CreateAnglesFromPoints(l2, cutP);
         
            //Add this shape to database
            db.AddShape(variable.ToString() , this);
        }
        /*public TwoLinesCut(Database db,Angle a)
        {
            //Init variables
            _db = db;
            Line l1 = (Line)db.FindKey(new Line(a.ToString()[0].ToString() + a.ToString()[1].ToString()));
            Line l2 = (Line)db.FindKey(new Line(a.ToString()[1].ToString() + a.ToString()[2].ToString()));
            variable = l1.ToString() + l2.ToString();
            line1 = l1;
            line2 = l2;
            this.cutP = l1.ToString().First((c)=>l2.ToString().Contains(c)).ToString();
            PointsKeys = new List<string>() { l1.PointsKeys[0], l1.PointsKeys[1], l2.PointsKeys[0], l2.PointsKeys[1] };
            PointsKeys = PointsKeys.Distinct().ToList();
            LinesKeys = new List<Line>() { (Line)_db.FindKey(l1), (Line)_db.FindKey(l2) };
            
            MainNode = new Node($"{cutP} נחתכים בנקודה {l1.variable},{l2.variable} הישרים", null,"");
            MainNode.typeName = "שני ישרים הנחתכים בנקודה";

            //Add this shape to database
            db.AddShape(variable.ToString(), this);
        }*/
        //s:127  parallelLine = BD cannot be DB!
        public void UpdateThalesTheorem2()
        {
            if (line1.ToString().Contains(cutP) || line2.ToString().Contains(cutP)) return;
            Line ab = ((Line)_db.FindKey(new Line(line1.ToString()[0].ToString() + line2.ToString()[0])));
            Line de = ((Line)_db.FindKey(new Line(line2.ToString()[1].ToString() + line1.ToString()[1])));
            int index = -1;
            if (_db.ParallelLines.ContainsKey(ab))
            {
                string strDE = ((Line)_db.FindKey(de)).ToString();
                index = _db.ParallelLines[ab].FindIndex(n => n.Expression.ToString().Equals(strDE));
            }

            if (index >= 0)
            {
                List<Node> parents = new List<Node>() { MainNode, _db.ParallelLines[ab][index] };

                Line ac = (Line)_db.FindKey(new Line(ab.ToString()[0] + cutP));
                Line ce = (Line)_db.FindKey(new Line(cutP + de.ToString()[1]));
                Line bc = (Line)_db.FindKey(new Line(ab.ToString()[1] + cutP));
                Line cd = (Line)_db.FindKey(new Line(cutP + de.ToString()[0]));
                ab = (Line)_db.FindKey(ab);
                de = (Line)_db.FindKey(de);


                Entity mainE1 = $"({ac.variable}) / ({ce.variable}) = ({bc.variable}) / ({cd.variable})";
                Entity mainE2 = $"({ac.variable}) / ({ce.variable}) = ({ab.variable}) / ({de.variable})";
                Entity mainE3 = $"({ab.variable}) / ({de.variable}) = ({bc.variable}) / ({cd.variable})";


                Node n1 = new Node($"({ac.variable}) / ({ce.variable})", $"({bc.variable}) / ({cd.variable})", "הרחבה שנייה של משפט תלס", parents);
                Node n2 = new Node($"({ac.variable}) / ({ce.variable})", $"({ab.variable}) / ({de.variable})", "הרחבה שנייה של משפט תלס", parents);
                Node n3 = new Node($"({ab.variable}) / ({de.variable})", $"({bc.variable}) / ({cd.variable})", "הרחבה שנייה של משפט תלס", parents);


                _db.Update(ab, new Node(ab.ToString(), mainE2.Solve(ab.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n2), DataType.Equations);
                _db.Update(ab, new Node(ab.ToString(), mainE3.Solve(ab.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n3), DataType.Equations);

                _db.Update(de, new Node(de.ToString(), mainE2.Solve(de.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n2), DataType.Equations);
                _db.Update(de, new Node(de.ToString(), mainE3.Solve(de.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n3), DataType.Equations);

                _db.Update(ac, new Node(ac.ToString(), mainE1.Solve(ac.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n1), DataType.Equations);
                _db.Update(ac, new Node(ac.ToString(), mainE2.Solve(ac.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n2), DataType.Equations);

                _db.Update(ce, new Node(ce.ToString(), mainE1.Solve(ce.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n1), DataType.Equations);
                _db.Update(ce, new Node(ce.ToString(), mainE2.Solve(ce.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n2), DataType.Equations);

                _db.Update(bc, new Node(bc.ToString(), mainE3.Solve(bc.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n3), DataType.Equations);
                _db.Update(bc, new Node(bc.ToString(), mainE1.Solve(bc.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n1), DataType.Equations);

                _db.Update(cd, new Node(cd.ToString(), mainE3.Solve(cd.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n3), DataType.Equations);
                _db.Update(cd, new Node(cd.ToString(), mainE1.Solve(cd.variable), "חישוב לפי הרחבה שנייה של משפט תלס", n1), DataType.Equations);

                //Update because paralle:
                Angle abc = (Angle)_db.FindKey(new Angle(ab.ToString() + cutP));
                Angle cde = (Angle)_db.FindKey(new Angle(cutP + de.ToString()));
                AlternateInterior(abc, cde);
                Angle cab = (Angle)_db.FindKey(new Angle(cutP + ab.ToString()));
                Angle dec = (Angle)_db.FindKey(new Angle(de.ToString() + cutP));
                AlternateInterior(cab, dec);

            }
        }


        //Private: 
        //s:1
        private void Complete180(Angle a1, Angle a2)
        {
            if (a1 != null && a2 != null)
            {
            _db.Update(a1, new Node(a1.ToString(), 180 - a2.variable, "הסכום של שתי זוויות צמודות הוא 180", MainNode), DataType.Equations);
            _db.Update(a2, new Node(a2.ToString(), 180 - a1.variable, "הסכום של שתי זוויות צמודות הוא 180", MainNode), DataType.Equations);

            }
        }
        //s:2
        private void vertical(Angle a1, Angle a2)
        {
            if (a1 != null && a2 != null)
            {
                List<Node> parents = new List<Node>() { MainNode };
                _db.UpdateInputsEqual(a1, a2,
                    "כל שתי זוויות קדקודיות בעלי קדקוד משותף שוות זו לזו",
                    parents, parents);
            }
        }
        private void AlternateInterior(Angle a1, Angle a2)
        {
            List<Node> parents = new List<Node>() { MainNode };
            _db.UpdateInputsEqual(a1, a2,
                "זוויות מתחלפות שוות זו לזו",
                parents, parents);

        }
        public void CreateAnglesFromPoints(Line l1, string p1)
        {
            List<Shape> filteredShapes1 = _db.GetShapes()
                .Where(pair => l1.ToString().All(c => pair.Key.Contains(c)))
                .SelectMany(pair => pair.Value)
                .ToList();

            HashSet<string> pointsSet = new HashSet<string>();

            Dictionary<string, Shape> pointShapeDict = new Dictionary<string, Shape>();

            foreach (Shape shape in filteredShapes1)
            {
                pointsSet.UnionWith(shape.PointsKeys);

                foreach (string point in shape.PointsKeys)
                {
                    if (!pointShapeDict.ContainsKey(point))
                    {
                        pointShapeDict[point] = shape;
                    }
                }
            }

            pointsSet.ExceptWith(l1.PointsKeys);
            pointsSet.Remove(p1);

            foreach (string point in pointsSet)
            {

                List<Node> parents = new List<Node>() { this.MainNode, pointShapeDict[point].MainNode };
                string r = "אותה הזווית";
                if (!p1.Equals(l1.PointsKeys[1]) && l1.PointsKeys[0] != p1)
                {
                    Angle angle1 = (Angle)_db.FindKey(new Angle(l1.PointsKeys[0] + l1.PointsKeys[1] + point));
                    Angle angle2 = (Angle)_db.FindKey(new Angle(p1 + l1.PointsKeys[1] + point));

                    _db.Update(angle1, new Node(angle1.ToString(), angle2.variable, r, parents), Database.DataType.Equations);
                    _db.Update(angle2, new Node(angle2.ToString(), angle1.variable, r, parents), Database.DataType.Equations);
                }
                if (!p1.Equals(l1.PointsKeys[0]) && l1.PointsKeys[1] != p1)
                {
                    Angle angle3 = (Angle)_db.FindKey(new Angle(l1.PointsKeys[1] + l1.PointsKeys[0] + point));
                    Angle angle4 = (Angle)_db.FindKey(new Angle(p1 + l1.PointsKeys[0] + point));

                    _db.Update(angle3, new Node(angle3.ToString(), angle4.variable, r, parents), Database.DataType.Equations);
                    _db.Update(angle4, new Node(angle4.ToString(), angle3.variable, r, parents), Database.DataType.Equations);
                }

            }
        }
        public string GetCutPoint()
        {
            return cutP;
        }
        public Line GetLine1()
        {
            return line1;
        }
        public Line GetLine2()
        {
            return line2;
        }
        /*
        private void HandleLines(string line)
        {
            Line mainLine = (Line) _db.FindKey(new Line(line));
            Line part1 = (Line)_db.FindKey(new Line(line[0] + cutP));
            Line part2 = (Line)_db.FindKey(new Line(cutP + line[1]));
            _db.Update(mainLine, new Node(line, part1.variable + part2.variable, "חיבור צלעות", MainNode), DataType.Equations);
            _db.Update(part1, new Node(line, mainLine.variable - part2.variable, "חיסור צלעות", MainNode), DataType.Equations);
            _db.Update(part2, new Node(line, mainLine.variable - part1.variable, "חיסור צלעות", MainNode), DataType.Equations);
        }
        private void HandleAngle(string angle1, string angle2, string mainAngle)
        {
            Angle abf = (Angle)_db.FindKey(new Angle(angle1[0].ToString() + angle1[1].ToString() + angle1[2].ToString()));
            Angle pbf = (Angle)_db.FindKey(new Angle(cutP + angle1[1].ToString() + angle1[2].ToString()));

            Angle abe = (Angle)_db.FindKey(new Angle(angle2[0].ToString() + angle2[1].ToString() + angle2[2].ToString()));
            Angle pbe = (Angle)_db.FindKey(new Angle(cutP + angle2[1].ToString() + angle2[2].ToString()));

            Angle fbe = (Angle)_db.FindKey(new Angle(mainAngle[0].ToString() + mainAngle[1].ToString() + mainAngle[2].ToString()));

            _db.Update(abf, new Node(abf.ToString(), fbe.variable - abe.variable, "חיסור זוויות", MainNode), DataType.Equations);
            _db.Update(abf, new Node(abf.ToString(), fbe.variable - pbe.variable, "חיסור זוויות", MainNode), DataType.Equations);

            _db.Update(pbf, new Node(pbf.ToString(), fbe.variable - abe.variable, "חיסור זוויות", MainNode), DataType.Equations);
            _db.Update(pbf, new Node(pbf.ToString(), fbe.variable - pbe.variable, "חיסור זוויות", MainNode), DataType.Equations);

            _db.Update(abe, new Node(abe.ToString(), fbe.variable - abf.variable, "חיסור זוויות", MainNode), DataType.Equations);
            _db.Update(abe, new Node(abe.ToString(), fbe.variable - pbf.variable, "חיסור זוויות", MainNode), DataType.Equations);

            _db.Update(pbe, new Node(pbe.ToString(), fbe.variable - abf.variable, "חיסור זוויות", MainNode), DataType.Equations);
            _db.Update(pbe, new Node(pbe.ToString(), fbe.variable - pbf.variable, "חיסור זוויות", MainNode), DataType.Equations);

            _db.Update(fbe, new Node(fbe.ToString(), abf.variable + abe.variable, "חיבור זוויות", MainNode), DataType.Equations);
            _db.Update(fbe, new Node(fbe.ToString(), abf.variable + pbe.variable, "חיבור זוויות", MainNode), DataType.Equations);

            _db.Update(fbe, new Node(fbe.ToString(), pbf.variable + abe.variable, "חיבור זוויות", MainNode), DataType.Equations);
            _db.Update(fbe, new Node(fbe.ToString(), pbf.variable + pbe.variable, "חיבור זוויות", MainNode), DataType.Equations);

        }
        private void UpdateAnglesAndLines(string ab, string ef)
        {
            
            Angle agf = (Angle)_db.FindKey(new Angle(ab[0] + cutP + ef[1]));
            Angle age = (Angle)_db.FindKey(new Angle(ab[0] + cutP + ef[0]));
            Angle bge = (Angle)_db.FindKey(new Angle(ab[1] + cutP + ef[0]));
            Angle bgf = (Angle)_db.FindKey(new Angle(ab[1] + cutP + ef[1]));

            if (cutP[0] == ab[0])
            {
                Complete180(bge, bgf);
                HandleLines(ef);
                new Triangle(_db, cutP, ab[1].ToString(), ef[0].ToString(), "");
                new Triangle(_db, cutP, ab[1].ToString(), ef[1].ToString(), "");
                new Triangle(_db, ef[0].ToString(), ab[1].ToString(), ef[1].ToString(), "");
            }
            else if (cutP[0] == ab[1])
            {
                Complete180(agf, age);
                HandleLines(ef);
                new Triangle(_db, ab[0].ToString(), cutP, ef[0].ToString(), "");
                new Triangle(_db, ab[0].ToString(), cutP, ef[1].ToString(), "");
                new Triangle(_db, ef[0].ToString(), ab[0].ToString(), ef[1].ToString(), "");
            }
            else if (cutP[0] == ef[0])
            {
                Complete180(agf, bgf);
                HandleLines(ab);
                new Triangle(_db, cutP, ef[1].ToString(), ab[0].ToString(), "");
                new Triangle(_db, cutP, ef[1].ToString(), ab[1].ToString(), "");
                new Triangle(_db, ef[1].ToString(), ab[1].ToString(), ab[0].ToString(), "");
            }
            else if (cutP[0] == ef[1])
            {
                Complete180(age, bge);
                HandleLines(ab);
                new Triangle(_db, ef[0].ToString(), cutP, ab[0].ToString(), "");
                new Triangle(_db, ef[0].ToString(), cutP, ab[1].ToString(), "");
                new Triangle(_db, ef[0].ToString(), ab[1].ToString(), ab[0].ToString(), "");
            }
            else
            {
                Complete180(bge, bgf);
                Complete180(agf, age);
                Complete180(agf, bgf);
                Complete180(age, bge);
                vertical(age, bgf);
                vertical(agf, bge);
                HandleLines(ab);
                HandleLines(ef);
                string a = ab[0].ToString();
                string b = ab[1].ToString();
                string e = ef[0].ToString();
                string f = ef[1].ToString();

                new Triangle(_db, a, b, e, "").AddParent(MainNode);
                new Triangle(_db, a, b, f, "").AddParent(MainNode);

                new Triangle(_db, e, f, a, "").AddParent(MainNode);
                new Triangle(_db, e, f, b, "").AddParent(MainNode);

                new Triangle(_db, e, cutP, a, "").AddParent(MainNode);
                new Triangle(_db, e, cutP, b, "").AddParent(MainNode);
                new Triangle(_db, f, cutP, a, "").AddParent(MainNode);
                new Triangle(_db, f, cutP, b, "").AddParent(MainNode);

                //Add and sub
                HandleAngle(a + b + f, a + b + e, f + b + e);
                HandleAngle(f + e + b, f + e + a, b + e + a);
                HandleAngle(b + a + e, b + a + f, e + a + f);
                HandleAngle(e + f + a, e + f + b, a + f + b);
            }
        }
       
       
        */
    }

}
