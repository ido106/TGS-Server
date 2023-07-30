

using AngouriMath;
using DatabaseLibrary;
using Domain;
using Domain.Quadrilateral;
using Domain.Solutions;
using Domain.Triangles;
using Microsoft.AspNetCore.Mvc;
using static DatabaseLibrary.Database;
using static Domain.Solutions.Question;

namespace TGS_Server.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TestsController : ControllerBase

    {
        // #85 Isosceles Trapezoid
        private Database test43()
        {
            Database database = new Database();
            Line l = new Line("DA");
            Trapezoid t = new Trapezoid(database, "A", "B", "C", "D", l, "נתון");

            database.Update(new Angle("ABC"), new Node("ABC", "BCD", "נתון", t.GetMainNode()), DataType.Equations);

            IsoscelesTrapezoid isos_t = IsoscelesTrapezoid.IsShape(t, database);
            if (isos_t == null)
                throw new Exception("test 43 failed");

            //Line l2 = new Line("HE");
            Quadrangle quad = new Quadrangle(database, "E", "F", "G", "H", "נתון");
            Line l1 = quad.LinesKeys[1];
            Line l2 = quad.LinesKeys[3];
            database.Update(l2, new Node(l2.ToString(), l1.variable, "נתון", quad.GetMainNode()), DataType.ParallelLines);
            database.Update(l1, new Node(l1.ToString(), l2.variable, "נתון", quad.GetMainNode()), DataType.ParallelLines);
            Angle a1 = quad.AnglesKeys[0];
            Angle a2 = quad.AnglesKeys[3];
            database.Update(a1, new Node(a1.ToString(), a2.variable, "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(a2, new Node(a2.ToString(), a1.variable, "נתון", quad.GetMainNode()), DataType.Equations);

            IsoscelesTrapezoid isos_t2 = IsoscelesTrapezoid.IsShape(quad, database);
            if (isos_t2 == null)
                throw new Exception("test 43 failed");

            Quadrangle fail_q = new Quadrangle(database, "I", "J", "K", "L", "נתון");
            IsoscelesTrapezoid isos_t3 = IsoscelesTrapezoid.IsShape(fail_q, database);
            if (isos_t3 != null)
                throw new Exception("test 43 failed");

            return database;
        }

        // Regular Trapezoid - By definition (no #)
        private Database test44()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");


            database.Update(new Line("BC"), new Node("BC", "DA", "נתון", quad.GetMainNode()), DataType.ParallelLines);
            database.Update(new Line("DA"), new Node("DA", "BC", "נתון", quad.GetMainNode()), DataType.ParallelLines);

            Trapezoid t = Trapezoid.IsShape(quad, database);

            if (t == null)
                throw new Exception("test 44 failed");

            Quadrangle quad2 = new Quadrangle(database, "E", "F", "G", "H", "נתון");
            Trapezoid t2 = Trapezoid.IsShape(quad2, database);
            if (t2 != null)
                throw new Exception("test 44 failed");

            return database;
        }

        // Right angled Trapezoid - By definition (no #)
        private Database test45()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            database.Update(quad.LinesKeys[1],
                new Node(quad.LinesKeys[1].ToString(), quad.LinesKeys[3].variable, "נתון", quad.GetMainNode()), DataType.ParallelLines);
            database.Update(quad.LinesKeys[3],
                new Node(quad.LinesKeys[3].ToString(), quad.LinesKeys[1].variable, "נתון", quad.GetMainNode()), DataType.ParallelLines);

            RightTrapezoid rt = RightTrapezoid.IsShape(quad, database);
            // stil shouldn't work
            if (rt != null)
                throw new Exception("test 45 failed");

            // should also work with DAB only when parallel lines are implemented s.t. it updates the Shape to Trapezoid
            database.Update(quad.AnglesKeys[0],
                               new Node(quad.AnglesKeys[0].ToString(), 90, "נתון", quad.GetMainNode()), DataType.Equations);

            rt = RightTrapezoid.IsShape(quad, database);
            // now should work
            if (rt == null)
                throw new Exception("test 45 failed");

            return database;
        }

        // #46 Parallelogram
        private Database test46()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            //Node parallelines = new Node("", "AB||CD", "נתון", quad.GetMainNode());
            // parallelines.type = "parallelLines";

            database.Update(new Line("AB"), new Node("AB", "CD", "נתון", quad.GetMainNode()), DataType.ParallelLines);
            database.Update(new Line("CD"), new Node("CD", "AB", "נתון", quad.GetMainNode()), DataType.ParallelLines);

            Parallelogram p = Parallelogram.IsShape(quad, database);
            // should fail
            if (p != null)
                throw new Exception("test 46 failed");

            database.Update(new Line("AB"), new Node("AB", "CD", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("CD"), new Node("CD", "AB", "נתון", quad.GetMainNode()), DataType.Equations);

            p = Parallelogram.IsShape(quad, database);
            // should work
            if (p == null)
                throw new Exception("test 46 failed");

            return database;
        }

        // #47 Parallelogram
        private Database test47()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            database.Update(new Angle("ABC"), new Node("ABC", new Angle("CDA").variable, "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Angle("DAB"), new Node("DAB", new Angle("BCD").variable, "נתון", quad.GetMainNode()), DataType.Equations);

            Parallelogram p = Parallelogram.IsShape(quad, database);
            if (p == null)
                throw new Exception("test 47 failed");

            return database;
        }

        // #48 Parallelogram

        private Database test48()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            database.Update(new Line("AB"),
                new Node("AB", "CD", "נתון", quad.GetMainNode()),
                DataType.Equations);

            database.Update(new Line("CD"),
                new Node("CD", "AB", "נתון", quad.GetMainNode()),
                DataType.Equations);

            Parallelogram p = Parallelogram.IsShape(quad, database);
            // should fail
            if (p != null)
                throw new Exception("test 48 failed");

            database.Update(new Line("BC"),
                new Node("BC", "DA", "נתון", quad.GetMainNode()),
                DataType.Equations);

            database.Update(new Line("DA"),
                new Node("DA", "BC", "נתון", quad.GetMainNode()),
                DataType.Equations);

            p = Parallelogram.IsShape(quad, database);
            // should succeed
            if (p == null)
                throw new Exception("test 48 failed");

            return database;
        }

        // S: 49
        private Database i_test1()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            Line diag1 = new Line("AC");
            Line diag2 = new Line("BD");
            quad.UpdateDiagonals(diag1, diag2, "E");

            database.Update(quad.Diag1Split[0], new Node(quad.Diag1Split[0].ToString(), quad.Diag1Split[1].variable,
                "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(quad.Diag1Split[1], new Node(quad.Diag1Split[1].ToString(), quad.Diag1Split[0].variable,
                "נתון", quad.GetMainNode()), DataType.Equations);

            // should fail
            Parallelogram p = Parallelogram.IsShape(quad, database);
            if (p != null)
                throw new Exception("test i_test1 failed");

            database.Update(quad.Diag2Split[0], new Node(quad.Diag2Split[0].ToString(), quad.Diag2Split[1].variable,
                "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(quad.Diag2Split[1], new Node(quad.Diag2Split[1].ToString(), quad.Diag2Split[0].variable,
                "נתון", quad.GetMainNode()), DataType.Equations);

            // should succeed
            p = Parallelogram.IsShape(quad, database);
            if (p == null)
                throw new Exception("test i_test1 failed");

            return database;
        }


        // #50 Parallelogram
        private Database test49()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            Parallelogram p = Parallelogram.IsShape(quad, database);
            // should fail
            if (p != null)
                throw new Exception("test 49 failed");

            database.Update(new Angle("DAB"), new Node("DAB", 80, "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Angle("ABC"), new Node("ABC", 100, "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Angle("BCD"), new Node("BCD", 80, "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Angle("CDA"), new Node("CDA", 100, "נתון", quad.GetMainNode()), DataType.Equations);

            p = Parallelogram.IsShape(quad, database);
            // should succeed
            if (p == null)
                throw new Exception("test 49 failed");

            return database;
        }



        // #54 Rectangle
        private Database test50()
        {
            Database database = new Database();
            Parallelogram quad = new Parallelogram(database, "A", "B", "C", "D", "נתון");

            Rectangle r = Rectangle.IsShape(quad, database);
            // should fail
            if (r != null)
                throw new Exception("test 50 failed");

            database.Update(new Angle("DAB"), new Node("DAB", 90, "נתון", quad.GetMainNode()), DataType.Equations);

            r = Rectangle.IsShape(quad, database);
            // should succeed
            if (r == null)
                throw new Exception("test 50 failed");

            return database;
        }


        // #55 Rectangle
        private Database test51()
        {
            Database database = new Database();
            Parallelogram quad = new Parallelogram(database, "D", "A", "B", "C", "נתון");

            // should fail
            Rectangle r = Rectangle.IsShape(quad, database);
            if (r != null)
                throw new Exception("test 51 failed");

            database.Update(new Line("DB"), new Node("DB", "AC", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("AC"), new Node("AC", "DB", "נתון", quad.GetMainNode()), DataType.Equations);

            r = Rectangle.IsShape(quad, database);
            if (r == null)
                throw new Exception("test 51 failed");

            return database;
        }


        // #62 Rhombus
        private Database test52()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            database.Update(new Line("AB"), new Node("AB", "BC", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("BC"), new Node("BC", "CD", "נתון", quad.GetMainNode()), DataType.Equations);

            Rhombus r = Rhombus.IsShape(quad, database);
            // should fail
            if (r != null)
                throw new Exception("test 52 failed");

            database.Update(new Line("CD"), new Node("CD", "DA", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("DA"), new Node("DA", "AB", "נתון", quad.GetMainNode()), DataType.Equations);

            r = Rhombus.IsShape(quad, database);
            // should succeed
            if (r == null)
                throw new Exception("test 52 failed");

            return database;
        }


        // #63 Rhombus
        private Database test53()
        {
            Database database = new Database();
            Parallelogram quad = new Parallelogram(database, "A", "B", "C", "D", "נתון");

            // should not work
            database.Update(quad.LinesKeys[0], new Node(quad.LinesKeys[0].ToString(), quad.LinesKeys[2].variable, "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(quad.LinesKeys[2], new Node(quad.LinesKeys[2].ToString(), quad.LinesKeys[0].variable, "נתון", quad.GetMainNode()), DataType.Equations);
            Rhombus r = Rhombus.IsShape(quad, database);
            if (r != null)
                throw new Exception("test 53 failed");

            // should work
            database.Update(quad.LinesKeys[3], new Node(quad.LinesKeys[3].ToString(), quad.LinesKeys[0].variable, "נתון", quad.GetMainNode()), DataType.Equations);
            r = Rhombus.IsShape(quad, database);
            if (r == null)
                throw new Exception("test 53 failed");

            return database;
        }


        // #72 Square
        private Database test54()
        {
            Database database = new Database();
            Quadrangle quad = new Quadrangle(database, "A", "B", "C", "D", "נתון");

            // all lines are equal to each other
            database.Update(new Line("AB"), new Node("AB", "BC", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("BC"), new Node("BC", "CD", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("CD"), new Node("CD", "DA", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("DA"), new Node("DA", "AB", "נתון", quad.GetMainNode()), DataType.Equations);

            // still shouldn't work
            Square s = Square.IsShape(quad, database);
            if (s != null)
                throw new Exception("test 54 failed");

            database.Update(new Angle("ABC"), new Node("ABC", 90, "נתון", quad.GetMainNode()), DataType.Equations);

            // now it should work
            s = Square.IsShape(quad, database);
            if (s == null)
                throw new Exception("test 54 failed");

            return database;
        }


        // #76 Square
        private Database test55()
        {
            Database database = new Database();
            Rhombus quad = new Rhombus(database, "D", "A", "B", "C", "נתון");

            // should fail now
            Square s = Square.IsShape(quad, database);
            if (s != null)
                throw new Exception("test 55 failed");

            database.Update(new Line("DB"), new Node("DB", "AC", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("AC"), new Node("AC", "DB", "נתון", quad.GetMainNode()), DataType.Equations);

            //should work now
            s = Square.IsShape(quad, database);
            if (s == null)
                throw new Exception("test 55 failed");

            return database;
        }


        // #77 Square
        private Database test56()
        {
            Database database = new Database();
            Rhombus quad = new Rhombus(database, "D", "A", "B", "C", "נתון");

            // should fail now
            Square s = Square.IsShape(quad, database);
            if (s != null)
                throw new Exception("test 56 failed");

            database.Update(new Angle("ABC"), new Node("ABC", 90, "נתון", quad.GetMainNode()), DataType.Equations);

            //should work now
            s = Square.IsShape(quad, database);
            if (s == null)
                throw new Exception("test 56 failed");

            return database;
        }



        // #79 Square
        private Database test57()
        {
            Database database = new Database();
            Rectangle quad = new Rectangle(database, "D", "A", "B", "C", "נתון");

            // should fail now
            Square s = Square.IsShape(quad, database);
            if (s != null)
                throw new Exception("test 57 failed");

            database.Update(new Line("BC"), new Node("BC", "CD", "נתון", quad.GetMainNode()), DataType.Equations);

            //should work now
            s = Square.IsShape(quad, database);
            if (s == null)
                throw new Exception("test 57 failed");

            return database;
        }

        // #86 Isosceles Trapezoid
        private Database test58()
        {
            Database database = new Database();
            Line l = new Line("CD");
            Trapezoid quad = new Trapezoid(database, "D", "A", "B", "C", l, "נתון");

            // should fail now
            IsoscelesTrapezoid it = IsoscelesTrapezoid.IsShape(quad, database);
            if (it != null)
                throw new Exception("test 58 failed");

            database.Update(new Line("DB"), new Node("DB", "AC", "נתון", quad.GetMainNode()), DataType.Equations);
            database.Update(new Line("AC"), new Node("AC", "DB", "נתון", quad.GetMainNode()), DataType.Equations);

            //should work now
            it = IsoscelesTrapezoid.IsShape(quad, database);
            if (it == null)
                throw new Exception("test 58 failed");

            return database;
        }

   
        private void runTests()
        {
            /**
            test1();
            test2();
            test2();
            test3();
            test4();
            test5();
            test6();
            test7();
            test8();
            test9();
            test10();
            test11();
            test12();
            test13();
            test14();
            test15();
            test16();
            test17();
            test18();
            test19();
            test20();
            test21();
            test22();
            test23();
            test24();
            test25();
            test26();
            test27();
            test28();
            test29();
            test30();
            test31();
            test32();
            test33();
            test34();
            test35();
            test36();
            test37();
            test38();
            test39();
            test40();
            test41();
            test42();
            */

            test43();
            test44(); // parallel lines
            test45(); // parallel lines
            test46(); // parallel lines
            test47();
            test48();
            test49();
            test50();
            test51();
            test52();
            test53();
            test54();
            test55();
            test56();
            test57();
            test58();

            i_test1();
        }

 
        [HttpGet]
        public IActionResult GetMath()
        {
             runTests();

             return Ok();

            
        }


    }
}