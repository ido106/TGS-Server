

using Antlr4.Runtime.Misc;
using Domain;
using Domain.Triangles;

namespace Domain.Solutions
{
    public class Question
    {
        private List<PairWrapper<string, List<string>>> Data = new List<PairWrapper<string, List<string>>>()
        {
            new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>()),//שני ישרים נחתכים
            new PairWrapper<string, List<string>>("ParallelLinesWithTransversal",new List<string>()),//שני ישרים מקבילים נחתכים על ידי ישר שלישי
            new PairWrapper<string, List<string>>("Triangle", new List<string>()),//משולש
            new PairWrapper<string, List<string>>("RightTriangle", new List<string>()),//משולש ישר זווית
            new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>()), //משולש שווה שוקיים
            new PairWrapper<string, List<string>>("EquilateralTriangle", new List<string>()),// משולש שווה צלעות
            new PairWrapper<string, List<string>>("Height_Triangle", new List<string>()),//משולש גובה
            new PairWrapper<string, List<string>>("Median_Triangle", new List<string>()),//תיכון משולש
            new PairWrapper<string, List<string>>("AngleBisector_Triangle", new List<string>()),//חוצה זווית משולש
            new PairWrapper<string, List<string>>("PerpendicularBisector_Triangle", new List<string>()),//אנך אמצעי משולש
            new PairWrapper<string, List<string>>("MeansSection_Triangle", new List<string>()),//קטע אמצעים משולש
            new PairWrapper<string, List<string>>("ExternAngle_Triangle", new List<string>()),//זווית חיצונית משולש
            new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>()),//משוואות ישר
            new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>()),//משוואות זווית = קבוע
            new PairWrapper<string, List<string>>("Area_Triangle", new List<string>()),//שטח
            new PairWrapper<string, List<string>>("Perimeter_Triangle", new List<string>()),//היקף    
            new PairWrapper<string, List<string>>("TrianglesCongruent", new List<string>()),//משולשים חופפים
            new PairWrapper<string, List<string>>("TriangleSimilarity", new List<string>()),//משולשים דומים
            new PairWrapper<string, List<string>>("Inequalities", new List<string>()),//אי שוויונים
            new PairWrapper<string, List<string>>("ParallelLines", new List<string>()),//מקבילים

        };


        public enum TypeQ
        {
            Given,
            Find,
            Prove
        }
        private Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> question;
        public Question(Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q)
        {
            question = q;
            if (!q.ContainsKey(TypeQ.Given))
            {
                question.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>());

            }
            if (!q.ContainsKey(TypeQ.Find))
            {
                question.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>());

            }
            if (!q.ContainsKey(TypeQ.Prove))
            {
                question.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>());

            }

        }

        public List<PairWrapper<string, List<string>>> GetGivenData()
        {
            return question[TypeQ.Given];
        }
        public List<PairWrapper<string, List<string>>> GetFindData()
        {
            return question[TypeQ.Find];
        }
        public List<PairWrapper<string, List<string>>> GetProveData()
        {
            return question[TypeQ.Prove];
        }
        

    }
}
