using AngouriMath;
using Antlr4.Runtime.Misc;
using DatabaseLibrary;
using Domain.Lines;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain.Triangles
{
    public class Triangle : Shape
    {
        //Variables:

        public Dictionary<string, Height> Heights;
        public Dictionary<string, Median> Medians;
        public Dictionary<string, AngleBisector> AngleBisectors;
        //public Dictionary<Pair<Line,Line>, MeansSection> MeansSections { get; private set; } = null;


        //Constractor:
        public Triangle(Database db, string p1, string p2, string p3, string reason,bool isRightTriangle=false) : base()
        {
            //Database
            _db = db;
            //Shape name
            variable = p1 + p2 + p3;

            //Main Node
            MainNode = new Node(variable.ToString(), null, reason);
            MainNode.typeName = "משולש";


            //Init variables
            PointsKeys = new List<string> { p1, p2, p3 };
            LinesKeys = new List<Line> { (Line)db.FindKey(new Line(p1 + p2)), (Line)db.FindKey(new Line(p2 + p3)), (Line)db.FindKey(new Line(p3 + p1)) };
            AnglesKeys = new List<Angle> { (Angle)db.FindKey(new Angle(p3 + p1 + p2)), (Angle)db.FindKey(new Angle(p1 + p2 + p3)), (Angle)db.FindKey(new Angle(p2 + p3 + p1)) };

            //Create Triangle by TwoLineCut
            // new TwoLinesCut(db, AnglesKeys[0]);
            //new TwoLinesCut(db, AnglesKeys[1]);
            //new TwoLinesCut(db, AnglesKeys[2]);


            //Update angles
            _db.Update(AnglesKeys[0],
                new Node(AnglesKeys[0].ToString(), 180 - AnglesKeys[1].variable - AnglesKeys[2].variable,
                "סכום זוויות במשולש הוא 180", MainNode), DataType.Equations);
            _db.Update(AnglesKeys[1],
                new Node(AnglesKeys[1].ToString(), 180 - AnglesKeys[0].variable - AnglesKeys[2].variable,
                "סכום זוויות במשולש הוא 180", MainNode), DataType.Equations);
            _db.Update(AnglesKeys[2],
               new Node(AnglesKeys[2].ToString(), 180 - AnglesKeys[0].variable - AnglesKeys[1].variable,
                "סכום זוויות במשולש הוא 180", MainNode), DataType.Equations);

            //Update lines
            int len = LinesKeys.Count;
            for (int i = 0; i < len; ++i)
            {
                _db.Update(LinesKeys[i], null, DataType.Equations);
                _db.Update(LinesKeys[i], new Node(LinesKeys[i].ToString(),
                    LinesKeys[i].variable < LinesKeys[(i + 1) % 3].variable + LinesKeys[(i + 2) % 3].variable,
                    "סכום כל שתי צלעות במשולש גדול מהצלע השלישית", MainNode), DataType.Inequalities);
            }
            //Update Height, Medians, AngleBisectors
            Heights = new Dictionary<string, Height>();
            Medians = new Dictionary<string, Median>();
            AngleBisectors = new Dictionary<string, AngleBisector>();
            foreach (string p in PointsKeys)
            {
                Heights.TryAdd(p, null);
                Medians.TryAdd(p, null);
                AngleBisectors.TryAdd(p, null);
            }

            if (_db.IsTrigo && !isRightTriangle)
            {
                //Law of cosines
                LawOfCosines(LinesKeys[0], LinesKeys[1].variable, LinesKeys[2].variable, AnglesKeys[2]);
                LawOfCosines(LinesKeys[1], LinesKeys[0].variable, LinesKeys[2].variable, AnglesKeys[0]);
                LawOfCosines(LinesKeys[2], LinesKeys[0].variable, LinesKeys[1].variable, AnglesKeys[1]);

                //Law of sines
                LawOfSines(LinesKeys[0], LinesKeys[1], AnglesKeys[2], AnglesKeys[0]);
                LawOfSines(LinesKeys[0], LinesKeys[2], AnglesKeys[2], AnglesKeys[1]);
                LawOfSines(LinesKeys[1], LinesKeys[2], AnglesKeys[0], AnglesKeys[1]);

            }

            //Add this shape to database
            db.AddShape(p1 + p2 + p3, this);

        }
        //Public:
        public virtual void UpdateHeight(string p1, string p2, string reason, Node mainParent)
        {
            if (Heights.ContainsKey(p1))
            {
                if (Heights[p1] != null) return;
            }
            else if (Heights.ContainsKey(p2))
            {
                if (Heights[p2] != null) return;
            }
            if (PointsKeys.Contains(p1) && !PointsKeys.Contains(p2))
            {
                Heights[p1] = new Height(p1 + p2, reason);
                handleUpdateHeight(p1, p2, reason, mainParent);

            }

            else if (PointsKeys.Contains(p2) && !PointsKeys.Contains(p1))
            {
                Heights[p2] = new Height(p1 + p2, reason);
                handleUpdateHeight(p2, p1, reason, mainParent);

            }
            else if (PointsKeys.Contains(p2) && PointsKeys.Contains(p1))
            {
                Line h = (Line)_db.FindKey(new Line(p1 + p2));
                Heights[p1] = new Height(h.ToString(), reason, MainNode);
                Angle rightAngle = AnglesKeys.Find(a => a.ToString()[1].ToString() == p2);
                string r = "גובה מאונך לצלע";
                _db.Update(rightAngle, new Node(rightAngle.ToString(), "90", r, Heights[p1]), DataType.Equations);
            }
            else
            {
                throw new Exception($"wrong height point in newTriangle");
            }
        }
        public virtual void UpdateMedian(string p1, string p2, string reason, Node mainParent)
        {
            if (PointsKeys.Contains(p1) && !PointsKeys.Contains(p2))
            {

                Medians[p1]= new Median(p1 + p2, reason, mainParent);
                HandleUpdateMedian(p1, p2, reason, mainParent);

            }

            else if (PointsKeys.Contains(p2) && !PointsKeys.Contains(p1))
            {
                Medians[p2]= new Median(p1 + p2, reason, mainParent);
                HandleUpdateMedian(p2, p1, reason, mainParent);

            }
            else
            {
                throw new Exception($"wrong median point p1:{p1} , p2:{p2}");
            }

        }
        public virtual void UpdateAngleBisector(string p1, string p2, string reason, Node mainParent)
        {
            if (PointsKeys.Contains(p1) && !PointsKeys.Contains(p2))
            {
                AngleBisectors[p1]= new AngleBisector(p1 + p2, reason);
                HandleUpdateAngleBisector(p1, p2, reason, mainParent);
                
            }

            else if (PointsKeys.Contains(p2) && !PointsKeys.Contains(p1))
            {
                AngleBisectors[p2]= new AngleBisector(p1 + p2, reason);
                HandleUpdateAngleBisector(p2, p1, reason, mainParent);
            }
            else
            {
                throw new Exception("wrong angle bisector point");
            }

        }


        public virtual void UpdatePerpendicularBisector(string p90, Line p1Line, string p2, Line p2Line, string reason)
        {
            if (!LinesKeys.Contains(p1Line) || !LinesKeys.Contains(p2Line))
            {
                throw new Exception("lines not exist in triangle");
            }
            PerpendicularBisector perpendicularBisector = new PerpendicularBisector(p90 + p2, reason, MainNode);

            string commonDot = PointsKeys.Find(p => p1Line.ToString().Contains(p) && p2Line.ToString().Contains(p));

            //Create new Triangle
            RightTriangle newT = new RightTriangle(_db, p90, commonDot, p2, new Angle(commonDot + p90 + p2), "לפי הגדרת אנך אמצעי");
            newT.MainNode.Parents.Add(perpendicularBisector);

            //Update angles
            Angle a1 = (Angle)_db.FindKey(new Angle(commonDot + p90 + p2));
            Angle a2 = new Angle(p2 + p90 + p1Line.ToString().First(c => c != commonDot[0]));
            Angle commonA = (Angle)_db.FindKey(new Angle(p2 + commonDot + p90));
            _db.Update(a1, new Node(a1.ToString(), 90, "לפי הגדרת אנך אמצעי", perpendicularBisector), DataType.Equations);
            _db.Update(a2, new Node(a2.ToString(), 90, "לפי הגדרת אנך אמצעי", perpendicularBisector), DataType.Equations);
            Angle equalA = AnglesKeys.Find(a => a.ToString()[1] == commonA.ToString()[1]);
            _db.Update(commonA, new Node(commonA.ToString(),
                equalA.variable, "זווית משותפת", new List<Node>() { MainNode, newT.MainNode })
                , DataType.Equations);
            _db.Update(equalA, new Node(equalA.ToString(),
               commonA.variable, "זווית משותפת", new List<Node>() { MainNode, newT.MainNode })
               , DataType.Equations);

            //Update lines
            Line p1Main = (Line)_db.FindKey(p1Line);
            Line p1Part1 = (Line)_db.FindKey(new Line(commonDot + p90));
            Line p1Part2 = (Line)_db.FindKey(new Line(p1Line.ToString().First(c => c != commonDot[0]) + p90));

            Line p2Main = (Line)_db.FindKey(p2Line);
            Line p2Part1 = (Line)_db.FindKey(new Line(commonDot + p2));
            Line p2Part2 = (Line)_db.FindKey(new Line(p2Line.ToString().First(c => c != commonDot[0]) + p2));

            UpdateLinesPerpendicularBisector(p1Main, p1Part1, p1Part2, perpendicularBisector);

            _db.Update(p2Part1, new Node(p2Part1.ToString(), p2Main.variable - p2Part2.variable, "חיסור צלעות", perpendicularBisector), DataType.Equations);
            _db.Update(p2Part2, new Node(p2Part2.ToString(), p2Main.variable - p2Part1.variable, "חיסור צלעות", perpendicularBisector), DataType.Equations);
            _db.Update(p2Main, new Node(p2Main.ToString(), p2Part1.variable + p2Part2.variable, "חיבור צלעות", perpendicularBisector), DataType.Equations);



            TwoLinesCut tlc1 = new TwoLinesCut(_db, new Line(perpendicularBisector.name), p1Line, p90, "חוצה זווית חותך את הישר");
            tlc1.AddParent(perpendicularBisector);

            TwoLinesCut tlc2 = new TwoLinesCut(_db, new Line(perpendicularBisector.name), p2Line, p2, "חוצה זווית חותך את הישר");
            tlc1.AddParent(perpendicularBisector);

        }
        //s:87
        public virtual void UpdateMeansSection(Line l1, Line l2, string p1, string p2, string reason,List< Node> mainParent)
        {
            //if (MeansSections == null)
            //{
            //    MeansSections = new Dictionary<Pair<Line, Line>, MeansSection>();
           // }
           // MeansSection meansSection = new MeansSection(p1 + p2, reason, mainParent);
           //MeansSections[new Pair<Line, Line>(l1, l2)] = meansSection;
           // handleUpdateMeansSection(l1, l2, p1, p2, meansSection);

            /********************************************/
            if (!LinesKeys.Contains(l1) || !LinesKeys.Contains(l2)) return;
            if (p1 == p2) return;
            if (l1.PointsKeys.Contains(p1) ||
                l1.PointsKeys.Contains(p2) ||
                l2.PointsKeys.Contains(p1) ||
                l2.PointsKeys.Contains(p2)) return;

           
            Line meansSection = (Line)_db.FindKey(new Line(p1 + p1));
            Node msNode = new Node(meansSection.ToString(), null, reason, mainParent);
            Line baseLine = LinesKeys.Find((l) => !l.Equals(l1) && !l.Equals(l2));
            string r = "קטע אמצעים במשולש המחבר אמצעי שתי צלעות  מקביל לצלע השלישית ושווה למחציתה";
            _db.Update(meansSection, new Node(meansSection.ToString(), baseLine.variable/2,r , msNode), DataType.Equations);
            _db.Update(baseLine, new Node(baseLine.ToString(), meansSection.variable * 2, r, msNode), DataType.Equations);
            _db.Update(meansSection, new Node(meansSection.ToString(), baseLine.variable, r, msNode), DataType.Equations);
            _db.Update(baseLine, new Node(baseLine.ToString(), meansSection.variable, r, msNode), DataType.ParallelLines);
        }
        public virtual bool IsMeansSection(Line l1, Line l2,string p1, string p2)
        {
            if (!LinesKeys.Contains(l1) || !LinesKeys.Contains(l2)) return false;
            if (p1 == p2) return false;
            if (l1.PointsKeys.Contains(p1) ||
                l1.PointsKeys.Contains(p2) ||
                l2.PointsKeys.Contains(p1) ||
                l2.PointsKeys.Contains(p2)) return false;
            Line ms = (Line)_db.FindKey(new Line(p1 + p2));
            Line baseLine = LinesKeys.Find((l) => !l.Equals(l1) && !l.Equals(l2));
            if (CheckMeansSectionByEqualsParts(l1, l2, p1, p2)) return true;
            if (CheckMeansSectionByHalfBase(l1, l2, p1, p2)) return true;
            return false;
        }
        //s:88
        private bool CheckMeansSectionByEqualsParts(Line l1, Line l2, string p1, string p2)
        {
            Line ms = (Line)_db.FindKey(new Line(p1 + p2));
            Line baseLine = LinesKeys.Find((l) => !l.Equals(l1) && !l.Equals(l2));
            Line part1 = (Line)_db.FindKey(new Line(l1.PointsKeys[0] + p1));
            Line part2 = (Line)_db.FindKey(new Line(l1.PointsKeys[1] + p1));
            Node eq = _db.GetEqualsNode(part1, part2);
            if (eq == null)
            {
                part1 = (Line)_db.FindKey(new Line(l2.PointsKeys[0] + p2));
                part2 = (Line)_db.FindKey(new Line(l2.PointsKeys[1] + p2));
                eq = _db.GetEqualsNode(part1, part2);
            }
            if (eq == null) return false;
            Node par = _db.ParallelLines[ms].Find((n) => n.Expression.Vars.ElementAt(0).Equals(baseLine));
            if (par == null) return false;

            UpdateMeansSection(l1, l2, p1, p2,
                "קטע היוצא מאמצע צלע אחת ומקביל לצלע השנייה הוא קטע אמצעים במשולש",
                new List<Node>() { par, eq });
            return true;
        }
        //s:89
        private bool CheckMeansSectionByHalfBase(Line l1, Line l2, string p1, string p2)
        {
            Line ms = (Line)_db.FindKey(new Line(p1 + p2));
            Line baseLine = LinesKeys.Find((l) => !l.Equals(l1) && !l.Equals(l2));
            Node par = _db.ParallelLines[ms].Find((n) => n.Expression.Vars.ElementAt(0).Equals(baseLine));
            if (par == null) return false;
            Node halfBase = _db.HandleEquations.Equations[ms].Find((n) => n.Expression.Equals(baseLine.variable / 2));
            if (halfBase == null) return false;
            UpdateMeansSection(l1, l2, p1, p2,
                "קטע המחבר שתי צלעות המשולש שמקביל לצלע השלישית ושווה למחציתה הוא קטע אמצעים במשולש",
                new List<Node>() { par, halfBase });
            return true;
        }



        public Height GetHeight(string p)
        {
            if (Heights == null) return null;
            if (Heights.ContainsKey(p))
                return Heights[p];
            return null;
        }
        public Median GetMedian(string p)
        {
            if (Medians == null) return null;
            if (Medians.ContainsKey(p))
                return Medians[p];
            return null;
        }
        public AngleBisector GetAngleBisector(string p)
        {
            if (AngleBisectors == null) return null;
            if (AngleBisectors.ContainsKey(p))
                return AngleBisectors[p];
            return null;
        }
       
        public void CopyToChild(Triangle child)
        {
            // Copy properties from triangle to child

            foreach (string point in PointsKeys)
            {
                if (child.GetHeight(point) != null) continue;
                Height line = GetHeight(point);
                if (line == null) continue;
                child.UpdateHeight(line.name[0].ToString(), line.name[1].ToString(), " in CopyToChild", line.Parents[0]);
            }

            foreach (string point in PointsKeys)
            {
                if (child.GetMedian(point) != null) continue;
                Median line = GetMedian(point);
                if (line == null) continue;
                child.UpdateMedian(line.name[0].ToString(), line.name[1].ToString(), "in CopyToChild", line.Parents[0]);
            }


            foreach (string point in PointsKeys)
            {
                if (child.GetAngleBisector(point) != null) continue;
                AngleBisector line = GetAngleBisector(point);
                if (line == null) continue;
                child.UpdateAngleBisector(line.name[0].ToString(), line.name[1].ToString(), " in CopyToChild", line.Parents[0]);
            }

        }

        public void UpdateExternAngle(Line line, string dot)
        {
            string fromLine1 = line.ToString()[0].ToString();
            string fromLine2 = line.ToString()[1].ToString();
            string c = this.PointsKeys.Find(p => p != fromLine1 && p != fromLine2);
            Triangle triangle1 = new Triangle(_db, fromLine1, dot, c, "");
            Triangle triangle2 = new Triangle(_db, fromLine2, dot, c, "");


            Angle alpha = triangle1.AnglesKeys.Find(a => a.ToString()[1].ToString() == c);
            Angle alphaPart2 = triangle2.AnglesKeys.Find(a => a.ToString()[1].ToString() == c);
            Angle alphaPart1 = this.AnglesKeys.Find(a => a.ToString()[1].ToString() == c);

            Angle gama = this.AnglesKeys.Find(a => a.ToString()[1].ToString() == fromLine1);
            Angle bigGama = triangle1.AnglesKeys.Find(a => a.ToString()[1] == gama.ToString()[1]);

            Angle betaPart1 = this.AnglesKeys.Find(a => a.ToString()[1].ToString() == fromLine2);
            Angle betaPart2 = triangle2.AnglesKeys.Find(a => a.ToString()[1].ToString() == fromLine2);


            _db.Update(betaPart1, new Node(betaPart1.ToString(), 180 - betaPart2.variable, "סכום זוויות צמודות 180", triangle2.MainNode), DataType.Equations);
            _db.Update(betaPart2, new Node(betaPart2.ToString(), 180 - betaPart1.variable, "סכום זוויות צמודות 180", this.MainNode), DataType.Equations);

            _db.Update(betaPart2, new Node(betaPart2.ToString(), alphaPart1.variable + gama.variable, "זווית חיצונית למשולש שווה לסכום שתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Equations);
            _db.Update(alphaPart1, new Node(alphaPart1.ToString(), betaPart2.variable - gama.variable, "זווית חיצונית למשולש שווה לסכום שתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Equations);
            _db.Update(gama, new Node(gama.ToString(), betaPart2.variable - alphaPart1.variable, "זווית חיצונית למשולש שווה לסכום שתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Equations);
            _db.Update(gama, new Node(gama.ToString(), bigGama.variable, "זווית משותפת", triangle1.MainNode), DataType.Equations);
            _db.Update(bigGama, new Node(bigGama.ToString(), gama.variable, "זווית משותפת", this.MainNode), DataType.Equations);

            _db.Update(alpha, new Node(alpha.ToString(), alphaPart1.variable + alphaPart2.variable, "חיבור זוויות", triangle1.MainNode), DataType.Equations);
            _db.Update(alphaPart1, new Node(alpha.ToString(), alpha.variable - alphaPart2.variable, "חיסור זוויות", triangle1.MainNode), DataType.Equations);
            _db.Update(alphaPart2, new Node(alpha.ToString(), alpha.variable - alphaPart1.variable, "חיסור זוויות", triangle1.MainNode), DataType.Equations);

            //Inequalities - s:29
            _db.Update(betaPart2, new Node(betaPart2.ToString(), betaPart2.variable > alphaPart1.variable, "זווית חיצונית למשולש גדולה מכל אחת משתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Inequalities);
            _db.Update(betaPart2, new Node(betaPart2.ToString(), betaPart2.variable > gama.variable, "זווית חיצונית למשולש גדולה מכל אחת משתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Inequalities);
            _db.Update(alphaPart1, new Node(alphaPart1.ToString(), betaPart2.variable > alphaPart1.variable, "זווית חיצונית למשולש גדולה מכל אחת משתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Inequalities);
            _db.Update(gama, new Node(gama.ToString(), betaPart2.variable > gama.variable, "זווית חיצונית למשולש גדולה מכל אחת משתי הזוויות הפנימיות שאינן צמודות לה", this.MainNode), DataType.Inequalities);

        }
        //s:30
        public void UpdateBiggerLine(Line bigger, Line smaller, string reason)
        {
            if (LinesKeys.Contains(bigger) && LinesKeys.Contains(smaller))
            {
                Node nodeB = new Node(bigger.ToString(), bigger.variable > smaller.variable, reason, this.MainNode);
                Node nodeS = new Node(smaller.ToString(), bigger.variable > smaller.variable, reason, this.MainNode);
                _db.Update(bigger, nodeB, DataType.Inequalities);
                _db.Update(smaller, nodeS, DataType.Inequalities);
                Angle frontB = AnglesKeys.Find(a => a.ToString()[1].ToString() == PointsKeys.Find(p => !bigger.ToString().Contains(p)));
                Angle frontS = AnglesKeys.Find(a => a.ToString()[1].ToString() == PointsKeys.Find(p => !smaller.ToString().Contains(p)));
                reason = "אם במשולש צלע אחת גדולה מצלע השנייה, אז הזווית שמול הצלע הגדולה יותר גדולה מהזווית שמול הצלע הקטנה";
                _db.Update(frontB, new Node(frontB.ToString(), frontB.variable > frontS.variable, reason, nodeB), DataType.Inequalities);
                _db.Update(frontS, new Node(frontS.ToString(), frontB.variable > frontS.variable, reason, nodeS), DataType.Inequalities);

            }
        }
        //s:31
        public void UpdateBiggerAngle(Angle bigger, Angle smaller, string reason)
        {
            if (AnglesKeys.Contains(bigger) && AnglesKeys.Contains(smaller))
            {
                Node nodeB = new Node(bigger.ToString(), bigger.variable > smaller.variable, reason, this.MainNode);
                Node nodeS = new Node(smaller.ToString(), bigger.variable > smaller.variable, reason, this.MainNode);
                _db.Update(bigger, nodeB, DataType.Inequalities);
                _db.Update(smaller, nodeS, DataType.Inequalities);
                Line frontB = LinesKeys.Find(l => !l.ToString().Contains(bigger.ToString()[1]));
                Line frontS = LinesKeys.Find(l => !l.ToString().Contains(smaller.ToString()[1]));
                reason = "אם במשולש זווית אחת גדולה מזווית שנייה, אז הצלע שמול הזווית הגדולה יותר גדולה מהצלע שמול הזווית הקטנה";
                _db.Update(frontB, new Node(frontB.ToString(), frontB.variable > frontS.variable, reason, nodeB), DataType.Inequalities);
                _db.Update(frontS, new Node(frontS.ToString(), frontB.variable > frontS.variable, reason, nodeS), DataType.Inequalities);

            }
        }

        //s:125,126  parallelLine = DE and BC cannot be ED and CB!
        public void UpdateThalesTheorem(Line triangleLine, Line parallelLine)
        {
            if (!LinesKeys.Contains(triangleLine)) throw new Exception("triangle not include given line");

            if (_db.ParallelLines.ContainsKey(triangleLine))
            {
                string str = _db.FindKey(parallelLine).ToString();
                int index = _db.ParallelLines[triangleLine].FindIndex(n => n.Expression.ToString().Equals(str));
                if (index >= 0)
                {
                    List<Node> parents = new List<Node>() { MainNode, _db.ParallelLines[triangleLine][index] };
                    //new ParallelLinesWithTransversal(db, LinesKeys[i], parallelLine, LinesKeys[(i + 1) % 3]);
                    //new ParallelLinesWithTransversal(db, LinesKeys[i], parallelLine, LinesKeys[(i + 2) % 3]);

                    string a = LinesKeys[0].Equals(triangleLine) ?
                        LinesKeys[1].ToString().First(c => !triangleLine.ToString().Contains(c)).ToString() :
                        LinesKeys[0].ToString().First(c => !triangleLine.ToString().Contains(c)).ToString();
                    //Init lines
                    Line bc = triangleLine;
                    Line de = parallelLine;
                    Line ae = (Line)_db.FindKey(new Line(a + de.ToString()[1].ToString()));
                    Line ac = (Line)_db.FindKey(new Line(a + bc.ToString()[1].ToString()));
                    Line ad = (Line)_db.FindKey(new Line(a + de.ToString()[0].ToString()));
                    Line ab = (Line)_db.FindKey(new Line(a + bc.ToString()[0].ToString()));
                    Line ec = (Line)_db.FindKey(new Line(de.ToString()[1].ToString() + bc.ToString()[1].ToString()));
                    Line db = (Line)_db.FindKey(new Line(de.ToString()[0].ToString() + bc.ToString()[0].ToString()));
                    bc = (Line)_db.FindKey(bc);
                    de = (Line)_db.FindKey(de);



                    //Triangle ADE
                    Triangle t = new Triangle(_db, a, de.ToString()[0].ToString(), de.ToString()[1].ToString(), "");
                    t.AddParents(parents);
                    //Update common angle cab=ead
                    Angle cab = AnglesKeys.Find((e) => e.ToString()[1].ToString() == a);
                    Angle ead = t.AnglesKeys.Find((e) => e.ToString()[1].ToString() == a);
                    _db.Update(cab, new Node(cab.ToString(), ead.variable, "אותה הזווית",t.MainNode), DataType.Equations);
                    _db.Update(ead, new Node(ead.ToString(), cab.variable, "אותה הזווית", t.MainNode), DataType.Equations);

                    //Update to database s:126
                    Entity mainS126 = $"({ad.variable}) / ({ab.variable}) = ({ae.variable}) / ({ac.variable})";
                    Entity mainE2 = $"({ad.variable}) / ({ab.variable}) = ({de.variable}) / ({bc.variable})";
                    Entity mainE3 = $"({ae.variable}) / ({ac.variable}) = ({de.variable}) / ({bc.variable})";


                    Node n1 = new Node($"({ad.variable}) / ({ab.variable})", $"({ae.variable}) / ({ac.variable})", "הרחבה ראשונה של משפט תלס", parents);
                    Node n2 = new Node($"({ad.variable}) / ({ab.variable})", $"({de.variable}) / ({bc.variable})", "הרחבה ראשונה של משפט תלס", parents);
                    Node n3 = new Node($"({ae.variable}) / ({ac.variable})", $"({de.variable}) / ({bc.variable})", "הרחבה ראשונה של משפט תלס", parents);


                    _db.Update(ae, new Node(ae.ToString(), mainS126.Solve(ae.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n1), DataType.Equations);
                    _db.Update(ae, new Node(ae.ToString(), mainE3.Solve(ae.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n3), DataType.Equations);

                    _db.Update(ac, new Node(ac.ToString(), mainS126.Solve(ac.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n1), DataType.Equations);
                    _db.Update(ac, new Node(ac.ToString(), mainE3.Solve(ac.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n3), DataType.Equations);

                    _db.Update(ad, new Node(ad.ToString(), mainS126.Solve(ad.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n1), DataType.Equations);
                    _db.Update(ad, new Node(ad.ToString(), mainE2.Solve(ad.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n2), DataType.Equations);

                    _db.Update(ab, new Node(ab.ToString(), mainS126.Solve(ab.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n1), DataType.Equations);
                    _db.Update(ab, new Node(ab.ToString(), mainE2.Solve(ab.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n2), DataType.Equations);

                    _db.Update(bc, new Node(bc.ToString(), mainE3.Solve(bc.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n3), DataType.Equations);
                    _db.Update(bc, new Node(bc.ToString(), mainE2.Solve(bc.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n2), DataType.Equations);

                    _db.Update(de, new Node(de.ToString(), mainE3.Solve(de.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n3), DataType.Equations);
                    _db.Update(de, new Node(de.ToString(), mainE2.Solve(de.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי הרחבה ראשונה של משפט תלס", n2), DataType.Equations);



                    //update to database s:125
                    Entity mainS125 = $"({ae.variable}) / ({ec.variable}) = ({ad.variable}) / ({db.variable})";

                    Node n = new Node($"({ae.variable}) / ({ec.variable})", $"({ad.variable}) / ({db.variable})", "משפט תלס", parents);

                    _db.Update(ae, new Node(ae.ToString(), mainS125.Solve(ae.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט תלס", n), DataType.Equations);
                    _db.Update(ec, new Node(ec.ToString(), mainS125.Solve(ec.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט תלס", n), DataType.Equations);
                    _db.Update(ad, new Node(ad.ToString(), mainS125.Solve(ad.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט תלס", n), DataType.Equations);
                    _db.Update(db, new Node(db.ToString(), mainS125.Solve(db.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט תלס", n), DataType.Equations);
                    return;
                }
            }

        }

    

        //Private:
        private void handleCommonAngles(Triangle t1, Triangle t2, List<string> list, string containPoint, string otherPoint)
        {
            //Init reason and parents nodes
            string r1 = $"{this} ו {t1} זווית משותפת במשולשים";
            string r2 = $"{this} ו {t2} זווית משותפת במשולשים";
            List<Node> parents1 = new List<Node>() { t1.MainNode, this.MainNode };
            List<Node> parents2 = new List<Node>() { t2.MainNode, this.MainNode };

            //Init common angles in origin triangle
            Angle commonAngle1 = AnglesKeys.Find((a) => a.ToString()[1].ToString() == list[0]);
            Angle commonAngle2 = AnglesKeys.Find((b) => b.ToString()[1].ToString() == list[1]);

            //Init common angles in new triangles 
            Angle temp1 = t1.AnglesKeys.Find((a) => a.ToString()[1] == commonAngle1.ToString()[1]);
            Angle temp2 =  t2.AnglesKeys.Find((a) => a.ToString()[1] == commonAngle2.ToString()[1]);

            //Update common angles:
            //1
            _db.Update(temp1,
                new Node(temp1.ToString(), commonAngle1.variable, r1,parents1 ), DataType.Equations);
            //2
            _db.Update(temp2,
                new Node(temp2.ToString(), commonAngle2.variable, r2, parents2), DataType.Equations);
            //3
            _db.Update(commonAngle1,
                new Node(commonAngle1.ToString(), temp1.variable, r1, parents1), DataType.Equations);
            //4
            _db.Update(commonAngle2,
                new Node(commonAngle2.ToString(), temp2.variable, r2, parents2), DataType.Equations);
        }
        private void handleDivideAngle(Triangle t1, Triangle t2, List<string> list, string containPoint, string otherPoint)
        {
            //Init parents nodes
            List<Node> nodes = new List<Node>() { t1.MainNode, t2.MainNode };

            //Init divided angles
            Angle dividedAngle1 = t1.AnglesKeys.Find((a) => a.Equals(new Angle(list[0] + containPoint + otherPoint)));
            Angle dividedAngle2 = t2.AnglesKeys.Find((a) => a.Equals(new Angle(list[1] + containPoint + otherPoint)));
            Angle mainAngle = AnglesKeys.Find((a) => a.Equals(new Angle(list[0] + containPoint + list[1])));

            //Update divided and main angles
            _db.Update(dividedAngle1,
                new Node(dividedAngle1.ToString(), mainAngle.variable - dividedAngle2.variable, "חיסור זוויות", nodes), DataType.Equations);
            _db.Update(dividedAngle2,
                new Node(dividedAngle2.ToString(), mainAngle.variable - dividedAngle1.variable, "חיסור זוויות", nodes), DataType.Equations);
            _db.Update(mainAngle,
                new Node(mainAngle.ToString(), dividedAngle1.variable + dividedAngle2.variable, "חיבור זוויות", nodes), DataType.Equations);

        }
        private void handleDivideLine(Triangle t1, Triangle t2, List<string> list, string otherPoint)
        {
            //Init parents nodes
            List<Node> nodes = new List<Node>() { t1.MainNode, t2.MainNode };
            //Init lines:
            Line line1 = t1.LinesKeys.Find((l) => l.Equals(new Line(list[0] + otherPoint)));
            Line line2 = t2.LinesKeys.Find((l) => l.Equals(new Line(list[1] + otherPoint)));
            Line mainLine = LinesKeys.Find((l) => l.Equals(new Line(list[0] + list[1])));
            //Update lines
            _db.Update(mainLine, new Node(mainLine.ToString(), line1.variable + line2.variable, "חיבור צלעות", nodes), DataType.Equations);
            _db.Update(line1, new Node(line1.ToString(), mainLine.variable - line2.variable, "חיסור צלעות", nodes), DataType.Equations);
            _db.Update(line2, new Node(line2.ToString(), mainLine.variable - line1.variable, "חיסור צלעות", nodes), DataType.Equations);
        }
        private void handleUpdateHeight(string containPoint, string otherPoint, string reason, Node mainParent)
        {
            List<string> list = PointsKeys.FindAll((p) => p != containPoint);
            if (list.Count < 2) throw new Exception("invalid hieght points");

            //Init current height 
            string r = "גובה מאונך לצלע";
            Height heightNode = Heights[containPoint];
            if (mainParent != null)
            {
                heightNode.Parents.Add(mainParent);
            }

            //Init new right angles
            Angle rightAngle1 = new Angle(containPoint + otherPoint + list[0]);
            Angle rightAngle2 = new Angle(containPoint + otherPoint + list[1]);

            //Init new right triangles
            RightTriangle rightTriangle1 = new RightTriangle(_db, containPoint, list[0], otherPoint, rightAngle1, r);
            rightTriangle1.AddParent(heightNode);
            RightTriangle rightTriangle2 = new RightTriangle(_db, containPoint, list[1], otherPoint, rightAngle2, r);
            rightTriangle2.AddParent(heightNode);

            handleCommonAngles(rightTriangle1, rightTriangle2, list, containPoint, otherPoint);

            handleDivideAngle(rightTriangle1, rightTriangle2, list, containPoint, otherPoint);

            handleDivideLine(rightTriangle1, rightTriangle2, list, otherPoint);

            TwoLinesCut tlc1 =  new TwoLinesCut(_db, new Line(heightNode.name), new Line(list[0] + list[1]), otherPoint, "גובה חותך את הישר");
            tlc1.AddParent(heightNode);

        }
        private void HandleUpdateMedian(string containPoint, string otherPoint, string reason, Node mainParent)
        {
            List<string> list = PointsKeys.FindAll((p) => p != containPoint);
            if (list.Count < 2) throw new Exception("invalid median points in newTriangle");

            //current median
            string r = "תיכון חוצה את הצלע";
            Median medianNode = Medians[containPoint];

            if (mainParent != null)
            {
                medianNode.Parents.Add(mainParent);
            }

            //Init new triangles
            Triangle triangle1 = new Triangle(_db, containPoint, list[0], otherPoint, "תיכון מחלק את המשולש ל2 משולשים");
            triangle1.AddParent(medianNode);
            Triangle triangle2 = new Triangle(_db, containPoint, list[1], otherPoint, "תיכון מחלק את המשולש ל2 משולשים");
            triangle2.AddParent(medianNode);

            handleCommonAngles(triangle1, triangle2, list, containPoint, otherPoint);
            handleDivideAngle(triangle1, triangle2, list, containPoint, otherPoint);

            //Init lines:
            Line line1 = triangle1.LinesKeys.Find((l) => l.Equals(new Line(list[0] + otherPoint)));
            Line line2 = triangle2.LinesKeys.Find((l) => l.Equals(new Line(list[1] + otherPoint)));
            Line mainLine = LinesKeys.Find((l) => l.Equals(new Line(list[0] + list[1])));
          

            //Update lines
            _db.Update(line1, new Node(line1.ToString(), line2.variable, r, medianNode), DataType.Equations);
            _db.Update(line1, new Node(line1.ToString(), mainLine.variable / 2, r, medianNode), DataType.Equations);
            _db.Update(line2, new Node(line2.ToString(), line1.variable, r, medianNode), DataType.Equations);
            _db.Update(line2, new Node(line2.ToString(), mainLine.variable / 2, r, medianNode), DataType.Equations);

            handleDivideLine(triangle1, triangle2, list, otherPoint);

            TwoLinesCut tlc1 = new TwoLinesCut(_db, new Line(medianNode.name), new Line(list[0] + list[1]), otherPoint, "תיכון חותך את הישר");
            tlc1.AddParent(medianNode);

        }
        private void HandleUpdateAngleBisector(string containPoint, string otherPoint, string reason, Node mainParent)
        {
            HandleLinesAngleBisector(containPoint, otherPoint, reason);
            List<string> list = PointsKeys.FindAll((p) => p != containPoint);
            if (list.Count < 2) throw new Exception("invalid hieght points");

            string r = "חוצה זווית חוצה את הזווית";
            AngleBisector angleBisectorNode = AngleBisectors[containPoint];
            if (mainParent != null)
            {
                angleBisectorNode.Parents.Add(mainParent);
            }
            //Init new triangles
            Triangle triangle1 = new Triangle(_db, containPoint, list[0], otherPoint, "חוצה זווית מחלק את המשולש ל2 משולשים");
            triangle1.AddParent(angleBisectorNode);
            Triangle triangle2 = new Triangle(_db, containPoint, list[1], otherPoint, "חוצה זווית מחלק את המשולש ל2 משולשים");
            triangle2.AddParent(angleBisectorNode);

            handleCommonAngles(triangle1, triangle2, list, containPoint, otherPoint);

            //Init divided angles
            Angle dividedAngle1 = triangle1.AnglesKeys.Find((a) => a.Equals(new Angle(list[0] + containPoint + otherPoint)));
            Angle dividedAngle2 = triangle2.AnglesKeys.Find((a) => a.Equals(new Angle(list[1] + containPoint + otherPoint)));
            Angle mainAngle = AnglesKeys.Find((a) => a.Equals(new Angle(list[0] + containPoint + list[1])));

            //Update divided and main angles
            _db.Update(dividedAngle1, new Node(dividedAngle1.ToString(), dividedAngle2.variable, r, angleBisectorNode), DataType.Equations);
            _db.Update(dividedAngle1, new Node(dividedAngle1.ToString(), mainAngle.variable / 2, r, angleBisectorNode), DataType.Equations);
            _db.Update(dividedAngle2, new Node(dividedAngle2.ToString(), dividedAngle1.variable, r, angleBisectorNode), DataType.Equations);
            _db.Update(dividedAngle2, new Node(dividedAngle2.ToString(), mainAngle.variable / 2, r, angleBisectorNode), DataType.Equations);

     

            handleDivideAngle(triangle1, triangle2, list, containPoint, otherPoint);

            handleDivideLine(triangle1, triangle2, list, otherPoint);


            TwoLinesCut tlc1 = new TwoLinesCut(_db, new Line(angleBisectorNode.name), new Line(list[0] + list[1]), otherPoint, "חוצה זווית חותך את הישר");
            tlc1.AddParent(angleBisectorNode);
      
        }
     
        //update the lines when dot cut First line
        private void UpdateLinesPerpendicularBisector(Line main, Line part1, Line part2, Node p)
        {
            Node mainN = new Node(main.ToString(), part1.variable + part2.variable, "חיבור צלעות", p);
            _db.Update(main, mainN, DataType.Equations);
            _db.Update(main, new Node(main.ToString(), 2 * part1.variable, "כלל המעבר", mainN), DataType.Equations);
            _db.Update(main, new Node(main.ToString(), 2 * part2.variable, "כלל המעבר", mainN), DataType.Equations);
            HandleEqualsLine(main, part1, part2, p);
            HandleEqualsLine(main, part2, part1, p);
        }
        private void HandleEqualsLine(Line main, Line part1, Line part2, Node p)
        {
            Node mainMinusPart2 = new Node(part1.ToString(), main.variable - part2.variable, "חיסור צלעות", p);
            _db.Update(part1, mainMinusPart2, DataType.Equations);
            Node equalN = new Node(part1.ToString(), part2.variable, "לפי הגדרת אנך אמצעי", p);
            _db.Update(part1, equalN, DataType.Equations);
            _db.Update(part1, new Node(part1.ToString(), main.variable / 2, "כלל המעבר", new List<Node>() { mainMinusPart2, equalN }), DataType.Equations);

        }
        //s:129
        private void HandleLinesAngleBisector(string containPoint, string otherPoint, string reason)
        {
            string c = PointsKeys[0] == containPoint ? PointsKeys[1] : PointsKeys[0];
            string b = PointsKeys[2] == containPoint ? PointsKeys[1] : PointsKeys[2];
            Line cd = (Line)_db.FindKey(new Line(c + otherPoint));
            Line bd = (Line)_db.FindKey(new Line(b + otherPoint));
            Line ac = LinesKeys.Find((l) => l.Equals(new Line(containPoint + c)));
            Line ab = LinesKeys.Find((l) => l.Equals(new Line(containPoint + b))); 
            Entity expr = $"({cd.variable}) / ({bd.variable}) = ({ac.variable}) / ({ab.variable})";
            Node n = new Node($"({cd.variable}) / ({bd.variable})", $"({ac.variable}) / ({ab.variable})", "משפט חוצה זווית", AngleBisectors[containPoint]);

            _db.Update(cd, new Node(cd.ToString(), expr.Solve(cd.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט חוצה זווית", n), DataType.Equations);
            _db.Update(bd, new Node(bd.ToString(), expr.Solve(bd.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט חוצה זווית", n), DataType.Equations);
            _db.Update(ac, new Node(ac.ToString(), expr.Solve(ac.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט חוצה זווית", n), DataType.Equations);
            _db.Update(ab, new Node(ab.ToString(), expr.Solve(ab.variable).ToString().Replace("{", "").Replace("}", ""), "חישוב לפי משפט חוצה זווית", n), DataType.Equations);
        }
        /*
         * c	=	length of side c
         * a	=	length of side a
         * b	=	length of side b
         * gamma	=	angle opposite c
         * 
         * c = sqrt(a^2 + b^2 - 2ab*cos(gamma))
         */
        private void LawOfCosines(Line c, Variable a, Variable b, Angle gamma)
        {
            _db.Update(c, new Node(c.ToString(),
                MathS.Sqrt(MathS.Sqr(a) + MathS.Sqr(b) - 2 * a * b * MathS.Cos(gamma.variable)), "על פי משפט הקוסינוסים", MainNode),
                DataType.Equations);

            _db.Update(gamma, new Node(gamma.ToString(),
               MathS.Arccos((MathS.Sqr(a) + MathS.Sqr(b) - MathS.Sqr(c.variable) / (2 * a * b)) ), "על פי משפט הקוסינוסים", MainNode),
               DataType.Equations);
        }
        /*
         * A	=	angle A
         * a	=	length of side a
         * B	=	angle B
         * b	=	length of side b
         * C	=	angle C
         * c	=	length side c
         * 
         * sin A/a = sin B/b = sin C/c
         */
        private void LawOfSines(Line a, Line b, Angle alpha, Angle beta)
        {
            _db.Update(a, new Node(a.ToString(),
               b.variable * (MathS.Sin(alpha.variable )/ MathS.Sin(beta.variable )), "על פי משפט הסינוסים", MainNode), DataType.Equations);

            _db.Update(b, new Node(b.ToString(),
              a.variable * (MathS.Sin(beta.variable ) / MathS.Sin(alpha.variable )), "על פי משפט הסינוסים", MainNode), DataType.Equations);

            _db.Update(alpha, new Node(alpha.ToString(),
            MathS.Arcsin((a.variable * MathS.Sin(beta.variable )) / b.variable) , "על פי משפט הסינוסים", MainNode), DataType.Equations);

            _db.Update(beta, new Node(beta.ToString(),
            MathS.Arcsin((b.variable * MathS.Sin(alpha.variable )) / a.variable) , "על פי משפט הסינוסים", MainNode), DataType.Equations);
        }
       
        
        public void UpdateArea(Node area)
        {
            this.Area = area;
            foreach (var pair in Heights)
            {
                Line h = (Line)_db.FindKey(new Line(pair.Value.name));
                Line b = LinesKeys.Find((l) => !l.ToString().Contains(pair.Key));
                _db.Update(h, new Node(h.ToString(), (area.Expression * 2) / b.variable, "לפי חישוב שטח משולש", new List<Node>() { pair.Value, area }), DataType.Equations);
                _db.Update(b, new Node(b.ToString(), (area.Expression * 2) / h.variable, "לפי חישוב שטח משולש", new List<Node>() { pair.Value, area }), DataType.Equations);
            }

        }
        public void UpdatePerimeter(Node perimeter)
        {
            this.Perimeter = perimeter;

            Line l1 = LinesKeys[0];
            Line l2 = LinesKeys[1];
            Line l3 = LinesKeys[2];
            _db.Update(l1, new Node(l1.ToString(), perimeter.Expression - l2.variable - l3.variable, "לפי חישוב היקף משולש", perimeter), DataType.Equations);
            _db.Update(l2, new Node(l2.ToString(), perimeter.Expression - l1.variable - l3.variable, "לפי חישוב היקף משולש", perimeter), DataType.Equations);
            _db.Update(l3, new Node(l3.ToString(), perimeter.Expression - l2.variable - l1.variable, "לפי חישוב היקף משולש", perimeter), DataType.Equations);
        }
        public Node CalculateArea()
        {
            if (this.Area !=null) return this.Area;
            foreach (string mainPoint in PointsKeys)
            {
                Height heightNode = Heights[mainPoint];
                if (heightNode == null)
                {
                    UpdateHeight(mainPoint, _db.GetUnusedPoint(), "קו עזר", MainNode);
                    heightNode = Heights[mainPoint];
                    if (heightNode == null) throw new Exception("cannot update new height");
                }
                Line heightLine = (Line)_db.FindKey(new Line(heightNode.name));
                Line baseLine = LinesKeys.Find((l) => !l.ToString().Contains(mainPoint));
                Entity heightExpr = _db.HandleEquations.Equations[heightLine][0].Expression;
                Entity baseLineExpr = _db.HandleEquations.Equations[baseLine][0].Expression;
                if (!heightExpr.Evaled is Number || !baseLineExpr.Evaled is Number) break;
                double heightLen = double.Parse(heightExpr.ToString());
                double baseLineLen = double.Parse(baseLineExpr.ToString());
                double calc = (heightLen * baseLineLen) / 2;
                this.Area = new Node($"{this.variable} שטח משולש", calc, "חישוב", heightNode);
                return this.Area;
            }
            return null;
        }

        public Node CalculatePerimeter()
        {
            if (this.Perimeter !=null) return this.Perimeter;
            Line l1 = LinesKeys[0];
            Line l2 = LinesKeys[1];
            Line l3 = LinesKeys[2];

            Entity l1Expr = _db.HandleEquations.Equations[l1][0].Expression;
            Entity l2Expr = _db.HandleEquations.Equations[l2][0].Expression;
            Entity l3Expr = _db.HandleEquations.Equations[l3][0].Expression;


            if (l1Expr.Evaled is Number && l2Expr.Evaled is Number && l3Expr.Evaled is Number)
            {
                double calc =double.Parse(l1Expr.ToString()) + 
                    double.Parse(l2Expr.ToString()) + 
                    double.Parse(l3Expr.ToString());
              this.Perimeter = new Node($"{this.variable} היקף משולש", calc, "חישוב", MainNode);
                return this.Perimeter;
            }
            return null;
        }

        internal Variable GetArea()
        {
            throw new NotImplementedException();
        }

        internal Variable GetPerimeter()
        {
            throw new NotImplementedException();
        }

        //override
        public override string ToString()
        {
            return PointsKeys[0] + PointsKeys[1] + PointsKeys[2];
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            // Compare additional properties of Triangle class if any
            Triangle otherTriangle = (Triangle)obj;
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