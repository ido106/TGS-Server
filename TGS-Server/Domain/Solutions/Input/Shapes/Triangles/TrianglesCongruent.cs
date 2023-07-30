using Antlr4.Runtime.Misc;
using DatabaseLibrary;

namespace Domain.Triangles
{
    public static class TrianglesCongruent
    {
        public static Node IsTrianglesCongruent(Database db, Triangle t1, Triangle t2)
        {
            Node answer = ByEdgeAngleEdge(db, t1, t2);
            if (answer != null) return answer;
            answer = ByAngleEdgeAngle(db, t1, t2);
            if (answer != null) return answer;
            answer = ByEdgeEdgeEdge(db, t1, t2);
            if (answer != null) return answer;
            answer = ByEdgeEdgeAngle(db, t1, t2);
            if (answer != null) return answer;
            return null;
        }

        //s:16
        private static Node ByEdgeAngleEdge(Database db, Triangle t1, Triangle t2)
        {
            //Find all pairs from t1's angles equal to t2's angles
            List<Pair<Angle, Angle>> equalsEngles = new List<Pair<Angle, Angle>>();
            int numberOfAngles = t2.AnglesKeys.Count;
            //Run over t1's angles
            foreach (Angle angle1 in t1.AnglesKeys)
            {
                //Run over t2's angles
                foreach (Angle angle2 in t2.AnglesKeys)
                {
                    if (db.GetEqualsNode(angle1, angle2) == null) continue;
                    equalsEngles.Add(new Pair<Angle, Angle>(angle1, angle2));
                }
            }

            //For every pair check if the corresponding lines opposite to the angles are equal
            foreach (Pair<Angle, Angle> angles in equalsEngles)
            {
                Angle a1 = angles.a;
                Angle a2 = angles.b;
                Line t1Line1 = t1.LinesKeys.Find((l) => l.ToString().Contains(a1.ToString()[1]));
                Line t1Line2 = t1.LinesKeys.Find((l) => l.ToString().Contains(a1.ToString()[1]) && l != t1Line1);
                Line t2Line1 = t2.LinesKeys.Find((l) => l.ToString().Contains(a2.ToString()[1]));
                Line t2Line2 = t2.LinesKeys.Find((l) => l.ToString().Contains(a2.ToString()[1]) && l != t2Line1);
                
                Node e1 = db.GetEqualsNode(t1Line1, t2Line1);
                Node e2 = db.GetEqualsNode(t1Line2, t2Line2);
                string nameT1 = null, nameT2=null;
                if (e1 != null && e2 != null)
                {

                    if (t1Line1.ToString()[0] != a1.ToString()[1])
                    {
                        string first1 = t1Line1.ToString()[0].ToString();
                        string second1 = a1.ToString()[1].ToString();
                        nameT1 = first1 + second1 + t1.PointsKeys.Find((p) => p != first1 && p != second1);
                    }
                    else
                    {
                        string first1 = t1Line1.ToString()[1].ToString();
                        string second1 = a1.ToString()[1].ToString();
                        nameT1 = first1 + second1 + t1.PointsKeys.Find((p) => p != first1 && p != second1);

                    }
                    if (t2Line1.ToString()[0] != a2.ToString()[1])
                    {
                        string first2 = t2Line1.ToString()[0].ToString();
                        string second2 = a2.ToString()[1].ToString();
                        nameT2 = first2 + second2 + t2.PointsKeys.Find((p) => p != first2 && p != second2);
                    }
                    else
                    {
                        string first2 = t2Line1.ToString()[1].ToString();
                        string second2 = a2.ToString()[1].ToString();
                        nameT2 = first2 + second2 + t2.PointsKeys.Find((p) => p != first2 && p != second2);

                    }


                    Node answer = new Node(
                        $"∆{nameT1} ≅ ∆{nameT2}",
                        null,
                        "(משפט חפיפה ראשון (צלע, זווית, צלע",
                        new List<Node>() { e1, e2, db.GetEqualsNode(a1, a2) }
                        );
                    answer.typeName = null;
                    UpdateLinesAndAngles(db,nameT1,nameT2,answer);
                    return answer;
                }

                Node e3 = db.GetEqualsNode(t1Line1, t2Line2);
                Node e4 = db.GetEqualsNode(t1Line2, t2Line1);
                if (e3 != null && e4 != null)
                {
                    if (t1Line1.ToString()[0] != a1.ToString()[1])
                    {
                        string first = t1Line1.ToString()[0].ToString();
                        string second = a1.ToString()[1].ToString();
                        nameT1 = first + second + t1.PointsKeys.Find((p) => p != first && p != second);
                        
                    }
                    else
                    {
                        string first = t1Line1.ToString()[1].ToString();
                        string second = a1.ToString()[1].ToString();
                        nameT1 = first + second + t1.PointsKeys.Find((p) => p != first && p != second);

                    }
                    if (t2Line2.ToString()[0] != a2.ToString()[1])
                    {
                        string first = t2Line2.ToString()[0].ToString();
                        string second = a2.ToString()[1].ToString();
                        nameT2 = first + second + t2.PointsKeys.Find((p) => p != first && p != second);

                    }
                    else
                    {
                        string first = t2Line2.ToString()[1].ToString();
                        string second = a2.ToString()[1].ToString();
                        nameT2 = first + second + t2.PointsKeys.Find((p) => p != first && p != second);

                    }

                    Node answer = new Node(
                       $"∆{nameT1} ≅ ∆{nameT2}",
                       null,
                       "(משפט חפיפה ראשון (צלע, זווית, צלע",
                       new List<Node>() { e3, e4, db.GetEqualsNode(a1, a2) }
                       );
                    answer.typeName = null;
                    UpdateLinesAndAngles(db, nameT1, nameT2, answer);
                    return answer;
                }
            }
            return null;
        }


        //s: 17
        private static Node ByAngleEdgeAngle(Database db, Triangle t1, Triangle t2)

        {
            //Find all pairs from t1's lines equal to t2's lines
            List<Pair<Line, Line>> equalsLines = new List<Pair<Line, Line>>();
            int numberOfLines = t2.LinesKeys.Count;
            //Run over t1's lines
            foreach (Line line1 in t1.LinesKeys)
            {
                //Run over t2's lines
                foreach (Line line2 in t2.LinesKeys)
                {
                    if (db.GetEqualsNode(line1, line2) == null) continue;
                    equalsLines.Add(new Pair<Line, Line>(line1, line2));
                }
            }

            //For every pair check if the corresponding angles between the lines are equal
            foreach (Pair<Line, Line> lines in equalsLines)
            {
                Line l1 = lines.a;
                Line l2 = lines.b;

                Angle t1Angle1 = t1.AnglesKeys.Find((a) => a.ToString()[1].Equals(l1.ToString()[0]));
                Angle t1Angle2 = t1.AnglesKeys.Find((a) => a.ToString()[1].Equals(l1.ToString()[1]) && a != t1Angle1);
                Angle t2Angle1 = t2.AnglesKeys.Find((a) => a.ToString()[1].Equals(l2.ToString()[0]));
                Angle t2Angle2 = t2.AnglesKeys.Find((a) => a.ToString()[1].Equals(l2.ToString()[1]) && a != t2Angle1);

                Node e1 = db.GetEqualsNode(t1Angle1, t2Angle1);
                Node e2 = db.GetEqualsNode(t1Angle2, t2Angle2);
                string nameT1 = null, nameT2 = null;
                if (e1 != null && e2 != null)
                {
                    string first1 = t1Angle1.ToString()[1].ToString();
                    string second1 = t1Angle2.ToString()[1].ToString();
                    nameT1 = first1+ second1+t1.PointsKeys.Find((p) => p != first1 && p != second1);
                    string first2 = t2Angle1.ToString()[1].ToString();
                    string second2 = t2Angle2.ToString()[1].ToString();
                    nameT2 = first2 + second2 + t2.PointsKeys.Find((p) => p != first2 && p != second2);

                  

                    Node answer = new Node(
                        $"∆{nameT1} ≅ ∆{nameT2}",
                        null,
                        "(משפט חפיפה שני (זווית, צלע, זווית",
                        new List<Node>() { e1, e2, db.GetEqualsNode(l1, l2) }
                        );
                    answer.typeName = null;
                    UpdateLinesAndAngles(db,nameT1,nameT2,answer);
                    return answer;
                }
                Node e3 = db.GetEqualsNode(t1Angle1, t2Angle2);
                Node e4 = db.GetEqualsNode(t1Angle2, t2Angle1);
                if (e3 != null && e4 != null)
                {
                    string first1 = t1Angle1.ToString()[1].ToString();
                    string second1 = t1Angle2.ToString()[1].ToString();
                    nameT1 = first1 + second1 + t1.PointsKeys.Find((p) => p != first1 && p != second1);

                    string first2 = t2Angle2.ToString()[1].ToString();
                    string second2 = t2Angle1.ToString()[1].ToString();
                    nameT1 = first2 + second2 + t2.PointsKeys.Find((p) => p != first2 && p != second2);
                    Node answer = new Node(
                       $"∆{t1.variable} ≅ ∆{t2.variable}",
                       null,
                       "(משפט חפיפה שני (זווית, צלע, זווית",
                       new List<Node>() { e3, e4, db.GetEqualsNode(l1, l2) }
                       );
                    answer.typeName = null;
                    UpdateLinesAndAngles(db, nameT1, nameT2, answer);
                    return answer;
                }
            }
            return null;
        }
        //s: 18
        private static Node ByEdgeEdgeEdge(Database db, Triangle t1, Triangle t2)
        {
            List<Line> copyLines2 = t2.LinesKeys.ToList();

            List<Node> equalsNodes = new List<Node>();
            foreach (Line line1 in t1.LinesKeys)
            {
                foreach (Line line2 in copyLines2)
                {
                    Node eqaulsNode = db.GetEqualsNode(line1, line2);
                    if (eqaulsNode != null)
                    {
                        equalsNodes.Add(eqaulsNode);
                        copyLines2.Remove(line2);
                        break;
                    }
                }
            }
            List<Line> temp = t2.LinesKeys;
            if (equalsNodes.Count == 3)
            {
                string t1l1 = equalsNodes[0].name;
                string t2l1 = equalsNodes[0].Expression.ToString();
                string t1l2 = equalsNodes[1].name;
                string t2l2 = equalsNodes[1].Expression.ToString();
                string a1 = t1l1.First((c) => t1l2.Contains(c)).ToString();
                string a2 = t2l1.First((c) => t2l2.Contains(c)).ToString();
                string nameT1 = a1 +
                    (t1l1[0].ToString() != a1 ? t1l1[0].ToString() : t1l1[1].ToString()) +
                    (t1l2[0].ToString() != a1 ? t1l2[0].ToString() : t1l2[1].ToString());
                string nameT2 = a2 +
                   (t2l1[0].ToString() != a2 ? t2l1[0].ToString() : t2l1[1].ToString()) +
                   (t2l2[0].ToString() != a2 ? t2l2[0].ToString() : t2l2[1].ToString());
                Node answer = new Node(
                        $"∆{t1.variable} ≅ ∆{t2.variable}",
                        null,
                        "(משפט חפיפה שלישי (צלע, צלע, צלע",
                        equalsNodes
                        );
                answer.typeName = null;
                UpdateLinesAndAngles(db,nameT1,nameT2,answer);
                return answer;
            }
            return null;
        }
        private static Node HelperCreateForS4(Triangle t1, Triangle t2, Node smallerEqual, List<Node> parents)
        {
            if (smallerEqual != null)
            {
                Node answer = new Node(
                                     $"∆{t1.variable} ≅ ∆{t2.variable}",
                                     null,
                                     "(משפט חפיפה רביעי (צלע, צלע, זווית",
                                     parents
                                     );
                answer.typeName = null;
                return answer;
            }
            return null;
        }
        //s: 19
        private static Node ByEdgeEdgeAngle(Database db, Triangle t1, Triangle t2)
        {
            //Find all pairs from t1's angles equal to t2's angles
            List<Pair<Angle, Angle>> equalsEngles = new List<Pair<Angle, Angle>>();
            int numberOfAngles = t2.AnglesKeys.Count;
            //Run over t1's angles
            foreach (Angle angle1 in t1.AnglesKeys)
            {
                //Run over t2's angles
                foreach (Angle angle2 in t2.AnglesKeys)
                {
                    if (db.GetEqualsNode(angle1, angle2) == null) continue;
                    equalsEngles.Add(new Pair<Angle, Angle>(angle1, angle2));
                }
            }

            //For every pair check if the corresponding lines are equals (when the bigger line opposite to angle)
            foreach (Pair<Angle, Angle> angles in equalsEngles)
            {
                //Init angles
                Angle a1 = angles.a;
                Angle a2 = angles.b;
                //Init t1's lines to compare
                Line t1Bigger = t1.LinesKeys.Find((l) =>
                l.ToString().Contains(a1.ToString()[0]) && l.ToString().Contains(a1.ToString()[2]));
                Line t1SmallerOpt1 = t1.LinesKeys.Find((l) => l != t1Bigger);
                Line t1SmallerOpt2 = t1.LinesKeys.Find((l) => l != t1Bigger && l != t1SmallerOpt1);

                //Init t2's lines to compare
                Line t2Bigger = t2.LinesKeys.Find((l) =>
                     l.ToString().Contains(a2.ToString()[0]) && l.ToString().Contains(a2.ToString()[2]));
                Line t2SmallerOpt1 = t2.LinesKeys.Find((l) => l != t2Bigger);
                Line t2SmallerOpt2 = t2.LinesKeys.Find((l) => l != t2Bigger && l != t2SmallerOpt1);

                Node biggerEqual = db.GetEqualsNode(t1Bigger, t2Bigger);
                //If bigger lines not equal
                if (biggerEqual == null) continue;

                //Check line is really bigger
                Node t1e1 = db.GetBiggerNode(t1Bigger, t1SmallerOpt1);
                Node t1e2 = db.GetBiggerNode(t1Bigger, t1SmallerOpt2);
                Node t2e1 = db.GetBiggerNode(t2Bigger, t2SmallerOpt1);
                Node t2e2 = db.GetBiggerNode(t2Bigger, t2SmallerOpt2);

                //Init result node
                Node answer = null;
                Node anglesEquals = db.GetEqualsNode(a1, a2);
                //Check all possible options
                if (t1e1 != null)
                {
                    Node smallerEqual = db.GetEqualsNode(t1SmallerOpt1, t2SmallerOpt1);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                        new List<Node>() { anglesEquals, t1e1, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                        //UpdateLinesAndAngles();
                        return answer;
                    }
                    smallerEqual = db.GetEqualsNode(t1SmallerOpt1, t2SmallerOpt2);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                      new List<Node>() { anglesEquals, t1e1, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                        //UpdateLinesAndAngles();
                        return answer;
                    }

                }
                if (t1e2 != null)
                {
                    Node smallerEqual = db.GetEqualsNode(t1SmallerOpt2, t2SmallerOpt1);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                        new List<Node>() { anglesEquals, t1e2, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                       // UpdateLinesAndAngles();
                        return answer;
                    }
                    smallerEqual = db.GetEqualsNode(t1SmallerOpt2, t2SmallerOpt2);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                      new List<Node>() { anglesEquals, t1e2, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                       // UpdateLinesAndAngles();
                        return answer;
                    }
                }
                if (t2e1 != null)
                {
                    Node smallerEqual = db.GetEqualsNode(t2SmallerOpt1, t1SmallerOpt1);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                        new List<Node>() { anglesEquals, t2e1, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                        //UpdateLinesAndAngles();
                        return answer;
                    }
                    smallerEqual = db.GetEqualsNode(t2SmallerOpt1, t1SmallerOpt2);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                      new List<Node>() { anglesEquals, t2e1, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                       // UpdateLinesAndAngles();
                        return answer;
                    }
                }
                if (t2e2 != null)
                {
                    Node smallerEqual = db.GetEqualsNode(t2SmallerOpt2, t1SmallerOpt1);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                        new List<Node>() { anglesEquals, t2e2, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                       // UpdateLinesAndAngles();
                        return answer;
                    }
                    smallerEqual = db.GetEqualsNode(t2SmallerOpt2, t1SmallerOpt2);
                    answer = HelperCreateForS4(t1, t2, smallerEqual,
                      new List<Node>() { anglesEquals, t2e2, smallerEqual, biggerEqual });
                    if (answer != null)
                    {
                       // UpdateLinesAndAngles();
                        return answer;
                    }
                }
            }
            return null;
        }
        private static void UpdateLinesAndAngles(Database db, string triangleName1, string triangleName2,Node parent)
        {
            string p11 = triangleName1[0].ToString();
            string p12 = triangleName1[1].ToString();
            string p13 = triangleName1[2].ToString();
            Input l11 = db.FindKey(new Line(p11 + p12));
            Input l12 = db.FindKey(new Line(p12 + p13));
            Input l13 = db.FindKey(new Line(p13 + p11));
            Input a11 = db.FindKey(new Angle(p11 + p12 + p13));
            Input a12 = db.FindKey(new Angle(p12 + p13 + p11));
            Input a13 = db.FindKey(new Angle(p13 + p11 + p12));
            string p21 = triangleName2[0].ToString();
            string p22 = triangleName2[1].ToString();
            string p23 = triangleName2[2].ToString();
            Input l21 = db.FindKey(new Line(p21 + p22));
            Input l22 = db.FindKey(new Line(p22 + p23));
            Input l23 = db.FindKey(new Line(p23 + p21));
            Input a21 = db.FindKey(new Angle(p21 + p22 + p23));
            Input a22 = db.FindKey(new Angle(p22 + p23 + p21));
            Input a23 = db.FindKey(new Angle(p23 + p21 + p22));
            db.Update(l11, new Node(l11.ToString(), l21.variable, "צלעות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(l12, new Node(l12.ToString(), l22.variable, "צלעות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(l13, new Node(l13.ToString(), l23.variable, "צלעות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(a11, new Node(a11.ToString(), a21.variable, "זוויות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(a12, new Node(a12.ToString(), a22.variable, "זוויות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(a13, new Node(a13.ToString(), a23.variable, "זוויות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);

            db.Update(l21, new Node(l21.ToString(), l11.variable, "צלעות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(l22, new Node(l22.ToString(), l12.variable, "צלעות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(l23, new Node(l23.ToString(), l13.variable, "צלעות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(a21, new Node(a21.ToString(), a11.variable, "זוויות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(a22, new Node(a22.ToString(), a12.variable, "זוויות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);
            db.Update(a23, new Node(a23.ToString(), a13.variable, "זוויות מתאימות בין משולשים חופפים", parent), Database.DataType.Equations);



        }
      
    }
}
