using DatabaseLibrary;
using Domain.Triangles;
using Domain;
using NUnit.Framework;
using static DatabaseLibrary.Database;
using static Domain.Solutions.Question;
using Domain.Solutions;

namespace TGS_Server
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            // Arrange
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            // Act
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ADC")][0].Expression.ToString());
        }
        [Test]
        public void Test2()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ADB")][0].Expression.ToString());

        }
        [Test]
        public void Test3()
        {

            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AB"), new Node("AB", 5, "נתון"), DataType.Equations);
            database.Update(new Line("BD"), new Node("BD", 4, "נתון"), DataType.Equations);

            // Assert
            Assert.AreEqual("3", database.HandleEquations.Equations[new Line("AD")][0].Expression.ToString());


        }
        [Test]
        public void Test4()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            database.Update(new Line("AB"), new Node("AB", 5, "נתון"), DataType.Equations);
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("BD"), new Node("BD", 4, "נתון"), DataType.Equations);

            // Assert
            Assert.AreEqual("3", database.HandleEquations.Equations[new Line("AD")][0].Expression.ToString());


        }
        [Test]
        public void Test5()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AD"), new Node("AD", 3, "נתון"), DataType.Equations);
            database.Update(new Line("BD"), new Node("BD", 4, "נתון"), DataType.Equations);

            // Assert
            Assert.AreEqual("5", database.HandleEquations.Equations[new Line("AB")][0].Expression.ToString());


        }
        [Test]
        public void Test6()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AD"), new Node("AD", 3, "נתון"), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", 5, "נתון"), DataType.Equations);

            // Assert
            Assert.AreEqual("4", database.HandleEquations.Equations[new Line("BD")][0].Expression.ToString());


        }
        [Test]
        public void Test7()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateHeight("B", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("BA"), new Node("BA", 5, "נתון"), DataType.Equations);
            database.Update(new Line("BD"), new Node("BD", 3, "נתון"), DataType.Equations);

            // Assert
            Assert.AreEqual("4", database.HandleEquations.Equations[new Line("AD")][0].Expression.ToString());


        }
        [Test]
        public void Test8()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            database.Update(new Line("AC"), new Node("AC", 5, "נתון"), DataType.Equations);
            triangle.UpdateHeight("C", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("BA"), new Node("BA", 10, "נתון"), DataType.Equations);
            database.Update(new Line("BD"), new Node("BD", 7, "נתון"), DataType.Equations);

            // Assert
            Assert.AreEqual("4", database.HandleEquations.Equations[new Line("DC")][0].Expression.ToString());

        }
        [Test]
        public void Test9()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            triangle.UpdateMedian("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("BD"), new Node("BD", 3, "נתון"), DataType.Equations);
            // Assert
            Assert.AreEqual("3", database.HandleEquations.Equations[new Line("DC")][0].Expression.ToString());


        }
        [Test]
        public void Test10()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AD"), new Node("AD", 3, "נתון"), DataType.Equations);
            database.Update(new Line("AC"), new Node("AC", 4, "נתון"), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", 5, "נתון"), DataType.Equations);

            //Dictionary<string, string> result = new Solution(database, new List<Input>() { new Line("BD"), new Line("DC"), new Line("BC") }, null).GetSolution();

            // Assert
            Assert.AreEqual("4", database.HandleEquations.Equations[new Line("BD")][0].Expression.ToString());
            Assert.AreEqual("sqrt(7)", database.HandleEquations.Equations[new Line("DC")][0].Expression.ToString());
            Assert.AreEqual("sqrt(7) + 4", database.HandleEquations.Equations[new Line("BC")][0].Expression.ToString());


        }
        [Test]
        public void Test11()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");
            triangle.UpdateMedian("B", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AD"), new Node("AD", 3, "נתון"), DataType.Equations);

            //Dictionary<string, string> result = new Solution(database, new List<Input>() { new Line("DC"), new Line("AC") }, null).GetSolution();

            // Assert
            Assert.AreEqual("3", database.HandleEquations.Equations[new Line("DC")][0].Expression.ToString());
            Assert.AreEqual("6", database.HandleEquations.Equations[new Line("AC")][0].Expression.ToString());


        }
        [Test]
        public void Test12()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            database.Update(new Angle("ACB"), new Node("ACB", 30, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("CAB"), new Node("CAB", 70, "נתון", triangle.GetMainNode()), DataType.Equations);

            triangle.UpdateAngleBisector("B", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AD"), new Node("AD", 3, "נתון"), DataType.Equations);

            // Dictionary<string, string> result = new Solution(database, new List<Input>()
            // { new Angle("ABD"), new Angle("ABC"),  new Angle("CBD") }, null).GetSolution();

            // Assert
            Assert.AreEqual("40", database.HandleEquations.Equations[new Angle("ABD")][0].Expression.ToString());
            Assert.AreEqual("80", database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("40", database.HandleEquations.Equations[new Angle("CBD")][0].Expression.ToString());


        }
        [Test]
        public void Test13()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateAngleBisector("A", "D", "נתון", triangle.GetMainNode());


            database.Update(new Angle("ABD"), new Node("ABD", 30, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("ACD"), new Node("ACD", 70, "נתון", triangle.GetMainNode()), DataType.Equations);

            database.Update(new Line("AD"), new Node("AD", 3, "נתון"), DataType.Equations);

            // Dictionary<string, string> result = new Solution(database, new List<Input>()
            // { new Angle("BAD"), new Angle("BAC"),  new Angle("DAC") }, null).GetSolution();
            // Assert
            Assert.AreEqual("40", database.HandleEquations.Equations[new Angle("DAC")][0].Expression.ToString());


        }

        //***********************       IsoscelesTriangle    שווה שוקיים  ***************************************************
        [Test]
        public void Test14()
        {
            Database database = new Database();
            IsoscelesTriangle triangle = new IsoscelesTriangle(database, "A", "B", "C", new Line("AB"), "נתון");
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());

            //Dictionary<string, string> result = new Solution(database, new List<Input>() { new Angle("ADC") }, null).GetSolution();

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ADC")][0].Expression.ToString());

        }
        [Test]
        public void Test15()
        {
            Database database = new Database();
            IsoscelesTriangle triangle = new IsoscelesTriangle(database, "A", "B", "C", new Line("AB"), "נתון");
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());

            //Dictionary<string, string> result = new Solution(database, new List<Input>() { new Angle("ADB") }, null).GetSolution();

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ADB")][0].Expression.ToString());


        }
        [Test]
        public void Test16()
        {
            Database database = new Database();
            IsoscelesTriangle triangle = new IsoscelesTriangle(database, "A", "B", "C", new Line("AC"), "נתון");
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AB"), new Node("AB", 5, "נתון"), DataType.Equations);
            database.Update(new Line("BD"), new Node("BD", 4, "נתון"), DataType.Equations);


            //Dictionary<string, string> result = new Solution(database, new List<Input>() { new Line("AD") }, null).GetSolution();

            // Assert
            Assert.AreEqual("3", database.HandleEquations.Equations[new Line("AD")][0].Expression.ToString());


        }
        [Test]
        public void Test17()
        {
            Database database = new Database();
            IsoscelesTriangle triangle = new IsoscelesTriangle(database, "A", "B", "C", new Line("CB"), "נתון");

            database.Update(new Line("AB"), new Node("AB", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("ABC"), new Node("ABC", 40, "נתון"), DataType.Equations);
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("BD"), new Node("BD", 4, "נתון"), DataType.Equations);


            //Dictionary<string, string> result = new Solution(database, new List<Input>()
            //{new Line("AB") ,new Line("AC") , new Line("AD") , new Line("BC") ,new Line("BD") ,  new Line("CD") ,
            //new Angle("ABD") ,new Angle("ACD") , new Angle("ADC") ,  new Angle("BCA") ,  new Angle("BDA") ,new Angle("CAD"),  new Angle("CAB"),}, null).GetSolution();

            // Assert
            Assert.AreEqual("100", database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
            Assert.AreEqual("4", database.HandleEquations.Equations[new Line("CD")][0].Expression.ToString());


        }
        [Test]
        public void Test18()
        {
            Database database = new Database();
            IsoscelesTriangle triangle = new IsoscelesTriangle(database, "A", "B", "C", new Line("CB"), "נתון");

            database.Update(new Line("AB"), new Node("AB", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("ABC"), new Node("ABC", 40, "נתון"), DataType.Equations);
            triangle.UpdateMedian("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("BD"), new Node("BD", 4, "נתון"), DataType.Equations);

            // Dictionary<string, string> result = new Solution(database, new List<Input>()
            //  {  new Line("BC") ,  new Angle("ADC") }, null).GetSolution();

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ADC")][0].Expression.ToString());
            Assert.AreEqual("8", database.HandleEquations.Equations[new Line("BC")][0].Expression.ToString());


        }
        [Test]
        public void Test19()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");


            database.Update(new Line("AB"), new Node("AB", 5, "נתון", triangle.GetMainNode()), DataType.Equations);

            database.Update(new Line("BC"), new Node("BC", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("BAC"), new Node("BAC", 50, "נתון", triangle.GetMainNode()), DataType.Equations);
            IsoscelesTriangle.IsShape(triangle, database);

            //Dictionary<string, string> result = new Solution(database, new List<Input>() { new Angle("ACB") }, null).GetSolution();

            // Assert
            Assert.AreEqual("50", database.HandleEquations.Equations[new Angle("ACB")][0].Expression.ToString());

        }

        //***********************       RightTriangle    ישר זווית  ***************************************************
        [Test]
        public void Test20()
        {
            Database database = new Database();
            RightTriangle triangle = new RightTriangle(database, "A", "B", "C", new Angle("ABC"), "נתון");

            triangle.UpdateMedian("B", "D", "נתון", triangle.GetMainNode());
            database.Update(new Angle("BAC"), new Node("BAC", 50, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("BD"), new Node("BD", 5, "נתון"), DataType.Equations);

            IsoscelesTriangle.IsShape(triangle, database);

            /* Dictionary<string, string> result = new Solution(database, new List<Input>()
             {
                 new Angle("BCA") ,
                 new Line("AD"),
                 new Line("DC"),
             },
             null).GetSolution();*/

            // Assert
            Assert.AreEqual("5", database.HandleEquations.Equations[new Line("DC")][0].Expression.ToString());

        }
        [Test]
        public void Test21()
        {
            Database database = new Database();
            RightTriangle triangle = new RightTriangle(database, "A", "B", "C", new Angle("ABC"), "נתון");

            triangle.UpdateMedian("B", "D", "נתון", triangle.GetMainNode());
            database.Update(new Angle("BAC"), new Node("BAC", 50, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("AD"), new Node("BD", 5, "נתון"), DataType.Equations);

            IsoscelesTriangle.IsShape(triangle, database);

            /*Dictionary<string, string> result = new Solution(database, new List<Input>()
            {
                new Angle("BCA") ,
                new Line("DB"),
                new Line("DC"),
            },
            null).GetSolution();*/

            // Assert
            Assert.AreEqual("5", database.HandleEquations.Equations[new Line("DC")][0].Expression.ToString());

        }
        [Test]
        public void Test22()
        {
            Database database = new Database();
            RightTriangle triangle = new RightTriangle(database, "A", "B", "C", new Angle("CAB"), "נתון");

            triangle.UpdateMedian("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Angle("ABC"), new Node("ABC", 50, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("DC"), new Node("DC", 5, "נתון"), DataType.Equations);

            IsoscelesTriangle.IsShape(triangle, database);

            /* Dictionary<string, string> result = new Solution(database, new List<Input>()
             {
                 new Angle("BCA") ,
                 new Line("DB"),
                 new Line("AD"),
             },
             null).GetSolution();*/

            // Assert
            Assert.AreEqual("5", database.HandleEquations.Equations[new Line("AD")][0].Expression.ToString());

        }
        [Test]
        public void Test23()
        {
            Database database = new Database();
            RightTriangle triangle = new RightTriangle(database, "A", "B", "C", new Angle("CAB"), "נתון");

            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Angle("ABC"), new Node("ABC", 50, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", 5, "נתון"), DataType.Equations);
            database.Update(new Line("BC"), new Node("BC", 15, "נתון"), DataType.Equations);

            IsoscelesTriangle.IsShape(triangle, database);

            /* Dictionary<string, string> result = new Solution(database, new List<Input>()
             {
                 new Angle("BCA") ,
                  new Angle("CAB") ,
                   new Angle("ABC") ,
                    new Angle("BAD") ,
                     new Angle("DAC") ,
                         new Angle("CDA") ,
                             new Angle("ADB") ,
                 new Line("AD"),
                 new Line("AC"),
             },
             null).GetSolution();*/

            // Assert
            Assert.AreEqual("sqrt(200)", database.HandleEquations.Equations[new Line("CA")][0].Expression.ToString());

        }
        [Test]
        public void Test24()
        {
            Database database = new Database();
            RightTriangle triangle = new RightTriangle(database, "A", "B", "C", new Angle("ACB"), "נתון");

            database.Update(new Line("BC"), new Node("BC", 5, "נתון", triangle.GetMainNode()), DataType.Equations);

            database.Update(new Line("AB"), new Node("AB", 10, "נתון", triangle.GetMainNode()), DataType.Equations);

            triangle.UpdateAngleBisector("B", "D", "נתון", triangle.GetMainNode());

            triangle.IsAngle30();
            triangle.ReverseIsAngle30();

            IsoscelesTriangle.IsShape(triangle, database);


            /*Dictionary<string, string> result = new Solution(database, new List<Input>()
            {
                new Angle("ABD") ,
                 new Angle("CAB") ,
               new Line("AC"),
            },
            null).GetSolution();*/

            // Assert
            Assert.AreEqual("sqrt(75)", database.HandleEquations.Equations[new Line("AC")][0].Expression.ToString());

        }
        [Test]
        public void Test25()
        {
            Database database = new Database();
            RightTriangle triangle = new RightTriangle(database, "A", "B", "C", new Angle("ACB"), "נתון");

            database.Update(new Line("BC"), new Node("BC", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("ABC"), new Node("ABC", 60, "נתון", triangle.GetMainNode()), DataType.Equations);

            triangle.IsAngle30();
            triangle.ReverseIsAngle30();

            /*Dictionary<string, string> result = new Solution(database, new List<Input>()
            {
               new Line("AC"),
               new Line("AB"),
            },
            null).GetSolution();*/

            // Assert
            Assert.AreEqual("sqrt(75)", database.HandleEquations.Equations[new Line("CA")][0].Expression.ToString());

        }
        [Test]
        public void Test26()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateMedian("C", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("CD"), new Node("CD", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", 10, "נתון", triangle.GetMainNode()), DataType.Equations);

            RightTriangle.IsShape(triangle, database);

            /* Dictionary<string, string> result = new Solution(database, new List<Input>()
             {
                new Angle("ACB")
             },
             null).GetSolution();*/

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ACB")][0].Expression.ToString());

        }
        [Test]
        public void Test27()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateMedian("C", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("CD"), new Node("CD", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("AD"), new Node("AD", new Line("DC").variable, "נתון", triangle.GetMainNode()), DataType.Equations);

            RightTriangle.IsShape(triangle, database);


            /*Dictionary<string, string> result = new Solution(database, new List<Input>()
            {
               new Angle("ACB")
            },
            null).GetSolution();*/

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ACB")][0].Expression.ToString());

        }
        [Test]
        public void Test28()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateMedian("C", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("CD"), new Node("CD", new Line("AB").variable / 2, "נתון", triangle.GetMainNode()), DataType.Equations);

            RightTriangle.IsShape(triangle, database);

            /* Dictionary<string, string> result = new Solution(database, new List<Input>()
             {
                new Angle("ACB")
             },
             null).GetSolution();*/

            // Assert
            Assert.AreEqual("90", database.HandleEquations.Equations[new Angle("ACB")][0].Expression.ToString());

        }
        [Test]
        public void Test29()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            database.Update(new Angle("ABC"), new Node("ABC", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("BCA"), new Node("BCA", 5, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", 7, "נתון", triangle.GetMainNode()), DataType.Equations);

            IsoscelesTriangle.IsShape(triangle, database);


            // Dictionary<string, string> result = new Solution(database, new List<Input>()
            // { new Line("AC")}, null).GetSolution();

            // Assert
            Assert.AreEqual("7", database.HandleEquations.Equations[new Line("CA")][0].Expression.ToString());

        }
        [Test]
        public void Test30()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            database.Update(new Angle("ABC"), new Node("ABC", new Angle("BCA").variable, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Angle("BCA"), new Node("BCA", new Angle("ABC").variable, "נתון", triangle.GetMainNode()), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", 7, "נתון", triangle.GetMainNode()), DataType.Equations);

            IsoscelesTriangle.IsShape(triangle, database);


            //Dictionary<string, string> result = new Solution(database, new List<Input>()
            //{new Line("AC") },  null).GetSolution();

            // Assert
            Assert.AreEqual("7", database.HandleEquations.Equations[new Line("AC")][0].Expression.ToString());

        }
        [Test]
        public void Test31()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateMedian("A", "D", "נתון", triangle.GetMainNode());
            triangle.UpdateHeight("A", "D", "נתון", triangle.GetMainNode());

            IsoscelesTriangle.IsShape(triangle, database);

            database.Update(new Line("AB"), new Node("AB", 7, "נתון", triangle.GetMainNode()), DataType.Equations);


            // Dictionary<string, string> result = new Solution(database, new List<Input>()
            // { new Line("AC") },null).GetSolution();

            // Assert
            Assert.AreEqual("7", database.HandleEquations.Equations[new Line("AC")][0].Expression.ToString());

        }
        [Test]
        public void Test32()
        {
            Database database = new Database();
            Triangle triangle = new Triangle(database, "A", "B", "C", "נתון");

            triangle.UpdateMedian("A", "D", "נתון", triangle.GetMainNode());
            triangle.UpdateAngleBisector("A", "D", "נתון", triangle.GetMainNode());
            database.Update(new Line("AB"), new Node("AB", 7, "נתון", triangle.GetMainNode()), DataType.Equations);
            IsoscelesTriangle.IsShape(triangle, database);

            // Dictionary<string, string> result = new Solution(database, new List<Input>()
            //{ new Line("AC") },null).GetSolution();

            // Assert
            Assert.AreEqual("7", database.HandleEquations.Equations[new Line("AC")][0].Expression.ToString());

        }
        //************************************** חפיפת משולשים ************************************************
        [Test]
        public void Test33()
        {
            Database database = new Database();
            Triangle t1 = new Triangle(database, "A", "B", "C", "נתון");
            Triangle t2 = new Triangle(database, "D", "E", "F", "נתון");
            database.Update(new Line("AB"), new Node("AB", 10, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(new Line("DE"), new Node("DE", 10, "נתון", t2.GetMainNode()), DataType.Equations);
            database.Update(new Line("BC"), new Node("BC", 15, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(new Line("EF"), new Node("EF", 15, "נתון", t2.GetMainNode()), DataType.Equations);
            database.Update(new Angle("ABC"), new Node("ABC", 100, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(new Angle("DEF"), new Node("AB", 100, "נתון", t2.GetMainNode()), DataType.Equations);

            // Assert
            Assert.NotNull(TrianglesCongruent.IsTrianglesCongruent(database, t1, t2));

        }
        [Test]
        public void Test34()
        {
            Database database = new Database();
            IsoscelesTriangle t1 = new IsoscelesTriangle(database, "A", "B", "C", new Line("CA"), "נתון");
            Triangle t2 = new Triangle(database, "D", "E", "F", "נתון");

            List<Node> list = new List<Node>() { t1.GetMainNode(), t2.GetMainNode() };

            database.Update(new Line("AB"), new Node("AB", new Line("DE").variable, "נתון", list), DataType.Equations);
            database.Update(new Line("DE"), new Node("DE", new Line("AB").variable, "נתון", list), DataType.Equations);
            database.Update(new Line("BC"), new Node("BC", new Line("EF").variable, "נתון", list), DataType.Equations);
            database.Update(new Line("EF"), new Node("EF", new Line("BC").variable, "נתון", list), DataType.Equations);
            database.Update(new Angle("ABC"), new Node("ABC", new Angle("DEF").variable, "נתון", list), DataType.Equations);
            database.Update(new Angle("DEF"), new Node("AB", new Angle("ABC").variable, "נתון", list), DataType.Equations);


            // Assert
            Assert.NotNull(TrianglesCongruent.IsTrianglesCongruent(database, t1, t2));

        }
        [Test]
        public void Test35()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("TrianglesCongruent", new List<string>() {"ABC","DEF" }),
            });
            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            Database database = handleQuestion.database;


            IsoscelesTriangle t1 = new IsoscelesTriangle(database, "A", "B", "C", new Line("CA"), "נתון");
            Triangle t2 = new Triangle(database, "D", "E", "F", "נתון");

            List<Node> list = new List<Node>() { t1.GetMainNode(), t2.GetMainNode() };
            Line ab = (Line)database.FindKey(new Line("AB"));
            Line de = (Line)database.FindKey(new Line("DE"));
            Line ef = (Line)database.FindKey(new Line("EF"));
            Angle bca = (Angle)database.FindKey(new Angle("BCA"));
            Angle efd = (Angle)database.FindKey(new Angle("EFD"));
            database.Update(ab, new Node(ab.ToString(), de.variable, "נתון", list), DataType.Equations);
            database.Update(de, new Node(de.ToString(), ab.variable, "נתון", list), DataType.Equations);

            database.Update(ab, new Node(ab.ToString(), 10, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(ef, new Node(ef.ToString(), 10, "נתון", t2.GetMainNode()), DataType.Equations);

            database.Update(bca, new Node(bca.ToString(), efd.variable, "נתון", list), DataType.Equations);
            database.Update(efd, new Node(efd.ToString(), bca.variable, "נתון", list), DataType.Equations);


            handleQuestion.GetSolution();
            // Assert
            Assert.NotNull(TrianglesCongruent.IsTrianglesCongruent(database, t1, t2));

        }
        [Test]
        public void Test36()
        {
            Database database = new Database();
            Triangle t1 = new Triangle(database, "A", "B", "C", "נתון");
            Triangle t2 = new Triangle(database, "D", "E", "F", "נתון");

            List<Node> list = new List<Node>() { t1.GetMainNode(), t2.GetMainNode() };

            database.Update(new Angle("ABC"), new Node("ABC", new Angle("DEF").variable, "נתון", list), DataType.Equations);
            database.Update(new Angle("DEF"), new Node("DEF", new Angle("ABC").variable, "נתון", list), DataType.Equations);

            database.Update(new Line("BC"), new Node("BC", 10, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(new Line("EF"), new Node("EF", 10, "נתון", t2.GetMainNode()), DataType.Equations);

            database.Update(new Angle("BCA"), new Node("BCA", new Angle("EFD").variable, "נתון", list), DataType.Equations);
            database.Update(new Angle("EFD"), new Node("EFD", new Angle("BCA").variable, "נתון", list), DataType.Equations);

            // Assert
            Assert.NotNull(TrianglesCongruent.IsTrianglesCongruent(database, t1, t2));

        }
        [Test]
        public void Test37()
        {

            Database database = new Database();
            Triangle t1 = new Triangle(database, "A", "B", "C", "נתון");
            Triangle t2 = new Triangle(database, "D", "E", "F", "נתון");

            List<Node> list = new List<Node>() { t1.GetMainNode(), t2.GetMainNode() };

            database.Update(new Angle("ABC"), new Node("ABC", new Angle("DEF").variable, "נתון", list), DataType.Equations);
            database.Update(new Angle("DEF"), new Node("DEF", new Angle("ABC").variable, "נתון", list), DataType.Equations);

            database.Update(new Line("AB"), new Node("AB", 10, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(new Line("DE"), new Node("DE", 10, "נתון", t2.GetMainNode()), DataType.Equations);

            database.Update(new Line("CA"), new Node("CA", 15, "נתון", t1.GetMainNode()), DataType.Equations);
            database.Update(new Line("FD"), new Node("FD", 15, "נתון", t2.GetMainNode()), DataType.Equations);

            // Assert
            Assert.NotNull(TrianglesCongruent.IsTrianglesCongruent(database, t1, t2));

        }
        [Test]
        public void Test38()
        {

            Database database = new Database();
            Triangle t1 = new Triangle(database, "A", "B", "C", "נתון");
            Triangle t2 = new Triangle(database, "D", "E", "F", "נתון");

            List<Node> list = new List<Node>() { t1.GetMainNode(), t2.GetMainNode() };

            database.Update(new Angle("BCA"), new Node("BCA", new Angle("EFD").variable, "נתון", list), DataType.Equations);
            database.Update(new Angle("EFD"), new Node("EFD", new Angle("BCA").variable, "נתון", list), DataType.Equations);

            Node inequalities = new Node("", "AB>CA", "נתון", list);
            inequalities.typeName = "inequalities";
            database.Update(new Line("AB"), inequalities, DataType.Inequalities);
            database.Update(new Line("CA"), inequalities, DataType.Inequalities);


            database.Update(new Line("DE"), new Node("DE", "AB", "נתון", list), DataType.Equations);
            database.Update(new Line("AB"), new Node("AB", "DE", "נתון", list), DataType.Equations);

            database.Update(new Line("FD"), new Node("FD", "CA", "נתון", list), DataType.Equations);
            database.Update(new Line("CA"), new Node("CA", "FD", "נתון", list), DataType.Equations);

            // Assert
            Assert.NotNull(TrianglesCongruent.IsTrianglesCongruent(database, t1, t2));
        }

        //************************************** שאלות ************************************************

        [Test]
        public void Test39()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //3
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Triangle", new List<string>() {"A", "B", "C" }),
                    new PairWrapper<string, List<string>>("Height_Triangle", new List<string>() {"A", "D", "A", "B", "C" }),
                    new PairWrapper<string, List<string>>("AngleBisector_Triangle", new List<string>(){"C", "E", "A", "B", "C" }),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AD","CE", "O" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"AOE", "63" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"AEC", "75" }),

             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.AreEqual("48", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
        }
        [Test]
        public void Test40()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //4
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("ParallelLinesWithTransversal", new List<string>() {"AB", "CD", "BC", "B", "C" }),
                new PairWrapper<string, List<string>>("ParallelLinesWithTransversal", new List<string>() {"AB", "CD", "EF", "E", "F" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() { "EF", "BC","O" }),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"BOF", "110" }),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"OFC", "64" }),

            });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("46", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
        }
        [Test]
        public void Test41()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //5
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
                   {
                        new PairWrapper<string, List<string>>("Triangle", new List<string>() {"A","B","C" }),
                        new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ABC","BCA / 2"}),
                        new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "BCA", "2 * ABC" }),
                        new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "CAB", "6 * ABC" }),
                        new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "ABC", "CAB / 6" }),

                   });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"BCA" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"CAB" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("20", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("40", handleQuestion.database.HandleEquations.Equations[new Angle("BCA")][0].Expression.ToString());
            Assert.AreEqual("120", handleQuestion.database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
        }
        [Test]
        public void Test42()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //7
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("Height_Triangle", new List<string>() {"B","E","A","B","C" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"EBC", "24" }),
             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"BCA" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"CAB" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("66", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("66", handleQuestion.database.HandleEquations.Equations[new Angle("BCA")][0].Expression.ToString());
            Assert.AreEqual("48", handleQuestion.database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
        }
        [Test]
        public void Test43()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //6.A
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ACB", "65" }),

             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"BCA" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"CAB" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("65", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("65", handleQuestion.database.HandleEquations.Equations[new Angle("BCA")][0].Expression.ToString());
            Assert.AreEqual("50", handleQuestion.database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
        }
        [Test]
        public void Test44()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //6.B
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"BAC", "6*BCA+4" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "BCA", "(CAB -4 )/6" }),
             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"BCA" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"CAB" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("22", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("22", handleQuestion.database.HandleEquations.Equations[new Angle("BCA")][0].Expression.ToString());
            Assert.AreEqual("136", handleQuestion.database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
        }
        [Test]
        public void Test45()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //10
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"BN","AC","N" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"MN","AB","M" }),
                new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"B", "M", "N","MN"}),
                new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"B", "N", "C","NC"}),
                new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"B", "M", "C","MC"}),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "CAB", "32" }),

            });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"BCA" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"CAB" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("74", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("74", handleQuestion.database.HandleEquations.Equations[new Angle("BCA")][0].Expression.ToString());
            Assert.AreEqual("32", handleQuestion.database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
        }
        [Test]
        public void Test46()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //11
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AB","KN","K" }),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"BC", "KN","E" }),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AN", "BC","C" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"AKN", "90" }),
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"C","E","N","EN" }),
             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"BCA" }),
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"CAB" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();
            // Assert
            Assert.AreEqual("60", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());
            Assert.AreEqual("60", handleQuestion.database.HandleEquations.Equations[new Angle("BCA")][0].Expression.ToString());
            Assert.AreEqual("60", handleQuestion.database.HandleEquations.Equations[new Angle("CAB")][0].Expression.ToString());
        }
        [Test]
        public void Test47()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //12
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                new PairWrapper<string, List<string>>("AngleBisector_Triangle", new List<string>() {"A","G","A","B","C" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"MB","AG","M" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"MC","AG","M" }),
            });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"BM","CM" }),
            });
            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);

            handleQuestion.GetSolution();
            // Assert
            Assert.NotNull(handleQuestion.database.GetEqualsNode(new Line("BM"), new Line("CM")));

        }
        [Test]
        public void Test48()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //14
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AB","CD","O" }),
                new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"CO","OD" }),
                new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"AO","OB" }),
            });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("TrianglesCongruent", new List<string>() {"ACO","BDO" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);


            // Assert
            Assert.IsTrue(handleQuestion.GetSolution().Count > 0);

        }
        [Test]
        public void Test49()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //15
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() { "AE", "BD", "D" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() { "AE", "CD", "D" }),
                new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() { "BD", "CD" }),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "BDE", "CDE" }),
            });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("TrianglesCongruent", new List<string>() {"ABD","ACD" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);


            // Assert
            Assert.IsTrue(handleQuestion.GetSolution().Count > 0);

        }
        [Test]
        public void Test50()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //16
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("Triangle", new List<string>() {"A","B","C" }),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ABC","ACB" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AD","BC","D" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AE","BC","E" }),
                new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"BE","CD" }),


            });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("TrianglesCongruent", new List<string>() {"ABD","ACE" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);


            // Assert
            Assert.IsTrue(handleQuestion.GetSolution().Count > 0);

        }
        [Test]
        public void Test51()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //17
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("Triangle", new List<string>() {"A","B","D" }),
                new PairWrapper<string, List<string>>("Triangle", new List<string>() {"C","B","D" }),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ABD","DBC" }),
                new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ADB","BDC" }),



            });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("TrianglesCongruent", new List<string>() {"ABD","CBD" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);


            // Assert
            Assert.IsTrue(handleQuestion.GetSolution().Count > 0);

        }
        [Test]
        public void Test52()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //31
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
            {
                new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A","B","C","BC" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AD","BC","D" }),
                new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AE","DC","E" }),
                new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"BD","CE" }),

            });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"AD","AE" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.NotNull(handleQuestion.database.GetEqualsNode(new Line("AD"), new Line("AE")));

        }
        [Test]
        public void Test53()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //37
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ACB", "75" }),
                    new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"AC", "16" }),
                    new PairWrapper<string, List<string>>("Height_Triangle", new List<string>() {"B","K", "A", "B", "C" }),

             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Line", new List<string>() {"BK" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.AreEqual("8", handleQuestion.database.HandleEquations.Equations[new Line("BK")][0].Expression.ToString());

        }
        [Test]
        public void Test54()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //38
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AD","BC","D" }),

                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"ABD", "30" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"DAC", "90" }),
                    new PairWrapper<string, List<string>>("Equation_Line_Expr", new List<string>() {"BC", "18" }),


             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
            {
                    new PairWrapper<string, List<string>>("Line", new List<string>() {"BD" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.AreEqual("9", handleQuestion.database.HandleEquations.Equations[new Line("BD")][0].Expression.ToString());

        }
        [Test]
        public void Test55()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //mine
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("IsoscelesTriangle", new List<string>() {"A", "B", "C","BC" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"BAC", "6*BCA+4" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "BCA", "(CAB -4 )/6" }),
             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"ABC" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.AreEqual("22", handleQuestion.database.HandleEquations.Equations[new Angle("ABC")][0].Expression.ToString());

        }

        [Test]
        public void Test56()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();
            //1 in parallel
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
                 {
                        new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AB","EF", "G" }),
                        new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"CD", "GF","H" }),
                        new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() {"EHD", "AGF" }),


                 });
            q.Add(TypeQ.Prove, new List<PairWrapper<string, List<string>>>()
                 {
                        new PairWrapper<string, List<string>>("ParallelLines", new List<string>() {"AB","CD" }),
                });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.IsTrue(handleQuestion.GetSolution().Count > 0);

        }

        public void Test57()
        {
            Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q = new Dictionary<TypeQ, List<PairWrapper<string, List<string>>>>();

            //2 in parallel
            q.Add(TypeQ.Given, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("ParallelLines", new List<string>() {"AB","CD"}),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() {"AB","EF", "G" }),
                    new PairWrapper<string, List<string>>("TwoLinesCut", new List<string>() { "CD", "GF", "H" }),
                    new PairWrapper<string, List<string>>("Equation_Angle_Expr", new List<string>() { "EHD", "40" }),


             });
            q.Add(TypeQ.Find, new List<PairWrapper<string, List<string>>>()
             {
                    new PairWrapper<string, List<string>>("Angle", new List<string>() {"AGF" }),
            });

            Question question = new Question(q);
            HandleQuestion handleQuestion = new HandleQuestion(question, false);
            handleQuestion.GetSolution();

            // Assert
            Assert.AreEqual("22", handleQuestion.database.HandleEquations.Equations[new Angle("AGF")][0].Expression.ToString());

        }
    }
}




