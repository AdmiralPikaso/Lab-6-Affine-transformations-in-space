namespace Lab6.Geometry;


public class Point3D
{
    public double X;
    public double Y;
    public double Z;

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Point3D Clone() => new Point3D(X, Y, Z);

    
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y) && double.IsFinite(Z);

    

    public static Point3D operator +(Point3D a, Point3D b) => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Point3D operator -(Point3D a, Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Point3D operator -(Point3D a) => new Point3D(-a.X, -a.Y, -a.Z);

    public static Point3D operator *(Point3D a, double k) => new Point3D(a.X * k, a.Y * k, a.Z * k);

    public static Point3D operator *(double k, Point3D a) => a * k;

    public static Point3D operator /(Point3D a, double k) => new Point3D(a.X / k, a.Y / k, a.Z / k);

    
    public static double Dot(Point3D a, Point3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

   
    public static Point3D Cross(Point3D a, Point3D b) =>
        new Point3D(a.Y * b.Z - a.Z * b.Y,
                    a.Z * b.X - a.X * b.Z,
                    a.X * b.Y - a.Y * b.X);

   
    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

   
    public Point3D Normalized()
    {
        double len = Length;
        return len < 1e-12 ? new Point3D(0, 0, 0) : this / len;
    }

    public override string ToString() =>
        FormattableString.Invariant($"({X:0.###}; {Y:0.###}; {Z:0.###})");
}
