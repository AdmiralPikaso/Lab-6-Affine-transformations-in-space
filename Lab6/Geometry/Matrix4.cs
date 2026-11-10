using System.Globalization;
using System.Text;

namespace Lab6.Geometry;


public class Matrix4
{
    private readonly double[,] m = new double[4, 4];


    public Matrix4()
    {
    }


    public Matrix4(double[,] values)
    {
        if (values.GetLength(0) != 4 || values.GetLength(1) != 4)
            throw new ArgumentException("Нужна матрица 4×4", nameof(values));

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                m[i, j] = values[i, j];
    }

    public double this[int row, int col]
    {
        get => m[row, col];
        set => m[row, col] = value;
    }

    public static Matrix4 Identity()
    {
        var r = new Matrix4();
        for (int i = 0; i < 4; i++)
            r.m[i, i] = 1;
        return r;
    }


    public static Matrix4 operator *(Matrix4 a, Matrix4 b)
    {
        var r = new Matrix4();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                double s = 0;
                for (int k = 0; k < 4; k++)
                    s += a.m[i, k] * b.m[k, j];
                r.m[i, j] = s;
            }
        }
        return r;
    }


    public Point3D Transform(Point3D p)
    {
        double x = p.X * m[0, 0] + p.Y * m[1, 0] + p.Z * m[2, 0] + m[3, 0];
        double y = p.X * m[0, 1] + p.Y * m[1, 1] + p.Z * m[2, 1] + m[3, 1];
        double z = p.X * m[0, 2] + p.Y * m[1, 2] + p.Z * m[2, 2] + m[3, 2];
        double w = p.X * m[0, 3] + p.Y * m[1, 3] + p.Z * m[2, 3] + m[3, 3];

        if (Math.Abs(w) < 1e-12)
            return new Point3D(double.NaN, double.NaN, double.NaN);

        if (w != 1)
        {
            x /= w;
            y /= w;
            z /= w;
        }
        return new Point3D(x, y, z);
    }

    
    public static Point3D operator *(Point3D p, Matrix4 matrix) => matrix.Transform(p);

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
                sb.Append(m[i, j].ToString("0.###", CultureInfo.InvariantCulture).PadLeft(9));
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
