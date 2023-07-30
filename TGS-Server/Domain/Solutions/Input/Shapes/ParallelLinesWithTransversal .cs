using DatabaseLibrary;
using System.Linq;
using static DatabaseLibrary.Database;

namespace Domain
{
    /* 2 lines parallel and one transversal, can be:
     * for AB||CD
     * transversal = EF or ED or EC or AF or BF 
     * 
     * Transversal CANNOT be BD or AC because then it is quadrilateral
     */
    public class ParallelLinesWithTransversal : Shape
    {
        protected TwoLinesCut TwoLinesCut1 { get; set; } = null;
        protected TwoLinesCut TwoLinesCut2 { get; set; } = null;

        //constractor
        public ParallelLinesWithTransversal(Database db, Line l1, Line l2, Line transversal, string cutP1, string cutP2, string reason) : base()
        {
            _db = db;
            MainNode = new Node($"בהתאמה {cutP1},{cutP2} בנקודות {transversal.variable} מקבילים הנחתכים על ידי ישר שלישי {l1.variable},{l2.variable} הישרים", null, reason);
            MainNode.typeName = "שני ישרים מקבילים הנחתכים על ידי ישר שלישי";

            TwoLinesCut1 = new TwoLinesCut(db, l1, transversal, cutP1, reason);
            TwoLinesCut1.AddParent(MainNode);
            TwoLinesCut2 = new TwoLinesCut(db, l2, transversal, cutP2, reason);
            TwoLinesCut2.AddParent(MainNode);


            string ab = l1.ToString();
            string cd = l2.ToString();
            string ef = transversal.ToString();
            char p1 = cutP1[0];
            char p2 = cutP2[0];
            char a = ab[0];
            char b = ab[1];
            char c = cd[0];
            char d = cd[1];
            char e = ef[0];
            char f = ef[1];
            Angle ap1p2 = null, bp1p2 = null, cp2p1 = null, dp2p1 = null;
            if (a != p1 && a != p2)
            {
                ap1p2 = (Angle)db.FindKey(new Angle(a.ToString() + cutP1 + cutP2));

            }
            if (b != p1 && b != p2)
            {
                bp1p2 = (Angle)db.FindKey(new Angle(b.ToString() + cutP1 + cutP2));

            }
            if (c != p1 && c != p2)
            {
                cp2p1 = (Angle)db.FindKey(new Angle(c.ToString() + cutP2 + cutP1));
            }
            if (d != p1 && d != p2)
            {
                dp2p1 = (Angle)db.FindKey(new Angle(d.ToString() + cutP2 + cutP1));
            }
            if (ap1p2 != null && cp2p1 != null)
            {
                ConsecutiveInterior(ap1p2, cp2p1);
            }
            if (dp2p1 != null && bp1p2 != null)
            {
                ConsecutiveInterior(dp2p1, bp1p2);

            }
            if (ap1p2 != null && dp2p1 != null)
            {
                AlternateInterior(ap1p2, dp2p1);
            }
            if (bp1p2 != null && cp2p1 != null)
            {
                AlternateInterior(bp1p2, cp2p1);
            }

            if (e.ToString() != cutP2)
            {
                Line eh = (Line)db.FindKey(new Line(e.ToString() + cutP2));
                if (!eh.ToString().Contains(cutP1))
                    TwoLinesCut1.CreateAnglesFromPoints(eh, cutP1);
            }
            if (f.ToString() != cutP1)
            {
                Line fg = (Line)db.FindKey(new Line(f.ToString() + cutP1));
                if (!fg.ToString().Contains(cutP2))
                    TwoLinesCut1.CreateAnglesFromPoints(fg, cutP2);
            }


        }
        public ParallelLinesWithTransversal(Database db, TwoLinesCut twoLinesCut1, TwoLinesCut twoLinesCut2, string reason) : base()
        {
            _db = db;
            Line line1, line2, commonLine;
            HashSet<Line> lines = new HashSet<Line>() { twoLinesCut1.GetLine1(), twoLinesCut1.GetLine2() };
            Line temp1 = twoLinesCut2.GetLine1(), temp2 = twoLinesCut2.GetLine2();
            line1 = lines.ElementAt(0);
            line2 = lines.ElementAt(1);
            if (!lines.Add(temp1))
            {
                commonLine = temp1;
                line1 = line1.Equals(commonLine) ? line2 : line1;
                line2 = temp2;
            }
            else// if(!lines.Add(temp2))
            {
                commonLine = temp2;
                line1 = line1.Equals(commonLine) ? line2 : line1;
                line2 = temp1;
            }
            List<Node> parents = new List<Node>() { twoLinesCut1.MainNode, twoLinesCut2.MainNode };
            Node parallelNode = db.ParallelLines[line1].Find(n => n.Expression.Equals(line2.variable));
            if (parallelNode == null) throw new Exception("not parallel in ParallelLinesWithTransversal");
            parents.Add(parallelNode);
            string cutP1 = twoLinesCut1.GetCutPoint();
            string cutP2 = twoLinesCut2.GetCutPoint();

            MainNode = new Node($"בהתאמה {cutP1},{cutP2} בנקודות {commonLine.variable} מקבילים הנחתכים על ידי ישר שלישי {line1.variable},{line2.variable} הישרים", null, reason, parents);
            MainNode.typeName = "שני ישרים מקבילים הנחתכים על ידי ישר שלישי";

            TwoLinesCut1 = twoLinesCut1;
            TwoLinesCut2 = twoLinesCut2;


            string ab = line1.ToString();
            string cd = line2.ToString();
            string ef = commonLine.ToString();
            char p1 = cutP1[0];
            char p2 = cutP2[0];
            char a = ab[0];
            char b = ab[1];
            char c = cd[0];
            char d = cd[1];
            char e = ef[0];
            char f = ef[1];
            Angle ap1p2 = null, bp1p2 = null, cp2p1 = null, dp2p1 = null;
            if (a != p1 && a != p2)
            {
                ap1p2 = (Angle)db.FindKey(new Angle(a.ToString() + cutP1 + cutP2));

            }
            if (b != p1 && b != p2)
            {
                bp1p2 = (Angle)db.FindKey(new Angle(b.ToString() + cutP1 + cutP2));

            }
            if (c != p1 && c != p2)
            {
                cp2p1 = (Angle)db.FindKey(new Angle(c.ToString() + cutP2 + cutP1));
            }
            if (d != p1 && d != p2)
            {
                dp2p1 = (Angle)db.FindKey(new Angle(d.ToString() + cutP2 + cutP1));
            }
            if (ap1p2 != null && cp2p1 != null)
            {
                ConsecutiveInterior(ap1p2, cp2p1);
            }
            if (dp2p1 != null && bp1p2 != null)
            {
                ConsecutiveInterior(dp2p1, bp1p2);

            }
            if (ap1p2 != null && dp2p1 != null)
            {
                AlternateInterior(ap1p2, dp2p1);
            }
            if (bp1p2 != null && cp2p1 != null)
            {
                AlternateInterior(bp1p2, cp2p1);
            }

            Line eh = (Line)db.FindKey(new Line(e.ToString() + cutP2));
            Line fg = (Line)db.FindKey(new Line(f.ToString() + cutP1));

            if (!eh.ToString().Contains(cutP1))
                TwoLinesCut1.CreateAnglesFromPoints(eh, cutP1);

            if (!fg.ToString().Contains(cutP2))
                TwoLinesCut1.CreateAnglesFromPoints(fg, cutP2);

        }
        private void ConsecutiveInterior(Angle a1, Angle a2)
        {
            _db.Update(a1, new Node(a1.ToString(), 180 - a2.variable, "סכום זוויות חד צדדיות פנימיות הוא 180", MainNode), DataType.Equations);
            _db.Update(a2, new Node(a2.ToString(), 180 - a1.variable, "סכום זוויות חד צדדיות פנימיות הוא 180", MainNode), DataType.Equations);
        }
        private void AlternateInterior(Angle a1, Angle a2)
        {
            List<Node> parents = new List<Node>() { MainNode };
            _db.UpdateInputsEqual(a1, a2,
                "זוויות מתחלפות שוות זו לזו",
                parents, parents);

        }
        private void corresponding(Angle a1, Angle a2)
        {
            List<Node> parents = new List<Node>() { MainNode };
            _db.UpdateInputsEqual(a1, a2,
                "זוויות מתאימות שוות זו לזו",
                parents, parents);
        }
  
        private void SameAngles(Angle a1,Angle a2)
        {
            _db.Update(a1, new Node(a1.ToString(), a2.variable, "אותה זווית", MainNode), DataType.Equations);
            _db.Update(a2, new Node(a2.ToString(), a1.variable, "אותה זווית", MainNode), DataType.Equations);
        }


    }
}
