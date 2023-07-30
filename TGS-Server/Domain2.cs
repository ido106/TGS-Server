using System;

public class Triangle
{
    public Point A { get; set; }
    public Point B { get; set; }
    public Point C { get; set; }

    public Edge AB { get; set; }
    public Edge BC { get; set; }
    public Edge CA { get; set; }

    public Angle ABC { get; set; }
    public Angle BCA { get; set; }
    public Angle CAB { get; set; }

    public Triangle(Point a, Point b, Point c)
    {
        A = a;
        B = b;
        C = c;

        AB = new Edge(A, B);
        BC = new Edge(B, C);
        CA = new Edge(C, A);

        ABC = new Angle(A, B, C);
        BCA = new Angle(B, C, A);
        CAB = new Angle(C, A, B);
    }

    public double GetAngle(Angle angle)
    {
        return angle.Value ?? angle.Evaluate(this);
    }

    public double GetEdge(Edge edge)
    {
        return edge.Value ?? edge.Evaluate(this);
    }
}

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}

public class Edge
{
    public Point A { get; set; }
    public Point B { get; set; }
    public double? Value { get; set; }
    public string Equation { get; set; }

    public Edge(Point a, Point b)
    {
        A = a;
        B = b;
    }

    public double Evaluate(Triangle triangle)
    {
        if (Value.HasValue)
        {
            return Value.Value;
        }
        else if (!string.IsNullOrEmpty(Equation))
        {
            // Parse equation and evaluate using triangle properties
            throw new NotImplementedException();
        }
        else
        {
            // Calculate length using Pythagorean theorem
            return Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        }
    }
}

public class Angle
{
    public Point A { get; set; }
    public Point B { get; set; }
    public Point C { get; set; }
    public double? Value { get; set; }
    public string Equation { get; set; }

    public Angle(Point a, Point b, Point c)
    {
        A = a;
        B = b;
        C = c;
    }

    public double Evaluate(Triangle triangle)
    {
        if (Value.HasValue)
        {
            return Value.Value;
        }
        else if (!string.IsNullOrEmpty(Equation))
        {
            // Parse equation and evaluate using triangle properties
            throw new NotImplementedException();
        }
        else
        {
            // Calculate angle using law of cosines
            double a = triangle.GetEdge(new Edge(B, C));
            double b = triangle.GetEdge(new Edge(A, C));
            double c = triangle.GetEdge(new Edge(A, B));
            return Math.Acos((a * a + b * b - c * c) / (2 * a * b));
        }
    }
}
