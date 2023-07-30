using AngouriMath;
using DatabaseLibrary;
using Domain.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AngouriMath.Entity;
using static DatabaseLibrary.Database;

namespace Domain.Triangles
{
    public class TriangleSimilarity
    {
        public static Node IsTriangleSimilarity(Database db, Triangle t1, Triangle t2)
        {
            Node answer = ByAngleAngle(db, t1, t2);
            if (answer != null)
            {
                UpdateSimilarWithAngleBisector(db, t1, t2, answer);
                UpdateSimilarWithMedian(db, t1, t2, answer);
                UpdateSimilarWithHeight(db, t1, t2, answer);
                return answer;
            }
            answer = ByEdgeAngleEdge(db, t1, t2);
            if (answer != null)
            {
                UpdateSimilarWithAngleBisector(db, t1, t2, answer);
                UpdateSimilarWithMedian(db, t1, t2, answer);
                UpdateSimilarWithHeight(db, t1, t2, answer);
                return answer;
            }
            answer = ByEdgeEdgeEdge(db, t1, t2);
            if (answer != null)
            {
                UpdateSimilarWithAngleBisector(db, t1, t2, answer);
                UpdateSimilarWithMedian(db, t1, t2, answer);
                UpdateSimilarWithHeight(db, t1, t2, answer);
                return answer;
            }
            return null;
        }
        //s:133
        private static Node ByEdgeAngleEdge(Database db, Triangle t1, Triangle t2)
        {
            foreach (Angle angle1 in t1.AnglesKeys)
            {
                foreach (Angle angle2 in t2.AnglesKeys)
                {
                    Node eqAngles = db.GetEqualsNode(angle1, angle2);
                    if (eqAngles != null)
                    {
                        Line ab = t1.LinesKeys.Find(l => l.ToString().Contains(angle1.ToString()[1]));
                        Line ac = t1.LinesKeys.Find(l => l.ToString().Contains(angle1.ToString()[1]) && !l.Equals(ab));
                        Line de = t2.LinesKeys.Find(l => l.ToString().Contains(angle2.ToString()[1]));
                        Line df = t2.LinesKeys.Find(l => l.ToString().Contains(angle2.ToString()[1]) && !l.Equals(de));

                        Node answer = FindSimilarTriangles(db, t1, t2, ab, ac, de, df, angle1, angle2);
                        if (answer != null)
                            return answer;

                        answer = FindSimilarTriangles(db, t1, t2, ab, ac, de, df, angle1, angle2, swap: true);
                        if (answer != null)
                            return answer;
                    }
                }
            }

            return null;
        }

        private static Node FindSimilarTriangles(Database db, Triangle t1, Triangle t2, Line ab, Line ac, Line de, Line df, Angle angle1, Angle angle2, bool swap = false)
        {
            var t1Points = t1.PointsKeys;
            var t2Points = t2.PointsKeys;
            var a1 = angle1.ToString()[1];
            var a2 = angle2.ToString()[1];

            string nameT1 = GetNameTriangle(ab, a1, t1Points, swap);
            string nameT2 = GetNameTriangle(de, a2, t2Points, swap);

            foreach (Node abNode in db.HandleEquations.Equations[ab])
            {
                foreach (Node deNode in db.HandleEquations.Equations[de])
                {
                    Entity expr1 = abNode.Expression / deNode.Expression;

                    foreach (Node acNode in db.HandleEquations.Equations[ac])
                    {
                        foreach (Node dfNode in db.HandleEquations.Equations[df])
                        {
                            Entity expr2 = acNode.Expression / dfNode.Expression;
                            if (expr1.Simplify().Equals(expr2.Simplify()))
                            {
                                Node answer = CreateSimilarTriangleNode(nameT1, nameT2, abNode, deNode, acNode, dfNode);
                                UpdateLinesAndAngles(db, nameT1, nameT2, answer);
                                return answer;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static string GetNameTriangle(Line line, char angle, IEnumerable<string> points, bool swap)
        {
            string name;
            if (line.ToString()[0] == angle)
            {
                name = line.ToString()[1].ToString() + angle.ToString() + points.First(p => p != angle.ToString() && p != line.ToString()[1].ToString());
            }
            else
            {
                name = line.ToString()[0].ToString() + angle.ToString() + points.First(p => p != angle.ToString() && p != line.ToString()[0].ToString());
            }

            return swap ? ReverseTriangleName(name) : name;
        }

        private static string ReverseTriangleName(string name)
        {
            return name[2].ToString() + name[1] + name[0];
        }

        private static Node CreateSimilarTriangleNode(string nameT1, string nameT2, Node abNode, Node deNode, Node acNode, Node dfNode)
        {
            return new Node(
                $"∆{nameT1} ∼ ∆{nameT2}",
                null,
                "(משפט דמיון ראשון (צלע, זווית, צלע",
                new List<Node>() { abNode, deNode, acNode, dfNode }
            )
            {
                typeName = null
            };
        }

        //s: 134
        private static Node ByAngleAngle(Database db, Triangle t1, Triangle t2)
        {
            Node n1 = null;
            Node n2 = null;
            string nameT1 = "", nameT2="";
            foreach (Angle a1 in t1.AnglesKeys)
            {
                foreach (Angle a2 in t2.AnglesKeys)
                {
                    if (n1 == null)
                    {
                        n1 = db.GetEqualsNode(a1, a2);
                        if (n1 != null)
                        {
                            nameT1 += a1.ToString()[1].ToString();
                            nameT2 += a2.ToString()[1].ToString();
                            break;
                        }

                    }

                    else if (a1.ToString() != n1.name && a2.ToString() != n1.Expression.ToString() &&
                                a2.ToString() != n1.name && a1.ToString() != n1.Expression.ToString())
                    {
                        n2 = db.GetEqualsNode(a1, a2);
                        if (n2 != null)
                        {
                            nameT1 += a1.ToString()[1].ToString();
                            nameT2 += a2.ToString()[1].ToString();

                            nameT1 += t1.PointsKeys.Find((p) => !nameT1.Contains(p));
                            nameT2 += t2.PointsKeys.Find((p) => !nameT2.Contains(p));

                            Node answer = new Node(
                                $"∆{nameT1} ∼ ∆{nameT2}",
                                null,
                                "(משפט דמיון שני (זווית, זווית",
                                new List<Node>() { n1, n2 }
                                );
                            answer.typeName = null;
                            UpdateLinesAndAngles(db, nameT1, nameT2, answer);
                            return answer;
                        }
                    }

                }
            }
            return null;
        }
    
        // s:135
        private static Node ByEdgeEdgeEdge(Database db, Triangle t1, Triangle t2)
        {
            return null;
        }
        //s: 136
        private static void UpdateSimilarWithAngleBisector(Database db, Triangle t1, Triangle t2, Node similarNode)
        {
            foreach (string point1 in t1.PointsKeys)
            {
                if (t1.AngleBisectors.ContainsKey(point1))
                {
                    AngleBisector a1 = t1.AngleBisectors[point1];
                    if (a1 == null) continue;
                    Angle current = t1.AnglesKeys.Find((a) => a.ToString()[1].ToString() == point1);
                    foreach (Angle a in t2.AnglesKeys)
                    {
                        if (db.GetEqualsNode(current, a) != null)
                        {
                            string point2 = a.ToString()[1].ToString();
                            AngleBisector a2 = t2.AngleBisectors[point2];
                            if (a2 != null)
                            {
                                Line bk = new Line(a1.name);
                                Line eg = new Line(a2.name);
                                Line ab = t1.LinesKeys.Find((l) => l.Equals(new Line(current.ToString()[1].ToString() + point1)));
                                Line de = t2.LinesKeys.Find((l) => l.Equals(new Line(a.ToString()[1].ToString() + point2)));
                                Entity expr = $"({bk.variable}) / ({eg.variable}) = ({ab.variable}) / ({de.variable})";
                                Node n = new Node($"({bk.variable}) / ({eg.variable})", $"({ab.variable}) / ({de.variable})", "חוצי זוויות מתאימות במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", new List<Node>() { a1, a2, similarNode });

                                db.Update(bk, new Node(bk.ToString(), expr.Solve(bk.variable), "חישוב", n), DataType.Equations);
                                db.Update(eg, new Node(eg.ToString(), expr.Solve(eg.variable), "חישוב", n), DataType.Equations);
                                db.Update(ab, new Node(ab.ToString(), expr.Solve(ab.variable), "חישוב", n), DataType.Equations);
                                db.Update(de, new Node(de.ToString(), expr.Solve(de.variable), "חישוב", n), DataType.Equations);
                                break;
                            }

                        }

                    }


                }

            }
        }
        //s: 137
        private static void UpdateSimilarWithMedian(Database db, Triangle t1, Triangle t2, Node similarNode)
        {
            foreach (string point1 in t1.PointsKeys)
            {
                if (t1.AngleBisectors.ContainsKey(point1))
                {
                    Median a1 = t1.Medians[point1];
                    if (a1 == null) continue;
                    Angle current = t1.AnglesKeys.Find((a) => a.ToString()[1].ToString() == point1);
                    foreach (Angle a in t2.AnglesKeys)
                    {
                        if (db.GetEqualsNode(current, a) != null)
                        {
                            string point2 = a.ToString()[1].ToString();
                            Median a2 = t2.Medians[point2];
                            if (a2 != null)
                            {
                                Line bk = new Line(a1.name);
                                Line eg = new Line(a2.name);
                                Line ab = t1.LinesKeys.Find((l) => l.Equals(new Line(current.ToString()[1].ToString() + point1)));
                                Line de = t2.LinesKeys.Find((l) => l.Equals(new Line(a.ToString()[1].ToString() + point2)));
                                Entity expr = $"({bk.variable}) / ({eg.variable}) = ({ab.variable}) / ({de.variable})";
                                Node n = new Node($"({bk.variable}) / ({eg.variable})", $"({ab.variable}) / ({de.variable})", "תיכונים מתאימים במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", new List<Node>() { a1, a2, similarNode });

                                db.Update(bk, new Node(bk.ToString(), expr.Solve(bk.variable), "", n), DataType.Equations);
                                db.Update(eg, new Node(eg.ToString(), expr.Solve(eg.variable), "חישוב", n), DataType.Equations);
                                db.Update(ab, new Node(ab.ToString(), expr.Solve(ab.variable), "חישוב", n), DataType.Equations);
                                db.Update(de, new Node(de.ToString(), expr.Solve(de.variable), "חישוב", n), DataType.Equations);
                                break;
                            }

                        }

                    }


                }

            }
        }
        //s:138
        private static void UpdateSimilarWithHeight(Database db, Triangle t1, Triangle t2, Node similarNode)
        {
            foreach (string point1 in t1.PointsKeys)
            {
                if (t1.Heights.ContainsKey(point1))
                {
                    Height a1 = t1.Heights[point1];
                    if (a1 == null) continue;
                    Angle current = t1.AnglesKeys.Find((a) => a.ToString()[1].ToString() == point1);
                    foreach (Angle a in t2.AnglesKeys)
                    {
                        if (db.GetEqualsNode(current, a) != null)
                        {
                            string point2 = a.ToString()[1].ToString();
                            Height a2 = t2.Heights[point2];
                            if (a2 != null)
                            {
                                Line bk = new Line(a1.name);
                                Line eg = new Line(a2.name);
                                Line ab = t1.LinesKeys.Find((l) => l.Equals(new Line(current.ToString()[1].ToString() + point1)));
                                Line de = t2.LinesKeys.Find((l) => l.Equals(new Line(a.ToString()[1].ToString() + point2)));
                                Entity expr = $"({bk.variable}) / ({eg.variable}) = ({ab.variable}) / ({de.variable})";
                                Node n = new Node($"({bk.variable}) / ({eg.variable})", $"({ab.variable}) / ({de.variable})", "גבהים מתאימים במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", new List<Node>() { a1, a2, similarNode });

                                db.Update(bk, new Node(bk.ToString(), expr.Solve(bk.variable), "חישוב", n), DataType.Equations);
                                db.Update(eg, new Node(eg.ToString(), expr.Solve(eg.variable), "חישוב", n), DataType.Equations);
                                db.Update(ab, new Node(ab.ToString(), expr.Solve(ab.variable), "חישוב", n), DataType.Equations);
                                db.Update(de, new Node(de.ToString(), expr.Solve(de.variable), "חישוב", n), DataType.Equations);
                                break;
                            }

                        }

                    }


                }

            }
        }
        //s:139
        private static void UpdatePerimeter(Database db, Triangle t1, Triangle t2, Node similarNode)
        { 
            Line ab = t1.LinesKeys[0];
            Line de = null;
            foreach (Line l in t2.LinesKeys)
            {
                Node equals = db.GetEqualsNode(ab, l);
                if (equals != null)
                {
                    de = new Line(l.ToString());
                    break;
                }
            }
            Variable p1 = t1.GetPerimeter();
            Variable p2 = t2.GetPerimeter();
            //add to parent main node of p1 and p2
            Entity expr = $"({p1}) / ({p2}) = ({ab.variable}) / ({de.variable})";
            Node n = new Node($"({p1}) / ({p2})", $"({ab.variable}) / ({de.variable})", "ההיקפים של משולשים דומים מתייחסים ה לזה כמו יחס הדמיון שבין המשולשים", new List<Node>() { similarNode });

            //db.Update(bk, new Node(bk.ToString(), expr.Solve(bk.variable), "חוצי זוויות מתאימות במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", n), DataType.Equations);
            //db.Update(eg, new Node(eg.ToString(), expr.Solve(eg.variable), "חוצי זוויות מתאימות במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", n), DataType.Equations);
            db.Update(ab, new Node(ab.ToString(), expr.Solve(ab.variable), "חישוב", n), DataType.Equations);
            db.Update(de, new Node(de.ToString(), expr.Solve(de.variable), "חישוב", n), DataType.Equations);
        
        }
        //s:140
        private static void UpdateArea(Database db, Triangle t1, Triangle t2, Node similarNode)
        {
            Line ab = t1.LinesKeys[0];
            Line de = null;
            foreach (Line l in t2.LinesKeys)
            {
                Node equals = db.GetEqualsNode(ab, l);
                if (equals != null)
                {
                    de = new Line(l.ToString());
                    break;
                }
            }
            Variable a1 = t1.GetArea();
            Variable a2 = t2.GetArea();
            //add to parents main node of a1 and a2
            Entity expr = $"({a1}) / ({a2}) = (({ab.variable}) / ({de.variable}))^2";
            Node n = new Node($"({a1}) / ({a2})", $"(({ab.variable}) / ({de.variable}))^2", "שטחים של משולשים דומים מתייחסים זה לזה כמו ריבוע יחס הדמיון שבין המשולשים", new List<Node>() { similarNode });

            //db.Update(bk, new Node(bk.ToString(), expr.Solve(bk.variable), "חוצי זוויות מתאימות במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", n), DataType.Equations);
            //db.Update(eg, new Node(eg.ToString(), expr.Solve(eg.variable), "חוצי זוויות מתאימות במשולשים דומים מתייחסות זה לזה כמו יחס הדמיון שבין המשולשים", n), DataType.Equations);
            db.Update(ab, new Node(ab.ToString(), expr.Solve(ab.variable), "חישוב", n), DataType.Equations);
            db.Update(de, new Node(de.ToString(), expr.Solve(de.variable), "חישוב", n), DataType.Equations);

        }


        //Helper
        private static void UpdateLinesAndAngles(Database db, string triangleName1, string triangleName2, Node parent)
        {
            string p11 = triangleName1[0].ToString();
            string p12 = triangleName1[1].ToString();
            string p13 = triangleName1[2].ToString();
            Input ab = db.FindKey(new Line(p11 + p12));
            Input bc = db.FindKey(new Line(p12 + p13));
            Input ca = db.FindKey(new Line(p13 + p11));
            Input b = db.FindKey(new Angle(p11 + p12 + p13));
            Input c = db.FindKey(new Angle(p12 + p13 + p11));
            Input a = db.FindKey(new Angle(p13 + p11 + p12));
            string p21 = triangleName2[0].ToString();
            string p22 = triangleName2[1].ToString();
            string p23 = triangleName2[2].ToString();
            Input de = db.FindKey(new Line(p21 + p22));
            Input ef = db.FindKey(new Line(p22 + p23));
            Input fd = db.FindKey(new Line(p23 + p21));
            Input e = db.FindKey(new Angle(p21 + p22 + p23));
            Input f = db.FindKey(new Angle(p22 + p23 + p21));
            Input d = db.FindKey(new Angle(p23 + p21 + p22));

            db.Update(ab, new Node(ab.ToString(), de.variable * (bc.variable / ef.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(ab, new Node(ab.ToString(), de.variable * (ca.variable / fd.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);


            db.Update(bc, new Node(bc.ToString(), ef.variable * (ab.variable / de.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(bc, new Node(bc.ToString(), ef.variable * (ca.variable / fd.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);

            db.Update(ca, new Node(ca.ToString(), fd.variable * (ab.variable / de.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(ca, new Node(ca.ToString(), fd.variable * (bc.variable / ef.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);

            db.Update(b, new Node(b.ToString(), e.variable, "זוויות מתאימות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(c, new Node(c.ToString(), f.variable, "זוויות מתאימות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(a, new Node(a.ToString(), d.variable, "זוויות מתאימות בין משולשים דומים", parent), Database.DataType.Equations);

            db.Update(de, new Node(de.ToString(), ab.variable/ (ca.variable / fd.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(de, new Node(de.ToString(), ab.variable/ (bc.variable / ef.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);

            db.Update(ef, new Node(ef.ToString(), bc.variable/ (ca.variable / fd.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(ef, new Node(ef.ToString(), bc.variable/ (ab.variable / de.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);

            db.Update(fd, new Node(fd.ToString(), ca.variable/ (ab.variable / de.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(fd, new Node(fd.ToString(), ca.variable/ (bc.variable / ef.variable), "יחס צלעות בין משולשים דומים", parent), Database.DataType.Equations);

            db.Update(e, new Node(e.ToString(), b.variable, "זוויות מתאימות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(f, new Node(f.ToString(), c.variable, "זוויות מתאימות בין משולשים דומים", parent), Database.DataType.Equations);
            db.Update(d, new Node(d.ToString(), a.variable, "זוויות מתאימות בין משולשים דומים", parent), Database.DataType.Equations);

        }
  
    }
}
